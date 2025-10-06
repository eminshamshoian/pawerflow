# Pawerflow Technical Overview

## High-Level Architecture
- Pawerflow is a .NET Aspire distributed application composed of two microservices backed by shared infrastructure.
- The solution is orchestrated by the `DistributedApplication` defined in `Pawerflow.AppHost/AppHost.cs:1`, which wires up containerized dependencies and service references.
- Core infrastructure provisioned at startup:
  - Keycloak identity provider (port 6001) for JWT issuance.
  - PostgreSQL database with a dedicated `questionDb` logical database.
  - RabbitMQ message broker with the management plugin exposed on port 15672.
  - Typesense search engine (port 8108) secured by an API key parameter.
- Application services:
  - `QuestionService` — write-heavy API owning questions, answers, and tag catalog; persists to PostgreSQL and emits integration events.
  - `SearchService` — read model fed by events; maintains a Typesense collection and exposes query endpoints.
- Shared cross-cutting concerns (health checks, telemetry, resilience) are centralized in `Pawerflow.ServiceDefaults`.

## Infrastructure Orchestration (AppHost)
- `AppHost.cs` registers each dependency inside the Aspire `DistributedApplication` builder in a declarative manner.
- Keycloak, PostgreSQL, RabbitMQ, and Typesense all persist data using named Docker volumes so state survives restarts.
- The Typesense container receives its API key from a secret parameter (`typesense-api-key`). Aspire injects the resolved endpoint into dependent services via service discovery (`services:typesense:typesense:0`).
- `QuestionService` and `SearchService` projects are registered with `.WithReference(...)` dependencies and `.WaitFor(...)` guards, ensuring the containers are ready before the services start.
- Aspire handles service URL propagation and dynamic port assignment; service endpoints are visible in the AppHost dashboard/log output when running locally.

## Shared Service Defaults
- `Pawerflow.ServiceDefaults/Extensions.cs:1` defines extension methods that each service calls via `builder.AddServiceDefaults()`.
- The extension layer configures:
  - OpenTelemetry metrics (ASP.NET Core, HttpClient, runtime) and tracing, adding `builder.Environment.ApplicationName` as the service name and excluding health probes.
  - Log enrichment with scopes and formatted messages.
  - Default HTTP client resilience policies and service discovery integration.
  - Health checks with `/health` (readiness) and `/alive` (liveness) endpoints exposed during development.
- Optional exporters (OTLP, Azure Monitor) are scaffolded but disabled until corresponding configuration is supplied.

## Domain Contracts
- `Common/Contracts` declares record types shared by publisher and consumers, targeting both `net9.0` and `net10.0` for forward compatibility.
- Events emitted on the RabbitMQ `questions` exchange:
  - `QuestionCreated` — question snapshot including Markdown/HTML content, tags, and creation timestamp.
  - `QuestionUpdated` — title, content, and tag updates for existing questions.
  - `QuestionDeleted` — signals removal of a question and its answers.
  - `AnswerCountUpdated` — authoritative answer tally per question.
  - `AnswerAccepted` — flags the existence of an accepted answer.
- Keeping contracts in a dedicated project keeps message payloads versioned and reusable across future services.

## Question Service
### Responsibilities
- Exposes REST endpoints for creating, updating, and retrieving questions, answers, and tags.
- Owns persistence in PostgreSQL via Entity Framework Core and ensures database schema is migrated at startup.
- Publishes domain events after mutating operations to maintain other read models in sync.

### Configuration (`QuestionService/Program.cs:1`)
- Registers controllers, OpenAPI, memory cache, and the scoped `TagService`.
- Applies shared defaults (`AddServiceDefaults`) for telemetry and health.
- Configures Keycloak JWT bearer authentication through Aspire (`AddKeycloakJwtBearer`). Audience `pawerflow` is required; HTTPS metadata is disabled for local development.
- Wires PostgreSQL with `AddNpgsqlDbContext<QuestionDbContext>`; Aspire injects the connection string discovered from AppHost.
- Adds extra OpenTelemetry tracing for Wolverine message sources.
- Configures Wolverine to:
  - Use the named RabbitMQ connection `messaging`.
  - Auto-provision exchanges/queues.
  - Publish all messages to the `questions` exchange.
- Creates a scope on startup to run `context.Database.MigrateAsync()`, guaranteeing migrations are applied automatically.

### Data Model (`QuestionService/Models`)
- `Question` — GUID string primary key, title/content limits, asker identity, timestamps, `TagSlugs`, view counter, `HasAcceptedAnswer`, vote tally placeholder, `AnswerCount`, and navigation property to `Answers`.
- `Answer` — GUID string key with textual content, author metadata, timestamps, accepted flag, and back-reference to the owning question.
- `Tag` — string key with name, slug, description, and `UsageCount` field for future analytics.
- `QuestionDbContext` seeds a catalog of pet-related tags (`OnModelCreating`) and enforces cascading deletes between questions and answers. The seeded data powers tag validation and initial UI filters.

### Tag Validation & Caching
- `TagService` caches the entire tag list for two hours using `IMemoryCache`. Cache invalidation can be triggered manually by clearing the key if administrative updates are added later.
- `AreTagsValidAsync` ensures every requested slug exists before writes proceed.
- `TagListValidator` (used by `CreateQuestionDto`) enforces between one and five tags per question.

### REST API Surface
| Method | Route | Description | Auth |
| --- | --- | --- | --- |
| POST | `/questions` | Validates tags, persists a new question, publishes `QuestionCreated`, returns `201 Created`. | Required |
| GET | `/questions` | Retrieves questions ordered by `CreatedAt` desc; optional `tag` query filters by slug. | None |
| GET | `/questions/{id}` | Loads question with answers, increments `ViewCount` via EF `ExecuteUpdateAsync`. | None |
| POST | `/questions/{questionId}/answers` | Adds answer, increments `AnswerCount`, publishes `AnswerCountUpdated`. | Required |
| PUT | `/questions/{questionId}/answers/{answerId}` | Updates answer content; ensures ownership and existence. | Required |
| DELETE | `/questions/{questionId}/answers/{answerId}` | Deletes non-accepted answer, decrements count, publishes `AnswerCountUpdated`. | Required |
| POST | `/questions/{questionId}/answers/{answerId}/accept` | Marks answer accepted, toggles `HasAcceptedAnswer`, emits `AnswerAccepted`. | Required |
| PUT | `/questions/{id}` | Updates question fields and tags; only asker may perform the action; publishes `QuestionUpdated`. | Required |
| DELETE | `/questions/{id}` | Removes question and cascaded answers; publishes `QuestionDeleted`. | Required |
| GET | `/tags` | Returns alphabetized tag list from PostgreSQL. | None |

### Messaging Behaviour
- Wolverine injects `IMessageBus` into controllers; publish calls occur after EF Core persistence to ensure consistency.
- Messages use the shared contracts to avoid schema drift.
- RabbitMQ exchange `questions` fans out to any queue (currently `questions.search`) bound by consumers.

## Search Service
### Responsibilities
- Provides read-only search endpoints backed by Typesense.
- Listens for domain events to keep the search index synchronized with the write model.

### Configuration (`SearchService/Program.cs:1`)
- Applies shared defaults and registers the Typesense client with host/port parsed from service discovery metadata.
- Wolverine listens to RabbitMQ queue `questions.search`, binding to the `questions` exchange for fan-out subscription.
- Typesense API key pulled from environment variable `typesense-api-key`; missing configuration causes startup failure, protecting against silent misconfiguration.
- On startup, creates a scoped `ITypesenseClient` and invokes `SearchInitializer.EnsureIndexExists` to create the `questions` collection if absent.

### Typesense Schema (`SearchService/Data/SearchInitializer.cs:6`)
- Fields:
  - `id` (`string`) — primary document key.
  - `title` (`string`) and `content` (`string`) — searchable fields.
  - `tags` (`string[]`) — facetable tag slugs.
  - `createdAt` (`int64`) — default sorting field.
  - `answerCount` (`int32`) — used for weighting or UI display.
  - `hasAcceptedAnswer` (`bool`) — enabling accepted-answer filters.
- Gracefully handles pre-existing collections by catching `TypesenseApiNotFoundException`.

### Message Handlers
- `QuestionCreatedHandler` converts HTML to plain text, maps timestamps to Unix seconds, and inserts the document.
- `QuestionUpdatedHandler` patches title/content/tags without reindexing the full document.
- `QuestionDeletedHandler` removes the Typesense document when the source question is deleted.
- `AnswerCountUpdatedHandler` updates the `answerCount` field.
- `AcceptAnswerHandler` sets `hasAcceptedAnswer` to true when answers are accepted.

### HTTP API
| Method | Route | Description |
| --- | --- | --- |
| GET | `/search?query=...` | Full-text search over title and content. Optional `[tag]` token (e.g. `"crate training [dog-care]"`) filters results via `FilterBy`. |
| GET | `/search/similar-titles?query=...` | Lightweight title-only search for suggestions/autocomplete scenarios. |

- Both endpoints catch Typesense exceptions and convert them into RFC 7807 problem responses, surfacing the error message for debugging.

## Messaging & Data Flow
1. Client authenticates against Keycloak and calls `QuestionService` with a JWT.
2. `QuestionService` executes EF Core operations against PostgreSQL; migrations ensure the schema matches the latest model.
3. After successful persistence, Wolverine publishes integration events to RabbitMQ (`questions` exchange).
4. RabbitMQ routes messages to bound queues (currently `questions.search`).
5. `SearchService` handlers react to messages, mutating the Typesense `questions` collection accordingly.
6. Consumers call `SearchService` endpoints, which proxy queries to Typesense and return normalized `SearchQuestion` DTOs.
7. Accepted answers and answer count changes propagate through the same mechanism, keeping the search index aligned with the source of truth.

## Security & Authentication
- Keycloak container (defined in `Pawerflow.AppHost/AppHost.cs:3`) issues tokens for realm `pawerflow`.
- `QuestionService` uses `AddKeycloakJwtBearer` to validate tokens, pull realm metadata via service discovery, and enforce audience.
- Mutating endpoints require authorization attributes; read endpoints remain anonymous for broader discoverability.
- Controller logic stamps asker/answer metadata using `ClaimTypes.NameIdentifier` and the custom `name` claim from Keycloak.

## Observability & Health
- OpenTelemetry traces include Wolverine spans, enabling correlation between HTTP requests and downstream message publications/consumptions.
- Metrics cover ASP.NET Core request durations, .NET runtime stats, and HttpClient dependencies.
- Health endpoints:
  - `GET /health` — readiness (all checks).
  - `GET /alive` — liveness (tagged `live`).
- Configure `OTEL_EXPORTER_OTLP_ENDPOINT` to enable OTLP exporters; Azure Monitor blocks are scaffolded for future use.

## Running the Stack Locally
1. Ensure .NET SDK 9.0 is installed (`global.json` enforces version and allows prerelease).
2. Start Docker Desktop (or equivalent) for containerized dependencies.
3. From the solution root, run `dotnet run --project Pawerflow.AppHost` to launch the Aspire AppHost, which provisions infrastructure and starts both services.
4. Provide the `typesense-api-key` secret when prompted (it will be cached by Aspire).
5. Monitor AppHost output for service URLs; Aspire forwards ports and displays dashboards for Keycloak, RabbitMQ management, and Typesense.
6. Use the generated OpenAPI endpoints (`/openapi`) or the `.http` files under each service (`QuestionService/QuestionService.http`, `SearchService/SearchService.http`) to exercise APIs.
7. PostgreSQL migrations run automatically; Typesense schema is created on first launch.

## Extending the System
- Add new integration events to `Common/Contracts` and publish them from `QuestionService` to unlock additional read models.
- Additional consumers can subscribe to the `questions` exchange by creating new Wolverine-enabled services and binding queues.
- Service-wide observability can be enhanced by providing OTLP exporters or enabling Azure Monitor with minimal code changes in `Pawerflow.ServiceDefaults`.

var builder = DistributedApplication.CreateBuilder(args);

var keycloack = builder.AddKeycloak("keycloak", 6001)
    .WithDataVolume("keycloak.data");

builder.Build().Run();
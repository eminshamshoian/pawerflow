using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuestionService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    AskerId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    AskerDisplayName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    TagSlugs = table.Column<List<string>>(type: "text[]", nullable: false),
                    HasAcceptedAnswer = table.Column<bool>(type: "boolean", nullable: false),
                    Votes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Slug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Description", "Name", "Slug", "UsageCount" },
                values: new object[,]
                {
                    { "pet-breeding", "Responsible breeding practices, pregnancy care, whelping, and raising litters.", "Breeding", "pet-breeding", 0 },
                    { "pet-housing", "Advice on cages, tanks, hutches, terrariums, and enclosures that keep pets safe and comfortable.", "Housing & Habitats", "pet-housing", 0 },
                    { "pet-loss", "Support for coping with the loss of a beloved pet, memorial ideas, and community healing.", "Pet Loss & Grief", "pet-loss", 0 },
                    { "pet-rescue-stories", "A place to share and learn from inspiring rescue and adoption experiences.", "Rescue Stories", "pet-rescue-stories", 0 },
                    { "pet-safety", "Emergency care, first aid, toxic foods, and household hazards to watch out for with pets.", "Safety & First Aid", "pet-safety", 0 },
                    { "pet-tech", "Discussion about smart collars, GPS trackers, feeders, cameras, and other tech designed for pets.", "Pet Tech & Gadgets", "pet-tech", 0 },
                    { "pet-travel", "Tips for traveling safely with pets in cars, planes, or trains. Covers carriers, calming, regulations, and more.", "Travel with Pets", "pet-travel", 0 },
                    { "wildlife", "Dealing with pets interacting with wildlife—prevention, safety, and co-existence tips.", "Wildlife Encounters", "wildlife", 0 },
                    { "working-dogs", "Training, care, and legal aspects of service dogs, therapy animals, police dogs, and other working companions.", "Working & Service Dogs", "working-dogs", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}

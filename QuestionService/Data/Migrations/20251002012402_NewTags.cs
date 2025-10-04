using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuestionService.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Description", "Name", "Slug", "UsageCount" },
                values: new object[,]
                {
                    { "cat-care", "Tips and advice for caring for cats, including litter training, diet, scratching behavior, and vet visits.", "Cat Care", "cat-care", 0 },
                    { "dog-care", "Questions about feeding, grooming, exercise, training, and health for dogs of all breeds and sizes.", "Dog Care", "dog-care", 0 },
                    { "exotic-pets", "Care and handling information for reptiles, amphibians, birds, and other exotic or uncommon pets.", "Exotic Pets", "exotic-pets", 0 },
                    { "fish-care", "Aquarium setup, water quality, species compatibility, and health care for freshwater and saltwater fish.", "Fish & Aquatic Pets", "fish-care", 0 },
                    { "pet-adoption", "Discussions around adopting pets, rescue organizations, fostering, and integrating new pets into a home.", "Adoption & Rescue", "pet-adoption", 0 },
                    { "pet-grooming", "Bathing, brushing, nail trimming, and coat care for pets to keep them comfortable and healthy.", "Grooming", "pet-grooming", 0 },
                    { "pet-health", "Medical and wellness topics: vaccinations, common illnesses, preventative care, and vet recommendations.", "Pet Health", "pet-health", 0 },
                    { "pet-nutrition", "Questions on pet food, special diets, treats, supplements, and nutrition requirements by species.", "Pet Nutrition", "pet-nutrition", 0 },
                    { "pet-products", "Reviews and advice on toys, collars, leashes, beds, aquariums, and other pet supplies or equipment.", "Pet Products", "pet-products", 0 },
                    { "pet-training", "Guidance on obedience training, housebreaking, behavioral issues, and positive reinforcement methods.", "Training & Behavior", "pet-training", 0 },
                    { "small-animals", "Care for rabbits, guinea pigs, hamsters, ferrets, and other small pets including housing, diet, and play.", "Small Animals", "small-animals", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: "cat-care");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: "dog-care");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: "exotic-pets");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: "fish-care");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: "pet-adoption");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: "pet-grooming");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: "pet-health");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: "pet-nutrition");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: "pet-products");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: "pet-training");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: "small-animals");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using QuestionService.Models;

namespace QuestionService.Data;

public class QuestionDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Question>  Questions { get; set; }
    public DbSet<Tag>  Tags { get; set; }
    public DbSet<Answer> Answers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Question>()
            .HasMany(q => q.Answers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tag>()
            .HasData(
                // 🐶 Core Pet Care
                new Tag
                {
                    Id = "dog-care",
                    Name = "Dog Care",
                    Slug = "dog-care",
                    Description =
                        "Questions about feeding, grooming, exercise, training, and health for dogs of all breeds and sizes."
                },
                new Tag
                {
                    Id = "cat-care",
                    Name = "Cat Care",
                    Slug = "cat-care",
                    Description =
                        "Tips and advice for caring for cats, including litter training, diet, scratching behavior, and vet visits."
                },
                new Tag
                {
                    Id = "small-animals",
                    Name = "Small Animals",
                    Slug = "small-animals",
                    Description =
                        "Care for rabbits, guinea pigs, hamsters, ferrets, and other small pets including housing, diet, and play."
                },
                new Tag
                {
                    Id = "fish-care",
                    Name = "Fish & Aquatic Pets",
                    Slug = "fish-care",
                    Description =
                        "Aquarium setup, water quality, species compatibility, and health care for freshwater and saltwater fish."
                },
                new Tag
                {
                    Id = "exotic-pets",
                    Name = "Exotic Pets",
                    Slug = "exotic-pets",
                    Description =
                        "Care and handling information for reptiles, amphibians, birds, and other exotic or uncommon pets."
                },

                // 🩺 Health & Wellness
                new Tag
                {
                    Id = "pet-health",
                    Name = "Pet Health",
                    Slug = "pet-health",
                    Description =
                        "Medical and wellness topics: vaccinations, common illnesses, preventative care, and vet recommendations."
                },
                new Tag
                {
                    Id = "pet-nutrition",
                    Name = "Pet Nutrition",
                    Slug = "pet-nutrition",
                    Description =
                        "Questions on pet food, special diets, treats, supplements, and nutrition requirements by species."
                },
                new Tag
                {
                    Id = "pet-grooming",
                    Name = "Grooming",
                    Slug = "pet-grooming",
                    Description =
                        "Bathing, brushing, nail trimming, and coat care for pets to keep them comfortable and healthy."
                },
                new Tag
                {
                    Id = "pet-safety",
                    Name = "Safety & First Aid",
                    Slug = "pet-safety",
                    Description =
                        "Emergency care, first aid, toxic foods, and household hazards to watch out for with pets."
                },

                // 🎓 Training & Behavior
                new Tag
                {
                    Id = "pet-training",
                    Name = "Training & Behavior",
                    Slug = "pet-training",
                    Description =
                        "Guidance on obedience training, housebreaking, behavioral issues, and positive reinforcement methods."
                },
                new Tag
                {
                    Id = "working-dogs",
                    Name = "Working & Service Dogs",
                    Slug = "working-dogs",
                    Description =
                        "Training, care, and legal aspects of service dogs, therapy animals, police dogs, and other working companions."
                },

                // 🏠 Lifestyle & Ownership
                new Tag
                {
                    Id = "pet-adoption",
                    Name = "Adoption & Rescue",
                    Slug = "pet-adoption",
                    Description =
                        "Discussions around adopting pets, rescue organizations, fostering, and integrating new pets into a home."
                },
                new Tag
                {
                    Id = "pet-housing",
                    Name = "Housing & Habitats",
                    Slug = "pet-housing",
                    Description =
                        "Advice on cages, tanks, hutches, terrariums, and enclosures that keep pets safe and comfortable."
                },
                new Tag
                {
                    Id = "pet-travel",
                    Name = "Travel with Pets",
                    Slug = "pet-travel",
                    Description =
                        "Tips for traveling safely with pets in cars, planes, or trains. Covers carriers, calming, regulations, and more."
                },
                new Tag
                {
                    Id = "pet-products",
                    Name = "Pet Products",
                    Slug = "pet-products",
                    Description =
                        "Reviews and advice on toys, collars, leashes, beds, aquariums, and other pet supplies or equipment."
                },
                new Tag
                {
                    Id = "pet-tech",
                    Name = "Pet Tech & Gadgets",
                    Slug = "pet-tech",
                    Description =
                        "Discussion about smart collars, GPS trackers, feeders, cameras, and other tech designed for pets."
                },

                // ❤️ Community & Support
                new Tag
                {
                    Id = "pet-breeding",
                    Name = "Breeding",
                    Slug = "pet-breeding",
                    Description =
                        "Responsible breeding practices, pregnancy care, whelping, and raising litters."
                },
                new Tag
                {
                    Id = "pet-rescue-stories",
                    Name = "Rescue Stories",
                    Slug = "pet-rescue-stories",
                    Description =
                        "A place to share and learn from inspiring rescue and adoption experiences."
                },
                new Tag
                {
                    Id = "wildlife",
                    Name = "Wildlife Encounters",
                    Slug = "wildlife",
                    Description =
                        "Dealing with pets interacting with wildlife—prevention, safety, and co-existence tips."
                },
                new Tag
                {
                    Id = "pet-loss",
                    Name = "Pet Loss & Grief",
                    Slug = "pet-loss",
                    Description =
                        "Support for coping with the loss of a beloved pet, memorial ideas, and community healing."
                }
            );

    }
}

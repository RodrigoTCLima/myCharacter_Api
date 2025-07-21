using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using myCharacter.Models; 

namespace myCharacter.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<RpgSystem> RpgSystems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Definição dos Relacionamentos ---

            // Relacionamento: User -> Campaign
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Campaigns)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .IsRequired();

            // Relacionamento: User -> Character
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Characters)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .IsRequired();

            // Relacionamento: Campaign -> Character
            modelBuilder.Entity<Campaign>()
                .HasMany(c => c.Characters)
                .WithOne(c => c.Campaign)
                .HasForeignKey(c => c.CampaignId)
                .IsRequired(false); // Permite personagens "órfãos"

            // Relacionamento: RpgSystem -> Campaign
            modelBuilder.Entity<RpgSystem>()
                .HasMany(s => s.Campaigns)
                .WithOne(c => c.RpgSystem)
                .HasForeignKey(c => c.RpgSystemId)
                .IsRequired();


            // Relacionamento: RpgSystem -> Character
            modelBuilder.Entity<RpgSystem>()
                .HasMany(s => s.Characters) 
                .WithOne(c => c.RpgSystem)  
                .HasForeignKey(c => c.RpgSystemId) 
                .IsRequired(); 
        }
    }
}

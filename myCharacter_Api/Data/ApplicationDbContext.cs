// CÓDIGO INTEGRAL E ATUALIZADO

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using myCharacter.Models; // Lembre-se de ajustar 'myCharacter' para o namespace do seu projeto

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

            // --- Definição dos Relacionamentos (Conforme nosso plano) ---

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

            // ======================= INÍCIO DA MUDANÇA =======================
            //
            // NOVO RELACIONAMENTO: RpgSystem -> Character
            // Adicionamos esta configuração para formalizar o novo relacionamento
            // que você sugeriu.
            //
            modelBuilder.Entity<RpgSystem>()
                .HasMany(s => s.Characters) // Um RpgSystem tem muitos Characters
                .WithOne(c => c.RpgSystem)  // Um Character tem um RpgSystem
                .HasForeignKey(c => c.RpgSystemId) // A chave estrangeira está em Character.RpgSystemId
                .IsRequired(); // Um personagem DEVE pertencer a um sistema de regras.
            //
            // ======================== FIM DA MUDANÇA =========================
        }
    }
}

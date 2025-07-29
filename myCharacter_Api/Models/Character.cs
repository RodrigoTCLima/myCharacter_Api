using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myCharacter.Models 
{
    public class Character
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Race { get; set; }

        [MaxLength(50)]
        public string? Class { get; set; }

        public int Level { get; set; } = 1;

        public string? SystemSpecificData { get; set; }

        // --- Chaves Estrangeiras e Relacionamentos ---

        public int RpgSystemId { get; set; }
        [ForeignKey("RpgSystemId")]
        public RpgSystem RpgSystem { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;

        public Guid? CampaignId { get; set; }
        [ForeignKey("CampaignId")]
        public Campaign? Campaign { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myCharacter.Models
{
    public class Campaign
    {
        public Campaign()
        {
            Characters = new HashSet<Character>();
        }

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        // --- Chaves Estrangeiras e Relacionamentos ---

        // Relacionamento com RpgSystem
        public int RpgSystemId { get; set; }
        [ForeignKey("RpgSystemId")]
        public RpgSystem RpgSystem { get; set; } = null!;

        // Relacionamento com ApplicationUser
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;

        // Propriedade de navegação
        // Uma campanha pode ter muitos personagens.
        public ICollection<Character> Characters { get; set; }
    }
}

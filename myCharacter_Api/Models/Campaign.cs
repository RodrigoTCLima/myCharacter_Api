// Localização: myCharacter/Models/Campaign.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myCharacter.Models
{
    /// <summary>
    /// Representa uma campanha de RPG.
    /// </summary>
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
        public string Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        // --- Chaves Estrangeiras e Relacionamentos ---

        // Relacionamento com RpgSystem
        public int RpgSystemId { get; set; }
        [ForeignKey("RpgSystemId")]
        public virtual RpgSystem RpgSystem { get; set; }

        // Relacionamento com ApplicationUser
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // Propriedade de navegação
        // Uma campanha pode ter muitos personagens.
        public virtual ICollection<Character> Characters { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace myCharacter.Models
{
    public class RpgSystem
    {
        public RpgSystem()
        {
            Campaigns = new HashSet<Campaign>();
            Characters = new HashSet<Character>(); 
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? CharacterSheetTemplate { get; set; }

        // Propriedades de navegação (relacionamentos)
        public ICollection<Campaign> Campaigns { get; set; }
        public ICollection<Character> Characters { get; set; } 
    }
}

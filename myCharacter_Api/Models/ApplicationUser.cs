using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace myCharacter.Models 
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Campaigns = new HashSet<Campaign>();
            Characters = new HashSet<Character>();
        }

        // Propriedades de navegação para os relacionamentos
        public ICollection<Campaign> Campaigns { get; set; }
        public ICollection<Character> Characters { get; set; }
    }
}

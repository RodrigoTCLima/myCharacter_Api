using myCharacter.Models;

namespace myCharacter.DTOs.Campaign{
	public class CampaignDetailDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public int RpgSystemId { get; set; }
		public ICollection<Character> Characters { get; set; } = new List<Character>();
	}
}
namespace myCharacter.DTOs.Characters
{
	public class CreateCharacterDto
	{
		public string Name { get; set; } = string.Empty;
		public string? Race { get; set; }
		public string? Class { get; set; }
		public int Level { get; set; } = 1;
		public string? SystemSpecificData { get; set; }
		public int RpgSystemId { get; set; }
		public Guid? CampaignId { get; set; }
	}
}
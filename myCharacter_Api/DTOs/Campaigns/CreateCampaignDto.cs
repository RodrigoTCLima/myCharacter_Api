namespace myCharacter.DTOs.Campaign
{
    public class CreateCampaignDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int RpgSystemId { get; set; }
    }
}
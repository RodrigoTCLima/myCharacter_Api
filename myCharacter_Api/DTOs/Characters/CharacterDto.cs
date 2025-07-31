public class CharacterDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Race { get; set; }
    public string? Class { get; set; }
    public int Level { get; set; } = 1;
    public int RpgSystemId { get; set; }
    public Guid? CampaignId { get; set; }
}
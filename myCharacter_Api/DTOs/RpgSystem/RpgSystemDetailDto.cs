namespace myCharacter.DTOs.RpgSystems{
    public class RpgSystemDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CharacterSheetTemplate { get; set; }
    }
}
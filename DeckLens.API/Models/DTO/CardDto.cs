namespace DeckLens.API.Models.DTO
{
    public class CardDto
    {
        public string CardName { get; set; }
        public string ManaCost { get; set; }
        public string TypeLine { get; set; }
        public string OracleText { get; set; }
        public List<string> Keywords { get; set; } = new();
    }
}

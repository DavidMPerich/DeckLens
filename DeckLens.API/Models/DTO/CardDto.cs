namespace DeckLens.API.Models.DTO
{
    public class CardDto
    {
        public string CardName { get; set; }
        public string ManaCost { get; set; }
        public double ConvertedManaCost { get; set; }
        public string TypeLine { get; set; }
        public string OracleText { get; set; }
        public string Power {  get; set; }
        public string Toughness { get; set; }
        public List<string> Colors { get; set; } = new();
        public List<string> ColorIdentity { get; set; } = new();
        public List<string> Keywords { get; set; } = new();
        public string Rarity { get; set; }
        public int EDHRecRank { get; set; }

    }
}

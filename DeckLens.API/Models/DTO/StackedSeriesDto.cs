namespace DeckLens.API.Models.DTO
{
    public class StackedSeriesDto
    {
        public string Name { get; set; } = string.Empty;
        public List<int> Data { get; set; } = new();
    }
}

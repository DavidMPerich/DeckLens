namespace DeckLens.API.Services.Interface
{
    public interface IDeckImportService
    {
        List<string> Parse(string rawInput); 
    }
}

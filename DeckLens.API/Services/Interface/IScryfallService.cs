using DeckLens.API.Models.DTO;

namespace DeckLens.API.Services.Interface
{
    public interface IScryfallService
    {
        Task<CardDto?> GetCardByNameAsync(string name);
        Task<(List<CardDto> Cards, List<string> NotFound)> GetCardCollectionAsync(IEnumerable<string> names, TimeSpan ttl);
    }
}

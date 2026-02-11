using DeckLens.API.Models.DTO;

namespace DeckLens.API.Services.Interface
{
    public interface IScryfallService
    {
        Task<CardDto?> GetCardByNameAsync(string name);
    }
}

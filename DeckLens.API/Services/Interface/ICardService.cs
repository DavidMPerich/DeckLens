using DeckLens.API.Models.DTO;

namespace DeckLens.API.Services.Interface
{
    public interface ICardService
    {
        Task<List<CardDto>> GetDeckMetrics(string deck);
        Task<CardDto?> GetByNameAsync(string name);
    }
}

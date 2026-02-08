using DeckLens.API.Models.DTO;

namespace DeckLens.API.Services.Interface
{
    public interface ICardService
    {
        Task<CardDto?> GetByNameAsync(string name);
    }
}

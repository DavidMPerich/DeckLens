using DeckLens.API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DeckLens.API.Services.Interface
{
    public interface IDeckAnalysisService
    {
        Task<DeckAnalysisDto> AnalyzeAsync(List<string> cardNames);
        Task<List<CardDto>> VerifyAsync(List<string> cardNames);
    }
}

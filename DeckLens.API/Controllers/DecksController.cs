using DeckLens.API.Models.DTO;
using DeckLens.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeckLens.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecksController : ControllerBase
    {
        private readonly IDeckAnalysisService _deckAnalysisService;
        private readonly IDeckImportService _deckImportService;
        private readonly IWebHostEnvironment _env;  //Remove after testing

        public DecksController(IDeckAnalysisService deckAnalysisService, IDeckImportService deckImportService, IWebHostEnvironment env)
        {
            _deckAnalysisService = deckAnalysisService;
            _deckImportService = deckImportService;
            _env = env;
        }

        [HttpPost("analyze")]
        public async Task<ActionResult<DeckAnalysisDto>> AnalyzeDeck([FromBody] DeckImportRequestDto request)
        {
            //Test Data
            var path = Path.Combine(_env.ContentRootPath, "Data", "deck.txt");
            var text = await System.IO.File.ReadAllTextAsync(path);
            request.CardNames = text;

            var cardNames = _deckImportService.Parse(request.CardNames);

            var response = await _deckAnalysisService.AnalyzeAsync(cardNames);
            return Ok(response);
        }

        [HttpGet("verify")]
        public async Task<ActionResult<List<CardDto>>> VerifyDeck()
        {
            var path = Path.Combine(_env.ContentRootPath, "Data", "deck.txt");
            var text = await System.IO.File.ReadAllTextAsync(path);

            var cardNames = _deckImportService.Parse(text);

            var response = await _deckAnalysisService.VerifyAsync(cardNames);
            return Ok(response);
        }
    }
}

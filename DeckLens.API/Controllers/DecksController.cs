using DeckLens.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeckLens.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecksController : ControllerBase
    {
        private readonly ICardService _cardService;

        public DecksController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDeckMetrics(string deck)
        {
            var response = await _cardService.GetDeckMetrics(deck);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}

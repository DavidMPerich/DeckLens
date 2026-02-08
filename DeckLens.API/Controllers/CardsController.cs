using DeckLens.API.Services.Implementation;
using DeckLens.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeckLens.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardsController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByName(string name)
        {
            var response = await _cardService.GetByNameAsync(name);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}

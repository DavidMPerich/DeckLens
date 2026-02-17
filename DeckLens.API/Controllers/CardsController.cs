using DeckLens.API.Models.DTO;
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
        private readonly IScryfallService _cardService;

        public CardsController(IScryfallService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<ActionResult<CardDto>> GetByName(string name)
        {
            var response = await _cardService.GetCardByNameAsync(name);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}

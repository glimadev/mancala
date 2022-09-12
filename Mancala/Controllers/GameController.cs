using Mancala.Config;
using Mancala.Domains.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mancala.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        readonly IGameService _gameService;
        readonly IOptions<MancalaOptions> _mancalaOptions;

        public GameController(IGameService gameService, IOptions<MancalaOptions> mancalaOptions)
        {
            _gameService = gameService;
            _mancalaOptions = mancalaOptions;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return new OkObjectResult(await _gameService.GetState());
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
           var game = await _gameService.Setup(_mancalaOptions.Value.Rocks, _mancalaOptions.Value.Pits);

           return new CreatedResult("Get", game);
        }

        [HttpPut("{pit}")]
        public async Task<IActionResult> Put(int pit)
        {
            await _gameService.Move(pit);

            return NoContent();
        }
    }
}
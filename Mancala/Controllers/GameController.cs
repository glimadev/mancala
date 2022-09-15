using Mancala.Config;
using Mancala.Domains.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mancala.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    /// <summary>
    /// Game service
    /// </summary>
    readonly IGameService _gameService;

    /// <summary>
    /// Game options
    /// </summary>
    readonly IOptions<MancalaOptions> _mancalaOptions;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="gameService"></param>
    /// <param name="mancalaOptions"></param>
    public GameController(IGameService gameService, IOptions<MancalaOptions> mancalaOptions)
    {
        _gameService = gameService;
        _mancalaOptions = mancalaOptions;
    }

    /// <summary>
    /// Get game state
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Get()
        => new OkObjectResult(await _gameService.GetState());

    /// <summary>
    /// Create new game
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post()
        => new CreatedResult("Get", await _gameService.Setup(_mancalaOptions.Value.Rocks, _mancalaOptions.Value.Pits));

    /// <summary>
    /// Move rocks from pointed pit
    /// </summary>
    /// <param name="pit"></param>
    /// <returns></returns>
    [HttpPut("{pit}")]
    public async Task<IActionResult> Put(int pit)
    {
        await _gameService.Move(pit);

        return NoContent();
    }
}
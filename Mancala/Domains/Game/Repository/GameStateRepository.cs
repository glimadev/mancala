using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Mancala.Domains.Game.Repository;

public class GameStateRepository : IGameStateRepository
{
    /// <summary>
    /// Cache interface
    /// </summary>
    readonly IDistributedCache _cache;

    /// <summary>
    /// Key constant
    /// </summary>
    const string GameStateKey = "GameState";

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="cache">Cache implementation</param>
    public GameStateRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Get game state from requested id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<GameStateModel> Get(string id)
    {
        var gameState = await _cache.GetStringAsync($"{GameStateKey}_{id}");

        if (gameState == null) return null;

        return JsonSerializer.Deserialize<GameStateModel>(gameState);
    }

    /// <summary>
    /// Saves game state
    /// </summary>
    /// <param name="gameStateModel"></param>
    /// <returns></returns>
    public async Task Save(GameStateModel gameStateModel) => 
        await _cache.SetStringAsync($"{GameStateKey}_{gameStateModel.Id}", gameStateModel.ToString());
}

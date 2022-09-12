using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Mancala.Domains.Game.Repository;

[ExcludeFromCodeCoverage]
public class GameStateRepository : IGameStateRepository
{
    readonly IDistributedCache _cache;
    const string GameStateKey = "GameState";

    public GameStateRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<GameStateModel> Get(string id)
    {
        return JsonSerializer.Deserialize<GameStateModel>(await _cache.GetStringAsync($"{GameStateKey}_{id}"));
    }

    public async Task Save(GameStateModel gameStateModel)
    {
        await _cache.SetStringAsync($"{GameStateKey}_{gameStateModel.Id}", JsonSerializer.Serialize(gameStateModel));
    }
}

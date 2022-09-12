using Mancala.Domains.Game;

namespace Mancala.Domains.Game.Repository;

public interface IGameStateRepository
{
    Task<GameStateModel> Get(string id);
    Task Save(GameStateModel _gameState);
}

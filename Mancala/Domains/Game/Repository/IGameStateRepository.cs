namespace Mancala.Domains.Game.Repository;

public interface IGameStateRepository
{
    /// <summary>
    /// Get game state from requested id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<GameStateModel> Get(string id);

    /// <summary>
    /// Saves game state
    /// </summary>
    /// <param name="gameStateModel"></param>
    /// <returns></returns>
    Task Save(GameStateModel _gameState);
}

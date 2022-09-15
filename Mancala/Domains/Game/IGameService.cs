namespace Mancala.Domains.Game;

/// <summary>
/// Game service that will provide the intelligent and state managment
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Setup a new game
    /// </summary>
    /// <param name="rocks">number of rocks</param>
    /// <param name="pits">number of pits</param>
    /// <returns>The new game state model</returns>
    Task<GameStateModel> Setup(int rocks, int pits);

    /// <summary>
    /// Get game state for the current user
    /// </summary>
    /// <returns>Currently game state</returns>
    Task<GameStateModel> GetState();

    /// <summary>
    /// Move rocks from the pointed pit 
    /// </summary>
    /// <param name="pitPos">pit position</param>
    /// <returns>void</returns>
    Task Move(int pitPos);
}
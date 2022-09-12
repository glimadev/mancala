namespace Mancala.Domains.Game;
public interface IGameService
{
    Task<GameStateModel> Setup(int rocks, int pits);
    Task<GameStateModel> GetState();
    Task Move(int pitPos);
}
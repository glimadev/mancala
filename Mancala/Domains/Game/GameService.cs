using Mancala.Domains.Game.Repository;
using Mancala.Domains.Game.Models;

namespace Mancala.Domains.Game;

public class GameService : IGameService
{
    private readonly IGameStateRepository _gameStateRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private GameStateModel gameState;

    public GameService(IGameStateRepository gameStateRepository, IHttpContextAccessor httpContextAccessor)
    {
        _gameStateRepository = gameStateRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<GameStateModel> GetState()
    {
        return await _gameStateRepository.Get(GetCurrentUserIp());
    }

    public async Task<GameStateModel> Setup(int rocks, int pits)
    {
        if (rocks == 0)
        {
            throw new InvalidOperationException("Rocks cannot be 0");
        }

        if (pits == 0)
        {
            throw new InvalidOperationException("Pits cannot be 0");
        }

        gameState = new GameStateModel
        {
            Pits = new GameStatePits[(pits * 2) + 2],
            CurrentPlayer = Player.Player1
        };

        for (int i = 0; i < gameState.Pits.Length; i++)
        {
            gameState.Pits[i] = new GameStatePits
            {
                Player = ((decimal)i / (gameState.Pits.Length / 2)) < 1m ? Player.Player1 : Player.Player2,
                Rocks = rocks,
            };

            if (gameState.Pits.Length / 2 == i + 1)
            {
                gameState.Pits[i].Rocks = 0;
                gameState.Pits[i].IsBigPit = true;
            }
            else if (gameState.Pits.Length - 1 == i)
            {
                gameState.Pits[i].Rocks = 0;
                gameState.Pits[i].IsBigPit = true;
            }
        }

        gameState.Id = GetCurrentUserIp();

        await _gameStateRepository.Save(gameState);

        return gameState;
    }

    public async Task Move(int pitPos)
    {
        gameState = await GetState();

        CheckInput(pitPos);

        var (currentPit, lastPos) = MoveRocks(pitPos);

        ClaimOppositePit(currentPit, lastPos);

        if (IsGameOver())
        {
            var (totalPlayer1, totalPlayer2) = SumRemainingRocks();

            CheckWinner(totalPlayer1, totalPlayer2);

            gameState.GameOver = true;
        }
        else
        {
            NextPlayer(currentPit);
        }

        await _gameStateRepository.Save(gameState);
    }

    #region .: Private Methods :.

    private void CheckInput(int pitPos)
    {
        if (gameState.GameOver)
        {
            throw new InvalidOperationException("Game is over already!");
        }

        if (gameState.Pits[pitPos].IsBigPit)
        {
            throw new InvalidOperationException("Cannot take from big pit!");
        }

        if (gameState.Pits[pitPos].Player != gameState.CurrentPlayer)
        {
            throw new InvalidOperationException("Cannot move opponents rocks!");
        }

        if (gameState.Pits[pitPos].Rocks <= 0)
        {
            throw new InvalidOperationException("This pit has no rocks!");
        }
    }

    private (GameStatePits currentPit, int pitPos) MoveRocks(int pitPos)
    {
        var currentPit = gameState.Pits[pitPos];

        int rocks = currentPit.Rocks;

        currentPit.Rocks = 0;

        for (int i = 1; i <= rocks; i++)
        {
            pitPos = ++pitPos % gameState.Pits.Length;

            currentPit = gameState.Pits[pitPos];

            if (currentPit.IsBigPit && currentPit.Player != gameState.CurrentPlayer)
            {
                pitPos = ++pitPos % gameState.Pits.Length;
                currentPit = gameState.Pits[pitPos];
            }

            currentPit.Rocks++;
        }

        return (currentPit, pitPos);
    }

    private void ClaimOppositePit(GameStatePits currentPit, int pitPos)
    {
        if (currentPit.Rocks == 1 && currentPit.Player == gameState.CurrentPlayer && !currentPit.IsBigPit)
        {
            var midPit = gameState.Pits.Where(x => !x.IsBigPit).Count() / 2;

            var oppositePit = gameState.Pits[midPit + (midPit - pitPos)];

            var bigPit = gameState.Pits.First(x => x.Player == gameState.CurrentPlayer && x.IsBigPit);

            bigPit.Rocks += currentPit.Rocks + oppositePit.Rocks;

            currentPit.Rocks = oppositePit.Rocks = 0;
        }
    }

    private void CheckWinner(int totalPlayer1, int totalPlayer2)
    {
        if (totalPlayer1 > totalPlayer2)
            gameState.Winner = Player.Player1;
        else if (totalPlayer1 < totalPlayer2)
            gameState.Winner = Player.Player2;
    }

    private (int totalPlayer1, int totalPlayer2) SumRemainingRocks()
    {
        var bigPitPlayer1 = gameState.Pits.First(x => x.Player == Player.Player1 && x.IsBigPit);
        var bigPitPlayer2 = gameState.Pits.First(x => x.Player == Player.Player2 && x.IsBigPit);

        foreach (var pit in gameState.Pits.Where(x => !x.IsBigPit))
        {
            if (Player.Player1 == pit.Player)
                bigPitPlayer1.Rocks += pit.Rocks;
            else
                bigPitPlayer2.Rocks += pit.Rocks;

            pit.Rocks = 0;
        }

        return (bigPitPlayer1.Rocks, bigPitPlayer2.Rocks);
    }

    private bool IsGameOver() =>
           !gameState.Pits.Any(pit => pit.Player == Player.Player1 && pit.IsBigPit == false && pit.Rocks > 0)
        || !gameState.Pits.Any(pit => pit.Player == Player.Player2 && pit.IsBigPit == false && pit.Rocks > 0);


    private void NextPlayer(GameStatePits currentPit)
    {
        if (!(currentPit.IsBigPit && currentPit.Player == gameState.CurrentPlayer))
        {
            gameState.CurrentPlayer = gameState.CurrentPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
        }
    }

    private string GetCurrentUserIp() => _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

    #endregion
}
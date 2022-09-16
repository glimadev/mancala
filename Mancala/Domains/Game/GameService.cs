using Mancala.Domains.Game.Repository;
using Mancala.Domains.Game.Models;
using Mancala.Extensions;

namespace Mancala.Domains.Game;

/// <summary>
/// Game service that will provide the intelligent and state managment
/// </summary>
public class GameService : IGameService
{
    /// <summary>
    /// Game state repository
    /// </summary>
    readonly IGameStateRepository _gameStateRepository;

    /// <summary>
    /// HttpContextAccessor of the request
    /// </summary>
    readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Game state model
    /// </summary>
    GameStateModel gameState;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="gameStateRepository"></param>
    /// <param name="httpContextAccessor"></param>
    public GameService(IGameStateRepository gameStateRepository, IHttpContextAccessor httpContextAccessor)
    {
        _gameStateRepository = gameStateRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Get game state for the current user
    /// </summary>
    /// <returns>Currently game state</returns>
    public async Task<GameStateModel> GetState() => await _gameStateRepository.Get(_httpContextAccessor.GetIp());

    /// <summary>
    /// Setup a new game
    /// </summary>
    /// <param name="rocks">number of rocks</param>
    /// <param name="pits">number of pits</param>
    /// <returns>The new game state model</returns>
    public async Task<GameStateModel> Setup(int rocks, int pits)
    {
        //Validate arguments
        if (rocks == 0) throw new InvalidOperationException("Rocks cannot be 0");
        else if (pits == 0) throw new InvalidOperationException("Pits cannot be 0");

        //Initialize a game state model
        gameState = new GameStateModel
        {
            Pits = new GameStatePits[(pits * 2) + 2],
            CurrentPlayer = Player.Player1
        };

        //Populate it with the desired pits for each player
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

        gameState.Id = _httpContextAccessor.GetIp();

        //Save this new game
        await _gameStateRepository.Save(gameState);

        //Return created object
        return gameState;
    }

    /// <summary>
    /// Move rocks from the pointed pit 
    /// </summary>
    /// <param name="pitPos">pit position</param>
    /// <returns>void</returns>
    public async Task Move(int pitPos)
    {
        gameState = await GetState();

        //Validate argument
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

    /// <summary>
    /// Validate argument pitPos
    /// </summary>
    /// <param name="pitPos"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void CheckInput(int pitPos)
    {
        if (gameState.GameOver) throw new InvalidOperationException("Game is over already!");
        else if (gameState.Pits[pitPos].IsBigPit) throw new InvalidOperationException("Cannot take from big pit!");
        else if (gameState.Pits[pitPos].Player != gameState.CurrentPlayer) throw new InvalidOperationException("Cannot move opponents rocks!");
        else if (gameState.Pits[pitPos].Rocks <= 0) throw new InvalidOperationException("This pit has no rocks!");
    }

    /// <summary>
    /// Move the rocks from the pointed pit
    /// </summary>
    /// <param name="pitPos"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Claims opposite pit 
    /// </summary>
    /// <param name="currentPit"></param>
    /// <param name="pitPos"></param>
    private void ClaimOppositePit(GameStatePits currentPit, int pitPos)
    {
        //If currently stoped pit has only one rock, the pit isnt a big one and the current player did the movement
        //Then the opposite pit should be claimed
        if (currentPit.Rocks == 1 && currentPit.Player == gameState.CurrentPlayer && !currentPit.IsBigPit)
        {
            var midPit = gameState.Pits.Count(x => !x.IsBigPit) / 2;

            var oppositePit = gameState.Pits[midPit + (midPit - pitPos)];

            var bigPit = gameState.Pits.First(x => x.Player == gameState.CurrentPlayer && x.IsBigPit);

            bigPit.Rocks += currentPit.Rocks + oppositePit.Rocks;

            currentPit.Rocks = oppositePit.Rocks = 0;
        }
    }

    /// <summary>
    /// Sum the remaining rock in each side
    /// </summary>
    /// <returns>The amount from each player</returns>
    private (int totalPlayer1, int totalPlayer2) SumRemainingRocks()
    {
        var bigPitPlayer1 = gameState.Pits.First(x => x.Player == Player.Player1 && x.IsBigPit);
        var bigPitPlayer2 = gameState.Pits.First(x => x.Player == Player.Player2 && x.IsBigPit);

        foreach (var pit in gameState.Pits.Where(x => !x.IsBigPit))
        {
            if (Player.Player1 == pit.Player) bigPitPlayer1.Rocks += pit.Rocks;
            else bigPitPlayer2.Rocks += pit.Rocks;

            pit.Rocks = 0;
        }

        return (bigPitPlayer1.Rocks, bigPitPlayer2.Rocks);
    }

    /// <summary>
    /// Checks which player was the most higher score
    /// </summary>
    /// <param name="totalPlayer1">Amount of rocks from the player1</param>
    /// <param name="totalPlayer2">Amount of rocks from the player2</param>
    private void CheckWinner(int totalPlayer1, int totalPlayer2)
    {
        if (totalPlayer1 > totalPlayer2) gameState.Winner = Player.Player1;
        else if (totalPlayer1 < totalPlayer2) gameState.Winner = Player.Player2;
    }

    /// <summary>
    /// Check if is game over
    /// </summary>
    /// <returns></returns>
    private bool IsGameOver() =>
           !gameState.Pits.Any(pit => pit.Player == Player.Player1 && pit.IsBigPit == false && pit.Rocks > 0)
        || !gameState.Pits.Any(pit => pit.Player == Player.Player2 && pit.IsBigPit == false && pit.Rocks > 0);

    /// <summary>
    /// Check whos next
    /// </summary>
    /// <param name="currentPit"></param>
    private void NextPlayer(GameStatePits currentPit)
    {
        //Check if the last moviment didnt fell in the big pit from the current player
        if (!(currentPit.IsBigPit && currentPit.Player == gameState.CurrentPlayer))
            gameState.CurrentPlayer = gameState.CurrentPlayer is Player.Player1 ? Player.Player2 : Player.Player1;
    }

    #endregion
}
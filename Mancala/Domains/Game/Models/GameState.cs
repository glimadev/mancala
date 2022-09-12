using Mancala.Domains.Game.Models;
using System.Text.Json;

namespace Mancala.Domains.Game;

public class GameStateModel
{
    public string Id { get; set; }
    public GameStatePits[] Pits { get; set; }
    public Player CurrentPlayer { get; set; }
    public Player? Winner { get; set; }
    public bool GameOver { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this);
}
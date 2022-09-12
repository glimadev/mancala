namespace Mancala.Domains.Game.Models;
public class GameStatePits
{
    public int Rocks { get; set; }
    public Player Player { get; set; }
    public bool IsBigPit { get; set; }
}
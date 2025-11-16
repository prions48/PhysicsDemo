using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhysicsDemo.Data.GameData
{
    public class PhysicsGame
    {
        [Key] public Guid ID { get; set; }
        public Guid CreatorID { get; set; }
        public string JoinCode { get; set; }
        public DateTime GameStart { get; set; }
        public int NumPlayers { get; set; }
        public int CurrentRound { get; set; }
        public DateTime? GameEnd { get; set; }
        public Guid? WinningPlayerID { get; set; }
        public string? WinningPlayerName { get; set; }
        //Yes I know I should use navigation properties, I'm having a busy weekend
        [NotMapped] public List<PhysicsPlayer> Players { get; set; } = [];
        public List<PhysicsTurn> Turns => Players.SelectMany(e => e.Turns).ToList();
        public List<PhysicsTurn> CurrentTurns => Players.Where(e => e.CurrentTurn != null).Select(e => e.CurrentTurn!).ToList();

    }
}
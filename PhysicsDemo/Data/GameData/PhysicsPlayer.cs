using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhysicsDemo.Data.GameData
{
    public class PhysicsPlayer
    {
        [Key] public Guid ID { get; set; } = Guid.NewGuid();
        public Guid GameID { get; set; }
        public Guid PlayerID { get; set; }
        public string? PlayerName { get; set; }
        public bool GuestPlayer { get; set; }
        [NotMapped] public List<PhysicsTurn> Turns { get; set; } = [];
        public PhysicsTurn? CurrentTurn => Turns.MaxBy(e => e.RoundNumber);
    }
}
using System.ComponentModel.DataAnnotations;

namespace PhysicsDemo.Data.GameData
{
    public class PhysicsTurn
    {
        [Key] public Guid ID { get; set; } = Guid.NewGuid();
        public Guid GameID { get; set; }
        public Guid PlayerID { get; set; }
        public int RoundNumber { get; set; }
        public int TurnValue { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
    }
}
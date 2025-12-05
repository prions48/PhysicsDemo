using System.ComponentModel.DataAnnotations;

namespace PhysicsDemo.Data.GameData
{
    public class PhysicsDirection
    {
        [Key] public Guid ID { get; set; } = Guid.NewGuid();
        public Guid GameID { get; set; }
        public Guid PlayerID { get; set; }
        public int TurnValue { get; set; }
    }
}
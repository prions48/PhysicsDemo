using Microsoft.EntityFrameworkCore;

namespace PhysicsDemo.Data.GameData
{
    public class PhysicsContext : DbContext
    {
        public PhysicsContext(DbContextOptions<PhysicsContext> options) : base(options)
        {

        }
        public DbSet<PhysicsGame> PhysicsGames { get; set; }
        public DbSet<PhysicsPlayer> PhysicsPlayers { get; set; }
        public DbSet<PhysicsTurn> PhysicsTurns { get; set; }
    }
}
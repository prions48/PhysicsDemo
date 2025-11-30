using System.ComponentModel.DataAnnotations;
using PhysicsDemo.Data.Users;

namespace PhysicsDemo.Data.GameData
{
    public class PhysicsGuestUser : IUser
    {
        [Key] public Guid UserID { get; set; } = Guid.NewGuid();
        public string PlayerName { get; set; }
        public string Passcode { get; set; }
        public DateTime FirstLogin { get; set; }
        public bool ClosedOut { get; set; }
        public PhysicsGuestUser()
        {
            FirstLogin = DateTime.Now;
            ClosedOut = false;
        }
    }
}
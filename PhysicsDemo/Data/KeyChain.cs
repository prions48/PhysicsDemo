namespace PhysicsDemo.Data
{
    /// <summary>
    /// In-app settings not needed to be abstracted to user secrets
    /// </summary>
    public static class KeyChain
    {
        public static string AppName
        {
            get
            {
                return "Physics Demo";
            }
        }
        public static string AppCode
        {
            get
            {
                return "Physics";
            }
        }
        public static string Container
        {
            get
            {
                return "physicsfilestorage";
            }
        }
        public static string OwnerEmail
        {
            get
            {
                return "prions48@gmail.com";
            }
        }
    }
    public enum Environ
    {
        Physics
    }

}
namespace FurnitureMovement.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int FailedAttempts { get; set; }
        public bool IsLocked { get; set; }
    }
}

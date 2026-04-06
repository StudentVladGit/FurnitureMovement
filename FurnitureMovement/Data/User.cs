using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureMovement.Data
{
    public enum UserRole
    {
        //[Display(Name = "Пользователь")]
        User = 0,
        //[Display(Name = "Менеджер")]
        Manager = 1,
        //[Display(Name = "Администратор")]
        Admin = 2
    }
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int FailedAttempts { get; set; }
        public bool IsLocked { get; set; }
        public UserRole Role { get; set; } = UserRole.User; // По умолчанию - Пользователь

        // Added for one-to-one relationship with OrderAuthor
        public int? OrderAuthorId { get; set; } // Foreign key (nullable if no author yet)
        [ForeignKey("OrderAuthorId")]
        public virtual OrderAuthor OrderAuthor { get; set; } // Navigation property
    }
}

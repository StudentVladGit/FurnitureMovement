using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureMovement.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int FailedAttempts { get; set; }
        public bool IsLocked { get; set; }

        // Added for one-to-one relationship with OrderAuthor
        public int? OrderAuthorId { get; set; } // Foreign key (nullable if no author yet)
        [ForeignKey("OrderAuthorId")]
        public virtual OrderAuthor OrderAuthor { get; set; } // Navigation property
    }
}

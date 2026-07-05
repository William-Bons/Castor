using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Castor.database.tables
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Login { get; set; } = string.Empty;

        // Только хеш (BCrypt), без отдельной соли
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Role { get; set; } = "Врач"; // Врач / Медсестра / Администратор

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Опционально: если понадобится отслеживание последнего входа
        // public DateTime? LastLoginAt { get; set; }
    }
}

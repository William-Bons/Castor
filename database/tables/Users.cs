using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace Castor.database.tables
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Login { get; set; } = string.Empty;
        public long DocdepId { get; set; }

        // Только хеш (BCrypt), без отдельной соли
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Role { get; set; } = "Врач"; // Врач / Медсестра / Администратор

        public bool IsActive { get; set; } = true;
        public string? Color { get; set; } = Brushes.Black.ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Опционально: если понадобится отслеживание последнего входа
        // public DateTime? LastLoginAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace RumbaAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public ICollection<Rumba> Rumbas { get; set; } = new List<Rumba>();
        public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    }
}

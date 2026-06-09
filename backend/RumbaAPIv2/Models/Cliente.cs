using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RumbaAPI.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Instagram { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        public string Notas { get; set; } = string.Empty;

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        // FK Usuario
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // Navegación — rumbas asociadas a este cliente
        public ICollection<Rumba> Rumbas { get; set; } = new List<Rumba>();
    }
}

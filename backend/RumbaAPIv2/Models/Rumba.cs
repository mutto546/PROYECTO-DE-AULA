using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RumbaAPI.Models
{
    public class Rumba
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Tipo { get; set; } = "Producción";

        [Required, MaxLength(50)]
        public string Estado { get; set; } = "Pendiente";

        public bool Pagado { get; set; } = false;

        [MaxLength(50)]
        public string MontoPago { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FechaEntrega { get; set; } = string.Empty;

        public string Notas { get; set; } = string.Empty;

        public bool Urgente { get; set; } = false;

        [MaxLength(100)]
        public string Ep { get; set; } = string.Empty;

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;

        // FK Usuario
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // FK Cliente (opcional)
        public int? ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }
    }
}

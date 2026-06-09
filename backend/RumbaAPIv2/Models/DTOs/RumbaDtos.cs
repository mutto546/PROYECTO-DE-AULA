using System.ComponentModel.DataAnnotations;

namespace RumbaAPI.Models.DTOs
{
    // --- CREAR / EDITAR RUMBA ---
    public class RumbaUpsertDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Tipo { get; set; } = "Producción";

        [MaxLength(50)]
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

        // Id del cliente asociado (opcional)
        public int? ClienteId { get; set; }
    }

    // --- LO QUE SE DEVUELVE AL FRONTEND ---
    public class RumbaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool Pagado { get; set; }
        public string MontoPago { get; set; } = string.Empty;
        public string FechaEntrega { get; set; } = string.Empty;
        public string Notas { get; set; } = string.Empty;
        public bool Urgente { get; set; }
        public string Ep { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; }
        public DateTime ActualizadoEn { get; set; }

        // Cliente resumido (solo id y nombre)
        public ClienteResumenDto? Cliente { get; set; }
    }

    // --- ELIMINAR VARIOS ---
    public class EliminarVariosDto
    {
        [Required]
        public List<int> Ids { get; set; } = new();
    }

    // --- MÉTRICAS DEL DASHBOARD ---
    public class MetricasDto
    {
        public int Activas { get; set; }
        public int Completadas { get; set; }
        public int Urgentes { get; set; }
        public decimal IngresosMes { get; set; }
    }
}

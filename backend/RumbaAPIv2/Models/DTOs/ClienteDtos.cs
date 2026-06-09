using System.ComponentModel.DataAnnotations;

namespace RumbaAPI.Models.DTOs
{
    // --- CREAR / EDITAR CLIENTE ---
    public class ClienteUpsertDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Instagram { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        public string Notas { get; set; } = string.Empty;
    }

    // --- LO QUE SE DEVUELVE AL FRONTEND ---
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Instagram { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Notas { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; }

        // Cuántas rumbas tiene asociadas
        public int TotalRumbas { get; set; }
    }

    // --- RESUMEN para incluir dentro de RumbaDto ---
    public class ClienteResumenDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}

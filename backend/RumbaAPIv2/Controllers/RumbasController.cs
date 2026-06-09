using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RumbaAPI.Data;
using RumbaAPI.Models;
using RumbaAPI.Models.DTOs;
using System.Security.Claims;

namespace RumbaAPI.Controllers
{
    [ApiController]
    [Route("api/rumbas")]
    [Authorize]  // Todos los endpoints requieren JWT válido
    public class RumbasController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RumbasController(AppDbContext db)
        {
            _db = db;
        }

        // Extraer userId del token JWT
        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // GET /api/rumbas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var rumbas = await _db.Rumbas
                .Where(r => r.UserId == userId)
                .Include(r => r.Cliente)
                .OrderByDescending(r => r.CreadoEn)
                .Select(r => MapDto(r))
                .ToListAsync();

            return Ok(rumbas);
        }

        // GET /api/rumbas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            var rumba  = await _db.Rumbas
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (rumba == null) return NotFound(new { message = "Rumba no encontrada" });

            return Ok(MapDto(rumba));
        }

        // POST /api/rumbas
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RumbaUpsertDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();

            // Validar que el cliente pertenece al mismo usuario (si se envió)
            if (dto.ClienteId.HasValue)
            {
                bool clienteValido = await _db.Clientes
                    .AnyAsync(c => c.Id == dto.ClienteId && c.UserId == userId);
                if (!clienteValido)
                    return BadRequest(new { message = "Cliente no válido" });
            }

            var rumba = new Rumba
            {
                Nombre       = dto.Nombre,
                Tipo         = dto.Tipo,
                Estado       = dto.Estado,
                Pagado       = dto.Pagado,
                MontoPago    = dto.MontoPago,
                FechaEntrega = dto.FechaEntrega,
                Notas        = dto.Notas,
                Urgente      = dto.Urgente,
                Ep           = dto.Ep,
                ClienteId    = dto.ClienteId,
                UserId       = userId,
                CreadoEn     = DateTime.UtcNow,
                ActualizadoEn = DateTime.UtcNow
            };

            _db.Rumbas.Add(rumba);
            await _db.SaveChangesAsync();

            // Recargar con cliente incluido para devolver el DTO completo
            await _db.Entry(rumba).Reference(r => r.Cliente).LoadAsync();

            return CreatedAtAction(nameof(GetById), new { id = rumba.Id }, MapDto(rumba));
        }

        // PUT /api/rumbas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RumbaUpsertDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var rumba  = await _db.Rumbas
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (rumba == null) return NotFound(new { message = "Rumba no encontrada" });

            if (dto.ClienteId.HasValue)
            {
                bool clienteValido = await _db.Clientes
                    .AnyAsync(c => c.Id == dto.ClienteId && c.UserId == userId);
                if (!clienteValido)
                    return BadRequest(new { message = "Cliente no válido" });
            }

            rumba.Nombre        = dto.Nombre;
            rumba.Tipo          = dto.Tipo;
            rumba.Estado        = dto.Estado;
            rumba.Pagado        = dto.Pagado;
            rumba.MontoPago     = dto.MontoPago;
            rumba.FechaEntrega  = dto.FechaEntrega;
            rumba.Notas         = dto.Notas;
            rumba.Urgente       = dto.Urgente;
            rumba.Ep            = dto.Ep;
            rumba.ClienteId     = dto.ClienteId;
            rumba.ActualizadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            await _db.Entry(rumba).Reference(r => r.Cliente).LoadAsync();

            return Ok(MapDto(rumba));
        }

        // PATCH /api/rumbas/{id}/completar
        [HttpPatch("{id}/completar")]
        public async Task<IActionResult> Completar(int id)
        {
            var userId = GetUserId();
            var rumba  = await _db.Rumbas
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (rumba == null) return NotFound(new { message = "Rumba no encontrada" });

            rumba.Estado        = "Completado";
            rumba.ActualizadoEn = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Marcada como completada", id = rumba.Id });
        }

        // DELETE /api/rumbas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var rumba  = await _db.Rumbas
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (rumba == null) return NotFound(new { message = "Rumba no encontrada" });

            _db.Rumbas.Remove(rumba);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Rumba eliminada", id });
        }

        // DELETE /api/rumbas  (body: { "ids": [1, 2, 3] })
        [HttpDelete]
        public async Task<IActionResult> DeleteVarios([FromBody] EliminarVariosDto dto)
        {
            if (!dto.Ids.Any())
                return BadRequest(new { message = "Se requiere al menos un id" });

            var userId = GetUserId();
            var rumbas = await _db.Rumbas
                .Where(r => dto.Ids.Contains(r.Id) && r.UserId == userId)
                .ToListAsync();

            _db.Rumbas.RemoveRange(rumbas);
            await _db.SaveChangesAsync();

            return Ok(new { message = $"{rumbas.Count} rumbas eliminadas", eliminadas = rumbas.Count });
        }

        // GET /api/rumbas/metricas
        [HttpGet("metricas")]
        public async Task<IActionResult> Metricas()
        {
            var userId = GetUserId();
            var rumbas = await _db.Rumbas
                .Where(r => r.UserId == userId)
                .ToListAsync();

            // Sumar solo las que están pagadas y tienen monto
            decimal ingresosMes = rumbas
                .Where(r => r.Pagado && !string.IsNullOrEmpty(r.MontoPago))
                .Sum(r =>
                {
                    var soloNumeros = new string(r.MontoPago.Where(char.IsDigit).ToArray());
                    return decimal.TryParse(soloNumeros, out var n) ? n : 0;
                });

            return Ok(new MetricasDto
            {
                Activas     = rumbas.Count(r => r.Estado == "Pendiente" || r.Estado == "En progreso"),
                Completadas = rumbas.Count(r => r.Estado == "Completado"),
                Urgentes    = rumbas.Count(r => r.Urgente),
                IngresosMes = ingresosMes
            });
        }

        // --- Mapeo Rumba → RumbaDto ---
        private static RumbaDto MapDto(Rumba r) => new()
        {
            Id           = r.Id,
            Nombre       = r.Nombre,
            Tipo         = r.Tipo,
            Estado       = r.Estado,
            Pagado       = r.Pagado,
            MontoPago    = r.MontoPago,
            FechaEntrega = r.FechaEntrega,
            Notas        = r.Notas,
            Urgente      = r.Urgente,
            Ep           = r.Ep,
            CreadoEn     = r.CreadoEn,
            ActualizadoEn = r.ActualizadoEn,
            Cliente      = r.Cliente == null ? null : new ClienteResumenDto
            {
                Id     = r.Cliente.Id,
                Nombre = r.Cliente.Nombre
            }
        };
    }
}

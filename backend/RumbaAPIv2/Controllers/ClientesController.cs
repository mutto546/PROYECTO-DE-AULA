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
    [Route("api/clientes")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ClientesController(AppDbContext db)
        {
            _db = db;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // GET /api/clientes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId   = GetUserId();
            var clientes = await _db.Clientes
                .Where(c => c.UserId == userId)
                .Include(c => c.Rumbas)
                .OrderByDescending(c => c.CreadoEn)
                .Select(c => new ClienteDto
                {
                    Id          = c.Id,
                    Nombre      = c.Nombre,
                    Instagram   = c.Instagram,
                    Telefono    = c.Telefono,
                    Notas       = c.Notas,
                    CreadoEn    = c.CreadoEn,
                    TotalRumbas = c.Rumbas.Count
                })
                .ToListAsync();

            return Ok(clientes);
        }

        // GET /api/clientes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId  = GetUserId();
            var cliente = await _db.Clientes
                .Include(c => c.Rumbas)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (cliente == null) return NotFound(new { message = "Cliente no encontrado" });

            return Ok(new ClienteDto
            {
                Id          = cliente.Id,
                Nombre      = cliente.Nombre,
                Instagram   = cliente.Instagram,
                Telefono    = cliente.Telefono,
                Notas       = cliente.Notas,
                CreadoEn    = cliente.CreadoEn,
                TotalRumbas = cliente.Rumbas.Count
            });
        }

        // POST /api/clientes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteUpsertDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cliente = new Cliente
            {
                Nombre    = dto.Nombre,
                Instagram = dto.Instagram,
                Telefono  = dto.Telefono,
                Notas     = dto.Notas,
                UserId    = GetUserId(),
                CreadoEn  = DateTime.UtcNow
            };

            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, new ClienteDto
            {
                Id          = cliente.Id,
                Nombre      = cliente.Nombre,
                Instagram   = cliente.Instagram,
                Telefono    = cliente.Telefono,
                Notas       = cliente.Notas,
                CreadoEn    = cliente.CreadoEn,
                TotalRumbas = 0
            });
        }

        // PUT /api/clientes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClienteUpsertDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId  = GetUserId();
            var cliente = await _db.Clientes
                .Include(c => c.Rumbas)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (cliente == null) return NotFound(new { message = "Cliente no encontrado" });

            cliente.Nombre    = dto.Nombre;
            cliente.Instagram = dto.Instagram;
            cliente.Telefono  = dto.Telefono;
            cliente.Notas     = dto.Notas;

            await _db.SaveChangesAsync();

            return Ok(new ClienteDto
            {
                Id          = cliente.Id,
                Nombre      = cliente.Nombre,
                Instagram   = cliente.Instagram,
                Telefono    = cliente.Telefono,
                Notas       = cliente.Notas,
                CreadoEn    = cliente.CreadoEn,
                TotalRumbas = cliente.Rumbas.Count
            });
        }

        // DELETE /api/clientes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Nodo 1
            var userId  = GetUserId();
            // Nodo 2
            var cliente = await _db.Clientes
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            // Nodo 3
            if (cliente == null) return NotFound(new { message = "Cliente no encontrado" }); // Nodo 4

            // Nodo 5
            _db.Clientes.Remove(cliente);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Cliente eliminado", id }); // Nodo 6
        }
    }
}

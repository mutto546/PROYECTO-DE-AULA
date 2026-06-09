using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RumbaAPI.Data;
using RumbaAPI.Models;
using RumbaAPI.Models.DTOs;
using RumbaAPI.Services;
using BCrypt.Net;

namespace RumbaAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext db, TokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar username y email únicos
            bool usernameTomado = await _db.Users.AnyAsync(u => u.Username == dto.Username);
            if (usernameTomado)
                return BadRequest(new { message = "El username ya está en uso" });

            bool emailTomado = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailTomado)
                return BadRequest(new { message = "El email ya está registrado" });


            var user = new User
            {
                Name         = dto.Name,
                Username     = dto.Username.ToLower(),
                Email        = dto.Email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _tokenService.GenerarToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                User  = MapUserDto(user)
            });
        }

        // POST /api/auth/login
        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // Nodo 1 — validación del DTO
            if (dto == null || string.IsNullOrEmpty(dto.Email))
                return BadRequest(new { message = "Datos inválidos" }); // Nodo 2

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Nodo 3 — buscar usuario
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

            // Nodo 4
            if (user == null)
                return Unauthorized(new { message = "Usuario no encontrado" }); // Nodo 5

            // Nodo 6 — verificar contraseña
            bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            // Nodo 7
            if (!valid)
                return Unauthorized(new { message = "Contraseña incorrecta" }); // Nodo 8

            // Nodo 9 — generar token y responder
            var token = _tokenService.GenerarToken(user);
            return Ok(new AuthResponseDto
            {
                Token = token,
                User = MapUserDto(user)
            }); // Nodo 10
        }

        // GET /api/auth/me — verifica el token y devuelve el usuario
        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var user   = await _db.Users.FindAsync(userId);

            if (user == null) return NotFound();

            return Ok(MapUserDto(user));
        }

        // --- Mapeo interno ---
        private static UserDto MapUserDto(User u) => new()
        {
            Id        = u.Id,
            Name      = u.Name,
            Username  = u.Username,
            Email     = u.Email,
            CreatedAt = u.CreatedAt
        };
    }
}

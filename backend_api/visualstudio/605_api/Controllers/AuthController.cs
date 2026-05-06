using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using _605_api.Data;
using _605_api.Models;
using _605_api.DTOs;

namespace _605_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(Utilizador utilizador)
    {
        if (await _db.Utilizadores.AnyAsync(u => u.Email == utilizador.Email))
            return BadRequest("Email já registado.");

        utilizador.PasswordHash = BCrypt.Net.BCrypt.HashPassword(utilizador.PasswordHash);
        utilizador.DataCriacao = DateTime.UtcNow;
        utilizador.Role = "user";

        _db.Utilizadores.Add(utilizador);
        await _db.SaveChangesAsync();

        return Ok("Utilizador registado com sucesso.");
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {
        var utilizador = await _db.Utilizadores.FirstOrDefaultAsync(u => u.Email == login.Email);

        if (utilizador == null || !BCrypt.Net.BCrypt.Verify(login.Password, utilizador.PasswordHash))
            return Unauthorized("Email ou password incorretos.");

        var token = GerarToken(utilizador);
        return Ok(new { token });
    }

    private string GerarToken(Utilizador utilizador)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, utilizador.Id.ToString()),
            new Claim(ClaimTypes.Email, utilizador.Email),
            new Claim(ClaimTypes.Role, utilizador.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresInMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
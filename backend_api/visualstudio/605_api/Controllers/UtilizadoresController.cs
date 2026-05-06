using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _605_api.Data;
using _605_api.Models;

namespace _605_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UtilizadoresController : ControllerBase
{
    private readonly AppDbContext _db;

    public UtilizadoresController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/utilizadores
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var utilizadores = await _db.Utilizadores
            .Select(u => new { u.Id, u.Nome, u.Email, u.Role, u.DataCriacao })
            .ToListAsync();
        return Ok(utilizadores);
    }

    // GET api/utilizadores/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var utilizador = await _db.Utilizadores
            .Where(u => u.Id == id)
            .Select(u => new { u.Id, u.Nome, u.Email, u.Role, u.DataCriacao })
            .FirstOrDefaultAsync();

        if (utilizador == null) return NotFound();
        return Ok(utilizador);
    }

    // PUT api/utilizadores/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Utilizador dados)
    {
        var utilizador = await _db.Utilizadores.FindAsync(id);
        if (utilizador == null) return NotFound();

        utilizador.Nome = dados.Nome;
        utilizador.Email = dados.Email;
        utilizador.Role = dados.Role;

        if (!string.IsNullOrEmpty(dados.PasswordHash))
            utilizador.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dados.PasswordHash);

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/utilizadores/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var utilizador = await _db.Utilizadores.FindAsync(id);
        if (utilizador == null) return NotFound();
        _db.Utilizadores.Remove(utilizador);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
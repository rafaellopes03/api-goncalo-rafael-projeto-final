using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _605_api.Data;
using _605_api.Models;

namespace _605_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReservasController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/reservas
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reservas = await _db.Reservas.OrderByDescending(r => r.DataCriacao).ToListAsync();
        return Ok(reservas);
    }

    // GET api/reservas/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var reserva = await _db.Reservas.FindAsync(id);
        if (reserva == null) return NotFound();
        return Ok(reserva);
    }

    // POST api/reservas (usado pelo formulário do frontend)
    [HttpPost]
    public async Task<IActionResult> Create(Reserva reserva)
    {
        reserva.DataCriacao = DateTime.UtcNow;
        reserva.Estado = "pendente";
        _db.Reservas.Add(reserva);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = reserva.Id }, reserva);
    }

    // PUT api/reservas/1/estado
    [HttpPut("{id}/estado")]
    public async Task<IActionResult> UpdateEstado(int id, [FromBody] string estado)
    {
        var reserva = await _db.Reservas.FindAsync(id);
        if (reserva == null) return NotFound();
        reserva.Estado = estado;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/reservas/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var reserva = await _db.Reservas.FindAsync(id);
        if (reserva == null) return NotFound();
        _db.Reservas.Remove(reserva);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
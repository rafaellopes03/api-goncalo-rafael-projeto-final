using _605_api.Data;
using _605_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;

namespace _605_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExperienciasController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "experiencias_todas";

    public ExperienciasController(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    // GET api/experiencias
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (_cache.TryGetValue(CacheKey, out List<Experiencia>? cached))
            return Ok(cached);

        var experiencias = await _db.Experiencias.Where(e => e.Disponivel).ToListAsync();

        _cache.Set(CacheKey, experiencias, TimeSpan.FromMinutes(5));

        return Ok(experiencias);
    }

    // GET api/experiencias/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var experiencia = await _db.Experiencias.FindAsync(id);
        if (experiencia == null) return NotFound();
        return Ok(experiencia);
    }

    // POST api/experiencias
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Experiencia experiencia)
    {
        _db.Experiencias.Add(experiencia);
        await _db.SaveChangesAsync();
        _cache.Remove(CacheKey);
        return CreatedAtAction(nameof(GetById), new { id = experiencia.Id }, experiencia);
    }

    // PUT api/experiencias/1
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Experiencia experiencia)
    {
        if (id != experiencia.Id) return BadRequest();
        _db.Entry(experiencia).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        _cache.Remove(CacheKey);
        return NoContent();
    }

    // DELETE api/experiencias/1
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var experiencia = await _db.Experiencias.FindAsync(id);
        if (experiencia == null) return NotFound();
        _db.Experiencias.Remove(experiencia);
        await _db.SaveChangesAsync();
        _cache.Remove(CacheKey);
        return NoContent();
    }
}
using _605_api.Data;
using _605_api.Models;
using _605_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;

namespace _605_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VinhosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly RedisCacheService _redis;
    private const string CacheKey = "vinhos_todos";

    public VinhosController(AppDbContext db, IMemoryCache cache, RedisCacheService redis)
    {
        _db = db;
        _cache = cache;
        _redis = redis;
    }

    // GET api/vinhos
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // 1º nível - Polly in-memory
        if (_cache.TryGetValue(CacheKey, out List<Vinho>? cached))
            return Ok(cached);

        // 2º nível - Redis
        var redisData = await _redis.GetAsync<List<Vinho>>(CacheKey);
        if (redisData != null)
        {
            _cache.Set(CacheKey, redisData, TimeSpan.FromMinutes(2));
            return Ok(redisData);
        }

        // 3º nível - Base de dados
        var vinhos = await _db.Vinhos.Where(v => v.Disponivel).ToListAsync();

        await _redis.SetAsync(CacheKey, vinhos, TimeSpan.FromMinutes(10));
        _cache.Set(CacheKey, vinhos, TimeSpan.FromMinutes(2));

        return Ok(vinhos);
    }

    // GET api/vinhos/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vinho = await _db.Vinhos.FindAsync(id);
        if (vinho == null) return NotFound();
        return Ok(vinho);
    }

    // POST api/vinhos
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Vinho vinho)
    {
        _db.Vinhos.Add(vinho);
        await _db.SaveChangesAsync();
        _cache.Remove(CacheKey);
        await _redis.RemoveAsync(CacheKey);
        return CreatedAtAction(nameof(GetById), new { id = vinho.Id }, vinho);
    }

    // PUT api/vinhos/1
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Vinho vinho)
    {
        if (id != vinho.Id) return BadRequest();
        _db.Entry(vinho).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        _cache.Remove(CacheKey);
        await _redis.RemoveAsync(CacheKey);
        return NoContent();
    }

    // DELETE api/vinhos/1
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var vinho = await _db.Vinhos.FindAsync(id);
        if (vinho == null) return NotFound();
        _db.Vinhos.Remove(vinho);
        await _db.SaveChangesAsync();
        _cache.Remove(CacheKey);
        await _redis.RemoveAsync(CacheKey);
        return NoContent();
    }
}
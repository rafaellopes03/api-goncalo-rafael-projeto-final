using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace _605_api.Services;

public class RedisCacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);
        if (json == null) return default;
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string key, T valor, TimeSpan? expiracao = null)
    {
        var opcoes = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiracao ?? TimeSpan.FromMinutes(10)
        };
        var json = JsonSerializer.Serialize(valor);
        await _cache.SetStringAsync(key, json, opcoes);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}
using Microsoft.AspNetCore.Mvc;
using _605_api.Data;
using _605_api.Resilience;

namespace _605_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventarioController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _db;
    private readonly ResilienceService _resilience;

    public InventarioController(IHttpClientFactory httpClientFactory, AppDbContext db, ResilienceService resilience)
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
        _resilience = resilience;
    }

    // GET api/inventario/QA-001
    [HttpGet("{sku}")]
    public async Task<IActionResult> GetStock(string sku)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ImposterClient");

            var json = await _resilience.ExecuteAsync(async () =>
            {
                var resposta = await client.GetAsync($"http://localhost:3000/inventory/{sku}");
                resposta.EnsureSuccessStatusCode();
                return await resposta.Content.ReadAsStringAsync();
            });

            return Content(json, "application/json");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Inventario] Erro: {ex.Message}");
            return StatusCode(503, "Serviço de inventário indisponível.");
        }
    }

    // POST api/inventario/pagamento
    [HttpPost("pagamento")]
    public async Task<IActionResult> ProcessarPagamento([FromBody] object dados)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ImposterClient");

            var json = await _resilience.ExecuteAsync(async () =>
            {
                var conteudo = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(dados),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
                var resposta = await client.PostAsync("http://localhost:3000/payments", conteudo);
                resposta.EnsureSuccessStatusCode();
                return await resposta.Content.ReadAsStringAsync();
            });

            return Content(json, "application/json");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Pagamento] Erro: {ex.Message}");
            return StatusCode(503, "Serviço de pagamentos indisponível.");
        }
    }
}
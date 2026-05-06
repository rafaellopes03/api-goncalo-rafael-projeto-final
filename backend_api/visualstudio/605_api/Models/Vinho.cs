namespace _605_api.Models;

public class Vinho
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // "branco", "reserva", "colheita"
    public int Ano { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string Imagem { get; set; } = string.Empty;
    public bool Disponivel { get; set; } = true;
    public string Sku { get; set; } = string.Empty; // para o imposter/inventário
}
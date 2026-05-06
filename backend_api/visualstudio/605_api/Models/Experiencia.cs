namespace _605_api.Models;

public class Experiencia
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int DuracaoMinutos { get; set; }
    public int MaxPessoas { get; set; }
    public string Imagem { get; set; } = string.Empty;
    public bool Disponivel { get; set; } = true;
}
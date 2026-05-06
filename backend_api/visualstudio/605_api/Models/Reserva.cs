namespace _605_api.Models;

public class Reserva
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Assunto { get; set; } = string.Empty;
    public DateTime? DataPretendida { get; set; }
    public int NumeroPessoas { get; set; } = 1;
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public string Estado { get; set; } = "pendente"; // "pendente", "confirmada", "cancelada"
}
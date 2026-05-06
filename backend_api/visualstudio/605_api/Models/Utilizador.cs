namespace _605_api.Models;

public class Utilizador
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "admin"; // "user" ou "admin"
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
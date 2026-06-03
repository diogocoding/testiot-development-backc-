using System.ComponentModel.DataAnnotations.Schema;

namespace AccessControlAPI.Models;

[Table("Logs")] // O nome da tabela no Supabase
public class LogAcesso
{
    public int Id { get; set; }

    [Column("DataHora")]
    public DateTime Timestamp { get; set; }

    [Column("UidCartao")]
    public string RfidTag { get; set; } = string.Empty;

    [Column("NomeUsuario")]
    public string? UserName { get; set; } // Pode ser nulo se um cartão desconhecido tentar entrar

    [Column("Dispositivo")]
    public string DeviceToken { get; set; } = string.Empty;

    [Column("Autorizado")]
    public bool Authorized { get; set; }

    [Column("Mensagem")]
    public string Message { get; set; } = string.Empty;
}
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessControlAPI.Models;

[Table("Aparelhos")]
public class Aparelho
{
    public int Id { get; set; }

    [Column("TokenDispositivo")] // O ID único da placa (ex: ESP32-001)
    public string Token { get; set; } = string.Empty;

    [Column("Nome")] // Ex: Entrada Principal
    public string Name { get; set; } = string.Empty;

    [Column("LocalInstalacao")] // Ex: Térreo - Portão A
    public string Location { get; set; } = string.Empty;

    [Column("Ativo")]
    public bool Active { get; set; }

    [Column("StatusOnline")]
    public bool IsOnline { get; set; }

    [Column("UltimoPing")]
    public DateTime? LastPing { get; set; }

    [Column("IpRede")]
    public string? IpAddress { get; set; }
}
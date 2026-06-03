using System.ComponentModel.DataAnnotations.Schema;

namespace AccessControlAPI.Models;

[Table("Usuarios")]

public class Usuario
{
    public int Id { get; set; }

    [Column("Nome")] // O banco chama de "Nome", mas o C# e o React chamam de "Name"
    public string Name { get; set; } = string.Empty;

    [Column("Cargo")]
    public string Role { get; set; } = string.Empty;

    [Column("RfidTag")]
    public string Uid { get; set; } = string.Empty;

    [Column("Ativo")]
    public bool Active { get; set; }

    [NotMapped] // Avisa o banco para ignorar isso por enquanto (vamos calcular os acessos depois usando os Logs)
    public int Accesses { get; set; } 
}
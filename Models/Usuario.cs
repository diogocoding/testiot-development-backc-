namespace AccessControlAPI.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Uid { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool Active { get; set; }
    public int Accesses { get; set; }
}
using Microsoft.EntityFrameworkCore;

namespace AccessControlAPI.Models;

public class AppDbContext : DbContext
{
    // O construtor que recebe as configurações (como a nossa Connection String)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Avisando ao C# que existe uma tabela de Usuários!
    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<Aparelho> Aparelhos { get; set; }

    public DbSet<LogAcesso> Logs { get; set; }
}
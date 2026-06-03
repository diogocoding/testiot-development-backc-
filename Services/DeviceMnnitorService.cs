using AccessControlAPI.Models;

namespace AccessControlAPI.Services;

// A classe BackgroundService avisa ao .NET que isso deve rodar em segundo plano sem parar
public class DeviceMonitorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DeviceMonitorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Fica rodando em loop infinito enquanto o servidor estiver ligado
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var banco = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Define que o limite de tolerância é 3 minutos atrás
                var limiteTempo = DateTime.UtcNow.AddMinutes(-3);

                // Busca todas as placas que constam como ONLINE, mas que o último ping foi há mais de 3 minutos
                var aparelhosOffline = banco.Aparelhos
                    .Where(a => a.IsOnline && (a.LastPing == null || a.LastPing < limiteTempo))
                    .ToList();

                if (aparelhosOffline.Any())
                {
                    foreach (var aparelho in aparelhosOffline)
                    {
                        aparelho.IsOnline = false; // "Derruba" a placa no banco de dados
                    }
                    await banco.SaveChangesAsync(stoppingToken);
                }
            }

            // Espera 1 minuto antes de verificar o banco novamente
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
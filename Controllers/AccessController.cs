using AccessControlAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccessController : ControllerBase
{
    private readonly AppDbContext _banco;

    public AccessController(AppDbContext banco)
    {
        _banco = banco;
    }

    [HttpPost("validate")]
    public IActionResult ValidateAccess([FromBody] AccessRequest request)
    {
        // 1. Verifica a Placa
        var aparelho = _banco.Aparelhos.FirstOrDefault(a => a.Token == request.DeviceToken);
        
        if (aparelho == null)
        {
            RegistrarLog(request.RfidTag, null, request.DeviceToken, false, "Dispositivo não reconhecido.");
            return Ok(new { authorized = false, message = "Dispositivo não reconhecido." });
        }

        // Atualiza status da placa
        aparelho.LastPing = DateTime.UtcNow;
        aparelho.IsOnline = true;
        _banco.SaveChanges();

        // 2. Verifica o Usuário
        var usuario = _banco.Usuarios.FirstOrDefault(u => u.Uid == request.RfidTag);
        
        if (usuario == null)
        {
            RegistrarLog(request.RfidTag, "Desconhecido", request.DeviceToken, false, "Cartão não cadastrado.");
            return Ok(new { authorized = false, message = "Cartão não cadastrado." });
        }

        // 3. Verifica o Status (Toggle)
        if (!usuario.Active)
        {
            RegistrarLog(request.RfidTag, usuario.Name, request.DeviceToken, false, "Acesso bloqueado pelo administrador.");
            return Ok(new { authorized = false, message = "Acesso bloqueado pelo administrador." });
        }

        // 4. Sucesso!
        RegistrarLog(request.RfidTag, usuario.Name, request.DeviceToken, true, "Acesso Liberado");
        return Ok(new { authorized = true, userName = usuario.Name, message = "Acesso Liberado" });
    }

    [HttpGet("logs")]
        public IActionResult GetLogs()
        {
            var logs = _banco.Logs.OrderByDescending(l => l.Timestamp).ToList();
            return Ok(logs);
        }




    // --- A FUNÇÃO QUE FALTAVA ---
    // Esta função é quem pega o resultado e "anota" no nosso caderno de Histórico
    private void RegistrarLog(string rfid, string? nome, string token, bool autorizado, string mensagem)
    {
        var log = new LogAcesso
        {
            Timestamp = DateTime.UtcNow,
            RfidTag = rfid ?? "Desconhecido",
            UserName = nome,
            DeviceToken = token ?? "Desconhecido",
            Authorized = autorizado,
            Message = mensagem
        };

        _banco.Logs.Add(log);
        _banco.SaveChanges();
    }
}
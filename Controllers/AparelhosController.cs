using AccessControlAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlAPI.Controllers;


  

[ApiController]
[Route("api/[controller]")]
public class AparelhosController : ControllerBase
{
    private readonly AppDbContext _banco;

    public AparelhosController(AppDbContext banco)
    {
        _banco = banco;
    }

    [HttpGet]
    public IActionResult GetAparelhos()
    {
        var aparelhos = _banco.Aparelhos.ToList();
        return Ok(aparelhos);
    }

    [HttpPost]
    public IActionResult CreateAparelho([FromBody] Aparelho novoAparelho)
    {
        novoAparelho.Active = true;
        novoAparelho.IsOnline = false; // Começa offline até a placa se comunicar
        novoAparelho.LastPing = DateTime.UtcNow;
        
        _banco.Aparelhos.Add(novoAparelho);
        _banco.SaveChanges();

        return Created("", novoAparelho);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateAparelho(int id, [FromBody] Aparelho aparelhoAtualizado)
    {
        var aparelhoExistente = _banco.Aparelhos.FirstOrDefault(a => a.Id == id);
        if (aparelhoExistente == null) return NotFound();

        aparelhoExistente.Name = aparelhoAtualizado.Name;
        aparelhoExistente.Location = aparelhoAtualizado.Location;
        aparelhoExistente.Token = aparelhoAtualizado.Token;

        _banco.SaveChanges();
        return Ok(aparelhoExistente);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteAparelho(int id)
    {
        var aparelho = _banco.Aparelhos.FirstOrDefault(a => a.Id == id);
        if (aparelho == null) return NotFound();

        _banco.Aparelhos.Remove(aparelho);
        _banco.SaveChanges();

        return NoContent();
    }

// Crie esta classe pequenina logo acima ou abaixo da classe do AparelhosController (mas dentro do namespace)
    public class PingRequest 
    {
        public string DeviceToken { get; set; } = string.Empty;
    }

    // --- NOVA ROTA DE HEARTBEAT ---
    [HttpPost("ping")]
    public IActionResult Ping([FromBody] PingRequest request)
    {
        var aparelho = _banco.Aparelhos.FirstOrDefault(a => a.Token == request.DeviceToken);
        if (aparelho == null) return NotFound(new { message = "Dispositivo não encontrado." });

        aparelho.LastPing = DateTime.UtcNow;
        aparelho.IsOnline = true;
        _banco.SaveChanges();

        return Ok(new { message = "Ping recebido, placa online." });
    }

}


using AccessControlAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _banco;

    public UsersController(AppDbContext banco)
    {
        _banco = banco;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        var usuarios = _banco.Usuarios.ToList();
        return Ok(usuarios);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] Usuario novoUsuario)
    {
        if (string.IsNullOrEmpty(novoUsuario.Name) || string.IsNullOrEmpty(novoUsuario.Uid))
            return BadRequest(new { mensagem = "Nome e UID são obrigatórios." });

        novoUsuario.Active = true;
        
        _banco.Usuarios.Add(novoUsuario);
        _banco.SaveChanges(); 

        return Created("", novoUsuario);
    }

    // --- A ROTA QUE FAZ O EDITAR E O TOGGLE FUNCIONAREM (PUT) ---
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] Usuario usuarioAtualizado)
    {
        var usuarioExistente = _banco.Usuarios.FirstOrDefault(u => u.Id == id);
        if (usuarioExistente == null)
            return NotFound(new { mensagem = "Usuário não encontrado." });

        // Atualiza os dados no banco com o que veio do React
        usuarioExistente.Name = usuarioAtualizado.Name;
        usuarioExistente.Role = usuarioAtualizado.Role;
        usuarioExistente.Uid = usuarioAtualizado.Uid;
        usuarioExistente.Active = usuarioAtualizado.Active;

        _banco.SaveChanges();
        return Ok(usuarioExistente);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var usuario = _banco.Usuarios.FirstOrDefault(u => u.Id == id);
        if (usuario == null)
            return NotFound();

        _banco.Usuarios.Remove(usuario);
        _banco.SaveChanges(); 

        return NoContent();
    }
}
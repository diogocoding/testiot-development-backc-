using AccessControlAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetUsers()
    {
        // Lista simulada provisória para testar a integração inicial
        var mockUsers = new List<Usuario>
        {
            new Usuario { Id = 1, Name = "Diogo Nascimento", Uid = "A1 B2 C3 D4", Role = "Desenvolvedor", Active = true, Accesses = 42 },
            new Usuario { Id = 2, Name = "Maira Lourenço", Uid = "C9 04 AA 5E", Role = "Documentação", Active = true, Accesses = 27 }
        };

        return Ok(mockUsers);
    }

    [HttpPost] // O POST é o verbo HTTP usado para CRIAR coisas
    public IActionResult CreateUser([FromBody] Usuario novoUsuario)
    {
        // 1. O React enviou os dados e o C# converteu automaticamente para a classe Usuario
        
        // 2. Validação simples: se não mandou nome ou UID, devolve erro
        if (string.IsNullOrEmpty(novoUsuario.Name) || string.IsNullOrEmpty(novoUsuario.Uid))
        {
            return BadRequest(new { mensagem = "Nome e UID são obrigatórios." });
        }

        // 3. Aqui, futuramente, você vai salvar no Banco de Dados.
        // Por enquanto, vamos fingir que salvou no banco e devolver um status de sucesso (201 Created)
        
        novoUsuario.Id = new Random().Next(3, 1000); // Gera um ID falso só pra testar
        novoUsuario.Active = true;
        novoUsuario.Accesses = 0;

        return Created("", novoUsuario);
    }

}
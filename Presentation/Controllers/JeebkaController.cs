using Business.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("/v1/jeebka")]
public class JeebkaController : ControllerBase
{
    private JeebkaService _jeebkaService;

    public JeebkaController(JeebkaService jeebkaService)
    {
        _jeebkaService = jeebkaService;
    }
    
    [HttpPost("users")]
    public IActionResult  CreateUser(User user)
    {
        _jeebkaService.CreateUser(user);
        return Created("~/v1/jeebka/users/" + user.Email,user);
    }
    
    [HttpGet("users/{email}")]
    public IActionResult GetUserByEmail(string email)
    {
        
        var user = _jeebkaService.GetUserByEmail(email);
        if (user == null) return NotFound();
        return Ok(user);
    }
    
    [HttpDelete("users/{userid}")]
    public IActionResult DeleteUser(string userid)
    {
        _jeebkaService.DeleteUserByEmail(userid);
        return Ok();
    }
    
}
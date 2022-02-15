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
    public void CreateUser(User user)
    {
        _jeebkaService.CreateUser(user);
    }
    
    [HttpDelete("users/{userid}")]
    public void DeleteUser(string userid)
    {
        _jeebkaService.DeleteUser(userid);
    }
    
}
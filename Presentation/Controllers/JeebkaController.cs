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

    [HttpPost("links")]
    public IActionResult CreateLink(Link link)
    {
        _jeebkaService.CreateLink(link);
        return Created("~/v1/jeebka/links/"+link.getId(), link);
    }

    [HttpPut("links/group")]
    public IActionResult AddLinkToGroup(Link link, string groupId)
    {
        _jeebkaService.AddLinkToGroup(link, groupId);
        return Ok();
    }
    
}
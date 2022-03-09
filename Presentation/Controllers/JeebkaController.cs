using Business.Services;
using Domain.DTOs;
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
    public IActionResult CreateUser(UserDto user)
    {
        IActionResult response = Created("~/v1/jeebka/users/" + user.Email, user);
        if (!_jeebkaService.CreateUser(user)) response = Conflict();
        return response;
    }
    
    [HttpGet("users/{email}")]
    public IActionResult GetUser(string email)
    {
        var user = _jeebkaService.GetUserByEmail(email);
        if (user == null) return NotFound();
        return Ok(user);
    }
    
    [HttpDelete("users/{email}")]
    public IActionResult DeleteUser(string email)
    {
        _jeebkaService.DeleteUserByEmail(email);
        return Ok();
    }
    
    [HttpPost("users/{email}/groups")]
    public IActionResult CreateGroup(GroupDto group, string email)
    {
        IActionResult response = Created($"~/v1/jeebka/users/{email}/groups/{group.Name}", group);
        if (!_jeebkaService.CreateGroup(group, email)) response = Conflict();
        return response;
    }

    [HttpGet("users/{email}/groups/{name}")]
    public IActionResult GetGroup(string email, string name)
    {
        var group = _jeebkaService.GetGroup(name, email);
        if (group == null) return NotFound();
        return Ok(group);
    }
    
    [HttpDelete("users/{email}/groups/{name}")]
    public IActionResult DeleteGroup(string email, string name)
    {
        _jeebkaService.DeleteGroup(email, name);
        return Ok();
    }
    
    [HttpPut("users/{email}/groups/{name}/members/{newOwnerEmail}")]
    public IActionResult ShareGroup(string email, string name, string newOwnerEmail)
    {
        var userAdded = _jeebkaService.ShareGroup(name, email, newOwnerEmail);
        return userAdded ? Ok() : Conflict();
    }
    
    [HttpDelete("users/{email}/groups/{name}/members/{newOwnerEmail}")]
    public IActionResult UnshareGroup(string email, string name, string newOwnerEmail)
    {
        _jeebkaService.UnshareGroup(name, email, newOwnerEmail);
        return Ok();
    }


    [HttpGet("users/{email}/publics")]
    public IActionResult ShowPublicGroups(string userEmail)
    {
        return Ok(_jeebkaService.GetMostMatchingPublicGroupsByTags(userEmail));
    }

    [HttpPost("users/{email}/groups/{name}/links")]
    public IActionResult CreateLink(Link link, string email, string name)
    {
        IActionResult response = Created($"users/{email}/groups/{name}/links/{link.Name}", link);
        if (!_jeebkaService.CreateLink(link, email, name)) response = Conflict();
        return response;
    }
    
    [HttpDelete("users/{email}/groups/{name}/links/{linkId}")]
    public IActionResult RemoveLink(string email, string name, string linkId)
    {
        _jeebkaService.DeleteLinkFromGroup(email, name, linkId);
        return Ok();
    }
    
    [HttpDelete("users/{email}/groups/{name}/links/{linkId}")]
    public IActionResult DeleteLink(string email, string name, string linkId)
    {
        _jeebkaService.DeleteLink(email, name, linkId);
        return Ok();
    }

    [HttpGet("users/{email}/query/tags")]
    public IActionResult GetQuery()
    {
        return Ok(_jeebkaService.GetLinksByTags(new List<string>(), new List<string>()));
    }
    

    [HttpPut("users/{email}/groups/{name}/links/{linkName}/tags/{tagName}")]
    public IActionResult AddTag(string email, string name, string linkName, string tagName)
    {
        var tagAdded = _jeebkaService.AddTagToLink(email, name, linkName, tagName);
        return tagAdded ? Ok() : Conflict();
    }
    
    [HttpDelete("users/{email}/groups/{name}/links/{linkName}/tags/{tagName}")]
    public IActionResult DeleteTag(string email, string name, string linkName, string tagName)
    {
        _jeebkaService.DeleteTagFromLink(email, name, linkName, tagName);
        return Ok();
    }
}
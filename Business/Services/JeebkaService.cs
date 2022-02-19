using Domain.Entities;
using Infrastructure.Repositories;
using MongoDB.Driver;

namespace Business.Services;

public class JeebkaService
{
    private UserRepository _userRepository;
    private LinkRepository _linkRepository;
    private GroupRepository _groupRepository;

    public JeebkaService(UserRepository userRepository, GroupRepository groupRepository, LinkRepository linkRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _linkRepository = linkRepository;
    }
    
    public void CreateUser(User user)
    {
        _userRepository.CreateUser(user);
    }

    public User? GetUserByEmail(string email)
    {
        return _userRepository.GetUser(email);
    }
    
    public void DeleteUserByEmail(string email)
    {
        _userRepository.DeleteUser(email);
    }

    public Link GetLink(string linkId)
    {
        return _linkRepository.GetLink(linkId);
    }

    public bool CreateLink(Link link, string userEmail, string groupName)
    {
        var group = _groupRepository.GetGroup(userEmail, groupName);
        var notExists = _linkRepository.ValidateLinkInGroup(link.Name, link.Url, group.Id);
        if (notExists)
        {
            _linkRepository.CreateLink(link, out var linkId);
            _groupRepository.AddLinkToGroup(group.Id, linkId);
            _linkRepository.AddGroupToLink(group.Id, linkId);
        }
        return notExists;
    }
    public Link? GetLinkByName(string userEmail, string groupName, string linkName)
    {
        var group = _groupRepository.GetGroup(userEmail, groupName);
        return _linkRepository.GetLinkByName(linkName, group.Id);
    }

    public Link? GetLinkByUrl(string userEmail, string groupName, string linkUrl)
    {
        var group = _groupRepository.GetGroup(userEmail, groupName);
        return _linkRepository.GetLinkByUrl(linkUrl, group.Id);
    }

    public bool AddLinkToGroup(string userEmail, string groupName, string linkId)
    {
        var group = _groupRepository.GetGroup(userEmail, groupName);
        var notExists = group.Links.Contains(linkId);
        if (notExists)
        {
            _groupRepository.AddLinkToGroup(group.Id, linkId);
            _linkRepository.AddGroupToLink(group.Id, linkId);
        }

        return notExists;
    }
    
}
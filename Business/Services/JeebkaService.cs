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

    public void CreateLink(Link link, out string linkId)
    {
        _linkRepository.CreateLink(link, out linkId);
    }
    public void CreateLink(Link link)
    {
        _linkRepository.CreateLink(link);
    }

    public void AddLinkToGroup(string linkId, string groupId)
    {
        _groupRepository.AddLinkToGroup(groupId, linkId);
        _linkRepository.AddLinkToGroup(groupId, linkId);
    }
    
}
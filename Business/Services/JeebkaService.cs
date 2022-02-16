using Domain.Entities;
using Infrastructure.Repositories;

namespace Business.Services;

public class JeebkaService
{
    private UserRepository _userRepository;
    private LinkRepository _linkRepository;

    public JeebkaService(UserRepository userRepository, LinkRepository linkRepository)
    {
        _userRepository = userRepository;
        _linkRepository = linkRepository;
    }

    
    public void CreateUser(User user)
    {
        _userRepository.CreateUser(user);
    }
    
    public void DeleteUser(string userId)
    {
        _userRepository.DeleteUser(userId);
    }

    public void CreateLink(Link link)
    {
        _linkRepository.CreateLink(link);
    }

    public void AddLinkToGroup(Link link, String groupId)
    {
        _linkRepository.AddLinkToGroup(link, groupId);
    }    
}
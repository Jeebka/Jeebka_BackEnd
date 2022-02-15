using Domain.Entities;
using Infrastructure.Repositories;

namespace Business.Services;

public class JeebkaService
{
    private UserRepository _userRepository;

    public JeebkaService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public void CreateUser(User user)
    {
        _userRepository.CreateUser(user);
    }
    
}
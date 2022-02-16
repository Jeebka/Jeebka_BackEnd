using Domain.Entities;
using Infrastructure.Repositories;
using MongoDB.Driver;

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

    public User? GetUserByEmail(string email)
    {
        return _userRepository.GetUser(email);
    }
    
    public void DeleteUserByEmail(string email)
    {
        _userRepository.DeleteUser(email);
    }
    
}
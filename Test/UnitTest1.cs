using System;
using System.Threading.Tasks;
using Business.Services;
using Domain.Entities;
using Infrastructure.Clients;
using Infrastructure.Repositories;
using NUnit.Framework;

namespace Test;

public class Tests
{
    private UserRepository _userRepository;
    private JeebkaService _jeebkaService;
    const string mongoConnection = "mongodb+srv://dbUser:dbUserPassword@jeebkadb.6qdxo.mongodb.net/JeebkaDBTest?retryWrites=true&w=majority";
    const string dataBase = "JeebkaDBTest";
    const string userRepositoryCollectionName = "User";
    
    [SetUp]
    public void Setup()
    {
        _userRepository = new UserRepository(mongoConnection, dataBase, userRepositoryCollectionName);
        _jeebkaService = new JeebkaService(_userRepository);
    }

    [Test]
    public void ShouldCreateAnUser()
    {
        const string emailTest = "emailTestCreate";
        var user = new User
        {
            Name = "nameTest",
            Password = "passwordTest",
            Email = emailTest,
            
        };
        _jeebkaService.CreateUser(user);
        Assert.AreEqual(_jeebkaService.GetUserByEmail(emailTest).Email, emailTest);

    }
    
    [Test]
    public void ShouldDeleteAnUser()
    {
        const string emailTest = "emailTestDelete";
        var user = new User
        {
            Name = "nameTest",
            Password = "passwordTest",
            Email = emailTest,
            
        };
        _jeebkaService.CreateUser(user);
        _jeebkaService.DeleteUserByEmail(emailTest);
        
    }
}
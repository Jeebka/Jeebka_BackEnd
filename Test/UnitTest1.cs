using System;
using System.Collections.Generic;
using System.Linq;
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
    private LinkRepository _linkRepository;
    private GroupRepository _groupRepository;
    private JeebkaService _jeebkaService;
    const string mongoConnection = "mongodb+srv://dbUser:dbUserPassword@jeebkadb.6qdxo.mongodb.net/JeebkaDBTest?retryWrites=true&w=majority";
    const string dataBase = "JeebkaDBTest";
    const string userRepositoryCollectionName = "User";
    const string linkRepositoryCollectionName = "Link";
    const string groupRepositoryCollectionName = "Group";
    
    [SetUp]
    public void Setup()
    {
        var mongoClient = MongoDBClient.GetConnection(mongoConnection);
        var database = mongoClient.GetDatabase(dataBase);
        var linkCollection = database.GetCollection<Link>(linkRepositoryCollectionName);
        var groupCollection = database.GetCollection<Group>(groupRepositoryCollectionName);
        var userCollection = database.GetCollection<User>(userRepositoryCollectionName);
        _linkRepository = new LinkRepository(linkCollection);
        _userRepository = new UserRepository(userCollection);
        _groupRepository = new GroupRepository(_linkRepository, groupCollection);
        _jeebkaService = new JeebkaService(_userRepository, _groupRepository, _linkRepository);
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

    [Test]
    public void ShouldCreateALink()
    {
        var link = createGeneratedLink();
        _jeebkaService.CreateLink(link, out string linkId);
        Assert.AreEqual(_jeebkaService.GetLink(linkId), linkId);
        _jeebkaService.DeleteLink(linkId);
    }

    [Test]
    public void ShouldDeleteALink()
    {
        var link = createGeneratedLink();
        _jeebkaService.CreateLink(link, out string linkId);
        _jeebkaService.DeleteLink(linkId);
    }

    [Test]
    public void ShouldAddALinkToAGroup()
    {
        var link = createGeneratedLink();
        var group = createGeneratedGroup();
        _jeebkaService.CreateLink(link, out string linkId);
        _jeebkaService.CreateGroup(group, out string groupId);
        _jeebkaService.AddLinkToGroup(linkId, groupId);
        var groupLink = _jeebkaService.GetLink(linkId).Groups.First();
        Assert.AreEqual(groupLink, group);
    }

    [Test]
    public void ShouldDeleteALinkFromAGroup()
    {
        var link = createGeneratedLink();
        var group = createGeneratedGroup();
        _jeebkaService.CreateLink(link, out string linkId);
        _jeebkaService.CreateGroup(group, out string groupId);
        _jeebkaService.AddLinkToGroup(linkId, groupId);
        _jeebkaService.DeleteLinkFromGroup(groupId, linkId);
    }

    private Link createGeneratedLink()
    {
        var link = new Link
        {
            Url = "https://www.creationLinkTest.tst.jbk",
            Name = "GenericLinkNameTest",
            Date = DateTime.Now,
            Groups = new List<string>()
        };
        return link;
    }

    private Group createGeneratedGroup()
    {
        var group = new Group
        {
            Name = "GenericGroupNameTest",
            Description = "Mollit consequat sunt anim ut aliquip nulla excepteur fugiat labore quis et nulla culpa. Et eu labore nisi veniam enim amet est. Dolor in sint tempor pariatur. Incididunt aliqua commodo cupidatat esse tempor officia ad duis. Aliquip deserunt duis sint nulla Lorem eu ex magna. Sit deserunt enim aute tempor qui adipisicing consequat et labore qui anim qui anim. Excepteur pariatur adipisicing sit occaecat et reprehenderit excepteur quis quis eiusmod est ullamco est est.",
            Links = new List<string>(),
            Members = new List<string>()
        };
        return group;
    }
    
}
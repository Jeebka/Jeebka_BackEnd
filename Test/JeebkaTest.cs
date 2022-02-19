using System;
using System.Collections.Generic;
using System.Linq;
using Business.Services;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Clients;
using Infrastructure.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
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
        linkCollection.DeleteMany(_ => true);
        groupCollection.DeleteMany(_ => true);
        userCollection.DeleteMany(_ => true);
        _linkRepository = new LinkRepository(linkCollection);
        _userRepository = new UserRepository(userCollection);
        _groupRepository = new GroupRepository(groupCollection);
        _jeebkaService = new JeebkaService(_userRepository, _groupRepository, _linkRepository);
    }

    [Test]
    public void ShouldCreateAUser()
    {
        var user = CreateGenericUser();
        _jeebkaService.CreateUser(user);
        Assert.AreEqual(_jeebkaService.GetUserByEmail(user.Email).Email, user.Email);
    }
    
    [Test]
    public void ShouldReturnASpecificUser()
    {
        var user = CreateGenericUser();
        var user2 = CreateGenericUser(2);
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateUser(user2);
        Assert.AreEqual(_jeebkaService.GetUserByEmail("TestUser2@mail.com").Email, user2.Email);
    }
    
    [Test]
    public void ShouldDeleteAUser()
    {
        var user = CreateGenericUser();
        _jeebkaService.CreateUser(user);
        Assert.AreEqual(_jeebkaService.GetUserByEmail(user.Email).Email, user.Email);
        _jeebkaService.DeleteUserByEmail(user.Email);
        Assert.AreEqual(_jeebkaService.GetUserByEmail(user.Email), null);
    }

    //Groups Tests

    [Test]
    public void ShouldCreateAGroup()
    {
        var user = CreateGenericUser();
        var group = CreateGenericGroup();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        Assert.AreEqual(_jeebkaService.GetGroup(group.Name, user.Email).Name, group.Name);
    }
    
    [Test]
    public void ShouldReturnASpecificGroup()
    {
        var user = CreateGenericUser();
        var group = CreateGenericGroup();
        var group2 = CreateGenericGroup(2);
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        _jeebkaService.CreateGroup(group2, user.Email);
        Assert.AreEqual(_jeebkaService.GetGroup(group2.Name, user.Email).Name, group2.Name);
    }

    [Test]
    public void ShouldDeleteAGroup()
    {
        var user = CreateGenericUser();
        var group = CreateGenericGroup();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        Assert.AreEqual(_jeebkaService.GetGroup(group.Name, user.Email).Name, group.Name);
        _jeebkaService.DeleteGroup(user.Email, group.Name);
        Assert.AreEqual(_jeebkaService.GetGroup(group.Name, user.Email), null);
    }

    [Test]
    public void ShouldAddAUserToGroupMembers()
    {
        var user = CreateGenericUser();
        var user2 = CreateGenericUser(2);
        var group = CreateGenericGroup();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        _jeebkaService.ShareGroup(group.Name, user.Email, user2.Email);
        var members = _jeebkaService.GetGroup(group.Name, user.Email).Members;
        Assert.True(members.Contains(user.Email) && members.Contains(user2.Email));
    }
    
    [Test]
    public void ShouldCreateALink()
    {
        var user = CreateGenericUser();
        var group = CreateGenericGroup();
        var link = CreateGenericLink();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        _jeebkaService.CreateLink(link, user.Email, group.Name);
        Assert.AreEqual(_jeebkaService.GetLinkByName(user.Email, group.Name, link.Name).Url, link.Url);
    }

    [Test]
    public void ShouldReturnASpecificLink()
    {
        var user = CreateGenericUser();
        var group = CreateGenericGroup();
        var link = CreateGenericLink();
        var link2 = CreateGenericLink(2);
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        _jeebkaService.CreateLink(link, user.Email, group.Name);
        _jeebkaService.CreateLink(link2, user.Email, group.Name);

        Assert.AreEqual(_jeebkaService.GetLinkByName(user.Email, group.Name, link2.Name).Url, link2.Url);
    }
    
    [Test]
    public void ShouldAddTagToLink()
    {
        var link = CreateGenericLink();
        var group = CreateGenericGroup();
        var user = CreateGenericUser();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        _jeebkaService.CreateLink(link, user.Email, group.Name);
        var returnedLink = _jeebkaService.GetLinkByName(user.Email, group.Name, link.Name);
        _jeebkaService.AddLinkToGroup(user.Email, group.Name, returnedLink.Id);
        _jeebkaService.AddTagToLink(user.Email, group.Name, returnedLink.Name,"tag1");
        TestContext.Out.WriteLine(JsonConvert.SerializeObject(_jeebkaService.GetLinkByName(user.Email, group.Name, link.Name)));
        Assert.True(_jeebkaService.GetLinkByName(user.Email, group.Name, returnedLink.Name).Tags.Contains("tag1"));

    }

    [Test]
    public void ShouldNotAddTagToLinkIfAlreadyExists()
    {
        var link = CreateGenericLink();
        var group = CreateGenericGroup();
        var user = CreateGenericUser();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        _jeebkaService.CreateLink(link, user.Email, group.Name);
        var returnedLink = _jeebkaService.GetLinkByName(user.Email, group.Name, link.Name);
        _jeebkaService.AddLinkToGroup(user.Email, group.Name, returnedLink.Id);
        _jeebkaService.AddTagToLink(user.Email, group.Name, returnedLink.Name,"tag1");
        _jeebkaService.AddTagToLink(user.Email, group.Name, returnedLink.Name,"tag1");
        Assert.True(_jeebkaService.GetLinkByName(user.Email, group.Name, link.Name).Tags.Count() == 1);

    }
    
    [Test]
    public void ShouldDeleteTagFromLink()
    {
        var link = CreateGenericLink();
        var group = CreateGenericGroup();
        var user = CreateGenericUser();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        _jeebkaService.CreateLink(link, user.Email, group.Name);
        var returnedLink = _jeebkaService.GetLinkByName(user.Email, group.Name, link.Name);
        _jeebkaService.AddLinkToGroup(user.Email, group.Name, returnedLink.Id);
        _jeebkaService.AddTagToLink(user.Email, group.Name, returnedLink.Name,"tag1");
        _jeebkaService.DeleteTagFromLink(user.Email, group.Name, returnedLink.Name,"tag1");
        Assert.True(!_jeebkaService.GetLinkByName(user.Email, group.Name, link.Name).Tags.Any());
        
    }
    
    [Test]
    public void ShouldGetTheUsersLinksByTags()
    {
        var link = CreateGenericLink();
        var link2 = CreateGenericLink(2);
        var group = CreateGenericGroup();
        var user = CreateGenericUser();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        var groupId = _jeebkaService.GetGroup(group.Name, user.Email).Id;
        _jeebkaService.CreateLink(link, user.Email, group.Name); _jeebkaService.CreateLink(link2, user.Email, group.Name);
        _jeebkaService.AddTagToLink(user.Email, group.Name, link.Name,"tag1"); _jeebkaService.AddTagToLink(user.Email, group.Name, link2.Name,"tag1");
        _jeebkaService.AddTagToLink(user.Email, group.Name, link.Name,"tag2");
        var userLinks = _jeebkaService.GetLinksByTags(new List<string>{groupId}, new List<string> {"tag1", "tag2"});
        Assert.True(userLinks.Count == 1);
        Assert.True(userLinks[0].Name == link.Name);
    }
    
    [Test]
    public void ShouldGetTheUsersLinksByName()
    {
        var link = CreateGenericLink();
        var link2 = CreateGenericLink(2);
        var group = CreateGenericGroup();
        var user = CreateGenericUser();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        var groupId = _jeebkaService.GetGroup(group.Name, user.Email).Id;
        _jeebkaService.CreateLink(link, user.Email, group.Name); _jeebkaService.CreateLink(link2, user.Email, group.Name);
        _jeebkaService.AddTagToLink(user.Email, group.Name, link.Name,"tag1"); _jeebkaService.AddTagToLink(user.Email, group.Name, link2.Name,"tag1");
        _jeebkaService.AddTagToLink(user.Email, group.Name, link.Name,"tag2");
        var userLinks = _jeebkaService.GetLinksByName(new List<string>{groupId}, "LinkName");
        Assert.True(userLinks.Count == 2);
    }
    
    [Test]
    public void ShouldGetTheUsersLinksByDateRange()
    {
        var link = CreateGenericLink();
        var link2 = CreateGenericLink(2);
        link.Date = new DateTime(2022,2,17);
        link2.Date = new DateTime(2022, 2, 10);
        var group = CreateGenericGroup();
        var user = CreateGenericUser();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        var groupId = _jeebkaService.GetGroup(group.Name, user.Email).Id;
        _jeebkaService.CreateLink(link, user.Email, group.Name); _jeebkaService.CreateLink(link2, user.Email, group.Name);
        _jeebkaService.AddTagToLink(user.Email, group.Name, link.Name,"tag1"); _jeebkaService.AddTagToLink(user.Email, group.Name, link2.Name,"tag1");
        _jeebkaService.AddTagToLink(user.Email, group.Name, link.Name,"tag2");
        var userLinks = _jeebkaService.GetLinksByDateRange(new List<string>{groupId}, DateTime.Today, new DateTime(2022,2,15));
        Assert.True(userLinks.Count == 1);
        Assert.True(userLinks[0].Name == link.Name);
    }
    
    [Test]
    public void ShouldGetTheUsersLinksByUrl()
    {
        var link = CreateGenericLink();
        var link2 = CreateGenericLink(2);
        var group = CreateGenericGroup();
        var user = CreateGenericUser();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        var groupId = _jeebkaService.GetGroup(group.Name, user.Email).Id;
        _jeebkaService.CreateLink(link, user.Email, group.Name); _jeebkaService.CreateLink(link2, user.Email, group.Name);
        _jeebkaService.AddTagToLink(user.Email, group.Name, link.Name,"tag1"); _jeebkaService.AddTagToLink(user.Email, group.Name, link2.Name,"tag1");
        _jeebkaService.AddTagToLink(user.Email, group.Name, link.Name,"tag2");
        var userLinks = _jeebkaService.GetLinksByUrl(new List<string> {groupId}, "Test2");
        Assert.True(userLinks.Count == 1);
        Assert.True(userLinks[0].Name == link2.Name);
    }
    
    //Generic entities

    private LinkDto CreateGenericLink(int n = 1)
    {
        var link = new LinkDto
        {
            Url = $"https://www.creationLinkTest.tst{n}.jbk",
            Name = $"GenericLinkNameTest{n}",
            Date = DateTime.Now
        };
        return link;
    }

    private static GroupDto CreateGenericGroup(int n = 1)
    {
        var group = new GroupDto()
        {
            Name = $"GenericGroupNameTest{n}",
            Description = "Mollit consequat sunt anim ut aliquip nulla excepteur fugiat labore quis et nulla culpa. Et eu labore nisi veniam enim amet est. Dolor in sint tempor pariatur. Incididunt aliqua commodo cupidatat esse tempor officia ad duis. Aliquip deserunt duis sint nulla Lorem eu ex magna. Sit deserunt enim aute tempor qui adipisicing consequat et labore qui anim qui anim. Excepteur pariatur adipisicing sit occaecat et reprehenderit excepteur quis quis eiusmod est ullamco est est.",
        };
        return group;
    }
    
    private static UserDto CreateGenericUser(int n = 1)
    {
        return new UserDto
        {
            Email = $"TestUser{n}@mail.com",
            Name = $"TestUser{n}",
            Password = "Test1234"
        };
    }
}
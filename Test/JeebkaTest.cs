using System;
using System.Collections.Generic;
using System.Linq;
using Business.Services;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Clients;
using Infrastructure.Repositories;
using MongoDB.Driver;
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
    public void ShouldGetGroupsUserOnlyMember()
    {
        var user = CreateGenericUser();
        var user2 = CreateGenericUser(2);
        var group = CreateGenericGroup();
        var group2 = CreateGenericGroup(2);
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateUser(user2);
        _jeebkaService.CreateGroup(group, user.Email);
        _jeebkaService.CreateGroup(group2, user.Email);
        _jeebkaService.ShareGroup(group.Name, user.Email, user2.Email);
        var userGroups = _jeebkaService.GetGroupsUserOnlyMember(user.Email);
        Assert.True(userGroups.Count == 1);
        foreach (var userGroup in userGroups)
        {
            if (userGroup.Name.Equals(group2.Name))
            {
                Assert.True(true);
            }
            else
            {
                Assert.Fail();
            }
        }
    }

    [Test]
    public void ShouldGetGroupsWhereUsersInMembers()
    {
        var user = CreateGenericUser();
        var user2 = CreateGenericUser(2);
        var group = CreateGenericGroup();
        var group2 = CreateGenericGroup(2);
        var group3 = CreateGenericGroup(4);
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateUser(user2);
        _jeebkaService.CreateGroup(group, user.Email);
        _jeebkaService.CreateGroup(group2, user.Email);
        _jeebkaService.CreateGroup(group3, user.Email);
        _jeebkaService.ShareGroup(group.Name, user.Email, user2.Email);
        _jeebkaService.ShareGroup(group2.Name, user.Email, user2.Email);
        var userGroups = _jeebkaService.GetGroupsWhereUsersInMembers(user.Email);
        Assert.True(userGroups.Count == 2);
        foreach (var userGroup in userGroups)
        {
            if (userGroup.Name.Equals(group2.Name) || userGroup.Name.Equals(group.Name))
            {
                Assert.True(true);
            }
            else
            {
                Assert.Fail();
            }
        }
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
        Assert.True(_jeebkaService.GetLinkByName(user.Email, group.Name, returnedLink.Name).Tags.Contains("tag1"));
        Assert.True(_jeebkaService.GetGroup(group.Name, user.Email).LinksTags.Contains("tag1"));

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
    public void ShouldNotAddTagToLinkIfHasAlreadyReachTheMaximumAllowed()
    {
        var link = CreateGenericLink();
        var group = CreateGenericGroup();
        var user = CreateGenericUser();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        _jeebkaService.CreateLink(link, user.Email, group.Name);
        var returnedLink = _jeebkaService.GetLinkByName(user.Email, group.Name, link.Name);
        _jeebkaService.AddLinkToGroup(user.Email, group.Name, returnedLink.Id);
        for (var i = 0; i <= 7; i++)
        {
            _jeebkaService.AddTagToLink(user.Email, group.Name, returnedLink.Name,"tag"+i);    
        }
        Assert.False(_jeebkaService.GetLinkByName(user.Email, group.Name, link.Name).Tags.Contains("tag7"));
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
        var group2 = CreateGenericGroup(2);
        var user = CreateGenericUser();
        _jeebkaService.CreateUser(user);
        _jeebkaService.CreateGroup(group, user.Email);
        _jeebkaService.CreateGroup(group2, user.Email);
        var groupId = _jeebkaService.GetGroup(group.Name, user.Email).Id;
        var groupId2 = _jeebkaService.GetGroup(group2.Name, user.Email).Id;
        _jeebkaService.CreateLink(link, user.Email, group.Name); _jeebkaService.CreateLink(link2, user.Email, group.Name);
        _jeebkaService.CreateLink(link, user.Email, group2.Name);
        _jeebkaService.AddTagToLink(user.Email, group.Name, link.Name,"tag1"); _jeebkaService.AddTagToLink(user.Email, group.Name, link2.Name,"tag1");
        _jeebkaService.AddTagToLink(user.Email, group.Name, link.Name,"tag2");
        _jeebkaService.AddTagToLink(user.Email, group2.Name, link.Name,"tag2");
        var userLinks = _jeebkaService.GetLinksByTags(groupId, new List<string>{"tag1"});
        Assert.True(userLinks.Count == 2);
        Assert.True(userLinks[0].Name == link.Name || userLinks[0].Name == link2.Name);
        Assert.True(userLinks[1].Name == link.Name || userLinks[1].Name == link2.Name);
    }

    [Test]
    public void ShouldGetSuggestedGroups()
    {
        var user1Link = CreateGenericLink();
        var user1Link2 = CreateGenericLink(2);
        var user2Link = CreateGenericLink(3);
        var user2Link2 = CreateGenericLink(4);
        var user1Group = CreateGenericGroup();
        var user2Group = CreateGenericGroup(2);
        var user2Group2 = CreateGenericGroup(3);
        var user1 = CreateGenericUser();
        var user2 = CreateGenericUser(2);
        _jeebkaService.CreateUser(user1); _jeebkaService.CreateUser(user2);
        _jeebkaService.CreateGroup(user1Group, user1.Email); _jeebkaService.CreateGroup(user2Group, user2.Email); _jeebkaService.CreateGroup(user2Group2, user2.Email);
        _jeebkaService.CreateLink(user1Link ,user1.Email,user1Group.Name);
        _jeebkaService.CreateLink(user1Link2 ,user1.Email,user1Group.Name);
        _jeebkaService.CreateLink(user2Link ,user2.Email,user2Group.Name); _jeebkaService.CreateLink(user2Link2 ,user2.Email,user2Group2.Name);
        _jeebkaService.AddTagToLink(user1.Email, user1Group.Name, user1Link.Name, "TagPrueba");
        _jeebkaService.AddTagToLink(user1.Email, user1Group.Name, user1Link2.Name, "TagPrueba2");
        _jeebkaService.AddTagToLink(user2.Email, user2Group.Name, user2Link.Name, "TagPrueba");
        _jeebkaService.AddTagToLink(user2.Email, user2Group.Name, user2Link.Name, "TagPrueba2");
        _jeebkaService.AddTagToLink(user2.Email, user2Group2.Name, user2Link2.Name, "TagPrueba2");
        var tagsToMatch = _jeebkaService.GetUsersTags(user1.Email);
        var groupsMatch = _jeebkaService.GetMostMatchingPublicGroupsByTags(user1.Email);
        foreach (var group in groupsMatch)
        {
            Assert.True(group.Public);
            Assert.True(tagsToMatch.Intersect(group.LinksTags.ToList()).Any());
            Assert.False(group.Members.Contains(user1.Email));
        }
    }
    
    //Generic entities

    private LinkDto CreateGenericLink(int n = 1)
    {
        var link = new LinkDto
        {
            Url = $"https://www.creationLinkTest.tst{n}.jbk",
            Name = $"GenericLinkNameTest{n}"
        };
        return link;
    }

    private static GroupDto CreateGenericGroup(int n = 1)
    {
        var group = new GroupDto()
        {
            Name = $"GenericGroupNameTest{n}",
            Description = "Mollit consequat sunt anim ut aliquip nulla excepteur fugiat labore quis et nulla culpa. Et eu labore nisi veniam enim amet est. Dolor in sint tempor pariatur. Incididunt aliqua commodo cupidatat esse tempor officia ad duis. Aliquip deserunt duis sint nulla Lorem eu ex magna. Sit deserunt enim aute tempor qui adipisicing consequat et labore qui anim qui anim. Excepteur pariatur adipisicing sit occaecat et reprehenderit excepteur quis quis eiusmod est ullamco est est.",
            Public = true,
            Color = "Azul"
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
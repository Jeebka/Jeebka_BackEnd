﻿using Domain.DTOs;
using Domain.Entities;
using Helper;
using Helper.Hasher;
using Infrastructure.Repositories;

namespace Business.Services;

public class JeebkaService
{
    private UserRepository _userRepository;
    private LinkRepository _linkRepository;
    private GroupRepository _groupRepository;
    private int _maxTagNumberByTag = 7;

    public JeebkaService(UserRepository userRepository, GroupRepository groupRepository, LinkRepository linkRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _linkRepository = linkRepository;
    }
    
    //Users
    public bool CreateUser(User user)
    {
        var notExists = _userRepository.GetUser(user.Email) == null;
        if (notExists)
        {
            HashedPassword hashedPassword = HashHelper.Hash(user.Password);
            user.Password = hashedPassword.Password + " " + hashedPassword.Salt;
            _userRepository.CreateUser(user);
        }
        return notExists;
    }

    public User? GetUserByEmail(string email)
    {
        return _userRepository.GetUser(email);
    }
    
    public void DeleteUserByEmail(string email)
    {
        _userRepository.DeleteUser(email);
    }

    //Groups

    public bool CreateGroup(Group group, string userEmail)
    {
        var notExists = _groupRepository.GetGroup(userEmail, group.Name) == null;
        if (notExists)
        {
            _groupRepository.CreateGroup(group);
            _userRepository.AddGroupToUserGroups(userEmail, group.Name);
            _groupRepository.AddUserToGroupMembers(group, userEmail);
        }

        return notExists;
    }

    public Group? GetGroup(string groupName, string userEmail)
    {
        return _groupRepository.GetGroup(userEmail, groupName);
    }

    public void DeleteGroup(string userEmail, string groupName)
    {
        _groupRepository.DeleteGroup(userEmail, groupName);
        _userRepository.DeleteGroupFromUserGroups(userEmail, groupName);
    }

    public bool ShareGroup(string groupName, string userEmail, string newOwnerEmail)
    {
        var userAdded = false;
        var group = GetGroup(groupName, userEmail);
        if (group != null)
        {
            userAdded = _groupRepository.AddUserToGroupMembers(group, newOwnerEmail);
            _userRepository.AddGroupToUserGroups(newOwnerEmail, groupName);
        }
        return userAdded;
    }
    
    public void UnshareGroup(string groupName, string userEmail, string oldOwnerEmail)
    {
        var group = GetGroup(groupName, userEmail);
        if (group != null)
        {
            _groupRepository.DeleteUserFromGroupMembers(group, oldOwnerEmail);
        }
    }
    
    public List<Group> GetGroupsUserOnlyMember(string userEmail)
    {
        return _groupRepository.GetGroupsUserOnlyMember(userEmail);
    }
    
    public List<Group> GetGroupsWhereUsersInMembers(string userEmail)
    {
        return _groupRepository.GetGroupsWhereUsersInMembers(userEmail);
    }

    //Links
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

    public void DeleteLink(string userEmail, string groupName, string linkId)
    {
        var link = _linkRepository.GetLink(linkId);
        foreach (var groupId in link.Groups)
        {
            _groupRepository.DeleteLinkFromGroup(groupId, linkId);
        }
        _linkRepository.DeleteLink(linkId);
    }

    public void DeleteLinkFromGroup(string userEmail, string groupName, string linkId)
    {
        var group = _groupRepository.GetGroup(userEmail, groupName);
        _groupRepository.DeleteLinkFromGroup(group.Id, linkId);
        _linkRepository.DeleteGroupFromLink(group.Id, linkId);
    }

    public bool AddTagToLink(string userEmail, string groupName, string linkName, string tagName)
    {
        var group = _groupRepository.GetGroup(userEmail, groupName);
        var link = _linkRepository.GetLinkByName(linkName, group.Id);
        var notExists = false;
        if (_maxTagNumberByTag < link.Tags.Count() + 1) return notExists;
        notExists = !link.Tags.Contains(tagName);
        if (notExists)
        {
            _linkRepository.AddTagToLink(link.Id, tagName);
            _groupRepository.AddTagToGroup(userEmail, groupName, tagName);
        }

        return notExists;
    }

    public void DeleteTagFromLink(string userEmail, string groupName, string linkName, string tagName)
    {
        var group = _groupRepository.GetGroup(userEmail, groupName);
        var link = _linkRepository.GetLinkByName(linkName, group.Id);
        _linkRepository.DeleteTagFromLink(link.Id, tagName);
    }
    
    //Queries

    public Dictionary<Group, int> GetMostMatchingPublicGroupsByTags(string userEmail)
    {
        var userTags = GetUsersTags(userEmail).ToList();
        return _groupRepository.GetMostMatchingPublicGroupsByTags(userEmail, userTags);
    }

    public HashSet<string> GetUsersTags(string userEmail)
    {
        var usersTags = new HashSet<string>();
        foreach (var groupName in _userRepository.GetUser(userEmail).Groups)
        {
            usersTags.UnionWith(_groupRepository.GetGroup(userEmail, groupName).LinksTags);
        }
        return usersTags;
    }

    public List<Link> GetLinksByTags(List<string> groups, List<string> tags)
    {
        return _linkRepository.GetLinksByTags(groups, tags).Result;
    }

    public List<Link> GetLinksByDateRange(List<string> groups, DateTime upperBound, DateTime lowerBound)
    {
        return _linkRepository.GetLinksByDateRange(groups, upperBound, lowerBound).Result;
    }

    public List<Link> GetLinksByName(List<string> groups, string name)
    {
        return _linkRepository.GetLinksByName(groups, name).Result;
    }

    public List<Link> GetLinksByUrl(List<string> groups, string url)
    {
        return _linkRepository.GetLinksByName(groups, url).Result;
    }

    public bool Login(UserDto user)
    {
        bool loged = false;
        User? authenticationUser = GetUserByEmail(user.Email);
        if (authenticationUser==null)  return loged;
        string[] hashAndSalt = authenticationUser.Password.Split(' ');
        string hash = hashAndSalt[0], salt = hashAndSalt[1];
        loged = HashHelper.CheckHash(user.Password, hash, salt);
        return loged;
    }
}
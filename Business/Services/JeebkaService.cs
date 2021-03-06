using Domain.DTOs;
using Domain.Entities;
using Domain.Extensions;
using Domain.Responses;
using Helper;
using Helper.Hasher;
using Infrastructure.Repositories;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Business.Services;

//I said this looks like a job for me
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

    public UserResponse? GetUserByEmail(string email)
    {
        var user = _userRepository.GetUser(email);
        var response = (UserResponse) user;
        foreach (var groupName in user.Groups)
        {
            var group = !groupName.IsNullOrEmpty() ? GetGroup(groupName, email) : null;
            if (group != null) response.Groups.Add(group);
        }
        
        return response;
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

    private GroupResponse? GetGroup(string groupId)
    {
        var group = _groupRepository.GetGroup(groupId);
        if (group == null) return null;
        var response = (GroupResponse) group;
        foreach (var linkId in group.Links)
        {
            var link = !linkId.IsNullOrEmpty() ? _linkRepository.GetLink(linkId) : null;
            if (link == null) continue;
            response.Links.Add(link);
            //response.LinksTags.AddRange(link.Tags);
        }

        return response;
    }
    
    public GroupResponse? GetGroup(string groupName, string userEmail)
    {
        var group = _groupRepository.GetGroup(userEmail, groupName);
        if (group == null) return null;
        var response = (GroupResponse) group;
        foreach (var linkId in group.Links)
        {
            var link = !linkId.IsNullOrEmpty() ? _linkRepository.GetLink(linkId) : null;
            if (link == null) continue;
            response.Links.Add(link);
            //response.LinksTags.AddRange(link.Tags);
        }

        return response;
    }

    public void DeleteGroup(string userEmail, string groupName)
    {
        var group = _groupRepository.GetGroup(userEmail, groupName);
        foreach (var linkId in group.Links)
        {
            _linkRepository.DeleteLink(linkId);
        }
        _groupRepository.DeleteGroup(userEmail, groupName);
        _userRepository.DeleteGroupFromUserGroups(userEmail, groupName);
    }

    public bool ShareGroup(string groupName, string userEmail, string newOwnerEmail)
    {
        var userAdded = false;
        var group = _groupRepository.GetGroup(userEmail, groupName);
        if (group != null)
        {
            userAdded = _groupRepository.AddUserToGroupMembers(group, newOwnerEmail);
            _userRepository.AddGroupToUserGroups(newOwnerEmail, groupName);
        }
        return userAdded;
    }
    
    public void UnshareGroup(string groupName, string userEmail, string oldOwnerEmail)
    {
        var group = _groupRepository.GetGroup(userEmail, groupName);
        if (group != null)
        {
            _groupRepository.DeleteUserFromGroupMembers(group, oldOwnerEmail);
        }
    }
    
    public List<GroupResponse> GetGroupsUserOnlyMember(string userEmail)
    {
        var groups = _groupRepository.GetGroupsUserOnlyMember(userEmail);
        var response = new List<GroupResponse>();
        foreach (var group in groups)
        {
            var responseGroup = (GroupResponse) group;
            foreach (var linkId in group.Links)
            {
                var link = !linkId.IsNullOrEmpty() ? _linkRepository.GetLink(linkId) : null;
                if (link != null)
                {
                    responseGroup.Links.Add(link);
                    //responseGroup.LinksTags.AddRange(link.Tags);
                }
            }
            response.Add(responseGroup);
        }
        
        return response;
    }
    
    public List<GroupResponse> GetGroupsWhereUsersInMembers(string userEmail)
    {
        var groups = _groupRepository.GetGroupsWhereUsersInMembers(userEmail);
        var response = new List<GroupResponse>();
        foreach (var group in groups)
        {
            var responseGroup = (GroupResponse) group;
            foreach (var linkId in group.Links)
            {
                var link = !linkId.IsNullOrEmpty() ? _linkRepository.GetLink(linkId) : null;
                if (link != null)
                {
                    responseGroup.Links.Add(link);
                    //responseGroup.LinksTags.AddRange(link.Tags);
                }
            }
            response.Add(responseGroup);
        }
        
        return response;
    }

    //Links
    public bool CreateLink(Link link, string userEmail, string groupName)
    {
        var group = _groupRepository.GetGroup(userEmail, groupName);
        var notExists = _linkRepository.ValidateLinkInGroup(link.Name, link.Url, group.Id);
        if (notExists)
        {
            link.Tags = link.Tags.Select(tag => tag.ToLower()).ToList();
            _linkRepository.CreateLink(link, out var linkId);
            _groupRepository.AddLinkToGroup(group.Id, linkId);
            _linkRepository.AddGroupToLink(group.Id, linkId);
            foreach (var linkTag in link.Tags)
            {
                _groupRepository.AddTagToGroup(userEmail,groupName,linkTag);
            }
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
        tagName = tagName.ToLower();
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

    public List<GroupResponse> GetMostMatchingPublicGroupsByTags(string userEmail)
    {
        var userTags = GetUsersTags(userEmail).ToList();
        var matches = _groupRepository.GetMostMatchingPublicGroupsByTags(userEmail, userTags);
        var responses = new List<GroupResponse>();
        foreach (var match in matches)
        {
            responses.Add(GetGroup(match.Id));
        }

        return responses;
    }

    public HashSet<string> GetUsersTags(string userEmail)
    {
        var usersTags = new HashSet<string>();
        var groups = _userRepository.GetUser(userEmail)?.Groups;
        groups ??= new List<string>();
        foreach (var groupName in groups)
        {
            var groupTags = GetGroup(groupName, userEmail)?.LinksTags;
            if(groupTags != null) usersTags.AddRange(groupTags);
        }
        return usersTags;
    }

    public List<Link> GetLinksByTags(string group, List<string> tags)
    {
        return _linkRepository.GetLinksByTags(group, tags);
    }

    public void UpdateLink(string email, string groupName, string name, Link updatedLink)
    {
        var group = _groupRepository.GetGroup(email, groupName);
        var oldLink = _linkRepository.GetLinkByName(name, group.Id);
        if (oldLink == null) return;
        if (!_linkRepository.ValidateLinkInGroup(updatedLink.Name, DateTime.Now.ToString(), group.Id))
        {
            oldLink.Name = updatedLink.Name;
            oldLink.Tags = updatedLink.Tags;
            _linkRepository.UpdateLink(oldLink.Id, oldLink);
        }
        
    }

    public bool Login(UserDto user)
    {
        bool loged = false;
        User? authenticationUser = _userRepository.GetUser(user.Email);
        if (authenticationUser==null)  return loged;
        string[] hashAndSalt = authenticationUser.Password.Split(' ');
        string hash = hashAndSalt[0], salt = hashAndSalt[1];
        loged = HashHelper.CheckHash(user.Password, hash, salt);
        return loged;
    }

    public List<Link> GetAllLinks()
    {
        return _linkRepository.GetAllLinks();
    }
}
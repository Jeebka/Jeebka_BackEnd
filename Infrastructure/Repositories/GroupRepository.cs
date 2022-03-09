using Domain.Entities;
using Infrastructure.Clients;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class GroupRepository
{
    private IMongoCollection<Group> _collection;

    public GroupRepository(IMongoCollection<Group> groupsCollection)
    {
        _collection = groupsCollection;
    }
    
    public void CreateGroup(Group group)
    {
        _collection.InsertOne(group);
    }

    private Group? GetGroup(FilterDefinition<Group> filter)
    {
        var groupFound = _collection.Find(filter);
        return groupFound.Any() ? groupFound.First() : null;
    }

    public Group? GetGroup(string groupId)
    {
        var findByIdFilter = Builders<Group>.Filter.Eq("Id", groupId);
        return GetGroup(findByIdFilter);
    }

    public Group? GetGroup(string userEmail, string groupName)
    {
        var findByNameAndUserFilter = Builders<Group>.Filter.AnyEq("Members", userEmail) &
                                      Builders<Group>.Filter.Eq("Name", groupName);
        return GetGroup(findByNameAndUserFilter);
    }

    public Dictionary<Group, int> GetMostMatchingPublicGroupsByTags(string userEmail, List<string> tagsToMatch)
    {
        var groupsWithAMatch = new List<Group>();
        foreach (var tagToMatch in tagsToMatch)
        {
            var findByNameAndUserFilter = Builders<Group>.Filter.Eq("Public", true) 
                                          & Builders<Group>.Filter.AnyEq("linksTags", tagToMatch)
                                          & !Builders<Group>.Filter.AnyEq("Members", userEmail);
            groupsWithAMatch = _collection.Find(findByNameAndUserFilter).ToList();
            
        }
        
        var maxMatchGroups = new Dictionary<Group, int>();
        foreach (var group in groupsWithAMatch)
        {
            maxMatchGroups.Add(group, 0);
            foreach (var linksTags in @group.LinksTags.Where(linksTags => tagsToMatch.Contains(linksTags)))
            {
                maxMatchGroups[@group]++;
            }
        }

        return new Dictionary<Group, int>(maxMatchGroups.OrderByDescending(key => key.Value));
        
    }

    public void DeleteGroup(string userEmail, string groupName)
    {
        var group = GetGroup(userEmail, groupName);
        if (group == null) return;
        if(group.Members.Count == 1 && DeleteUserFromGroupMembers(group, userEmail)) DeleteGroup(group.Id);
    }

    private void DeleteGroup(string groupId)
    {
        var findByIdFilter = Builders<Group>.Filter.Eq("Id", groupId);
        _collection.DeleteOne(findByIdFilter);
    }
    
    public bool DeleteUserFromGroupMembers(Group group, string userEmail)
    {
        var userDeleted = false;
        if (group.Members.Contains(userEmail))
        { 
            var findById = Builders<Group>.Filter.Eq("Id", group.Id);
            var deleteUserUpdate = Builders<Group>.Update.Pull("Members", userEmail);
            _collection.UpdateOne(findById, deleteUserUpdate);
            userDeleted = true;
        }

        return userDeleted;
    }
    
    public bool AddUserToGroupMembers(Group group, string userEmail)
    {
        var userAdded = false;
        if (!group.Members.Contains(userEmail))
        { 
            var findById = Builders<Group>.Filter.Eq("Id", group.Id);
            var addUserUpdate = Builders<Group>.Update.Push("Members", userEmail);
            _collection.UpdateOne(findById, addUserUpdate);
            userAdded = true;
        }

        return userAdded;
    }
    public void AddLinkToGroup(string groupId, string linkId)
    {
        var findByIdFilter = Builders<Group>.Filter.Eq("Id", groupId);
        var addLinkUpdate = Builders<Group>.Update.Push("Links", linkId);
        _collection.UpdateOne(findByIdFilter, addLinkUpdate);
    }

    public void AddTagToGroup(string userEmail, string groupName, string tagName)
    {
        var group = GetGroup(userEmail, groupName);
        group?.LinksTags.Add(tagName);
        var addTagToGroupUpdate = Builders<Group>.Update.Set("LinksTags", group?.LinksTags);
            var findById = Builders<Group>.Filter.Eq("Id", group.Id);
        _collection.UpdateOne(findById, addTagToGroupUpdate);
    }

    public void DeleteLinkFromGroup(string groupId, string linkId)
    {
        var findByIdFilter = Builders<Group>.Filter.Eq("Id", groupId);
        var deleteLinkUpdate = Builders<Group>.Update.Pull("Links", linkId);
        _collection.UpdateOne(findByIdFilter, deleteLinkUpdate);
    }
}
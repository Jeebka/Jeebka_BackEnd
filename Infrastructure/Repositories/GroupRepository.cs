using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class GroupRepository : IDisposable
{
    private IMongoCollection<Group> _collection; 
    public GroupRepository(string urlConnection){}

    public Group GetGroup(string groupId)
    {
        return new Group();
    }

    public void AddLinkToGroup(string groupId, string linkId)
    {
        var findByIdFilter = Builders<Group>.Filter.Eq("Id",groupId);
        var addLinkUpdate = Builders<Group>.Update.Push("Links", linkId);
        _collection.UpdateOne(findByIdFilter, addLinkUpdate);
    }
    
    public void DeleteLinkFromGroup(string groupId, string linkId)
    {
        var findByIdFilter = Builders<Group>.Filter.Eq("Id",groupId);
        var deleteLinkUpdate = Builders<Group>.Update.Pull("Links", linkId);
        _collection.UpdateOne(findByIdFilter, deleteLinkUpdate);
    }

    public void Dispose()
    {
    }
}
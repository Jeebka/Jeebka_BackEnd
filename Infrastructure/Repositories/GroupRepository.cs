using Domain.Entities;
using Infrastructure.Clients;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class GroupRepository
{
    private readonly MongoClient _mongoClient;
    private IMongoDatabase _database;
    private IMongoCollection<Group> _collection;
    private LinkRepository _linkRepository;

    public GroupRepository(LinkRepository linkRepository, IMongoCollection<Group> groupsCollection)
    {
        _linkRepository = linkRepository;
        _collection = groupsCollection;
    }
    
    
    public void AddLinkToGroup(string groupId, string linkId)
    {
        var findByIdFilter = Builders<Group>.Filter.Eq("Id",groupId);
        var addLinkUpdate = Builders<Group>.Update.Push("Links", linkId);
        _collection.UpdateOne(findByIdFilter, addLinkUpdate);
    }
    
    
    }
    
    
}
using Domain.Entities;
using Infrastructure.Clients;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class LinkRepository
{
    
    private IMongoCollection<Link> _collection;

    public LinkRepository(IMongoCollection<Link> linksCollection)
    {
        _collection = linksCollection;
    }

    public async Task<List<Link>> GetLinks()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public void CreateLink(Link link, out string linkId)
    {
        link.Id = ObjectId.GenerateNewId().ToString();
        linkId = link.Id;
        _collection.InsertOne(link);
    }
    
    public void CreateLink(Link link)
    {
        link.Id = ObjectId.GenerateNewId().ToString();
        _collection.InsertOne(link);
    }

    public Link? GetLink(string linkId)
    {
        var findByIdFilter = Builders<Link>.Filter.Eq("Id", linkId);
        return _collection.Find(findByIdFilter).First();
    }
    
    public void AddLinkToGroup(string groupId, string linkId)
    {
        var findByIdFilter = Builders<Link>.Filter.Eq("Id",linkId);
        var addLinkUpdate = Builders<Link>.Update.Push("Groups", groupId);
        _collection.UpdateOne(findByIdFilter, addLinkUpdate);
    }
}
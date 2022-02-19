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

    public void CreateLink(Link link, out string linkId)
    {
        linkId = link.Id;
        _collection.InsertOne(link);
    }

    private Link? GetLink(FilterDefinition<Link> filter)
    {
        var linkFound = _collection.Find(filter);
        return linkFound.Any() ? linkFound.First() : null;
    }
    
    public Link? GetLink(string linkId)
    {
        var findByIdFilter = Builders<Link>.Filter.Eq("Id", linkId);
        return GetLink(findByIdFilter);
    }
    
    public bool ValidateLinkInGroup(string name, string url, string groupId)
    {
        var findByNameOrUrl = (Builders<Link>.Filter.Eq("Name", name) | Builders<Link>.Filter.Eq("Url", url)) &
                              Builders<Link>.Filter.AnyEq("Groups", groupId);
        return GetLink(findByNameOrUrl) == null;
    }
    
    public Link? GetLinkByName(string name, string groupId)
    {
        var findByNameOrUrl = Builders<Link>.Filter.Eq("Name", name) &
                              Builders<Link>.Filter.AnyEq("Groups", groupId);
        return GetLink(findByNameOrUrl);
    }
    
    public Link? GetLinkByUrl(string url, string groupId)
    {
        var findByNameOrUrl = Builders<Link>.Filter.Eq("Url", url) &
                              Builders<Link>.Filter.AnyEq("Groups", groupId);
        return GetLink(findByNameOrUrl);
    }

    public void AddGroupToLink(string groupId, string linkId)
    {
        var findByIdFilter = Builders<Link>.Filter.Eq("Id", linkId);
        var addLinkUpdate = Builders<Link>.Update.Push("Groups", groupId);
        _collection.UpdateOne(findByIdFilter, addLinkUpdate);
    }
}
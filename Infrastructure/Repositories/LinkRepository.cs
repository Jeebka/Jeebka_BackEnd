using System.Collections.Immutable;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class LinkRepository
{
    private readonly IMongoCollection<Link> _collection;

    public LinkRepository(IMongoCollection<Link> linksCollection)
    {
        _collection = linksCollection;
    }

    public void CreateLink(Link link, out string linkId)
    {
        linkId = link.Id;
        _collection.InsertOne(link);
    }

    public List<Link> GetAllLinks()
    {
        return _collection.Find(_ => true).ToList();
    }

    public void UpdateLink(string linkId, Link link)
    {
        var findByIdFilter = Builders<Link>.Filter.Eq("Id", linkId);
        _collection.ReplaceOne(findByIdFilter, link);
    }

    public void DeleteLink(string linkId)
    {
        var findByIdFilter = Builders<Link>.Filter.Eq("Id", linkId);
        _collection.DeleteOne(findByIdFilter);
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
        var findByNameOrUrl = Builders<Link>.Filter.Eq("Name", name);
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

    public void DeleteGroupFromLink(string groupId, string linkId)
    {
        var findByIdFilter = Builders<Link>.Filter.Eq("Id", linkId);
        var addLinkUpdate = Builders<Link>.Update.Pull("Groups", groupId);
        _collection.UpdateOne(findByIdFilter, addLinkUpdate);
    }

    public void AddTagToLink(string linkId, string tag)
    {
        var findByIdFilter = Builders<Link>.Filter.Eq("Id", linkId);
        var addTagUpdate = Builders<Link>.Update.Push("Tags", tag);
        _collection.UpdateOne(findByIdFilter, addTagUpdate);
    }

    public void DeleteTagFromLink(string linkId, string tag)
    {
        var findByIdFilter = Builders<Link>.Filter.Eq("Id", linkId);
        var deleteTagUpdate = Builders<Link>.Update.Pull("Tags", tag);
        _collection.UpdateOne(findByIdFilter, deleteTagUpdate);
    }

    //QUERIES
    
    public List<Link> GetLinksByTags(string group, List<string> tags)
    {
        var findByGroupsAndTags = Builders<Link>.Filter.AnyEq("groups", group);
        foreach (var tag in tags)
        {
            findByGroupsAndTags &= Builders<Link>.Filter.AnyEq("tags", tag);        
        }
        return _collection.Find(findByGroupsAndTags).ToList();
        
    }
}
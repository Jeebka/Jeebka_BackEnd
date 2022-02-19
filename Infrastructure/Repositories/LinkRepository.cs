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
    
    public async Task<List<Link>> GetLinksByTags(List<string> groups, List<string> tags)
    {
        var findByGroupsAndTags = Builders<Link>.Filter.Eq("tags", tags) &
                                  Builders<Link>.Filter.AnyIn("groups", groups);
        return await _collection.Find(findByGroupsAndTags).ToListAsync();
    }

    public async Task<List<Link>> GetLinksByDateRange(List<string> groups, DateTime upperBound, DateTime lowerBound)
    {
        var findByGroupsAndDates = Builders<Link>.Filter.AnyIn("groups", groups) &
                                   Builders<Link>.Filter.Gte("date", lowerBound) &
                                   Builders<Link>.Filter.Lte("date", upperBound);
        return await _collection.Find(findByGroupsAndDates).ToListAsync();
    }

    public async Task<List<Link>> GetLinksByName(List<string> groups, string name)
    {
        var findByGroupsAndName = Builders<Link>.Filter.AnyIn("groups", groups) &
                                   Builders<Link>.Filter.Regex("name", $"(.*)({name})(.*)");
        return await _collection.Find(findByGroupsAndName).ToListAsync();
    }
    
    public async Task<List<Link>> GetLinksByUrl(List<string> groups, string url)
    {
        var findByGroupsAndName = Builders<Link>.Filter.AnyIn("groups", groups) &
                                  Builders<Link>.Filter.Regex("url", $"(.*)({url})(.*)");
        return await _collection.Find(findByGroupsAndName).ToListAsync();
    }
}
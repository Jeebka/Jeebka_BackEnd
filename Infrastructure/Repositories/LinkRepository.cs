using Domain.Entities;
using Infrastructure.Clients;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class LinkRepository
{
    private readonly string _urlConnection;
    private readonly MongoClient _mongoClient;
    private IMongoDatabase _database;
    private IMongoCollection<Link> _collection;

    public LinkRepository(string urlConnection)
    {
        _urlConnection = urlConnection;
        _mongoClient = MongoDBClient.GetConnection(urlConnection);
        _database = _mongoClient.GetDatabase("JeebkaDB");
        _collection = _database.GetCollection<Link>("Link");
    }

    public async Task<List<Link>> GetLinks()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public void CreateLink(Link link)
    {
        link.Id = ObjectId.GenerateNewId().ToString();
        _collection.InsertOne(link);
    }

    public void AddLinkToGroup(string linkId, string groupId)
    {
        using var groupRepo = new GroupRepository(_urlConnection);
        groupRepo.AddLinkToGroup(groupId, linkId);
    }

    public async Task DeleteLink(string linkId)
    {
        using var groupRepo = new GroupRepository(_urlConnection);
        var findByIdFilter = Builders<Link>.Filter.Eq("id", linkId);
        var link = GetLink(linkId);
        if (link != null)
        {
            foreach (var groupId in link.Groups)
            {
                groupRepo.DeleteLinkFromGroup(groupId, linkId);
            }
            await _collection.DeleteOneAsync(findByIdFilter);
        }
    }

    public Link? GetLink(string linkId)
    {
        var findByIdFilter = Builders<Link>.Filter.Eq("id", linkId);
        var response = _collection.FindSync(findByIdFilter);
        return (response != null && response.Current.Any()) ? response.Current.First() : null;
    }

    public async Task DeleteLinkFromGroup(string linkId, string groupId)
    {
        using var groupRepo = new GroupRepository(_urlConnection);
        groupRepo.DeleteLinkFromGroup(groupId, linkId);
    }
}
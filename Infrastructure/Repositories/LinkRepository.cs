using Domain.Entities;
using Infrastructure.Clients;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class LinkRepository
{
    public readonly MongoClient _mongoClient;
    private IMongoDatabase _database;
    private IMongoCollection<Link> _collection;

    public LinkRepository(string urlConnection)
    {
        _mongoClient = MongoDBClient.GetConnection(urlConnection);
        _database = _mongoClient.GetDatabase("JeebkaDB");
        _collection = _database.GetCollection<Link>("Link");
    }

    public void CreateLink(Link link)
    {
         _collection.InsertOne(link);
    }

    public void AddLinkToGroup( Link linkId ,String groupId)
    {
        
    }
}
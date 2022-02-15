using Domain.Entities;
using Infrastructure.Clients;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class UserRepository
{
    private readonly MongoClient _mongoClient;
    private IMongoDatabase _database;
    private IMongoCollection<User> _collection;

    public UserRepository(string urlConnection)
    {
        _mongoClient = MongoDBClient.GetConnection(urlConnection);
        _database = _mongoClient.GetDatabase("JebkaDB");
        _collection = _database.GetCollection<User>("User");
    }
}
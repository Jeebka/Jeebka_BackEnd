using Domain.Entities;
using Infrastructure.Clients;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class UserRepository
{
    private readonly MongoClient _mongoClient;
    private IMongoDatabase _database;
    private IMongoCollection<User> _collection;

    public UserRepository(string urlConnection, string dataBase, string collectionName)
    {
        _mongoClient = MongoDBClient.GetConnection(urlConnection);
        _database = _mongoClient.GetDatabase("JeebkaDB");
        _collection = _database.GetCollection<User>("User");
    }
    
    public void CreateUser(User user)
    {
        _collection.InsertOne(user);
    }

    public void DeleteUser(string userId)
    {
        var deleteFilter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(userId));
        _collection.DeleteOne(deleteFilter);

    }

}
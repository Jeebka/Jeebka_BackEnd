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

    public User? GetUser(string email)
    {
        var findByEmailFilter = Builders<User>.Filter.Eq("email", email);
        var response = _collection.FindSync(findByEmailFilter);
        return (response != null && response.Current.Any()) ? response.Current.First() : null;
    }

    public void DeleteUser(string email)
    {
        var deleteFilter = Builders<User>.Filter.Eq("email", email);
        _collection.DeleteOne(deleteFilter);
    }

}
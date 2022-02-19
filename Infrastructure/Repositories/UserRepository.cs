using System.Text.Json;
using Domain.Entities;
using Infrastructure.Clients;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class UserRepository
{
    private IMongoCollection<User> _collection;

    public UserRepository(IMongoCollection<User> userCollection)
    {
        _collection = userCollection;
    }
    
    public void CreateUser(User user)
    {
        _collection.InsertOne(user);
    }

    public User? GetUser(string email)
    {
        var findByEmailFilter = Builders<User>.Filter.Eq("email", email);
        var userFound = _collection.Find(findByEmailFilter);
        return userFound.Any() ? userFound.First() : null;
    }

    public void DeleteUser(string email)
    {
        var deleteFilter = Builders<User>.Filter.Eq("email", email);
        _collection.DeleteOne(deleteFilter);
    }
    
    public bool AddGroupToUserGroups(string userEmail, string groupName)
    {
        var groupAdded = false;
        var user = GetUser(userEmail);
        if (user != null && !user.Groups.Contains(groupName))
        {
            var findByEmailFilter = Builders<User>.Filter.Eq("Email", userEmail);
            var addGroupUpdate = Builders<User>.Update.Push("Groups", groupName);
            _collection.UpdateOne(findByEmailFilter, addGroupUpdate);
            groupAdded = true;
        }
        
        return groupAdded;
    }
    
    public void DeleteGroupFromUserGroups(string userEmail, string groupName)
    {
        var findByEmailFilter = Builders<User>.Filter.Eq("Email", userEmail);
        var deleteLinkUpdate = Builders<User>.Update.Pull("Groups", groupName);
        _collection.UpdateOne(findByEmailFilter, deleteLinkUpdate);
    }

}
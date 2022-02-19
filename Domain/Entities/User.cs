using Domain.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    private string Id { get; set; }
    
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("password")]
    public string Password { get; set; }
    
    [BsonElement("email")]
    public string Email { get; set; }
    
    [BsonElement("groups")]
    private List<string> Groups { get; set; }
    
    public static implicit operator User(UserDto userDto)
    {
        return new User
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Email = userDto.Email,
            Groups = new List<string>(),
            Name = userDto.Name,
            Password = userDto.Password
        };
    }
}
using Domain.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Group
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("description")]
    public string Description { get; set; }
    
    [BsonElement("members")]
    public List<string> Members { get; set; }
    
    [BsonElement("links")]
    public List<string> Links { get; set; }
    
    public static implicit operator Group(GroupDto groupDto)
    {
        return new Group
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Description = groupDto.Description,
            Links = new List<string>(),
            Members = new List<string>(),
            Name = groupDto.Name
        };
    }
}
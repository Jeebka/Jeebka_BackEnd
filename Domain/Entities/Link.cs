using Domain.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Link
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("url")]
    public string Url { get; set; }
    
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("date")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime Date { get; set; }
    
    [BsonElement("groups")]
    public List<string> Groups { get; set; }
    
    [BsonElement("tags")]
    public IEnumerable<string> Tags { get; set; }
    
    public static implicit operator Link(LinkDto linkDto)
    {
        return new Link
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Groups = new List<string>(),
            Name = linkDto.Name,
            Date = linkDto.Date,
            Tags = new List<string>(),
            Url = linkDto.Url
        };
    }
}
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
    
}
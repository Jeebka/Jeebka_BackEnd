using Domain.Entities;
using MongoDB.Bson;

namespace Domain.Responses;

public class LinkResponse
{
    public string Id { get; set; }
    public string Url { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public IEnumerable<string> Tags { get; set; }
    
    public static implicit operator LinkResponse(Link link)
    {
        return new LinkResponse()
        {
            Id = link.Id,
            Url = link.Url,
            Name = link.Name,
            Date = link.Date,
            Tags = link.Tags
        };
    }
}
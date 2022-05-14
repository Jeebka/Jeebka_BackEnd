using System.Text.Json;
using Domain.Entities;

namespace Domain.Responses;

public class GroupResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Public { get; set; }
    public string Color { get; set; }
    public List<LinkResponse> Links { get; set; }
    public IEnumerable<string> LinksTags { get; set; }
    
    public static implicit operator GroupResponse(Group group)
    {
        Console.WriteLine(JsonSerializer.Serialize(group));
        return new GroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            Public =  group.Public,
            Color = group.Color,
            LinksTags = group.LinksTags,
            Links = new List<LinkResponse>()
        };
    }

}
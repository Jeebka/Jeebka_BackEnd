using Domain.Entities;

namespace Presentation.Request;

public class LinkUpdateRequest
{
    public string Name { get; set; }
    public IEnumerable<string> Tags { get; set; }

    public static explicit operator Link(LinkUpdateRequest request)
    {
        return new Link
        {
            Name = request.Name,
            Tags = request.Tags,
            Date = DateTime.Now
        };
    }
}
namespace Domain.DTOs;

public class TaggedLinkDto
{
    public string Url { get; set; }
    public string Name { get; set; }
    public List<string> Tags { get; set; }
}
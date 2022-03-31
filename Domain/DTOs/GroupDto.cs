namespace Domain.DTOs;

[Serializable]
public class GroupDto
{
    public string Name { get; set; }

    public string Description { get; set; }
    
    public bool Public { get; set; }
    
    public string Color { get; set; }

}
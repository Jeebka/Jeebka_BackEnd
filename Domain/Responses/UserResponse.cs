using Domain.Entities;
using MongoDB.Bson;

namespace Domain.Responses;

public class UserResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public List<GroupResponse> Groups { get; set; }
    
    public static implicit operator UserResponse(User user)
    {
        return new UserResponse()
        {
            Id = user.Id,
            Name = user.Name,
            Password = user.Password,
            Email = user.Email,
            Groups = new List<GroupResponse>()
        };
    }
}
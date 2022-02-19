using Domain.Entities;
using MongoDB.Bson;

namespace Domain.DTOs;

[Serializable]
public class UserDto
{
    public string Name { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }
}

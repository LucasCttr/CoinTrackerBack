using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace coninTracker.API.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;
    
    [BsonElement("password")]
    public string Password { get; set; } = string.Empty; // Hash
    
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;
}

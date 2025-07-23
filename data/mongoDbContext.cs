using MongoDB.Driver;
using coninTracker.API.Models;

namespace coninTracker.API.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    
    public MongoDbContext(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }
    
    public IMongoCollection<User> Users => 
        _database.GetCollection<User>("Users");
}
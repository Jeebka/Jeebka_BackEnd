using MongoDB.Driver;

namespace Infrastructure.Clients;

public class MongoDBClient
{
    public static MongoClient GetConnection(string urlConnection)
    {
        var settings = MongoClientSettings.FromConnectionString(urlConnection);
        return new MongoClient(settings);
    } 
}
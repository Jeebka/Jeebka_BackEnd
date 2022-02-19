using Business.Services;
using Domain.Entities;
using Infrastructure.Clients;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

//Environment
Environment.SetEnvironmentVariable("MongoConnection", "mongodb+srv://dbUser:dbUserPassword@jeebkadb.6qdxo.mongodb.net/JeebkaDB?retryWrites=true&w=majority");
Environment.SetEnvironmentVariable("DataBase", "JeebkaDB");
Environment.SetEnvironmentVariable("UserRepositoryCollectionName", "User");
Environment.SetEnvironmentVariable("GroupRepositoryCollectionName", "Group");
Environment.SetEnvironmentVariable("LinkRepositoryCollectionName", "Link");
var mongoClient = MongoDBClient.GetConnection(Environment.GetEnvironmentVariable("MongoConnection"));
var database = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("DataBase"));
var linkCollection = database.GetCollection<Link>(Environment.GetEnvironmentVariable("LinkRepositoryCollectionName"));
var groupCollection = database.GetCollection<Group>(Environment.GetEnvironmentVariable("GroupRepositoryCollectionName"));
var userCollection = database.GetCollection<User>(Environment.GetEnvironmentVariable("UserRepositoryCollectionName"));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<UserRepository>(new UserRepository(userCollection));
builder.Services.AddSingleton<GroupRepository>(new GroupRepository(groupCollection));
builder.Services.AddSingleton<LinkRepository>(new LinkRepository(linkCollection));

builder.Services.AddSingleton<JeebkaService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
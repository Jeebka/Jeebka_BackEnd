using System.Text;
using Business.Services;
using Domain.Entities;
using Infrastructure.Clients;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//Environment
Environment.SetEnvironmentVariable("MongoConnection", "mongodb+srv://dbUser:dbUserPassword@jeebkadb.6qdxo.mongodb.net/JeebkaDB?retryWrites=true&w=majority");
Environment.SetEnvironmentVariable("DataBase", "JeebkaDB");
Environment.SetEnvironmentVariable("UserRepositoryCollectionName", "User");
Environment.SetEnvironmentVariable("GroupRepositoryCollectionName", "Group");
Environment.SetEnvironmentVariable("LinkRepositoryCollectionName", "Link");
Environment.SetEnvironmentVariable("key", "f8aed011-f29c-4f7f-9916-c0be7bb5839b");
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
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    jwt.RequireHttpsMetadata = false;
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("key"))),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
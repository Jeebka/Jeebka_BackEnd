using Business.Services;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

Environment.SetEnvironmentVariable("MongoConnection", "mongodb+srv://dbUser:dbUserPassword@jeebkadb.6qdxo.mongodb.net/JeebkaDB?retryWrites=true&w=majority");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<UserRepository>(
    new UserRepository(Environment.GetEnvironmentVariable("MongoConnection")));
builder.Services.AddSingleton<LinkRepository>(
    new LinkRepository(Environment.GetEnvironmentVariable("MongoConnection")));
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
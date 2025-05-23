using ApiWeb.Data;
using ApiWeb.Controllers;
using ApiWeb.Services;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

//Links the class MongoDBSettings with the properties in appsetting.json
builder.Services.Configure<MongoDBSettings>(
builder.Configuration.GetSection("MongoDB"));

//Links the class CassandraSettings with the properties in appsetting.json
builder.Services.Configure<CassandraSettings>(
    builder.Configuration.GetSection("Cassandra"));

//RepositorioDBContext needs to be Singleton, otherwise it crashes
builder.Services.AddSingleton<RepositoryService>();
builder.Services.AddSingleton<CommentService>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<UserSessionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();



/*
  var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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
*/
using CinephoriaBackEnd.Configurations;
using CinephoriaBackEnd;
using CinephoriaBackEnd.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les fichiers de configuration (appsettings.json et appsettings.{Environment}.json)
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// En développement, ajouter les secrets utilisateurs
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}



// Ajout de CinepholiaDbContext pour la base de données PostgreSQL
builder.Services.AddDbContext<CinephoriaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql")));


// Configuration de MongoDbSettings avec les paramètres MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Ajout de MongoDbContext en tant que service Singleton
builder.Services.AddSingleton<MongoDbContext>();


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

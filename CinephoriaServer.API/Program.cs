using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Configurations.Extensions;
using CinephoriaServer.API.Data;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Services;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Npgsql;
using Serilog;
using System.Reflection;
using System.Text;
using System.Text.Json;




// Configurer le chemin racine pour les fichiers statiques si besoin
var options = new WebApplicationOptions
{
    ContentRootPath = AppContext.BaseDirectory,
    WebRootPath = "wwwroot"
};
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Charger le fichier .env en premier (pour le développement local)
if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>(optional: true);

// Configuration Vault
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IVaultService, VaultService>();

// Configuration Vault pour les secrets

// Ajouter le fournisseur de configuration Vault (désactivé en développement local)
if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddVaultConfiguration(builder.Services.BuildServiceProvider());
}
// Configuration de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/CinephoriaLog.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configuration de la base de données
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL") ??
                      builder.Configuration.GetConnectionString("PostgreSql") ??
                      builder.Configuration.GetConnectionString("PostgreSqlProd");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'PostgreSQL' not found.");
}

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<CinephoriaDbContext>(options =>
        options.UseNpgsql(connectionString,
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure())
               .EnableSensitiveDataLogging()
               .LogTo(Console.WriteLine, LogLevel.Information));
}
else
{
    builder.Services.AddDbContext<CinephoriaDbContext>(options =>
        options.UseNpgsql(connectionString,
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure())
               .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
               .LogTo(Console.WriteLine, LogLevel.Warning));
}

//Configuration de MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

// Ajout de MongoDbContext en tant que service Singleton
builder.Services.AddSingleton<MongoDbContext>();

// Enregistrement d'IMongoDatabase dans le conteneur DI
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = new MongoClient(settings.ConnectionString);
    return client.GetDatabase(settings.DatabaseName);
});

// Ajout de  Identity
builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<CinephoriaDbContext>()
    .AddDefaultTokenProviders();

// Configuration de Identity renforcée
builder.Services.Configure<IdentityOptions>(options =>
{
    // Politique de mots de passe forte
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 3;
    
    // Configuration de verrouillage de compte
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // Configuration utilisateur
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
    
    // Configuration de connexion
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
});

// Configuration des Health Checks
var mongoConnectionString = builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration.GetSection("MongoDbSettings:DatabaseName").Value ?? "CinephoriaDashboardDB";

builder.Services.AddHealthChecks()
    .AddCheck("postgresql", (cancellationToken) =>
    {
        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.ExecuteScalar();
            return HealthCheckResult.Healthy("PostgreSQL is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL is unhealthy", ex);
        }
    }, tags: new[] { "database", "ready" })
    .AddCheck("mongodb", (cancellationToken) =>
    {
        try
        {
            var client = new MongoClient(mongoConnectionString);
            var database = client.GetDatabase(mongoDatabaseName);
            database.RunCommand<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument("ping", 1));
            return HealthCheckResult.Healthy("MongoDB is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB is unhealthy", ex);
        }
    }, tags: new[] { "database", "ready" })
    .AddCheck("memory", () =>
    {
        var totalMemory = GC.GetTotalMemory(false) / 1024 / 1024; // MB
        
        if (totalMemory > 500) // 500MB threshold
            return HealthCheckResult.Degraded($"Memory usage high: {totalMemory}MB");
        
        return HealthCheckResult.Healthy($"Memory usage: {totalMemory}MB");
    }, tags: new[] { "live" });

// Gérer les injections de dépendances
builder.Services.AddDbServiceInjection();
 

// Exiger la confirmation d'email
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmailConfirmed", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "EmailConfirmed" && c.Value == "true")));
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});

//Configuration d'AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configuration et enregistrement du service TMDb
builder.Services.Configure<TMDbSettings>(builder.Configuration.GetSection("TMDbSettings"));
builder.Services.AddHttpClient<ITMDbService, TMDbService>();
builder.Services.AddScoped<TMDbMovieMapper>();
// Enregistrement des services de gestion des statuts des séances
builder.Services.AddScoped<IShowtimeStatusService, ShowtimeStatusService>();
builder.Services.AddHostedService<ShowtimeStatusBackgroundService>();

// Enregistrement des services de gestion de l'expiration des réservations
builder.Services.AddScoped<IReservationExpirationService, ReservationExpirationService>();
builder.Services.AddHostedService<ReservationExpirationBackgroundService>();

// Enregistrement des services de rappels et notifications
builder.Services.AddScoped<IReservationReminderService, ReservationReminderService>();
builder.Services.AddHostedService<ReservationReminderBackgroundService>();

// Enregistrement du service d'email (mock pour le développement)
builder.Services.AddScoped<IEmailService, MockEmailService>();


// Add AuthenticationSchema and JwtBearer
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ??
                throw new InvalidOperationException("JWT Secret not found in configuration")))
        };
    });



//Documentation swagger
builder.Services.AddSwaggerGen(options =>
{
    // Informations générales sur l'API
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Cinephoria API Documentation",
        Description = "Comprehensive API documentation for the Web, Mobile, and Desktop applications."
    });

    // Configuration pour l'authentification JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Description = "Please enter your token as follows: 'Bearer YOUR_TOKEN'",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "bearer",
    });

    builder.Services.AddHttpContextAccessor();

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    // Support de fichiers avec IFormFile
    options.OperationFilter<SwaggerFileOperationFilter>();

    // Inclusion des commentaires XML (optionnel si déjà configuré)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.OrderActionsBy((apiDesc) => $"{apiDesc.HttpMethod} {apiDesc.RelativePath}");
});

// Ajouter la configuration de sécurité (incluant les CORS) en utilisant SecurityExtensions
builder.Services.AddCustomSecurity(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



// Configuration Kestrel pour utiliser HTTPS même en développement
builder.WebHost.ConfigureKestrel((context, options) =>
{
    // Charger les paramètres depuis appsettings.json
    var certPassword = context.Configuration["Kestrel:Endpoints:Https:Certificate:Password"];
    Console.WriteLine($"Cert password config value: '{certPassword}'");
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

var app = builder.Build();

// Le fournisseur de configuration Vault est déjà intégré via builder.Configuration.AddVaultConfiguration()
// Les secrets sont automatiquement chargés et fusionnés avec la configuration


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Activer la redirection HTTPS même en développement
app.UseHttpsRedirection();
// Appliquez la politique CORS
app.UseCors(SecurityExtensions.DEFAULT_POLICY);
app.UseMiddleware<ErrorHandlingMiddleware>();

// Health Checks endpoints
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = async (context, report) =>
    {
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds
            })
        });
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result);
    }
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds
            })
        });
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result);
    }
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.WebRootPath, "images")),
    RequestPath = "/images"
});
app.UseAuthorization();
app.MapControllers();

// Appliquer les migrations de base de données au démarrage
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CinephoriaDbContext>();
        // Appliquer les migrations automatiquement
        await context.Database.MigrateAsync();
        Console.WriteLine("Migrations de base de données appliquées avec succès");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erreur lors de l'application des migrations : " + ex.Message);
        throw;
    }
}

//Exécutez la méthode de seeding d'administrateur lors du démarrage de l'application
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Initialisez les rôles et utilisateurs avec SeedAdmin
        await SeedAdmin.Initialize(services, userManager, roleManager);
    }
    catch (Exception ex)
    {
        // Loggez l'exception si nécessaire
        Console.WriteLine("Erreur lors de l'initialisation de l'administrateur par défaut : " + ex.Message);
    }
}
app.Run();

// Classe partielle pour exposer Program aux tests d'intégration
public partial class Program { }

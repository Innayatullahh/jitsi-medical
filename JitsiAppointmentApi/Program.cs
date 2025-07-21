using JitsiAppointmentApi.Application.Interfaces;
using JitsiAppointmentApi.Application.Services;
using JitsiAppointmentApi.Infrastructure.Config;
using JitsiAppointmentApi.Core.Interfaces;
using JitsiAppointmentApi.Infrastructure.Data;
using JitsiAppointmentApi.Infrastructure.Repositories;
using JitsiAppointmentApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure options
builder.Services.Configure<JitsiOptions>(builder.Configuration.GetSection("Jitsi"));

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
builder.Services.AddScoped<IConsultationTemplateRepository, ConsultationTemplateRepository>();
builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();

// Register application services
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();
builder.Services.AddScoped<IConsultationTemplateService, ConsultationTemplateService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHealthService, HealthService>();

// Register infrastructure services
builder.Services.AddSingleton<JitsiJwtService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "JitsiAppointmentApi v1", Version = "v1" });
    c.SwaggerDoc("v2", new() { Title = "JitsiAppointmentApi v2", Version = "v2" });
    
    // Hide version parameter from all operations
    c.OperationFilter<RemoveVersionParameterFilter>();
    
    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field. Example: 'Bearer {token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// JWT authentication code
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

//builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Attempting to connect to database...");
        logger.LogInformation("Connection string: {ConnectionString}", 
            context.Database.GetConnectionString()?.Replace("Password=123456", "Password=***"));
        
        // Test connection first
        var canConnect = await context.Database.CanConnectAsync();
        if (!canConnect)
        {
            logger.LogError("Cannot connect to database. Please check your connection string and ensure PostgreSQL is running.");
            logger.LogError("Connection string: {ConnectionString}", 
                context.Database.GetConnectionString()?.Replace("Password=123456", "Password=***"));
            throw new Exception("Database connection failed - cannot connect to database");
        }
        
        logger.LogInformation("Database connection successful. Checking if database exists...");
        
        // Check if database exists
        var databaseExists = await context.Database.CanConnectAsync();
        if (!databaseExists)
        {
            logger.LogError("Database 'jitsi_appointments' does not exist. Please create it in pgAdmin.");
            throw new Exception("Database 'jitsi_appointments' does not exist");
        }
        
        logger.LogInformation("Database exists. Applying migrations...");
        
        // Apply migrations
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully.");
        
        // Seed database
        await DatabaseSeeder.SeedAsync(context);
        logger.LogInformation("Database seeding completed.");
        
        // Final test - try to query the database
        var doctorCount = await context.DoctorProfiles.CountAsync();
        logger.LogInformation("Database is fully operational. Doctor count: {DoctorCount}", doctorCount);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error initializing database: {Message}", ex.Message);
        logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
        
        // For development, let's see the full error
        Console.WriteLine($"Database initialization failed: {ex.Message}");
        Console.WriteLine($"Full exception: {ex}");
        Console.WriteLine($"Connection string: {context.Database.GetConnectionString()?.Replace("Password=123456", "Password=***")}");
        
        // Don't throw here - let the app start even if database initialization fails
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JitsiAppointmentApi v1 (Core Features)");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "JitsiAppointmentApi v2 (Advanced Features)");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Jitsi Appointment API Documentation";
});

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();

// Filter to remove version parameter from Swagger
public class RemoveVersionParameterFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters != null)
        {
            var versionParameter = operation.Parameters.FirstOrDefault(p => 
                p.Name == "version" || 
                p.Name == "api-version" || 
                p.Name == "x-api-version" ||
                p.Name == "apiVersion");
            
            if (versionParameter != null)
            {
                operation.Parameters.Remove(versionParameter);
            }
        }
    }
}
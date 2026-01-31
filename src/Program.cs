using BlinktApi.Services;
using BlinktApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services
builder.Services.AddControllers();
builder.Services.AddSingleton<AnimationController>();
builder.Services.AddHostedService<AnimationPlayer>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Blinkt API",
        Version = "v1",
        Description = "ASP.NET Core Web API for controlling Pimoroni Blinkt LED strip (8 RGB LEDs)"
    });
});

var app = builder.Build();

// Configure middleware pipeline
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable Swagger UI at /swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Blinkt API v1");
    options.RoutePrefix = "swagger"; // Serve Swagger UI at /swagger
});

app.MapControllers();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Blinkt API starting on http://0.0.0.0:5001");

app.Run("http://0.0.0.0:5001");

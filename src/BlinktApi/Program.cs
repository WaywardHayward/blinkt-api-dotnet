using BlinktApi.Services;

var builder = WebApplication.CreateBuilder(args);

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

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Blinkt API v1");
    options.RoutePrefix = string.Empty; // Serve Swagger UI at root
});

app.MapControllers();

app.Run("http://0.0.0.0:5001");

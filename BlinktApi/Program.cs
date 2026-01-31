using BlinktApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddSingleton<AnimationController>();
builder.Services.AddHostedService<AnimationPlayer>();

var app = builder.Build();

app.MapControllers();
app.MapGet("/", () => new
{
    status = "ok",
    api_version = "1.0.0",
    hostname = Environment.MachineName
});

app.Run("http://0.0.0.0:5001");

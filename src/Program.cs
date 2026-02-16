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
builder.Services.AddHostedService<AnimationHotReloadService>();

// Register renderer factories
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.AircraftLightingFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.ScannerFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.FillFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.SparkleFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.RandomSingleFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.RandomColorsFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.CometTrailFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.ProgressBarFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.GlobalOscillatorFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.PixelOscillatorFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.TravelingWaveFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.CenterPulseFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.BurstOutwardFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.ColorCycleFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.BouncingDotFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.SpawnFadeFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.FireFlickerFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.AviationBeaconFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.OrganicWaveFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.OrganicFireFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.AuroraFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.TypingIndicatorFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.MatrixRainFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.BreatheFactory>();
builder.Services.AddSingleton<BlinktApi.Rendering.IRendererFactory, BlinktApi.Rendering.SpectrumFactory>();

// Register the main factory
builder.Services.AddSingleton<BlinktApi.Rendering.RendererFactory>();

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

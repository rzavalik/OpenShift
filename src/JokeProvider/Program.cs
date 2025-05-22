using System.Text.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Configuration.AddEnvironmentVariables();

WebApplication app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () =>
{
    return Results.Ok("JokeProvider API v1.0");
});

app.MapGet("/api", () =>
{
    string jokesPath = builder.Configuration["Jokes:FilePath"] ?? "jokes.json";
    if (!File.Exists(jokesPath))
    {
        jokesPath = "jokes.json";
    }

    string jokesJson = File.ReadAllText(jokesPath);
    string[]? jokes = JsonSerializer.Deserialize<string[]>(jokesJson)
                    ?.Select(joke => joke)
                    ?.ToArray();
    string? randomJoke = jokes == null ? null : jokes[new Random().Next(jokes.Length)];
    randomJoke ??= "Null não é piada.";
    string machineName = Environment.MachineName;

    return Results.Ok(new { joke = randomJoke, server = machineName });
});

app.MapHealthChecks("/health");

app.Run();
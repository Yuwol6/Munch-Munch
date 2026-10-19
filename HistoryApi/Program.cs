
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
                new Uri(origin).IsLoopback)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("LocalFrontend");

var connectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? "Host=localhost;Port=5432;Database=munchmunch;Username=munchmunch;Password=devpassword";

// Save a recommendation
app.MapPost("/api/history", async (HistoryRequest request) =>
{
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    const string sql = """
        INSERT INTO recommendation_history (user_id, food)
        VALUES (@userId, @food)
        """;

    await using var command = new NpgsqlCommand(sql, connection);
    command.Parameters.AddWithValue("userId", request.UserId);
    command.Parameters.AddWithValue("food", request.Food);

    await command.ExecuteNonQueryAsync();

    return Results.Ok(new { message = "History saved" });
});

// Retrieve a user's recommendation history
app.MapGet("/api/history/{userId}", async (string userId) =>
{
    var history = new List<object>();

    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    const string sql = """
        SELECT id, food, recommended_at
        FROM recommendation_history
        WHERE user_id = @userId
        ORDER BY recommended_at DESC, id DESC
        LIMIT 50
        """;

    await using var command = new NpgsqlCommand(sql, connection);
    command.Parameters.AddWithValue("userId", userId);

    await using var reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        history.Add(new
        {
            id = reader.GetInt64(0),
            food = reader.GetString(1),
            recommendedAt = reader.GetDateTime(2)
        });
    }

    return Results.Ok(history);
});

app.Run();

record HistoryRequest(string UserId, string Food);

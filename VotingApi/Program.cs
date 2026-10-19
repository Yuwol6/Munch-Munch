
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Allow requests from our local frontend
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

// Record a vote (maximum 3 per user per Brisbane day)
app.MapPost("/api/votes", async (VoteRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.UserId) ||
        string.IsNullOrWhiteSpace(request.Food))
    {
        return Results.BadRequest(new { error = "User ID and food are required." });
    }

    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    await using var transaction = await connection.BeginTransactionAsync();

    // Lock this user's voting operations until the transaction completes.
    // Identical user IDs acquire the same advisory lock.
    await using (var lockCommand = new NpgsqlCommand(
        "SELECT pg_advisory_xact_lock(hashtextextended(@userId, 0))",
        connection, transaction))
    {
        lockCommand.Parameters.AddWithValue("userId", request.UserId);
        await lockCommand.ExecuteNonQueryAsync();
    }

    const string countSql = """
        SELECT COUNT(*)
        FROM votes
        WHERE user_id = @userId
          AND (voted_at AT TIME ZONE 'Australia/Brisbane')::date =
              (NOW() AT TIME ZONE 'Australia/Brisbane')::date
        """;

    long voteCount;

    await using (var countCommand = new NpgsqlCommand(
        countSql, connection, transaction))
    {
        countCommand.Parameters.AddWithValue("userId", request.UserId);
        voteCount = (long)(await countCommand.ExecuteScalarAsync())!;
    }

    if (voteCount >= 3)
    {
        return Results.Json(
            new { error = "Daily vote limit reached.", remainingVotes = 0 },
            statusCode: StatusCodes.Status429TooManyRequests);
    }

    const string insertSql = """
        INSERT INTO votes (user_id, food)
        VALUES (@userId, @food)
        """;

    await using (var insertCommand = new NpgsqlCommand(
        insertSql, connection, transaction))
    {
        insertCommand.Parameters.AddWithValue("userId", request.UserId);
        insertCommand.Parameters.AddWithValue("food", request.Food);
        await insertCommand.ExecuteNonQueryAsync();
    }

    await transaction.CommitAsync();

    return Results.Ok(new
    {
        message = "Vote recorded!",
        remainingVotes = 2 - voteCount
    });
});

// Get today's leaderboard (Brisbane time)
app.MapGet("/api/leaderboard", async () =>
{
    var leaderboard = new List<object>();

    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    const string sql = """
        SELECT food, COUNT(*) AS vote_count
        FROM votes
        WHERE (voted_at AT TIME ZONE 'Australia/Brisbane')::date =
              (NOW() AT TIME ZONE 'Australia/Brisbane')::date
        GROUP BY food
        ORDER BY vote_count DESC, food ASC
        """;

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        leaderboard.Add(new
        {
            food = reader.GetString(0),
            votes = reader.GetInt64(1)
        });
    }

    return Results.Ok(leaderboard);
});

app.Run();

record VoteRequest(string UserId, string Food);

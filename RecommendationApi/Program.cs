
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

var foods = new[]
{
    new { Name = "Pizza", Cuisine = "Italian" },
    new { Name = "Pasta", Cuisine = "Italian" },
    new { Name = "Sushi", Cuisine = "Japanese" },
    new { Name = "Ramen", Cuisine = "Japanese" },
    new { Name = "Katsu", Cuisine = "Japanese" },
    new { Name = "Bibimbap", Cuisine = "Korean" },
    new { Name = "Fried Chicken", Cuisine = "Korean" },
    new { Name = "Burger", Cuisine = "American" }
};

app.MapGet("/api/recommend", (string? cuisine) =>
{
    var matchingFoods = string.IsNullOrWhiteSpace(cuisine)
        ? foods
        : foods.Where(f => f.Cuisine.Equals(cuisine, StringComparison.OrdinalIgnoreCase)).ToArray();

    if (matchingFoods.Length == 0)
        return Results.NotFound(new { error = "No matching foods found." });

    var selected = matchingFoods[Random.Shared.Next(matchingFoods.Length)];

    return Results.Ok(new { food = selected.Name, cuisine = selected.Cuisine });
});

app.Run();

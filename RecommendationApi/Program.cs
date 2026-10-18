var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string[] foods =
{
    "Pizza",
    "Sushi",
    "Ramen",
    "Katsu",
    "Burger",
    "Pasta",
    "Bibimbap",
    "Fried Chicken"
};

app.MapGet("/api/recommend", () =>
{
    string food = foods[Random.Shared.Next(foods.Length)];

    return Results.Ok(new { food });
});

app.Run();
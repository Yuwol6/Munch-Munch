
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

var foods = new[]
{
    // American
    new { Name = "Burger", Cuisine = "American", ImageUrl = "/images/burger.jpg" },
    new { Name = "Hot Dog", Cuisine = "American", ImageUrl = "/images/hot-dog.jpg" },
    new { Name = "Steak", Cuisine = "American", ImageUrl = "/images/steak.jpg" },

    // Italian
    new { Name = "Pizza", Cuisine = "Italian", ImageUrl = "/images/pizza.jpg" },
    new { Name = "Pasta", Cuisine = "Italian", ImageUrl = "/images/pasta.jpg" },
    new { Name = "Risotto", Cuisine = "Italian", ImageUrl = "/images/risotto.jpg" },

    // Korean
    new { Name = "Bibimbap", Cuisine = "Korean", ImageUrl = "/images/bibimbap.jpg" },
    new { Name = "Fried Chicken", Cuisine = "Korean", ImageUrl = "/images/fried-chicken.webp" },
    new { Name = "Korean BBQ", Cuisine = "Korean", ImageUrl = "/images/korean-bbq.jpg" },
    
    // Japanese
    new { Name = "Sushi", Cuisine = "Japanese", ImageUrl = "/images/sushi.webp" },
    new { Name = "Ramen", Cuisine = "Japanese", ImageUrl = "/images/ramen.jpg" },
    new { Name = "Katsu", Cuisine = "Japanese", ImageUrl = "/images/katsu.jpg" },
    new { Name = "Japanese Curry", Cuisine = "Japanese", ImageUrl = "/images/japanese-curry.jpg" },
    new { Name = "Sukiyaki", Cuisine = "Japanese", ImageUrl = "/images/sukiyaki.jpg" },
    
    // Chinese
    new { Name = "Dumplings", Cuisine = "Chinese", ImageUrl = "/images/dumplings.webp" },
    new { Name = "Fried Rice", Cuisine = "Chinese", ImageUrl = "/images/fried-rice.jpg" },
    new { Name = "BBQ Skewers", Cuisine = "Chinese", ImageUrl = "/images/bbq-skewers.webp" },
    new { Name = "Peking Duck", Cuisine = "Chinese", ImageUrl = "/images/peking-duck.webp" },
    new { Name = "Beef Noodles", Cuisine = "Chinese", ImageUrl = "/images/beef-noodles.jpg" },
    new { Name = "Malatang", Cuisine = "Chinese", ImageUrl = "/images/malatang.jpg" }
};

app.MapGet("/api/recommend", (string? cuisine) =>
{
    var matchingFoods = string.IsNullOrWhiteSpace(cuisine)
        ? foods
        : foods.Where(f => f.Cuisine.Equals(cuisine, StringComparison.OrdinalIgnoreCase)).ToArray();

    if (matchingFoods.Length == 0)
        return Results.NotFound(new { error = "No matching foods found." });

    var selected = matchingFoods[Random.Shared.Next(matchingFoods.Length)];

    return Results.Ok(new
    {
        food = selected.Name,
        cuisine = selected.Cuisine,
        imageUrl = selected.ImageUrl
    });
});

app.Run();

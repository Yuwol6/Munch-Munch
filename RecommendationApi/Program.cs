
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var foods = new[]
{
    // American
    new { Name = "Burger", Cuisine = "American", Price = "Moderate", ImageUrl = "/images/burger.jpg" },
    new { Name = "Hot Dog", Cuisine = "American", Price = "Cheap", ImageUrl = "/images/hot-dog.jpg" },
    new { Name = "Steak", Cuisine = "American", Price = "Expensive", ImageUrl = "/images/steak.jpg" },
    new { Name = "Sandwich", Cuisine = "American", Price = "Cheap", ImageUrl = "/images/sandwich.jpg" },
    new { Name = "Buffalo Wings", Cuisine = "American", Price = "Moderate", ImageUrl = "/images/buffalo-wings.jpg" },
    new { Name = "BBQ Ribs", Cuisine = "American", Price = "Expensive", ImageUrl = "/images/bbq-ribs.jpg" },
    new { Name = "Mac and Cheese", Cuisine = "American", Price = "Cheap", ImageUrl = "/images/mac-and-cheese.webp" },
    new { Name = "Garlic Bread", Cuisine = "American", Price = "Cheap", ImageUrl = "/images/garlic-bread.jpg" },

    // Italian
    new { Name = "Pizza", Cuisine = "Italian", Price = "Cheap", ImageUrl = "/images/pizza.jpg" },
    new { Name = "Spaghetti Bolognese", Cuisine = "Italian", Price = "Expensive", ImageUrl = "/images/spaghetti-bolognese.jpg" },
    new { Name = "Risotto", Cuisine = "Italian", Price = "Expensive", ImageUrl = "/images/risotto.jpg" },
    new { Name = "Lasagna", Cuisine = "Italian", Price = "Moderate", ImageUrl = "/images/lasagna.jpg" },
    new { Name = "Ravioli", Cuisine = "Italian", Price = "Moderate", ImageUrl = "/images/ravioli.webp" },
    new { Name = "Spaghetti Carbonara", Cuisine = "Italian", Price = "Expensive", ImageUrl = "/images/spaghetti-carbonara.jpg" },
    new { Name = "Bruschetta", Cuisine = "Italian", Price = "Cheap", ImageUrl = "/images/bruschetta.jpg" },
    new { Name = "Aglio e Olio", Cuisine = "Italian", Price = "Moderate", ImageUrl = "/images/aglio-e-olio.jpg" },

    // Korean
    new { Name = "Bibimbap", Cuisine = "Korean", Price = "Moderate", ImageUrl = "/images/bibimbap.jpg" },
    new { Name = "Fried Chicken", Cuisine = "Korean", Price = "Moderate", ImageUrl = "/images/fried-chicken.webp" },
    new { Name = "Korean BBQ", Cuisine = "Korean", Price = "Expensive", ImageUrl = "/images/korean-bbq.jpg" },
    new { Name = "Tteokbokki", Cuisine = "Korean", Price = "Cheap", ImageUrl = "/images/tteokbokki.jpg" },
    new { Name = "Pork Hock", Cuisine = "Korean", Price = "Expensive", ImageUrl = "/images/pork-hock.jpg" },
    new { Name = "Korean Pancake", Cuisine = "Korean", Price = "Cheap", ImageUrl = "/images/korean-pancake.jpg" },
    new { Name = "Instant Noodles", Cuisine = "Korean", Price = "Cheap", ImageUrl = "/images/instant-noodles.webp" },
    new { Name = "Kimchi Stew", Cuisine = "Korean", Price = "Moderate", ImageUrl = "/images/kimchi-stew.webp" },
    new { Name = "Cold Noodles", Cuisine = "Korean", Price = "Moderate", ImageUrl = "/images/cold-noodles.jpg" },
    new { Name = "Kimbap", Cuisine = "Korean", Price = "Cheap", ImageUrl = "/images/kimbap.jpg" },

    // Japanese
    new { Name = "Sushi", Cuisine = "Japanese", Price = "Expensive", ImageUrl = "/images/sushi.webp" },
    new { Name = "Ramen", Cuisine = "Japanese", Price = "Moderate", ImageUrl = "/images/ramen.jpg" },
    new { Name = "Katsu", Cuisine = "Japanese", Price = "Moderate", ImageUrl = "/images/katsu.jpg" },
    new { Name = "Japanese Curry", Cuisine = "Japanese", Price = "Cheap", ImageUrl = "/images/japanese-curry.jpg" },
    new { Name = "Sukiyaki", Cuisine = "Japanese", Price = "Expensive", ImageUrl = "/images/sukiyaki.jpg" },
    new { Name = "Udon", Cuisine = "Japanese", Price = "Moderate", ImageUrl = "/images/udon.jpg" },
    new { Name = "Takoyaki", Cuisine = "Japanese", Price = "Cheap", ImageUrl = "/images/takoyaki.jpeg" },
    new { Name = "Onigiri", Cuisine = "Japanese", Price = "Cheap", ImageUrl = "/images/onigiri.jpg" },
    new { Name = "Tempura", Cuisine = "Japanese", Price = "Moderate", ImageUrl = "/images/tempura.jpeg" },
    new { Name = "Yakiniku", Cuisine = "Japanese", Price = "Expensive", ImageUrl = "/images/yakiniku.jpg" },

    // Chinese
    new { Name = "Dumplings", Cuisine = "Chinese", Price = "Cheap", ImageUrl = "/images/dumplings.webp" },
    new { Name = "Fried Rice", Cuisine = "Chinese", Price = "Cheap", ImageUrl = "/images/fried-rice.jpg" },
    new { Name = "BBQ Skewers", Cuisine = "Chinese", Price = "Expensive", ImageUrl = "/images/bbq-skewers.webp" },
    new { Name = "Peking Duck", Cuisine = "Chinese", Price = "Expensive", ImageUrl = "/images/peking-duck.webp" },
    new { Name = "Beef Noodles", Cuisine = "Chinese", Price = "Moderate", ImageUrl = "/images/beef-noodles.jpg" },
    new { Name = "Malatang", Cuisine = "Chinese", Price = "Moderate", ImageUrl = "/images/malatang.jpg" },
    new { Name = "Xiao Long Bao", Cuisine = "Chinese", Price = "Cheap", ImageUrl = "/images/xiao-long-bao.jpg" },
    new { Name = "Spring Rolls", Cuisine = "Chinese", Price = "Cheap", ImageUrl = "/images/spring-rolls.jpg" },
    new { Name = "Hot pot", Cuisine = "Chinese", Price = "Expensive", ImageUrl = "/images/hot-pot.jpg" }
};

app.MapGet("/api/recommend", (string? cuisine, string? price) =>
{
    var matchingFoods = foods.Where(f =>
        (string.IsNullOrWhiteSpace(cuisine) ||
         f.Cuisine.Equals(cuisine, StringComparison.OrdinalIgnoreCase)) &&
        (string.IsNullOrWhiteSpace(price) ||
         f.Price.Equals(price, StringComparison.OrdinalIgnoreCase))
    ).ToArray();

    if (matchingFoods.Length == 0)
        return Results.NotFound(new { error = "No matching foods found." });

    var selected = matchingFoods[Random.Shared.Next(matchingFoods.Length)];

    return Results.Ok(new
    {
        food = selected.Name,
        cuisine = selected.Cuisine,
        price = selected.Price,
        imageUrl = selected.ImageUrl
    });
});

app.Run();


const button = document.getElementById("recommendButton");
const result = document.getElementById("result");
const cuisineSelect = document.getElementById("cuisineSelect");
const historyButton = document.getElementById("historyButton");
const historyList = document.getElementById("historyList");

// Replace this with HistoryApi's actual HTTPS port
const HISTORY_API = "https://localhost:7004";

// Give each browser a persistent anonymous ID
let userId = localStorage.getItem("munchmunchUserId");

if (!userId) {
    userId = crypto.randomUUID();
    localStorage.setItem("munchmunchUserId", userId);
}

button.addEventListener("click", async () => {
    result.textContent = "Finding something delicious...";

    try {
        const cuisine = cuisineSelect.value;
        const url = cuisine
            ? `/api/recommend?cuisine=${encodeURIComponent(cuisine)}`
            : "/api/recommend";

        const response = await fetch(url);

        if (!response.ok) {
            throw new Error("Recommendation failed");
        }

        const data = await response.json();
        result.textContent = `How about ${data.food}?`;

        // Save the recommendation to HistoryApi
        const saveResponse = await fetch(`${HISTORY_API}/api/history`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                userId: userId,
                food: data.food
            })
        });

        if (!saveResponse.ok) {
            throw new Error("History could not be saved");
        }

    } catch (error) {
        console.error(error);
        if (result.textContent === "Finding something delicious...") {
            result.textContent = "Couldn't get a recommendation.";
        }
    }
});

historyButton.addEventListener("click", async () => {
    try {
        const response = await fetch(
            `${HISTORY_API}/api/history/${encodeURIComponent(userId)}`
        );

        if (!response.ok) {
            throw new Error("History request failed");
        }

        const history = await response.json();

        historyList.replaceChildren();

        if (history.length === 0) {
            const item = document.createElement("li");
            item.textContent = "No recommendations yet.";
            historyList.appendChild(item);
            return;
        }

        for (const entry of history) {
            const item = document.createElement("li");
            const date = new Date(entry.recommendedAt).toLocaleString();
            item.textContent = `${entry.food} — ${date}`;
            historyList.appendChild(item);
        }

    } catch (error) {
        console.error(error);
        historyList.replaceChildren();
        const item = document.createElement("li");
        item.textContent = "Couldn't load history.";
        historyList.appendChild(item);
    }
});

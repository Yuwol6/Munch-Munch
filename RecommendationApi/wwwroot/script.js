
const button = document.getElementById("recommendButton");
const result = document.getElementById("result");
const cuisineSelect = document.getElementById("cuisineSelect");
const historyButton = document.getElementById("historyButton");
const historyList = document.getElementById("historyList");

const voteButton = document.getElementById("voteButton");
const voteMessage = document.getElementById("voteMessage");
const leaderboardButton = document.getElementById("leaderboardButton");
const leaderboardList = document.getElementById("leaderboardList");

// Replace with your VotingApi's actual HTTPS port
const VOTING_API = "http://localhost:8082";

let currentFood = null;

// Replace this with HistoryApi's actual HTTPS port
const HISTORY_API = "http://localhost:8081";

// Give each browser a persistent anonymous ID
let userId = localStorage.getItem("munchmunchUserId");

if (!userId) {
    userId = crypto.randomUUID();
    localStorage.setItem("munchmunchUserId", userId);
}

button.addEventListener("click", async () => {
    result.textContent = "Finding something delicious...";
    currentFood = null;
    voteButton.hidden = true;

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

        const foodImage = document.getElementById("foodImage");
        foodImage.src = `${data.imageUrl}?v=6`;
        foodImage.alt = data.food;
        foodImage.hidden = false;

        currentFood = data.food;
        voteButton.hidden = false;
        voteMessage.textContent = "";

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
            const date = new Date(entry.recommendedAt).toLocaleDateString("en-AU");
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

voteButton.addEventListener("click", async () => {
    if (!currentFood) return;

    // Open Google Maps in a new tab
    const query = encodeURIComponent(`${currentFood} restaurants near me`);
    window.open(
        `https://www.google.com/maps/search/?api=1&query=${query}`,
        "_blank",
        "noopener,noreferrer"
    );

    voteButton.disabled = true;

    try {
        const response = await fetch(`${VOTING_API}/api/votes`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                userId: userId,
                food: currentFood
            })
        });

        const data = await response.json();

        if (!response.ok) {
            voteMessage.textContent =
                data.error || "Couldn't record your vote.";
            return;
        }

        voteMessage.textContent =
            `Vote recorded! ${data.remainingVotes} votes remaining today.`;

        voteButton.hidden = true;
        await loadLeaderboard();

    } catch (error) {
        console.error(error);
        voteMessage.textContent = "Couldn't connect to VotingApi.";
    } finally {
        voteButton.disabled = false;
    }
});

async function loadLeaderboard() {
    try {
        const response = await fetch(`${VOTING_API}/api/leaderboard`);

        if (!response.ok) {
            throw new Error("Leaderboard request failed");
        }

        const leaderboard = await response.json();
        leaderboardList.replaceChildren();

        if (leaderboard.length === 0) {
            const item = document.createElement("li");
            item.textContent = "No votes yet today.";
            leaderboardList.appendChild(item);
            return;
        }

        for (const entry of leaderboard) {
            const item = document.createElement("li");
            item.textContent = `${entry.food} — ${entry.votes} votes`;
            leaderboardList.appendChild(item);
        }

    } catch (error) {
        console.error(error);
        leaderboardList.replaceChildren();

        const item = document.createElement("li");
        item.textContent = "Couldn't load leaderboard.";
        leaderboardList.appendChild(item);
    }
}

leaderboardButton.addEventListener("click", loadLeaderboard);

// Load leaderboard when the page opens
loadLeaderboard();
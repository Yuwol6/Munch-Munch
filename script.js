
const button = document.getElementById("recommendButton");
const result = document.getElementById("result");

button.addEventListener("click", async () => {
    result.textContent = "Finding something delicious...";

    try {
        const response = await fetch("/api/recommend");

        if (!response.ok) {
            throw new Error("API request failed");
        }

        const data = await response.json();
        result.textContent = `How about ${data.food}?`;
    } catch (error) {
        result.textContent = "Couldn't get a recommendation.";
        console.error(error);
    }
});

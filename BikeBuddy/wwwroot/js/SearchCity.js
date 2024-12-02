function filterCities() {
    const searchInput = document.getElementById("searchBar").value.toLowerCase();
    const cityCards = document.querySelectorAll(".city-card");
    cityCards.forEach((card) => {
        const cityName = card.querySelector("span").textContent.toLowerCase();
        if (cityName.includes(searchInput)) {
            card.style.display = "block"; // Show matching cards
        } else {
            card.style.display = "none"; // Hide non-matching cards
        }
    });
}

function clearFilter() {
    document.getElementById("searchBar").value = '';
    filterCities(); // Reset the filter
}
const API_URL = 'http://localhost:5127/api';

async function loadMenu() {
    try {
        const cachedIds = localStorage.getItem('selectedCategoryIds');
        if (!cachedIds) {
            window.location.href = 'index.html';
            return;
        }
        
        const response = await fetch(`${API_URL}/levels`);
        if (!response.ok) {
            throw new Error("Ошибка сервера");
        }

        const levels = await response.json();
        const container = document.getElementById('levels-container');
        container.replaceChildren();

        levels.forEach(level => {
            const card = document.createElement('div');
            card.className = 'quize-level';

            const title = document.createElement('h3');
            title.textContent = level.title;

            card.appendChild(title);

            card.addEventListener('click', () => {
                localStorage.setItem('selectedCategoryIds', JSON.stringify(level.categoryIds))
                window.location.href = 'game.html';
            });

            container.appendChild(card);
        });
    } catch (error) {
        console.error("Ошибка:", error);
    }
}

document.addEventListener('DOMContentLoaded', loadMenu);
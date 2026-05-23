const API_URL = "http://localhost:5127/api";
let currentQuizData = null;

async function loadGame() {
    try {
        const cachedIds = localStorage.getItem('selectedCategoryIds');
        if (!cachedIds) {
            window.location.href = 'index.html';
            return;
        }

        const ids = JSON.parse(cachedIds);
        const queryParams = ids.map(id => `ids=${id}`).join('&');

        const response = await fetch(`${API_URL}/quize/level?${queryParams}`);
        if (!response.ok) {
            throw new Error("Ошибка сервера");
        }

        const quize = await response.json();
        currentQuizData = quize;


        const container = document.getElementById('pictures-container');
        container.replaceChildren(); 

        const section = document.createElement('section');
        section.className = 'quize-description';

        const description = document.createElement('h4');
        description.textContent = `Кто этот персонаж? \n ${quize.description}`;
        section.appendChild(description);
        container.appendChild(section);

        const imagesSection = loadImages(quize);
        container.appendChild(imagesSection);

    } catch (error) {
        console.error("Ошибка:", error);
        document.getElementById('pictures-container').textContent = "Не удалось загрузить игру.";
    }
}

function loadImages(quize) {
    const imagesSection = document.createElement('div');
    imagesSection.className = "images-block";

    const images = quize.imageOptions;

    images.forEach(imageUrl => {
        const picture = document.createElement('img');
        picture.src = imageUrl;
        picture.className = 'quiz-img';
        picture.style.width = "200px"; 
        picture.style.margin = "10px";
        picture.style.cursor = "pointer";

        picture.addEventListener('click', () => {
            handleAnswer(picture, imageUrl);
        });

        imagesSection.appendChild(picture);
    });

    return imagesSection;
}

function handleAnswer(selectedImg, selectedUrl) {
    const allImages = document.querySelectorAll('.images-block img');
    allImages.forEach(img => img.style.pointerEvents = 'none');

    const isCorrect = selectedUrl === currentQuizData.correctImageUrl;

    const resultSection = document.createElement('div');
    resultSection.className = 'result-block';

    const resultText = document.createElement('h3');
    const factText = document.createElement('p');

    if (isCorrect) {
        resultText.textContent = `🎉 Правильно! Это ${currentQuizData.correctName}!`;
        selectedImg.style.border = "5px solid green";
    } else {
        resultText.textContent = `❌ Ошибка! Это ${currentQuizData.correctName}.`;
        selectedImg.style.border = "5px solid red";

        allImages.forEach(img => {
            if (img.getAttribute('src') === currentQuizData.correctImageUrl) {
                img.style.border = "5px solid green";
            }
        });
    }

    factText.textContent = `Интересный факт: ${currentQuizData.fact}`;

    const backBtn = document.createElement('button');
    backBtn.textContent = "В меню";
    backBtn.addEventListener('click', () => window.location.href = 'index.html');

    const nextBtn = document.createElement('button');
    nextBtn.textContent = "Следующий вопрос";
    nextBtn.addEventListener('click', () => loadGame());

    resultSection.appendChild(resultText);
    resultSection.appendChild(factText);
    resultSection.appendChild(backBtn);
    resultSection.appendChild(nextBtn);

    document.getElementById('pictures-container').appendChild(resultSection);
}

document.addEventListener('DOMContentLoaded', loadGame);
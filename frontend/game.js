const API_URL = "http://localhost:5127/api";
let currentQuizData = null;

async function loadGame() {
    try {
        const cachedIds = localStorage.getItem('selectedCategoryIds');
        if (!cachedIds) {
            window.location.href = window.location.protocol === 'file:' ? 'index.html' : './';
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

        const container = document.getElementById('quize-container');
        container.replaceChildren(); 

        const section = document.createElement('section');
        section.className = 'quize-description';

        const name = document.createElement('p');
        name.className = 'quiz-person-name';
        name.textContent = quize.correctName;
        
        const description = document.createElement('p');
        description.className = 'quiz-person-description';
        description.textContent = quize.description;
        section.appendChild(name);
        section.appendChild(description);

        const imagesSection = loadImages(quize);

        container.appendChild(section);
        container.appendChild(imagesSection);

    } catch (error) {
        console.error("Ошибка:", error);
        document.getElementById('quize-container').textContent = "Не удалось загрузить игру.";
    }
}

function loadImages(quize) {
    const imagesSection = document.createElement('div');
    imagesSection.className = "images-block";

    const images = quize.imageOptions;

    images.forEach(imageUrl => {
        const picture = document.createElement('img');
        picture.src = imageUrl.startsWith('images/') ? imageUrl : `images/persons/${imageUrl}`;

        picture.onerror = () => {
            picture.src = `https://ui-avatars.com/api/?name=${imageUrl.split('.')[0]}&background=random`;
        };

        picture.className = 'quiz-img';

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

    const correctUrl = currentQuizData.correctImageUrl;

    const isCorrect = selectedImg.getAttribute('src').endsWith(correctUrl);

    const resultSection = document.createElement('div');
    resultSection.className = 'result-block';

    const resultText = document.createElement('h3');
    const factText = document.createElement('p');

    if (isCorrect) {
        resultText.textContent = `Правильно! Это ${currentQuizData.correctName}!`;
        selectedImg.classList.add('correct');
        allImages.forEach(img => {
            if (img !== selectedImg) img.classList.add('dimmed');
        });
    } else {
        resultText.textContent = `Ошибка!`;
        selectedImg.classList.add('wrong');
        allImages.forEach(img => {
            if (img.getAttribute('src').endsWith(correctUrl)) {
                img.classList.add('correct');
            } else if (img !== selectedImg) {
                img.classList.add('dimmed');
            }
        });
    }

    factText.textContent = currentQuizData.fact;

    const nextBtn = document.createElement('button');
    nextBtn.textContent = "Следующий вопрос";
    nextBtn.addEventListener('click', () => loadGame());

    resultSection.appendChild(resultText);
    resultSection.appendChild(factText);
    resultSection.appendChild(nextBtn);

    document.getElementById('quize-container').appendChild(resultSection);
}

document.addEventListener('DOMContentLoaded', () => {
    const menuBtn = document.getElementById('menu-button');
    if (menuBtn) {
        menuBtn.addEventListener('click', () => {
            window.location.href = window.location.protocol === 'file:' ? 'index.html' : './';
        });
    }
    loadGame();
});
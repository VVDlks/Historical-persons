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
    const options = quize.imageOptions;
    options.forEach(opt => {
        const wrapper = document.createElement('div');
        wrapper.className = 'img-wrapper';
        wrapper.dataset.imageUrl = opt.imageUrl || opt;

        const picture = document.createElement('img');
        const imageUrl = opt.imageUrl || opt;
        picture.dataset.imageUrl = imageUrl;
        picture.src = imageUrl.startsWith('images/') ? imageUrl : `images/persons/${imageUrl}`;

        picture.onerror = () => {
            picture.src = `https://ui-avatars.com/api/?name=${imageUrl.split('.')[0]}&background=random`;
        };

        picture.className = 'quiz-img';

        const sticker = document.createElement('div');
        sticker.className = 'img-sticker';
        sticker.textContent = opt.name || "Неизвестно";

        wrapper.appendChild(picture);
        wrapper.appendChild(sticker);

        wrapper.addEventListener('click', () => {
            handleAnswer(wrapper, picture, imageUrl);
        });

        imagesSection.appendChild(wrapper);
    });

    return imagesSection;
}

function handleAnswer(selectedWrapper, selectedImg, selectedUrl) {
    const imagesBlock = document.querySelector('.images-block');
    if (imagesBlock) imagesBlock.classList.add('answered');

    const allWrappers = document.querySelectorAll('.images-block .img-wrapper');
    allWrappers.forEach(w => w.style.pointerEvents = 'none');

    const correctUrl = currentQuizData.correctImageUrl;

    const isCorrect = selectedUrl === correctUrl;

    const resultSection = document.createElement('div');
    resultSection.className = 'result-block';

    const resultText = document.createElement('h3');
    const factText = document.createElement('p');

    if (isCorrect) {
        resultText.textContent = `Правильно!`;
        selectedImg.classList.add('correct');
        allWrappers.forEach(w => {
            if (w !== selectedWrapper) w.classList.add('dimmed');
        });
    } else {
        resultText.textContent = `Ошибка!`;
        selectedImg.classList.add('wrong');
        allWrappers.forEach(w => {
            const imgElement = w.querySelector('img');
            if (w.dataset.imageUrl === correctUrl) {
                imgElement.classList.add('correct');
            } else if (w !== selectedWrapper) {
                w.classList.add('dimmed');
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
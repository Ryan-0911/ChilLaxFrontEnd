// const tarotCards = [
//     "愚者", "魔術師", "女祭司", "皇后", "皇帝", "教皇", "戀愛", "戰車", "力量", "隱士",
//     "命運之輪", "正義", "倒吊人", "死神", "節制", "惡魔", "高塔", "星星", "月亮", "太陽", "審判", "世界"
// ];

// const tarotCardElements = document.querySelectorAll(".tarot-card");
// const shuffleBtn = document.getElementById("shuffleBtn");
// let currentIndex = 0;
// let shuffledIndexes = [];

// //生成塔羅牌的圖片
// const tarotCardImages = tarotCards.map(card => './images/' + card + '.jpg');

// tarotCardElements.forEach((cardElement, index) => {
//     const imgElement = document.createElement("img");
//     imgElement.src = tarotCardImages[index];
//     cardElement.appendChild(imgElement);
// });
// shuffleBtn.addEventListener("click", () => {
//     shuffleTarotCards
// });
// //shuffleBtn.addEventListener("click", shuffleTarotCards);

// function shuffleTarotCards() {
//     if (currentIndex === 0) {
//         shuffleBtn.disabled = true; // 禁用按鈕，直到三張牌都出現
//         currentIndex = 1;

//         // 生成隨機的牌順序
//         shuffledIndexes = generateRandomIndexes();

//         // 添加動畫效果
//         tarotCardElements.forEach((card, index) => {
//             card.classList.add("card-removed");
//         });

//         // 更新塔羅牌圖案並移除動畫效果
//         setTimeout(showNextCard, 1000);
//     }
// }

// function generateRandomIndexes() {
//     const indexes = Array.from({ length: tarotCards.length }, (_, i) => i);
//     for (let i = indexes.length - 1; i > 0; i--) {
//         const j = Math.floor(Math.random() * (i + 1));
//         [indexes[i], indexes[j]] = [indexes[j], indexes[i]];
//     }
//     return indexes;
// }

// shuffleBtn.addEventListener("click", () => {
//     if (currentIndex === 0) {
//         currentIndex = 1;

//         // 添加動畫效果，這裡將洗牌按鈕視為第四張牌
//         shuffleBtn.classList.add("card-removed");

//         // 生成隨機的牌順序
//         shuffledIndexes = generateRandomIndexes();

//         // 更新塔羅牌圖案並移除動畫效果
//         setTimeout(showNextCard, 1000);
//     }
// });


// function showNextCard() {
//     if (currentIndex <= 3) {
//         const cardIndex = currentIndex - 1;
//         const tarotCardElement = tarotCardElements[cardIndex];

//         // 更新塔羅牌圖案並移除動畫效果
//         tarotCardElement.textContent = tarotCards[shuffledIndexes[cardIndex]];
//         tarotCardElement.classList.remove("card-removed");
//         tarotCardElement.classList.add("card-appeared");

//         setTimeout(showNextCard, 1000);
//         currentIndex++;
//     } else {
//         currentIndex = 0;
//         shuffleBtn.style.display = "none"; // 隱藏按鈕，使按鈕只需按一次後就會消失
//     }
// }

    const tarotCards = [
        "愚者", "魔術師", "女祭司", "皇后", "皇帝", "教皇", "戀愛", "戰車", "力量", "隱士",
        "命運之輪", "正義", "倒吊人", "死神", "節制", "惡魔", "高塔", "星星", "月亮", "太陽", "審判", "世界"
    ];

    const tarotCardElements = document.querySelectorAll(".tarot-card");
    const shuffleBtn = document.getElementById("shuffleBtn");
    let currentIndex = 0;
    let shuffledIndexes = [];

const tarotCardImages = tarotCards.map(card => '../../images/' + card + '.jpg');

    tarotCardElements.forEach(cardElement => {
        const imgElement = document.createElement("img");
        imgElement.src = '../../images/塔羅牌卡背.png';
        // imgElement.src = '~/'; // 背面圖片的URL
        cardElement.appendChild(imgElement);
    });

    shuffleBtn.addEventListener("click", shuffleTarotCards);

    function shuffleTarotCards() {
        if (currentIndex === 0) {
            shuffleBtn.disabled = true; // 禁用按鈕，直到三張牌都出現
            currentIndex = 1;

            // 生成隨機的牌順序
            shuffledIndexes = generateRandomIndexes();

            // 添加動畫效果
            tarotCardElements.forEach((card, index) => {
                card.classList.add("card-removed");
            });

            // 更新塔羅牌圖案並移除動畫效果
            setTimeout(showNextCard, 1000);
        }
    }

    function generateRandomIndexes() {
        const indexes = Array.from({ length: tarotCards.length }, (_, i) => i);
        for (let i = indexes.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [indexes[i], indexes[j]] = [indexes[j], indexes[i]];
        }
        return indexes;
    }

    function showNextCard() {
        if (currentIndex <= 3) {
            const cardIndex = currentIndex - 1;
            const tarotCardElement = tarotCardElements[cardIndex];
            const imgElement = tarotCardElement.querySelector("img");

            // 更新塔羅牌圖片和名稱並移除動畫效果
            imgElement.src = tarotCardImages[shuffledIndexes[cardIndex]];

            const tarotNameElement = document.getElementById("cardName" + currentIndex);
            tarotNameElement.textContent = tarotCards[shuffledIndexes[cardIndex]];

            tarotCardElement.classList.remove("card-removed");
            tarotCardElement.classList.add("card-appeared");

            setTimeout(showNextCard, 1000);
            currentIndex++;
        } else {
            currentIndex = 0;
            shuffleBtn.style.display = "none"; // 隱藏按鈕，使按鈕只需按一次後就會消失
        }
    }



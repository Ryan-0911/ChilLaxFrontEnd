

    const tarotCards = [
        "愚者", "魔術師", "女祭司", "皇后", "皇帝", "教皇", "戀愛", "戰車", "力量", "隱士",
        "命運之輪", "正義", "倒吊人", "死神", "節制", "惡魔", "高塔", "星星", "月亮", "太陽", "審判", "世界"
    ];
    let inputCount = 0;
    const maxInputCount = 5;
    const tarotCardElements = document.querySelectorAll(".tarot-card");
    const shuffleBtn = document.getElementById("shuffleBtn");
    let currentIndex = 0;
    let shuffledIndexes = [];
    let intervalId = null;  //為了在每次輸入後清空輸入欄位設定的計時器


    const tarotCardImages = tarotCards.map(card => '../../images/' + card + '.jpg');

    tarotCardElements.forEach(cardElement => {
        const imgElement = document.createElement("img");
        imgElement.src = '../../images/塔羅牌卡背.png';
        // imgElement.src = '~/'; // 背面圖片的URL
        cardElement.appendChild(imgElement);
    });

    document.getElementById("message").addEventListener("keydown", function (event) {
        //按下了ENTER鍵（keyCode為13）
        if (event.keyCode === 13) {
            // 模擬點擊傳送訊息按鈕
            document.getElementById("sendButton").click();
        }
    });

    document.getElementById("sendButton").addEventListener("click", function () {
        // 獲取輸入欄位中的訊息
        const messageInput = document.getElementById("message");
        const message = messageInput.value;
    
        // 在這裡處理訊息，例如傳送訊息給其他用戶等等...
        // 這裡只是一個範例，您可以根據您的需求進行相應處理。

        // 清除輸入欄位的值
        //messageInput.value = '';

        // 更新聊天內容
        
    

        // 計算輸入次數
        inputCount++;
   
    

        // 如果輸入次數達到五次，則設定輸入欄位為唯讀狀態並隱藏按鈕
        if (inputCount >= maxInputCount) {
            messageInput.setAttribute("readonly", "readonly");
            document.getElementById("sendButton").style.display = "none";
        }

        intervalId = setInterval(checkInputCount, 500);
    });


    function checkInputCount() {
        // 如果 inputCount 大於等於 1，則啟動判斷邏輯
        if (inputCount >= 1) {
            // 在這裡寫入您要執行的判斷邏輯
            // 例如顯示提示、觸發某些事件等等...

            
            const messageInput = document.getElementById("message");
            messageInput.value = '';

            // 清除定時器，以免重複觸發判斷
            clearInterval(intervalId);//停止計時器
        }

    
    }
    

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
        } else{
            currentIndex = 0;
            shuffleBtn.classList.add("fadeOutButton");
            // 隱藏按鈕，使按鈕只需按一次後就會消失
        }
        
    }



const loginsec=document.querySelector('.login-section')
const loginlink=document.querySelector('.login-link')
const registerlink=document.querySelector('.register-link')
registerlink.addEventListener('click',()=>{
    loginsec.classList.add('active')
})
loginlink.addEventListener('click',()=>{
    loginsec.classList.remove('active')
})

// 驗證信箱功能
// let emailInput = document.getElementById('email-input');
// let btn = document.getElementById("resendButton2");

// emailInput.addEventListener('input', function() {
//   btn.disabled = true;
//   btn.classList.add("clicked");
// });

// function startCountdown(seconds) {
//     let emailInput = document.getElementById('email-input');
//     let button = document.getElementById("resendButton");
//     // let verifyEmailBtn = document.getElementById('verifyEmail-Btn');

//     button.disabled = true; // 禁用按鈕
//     button.innerText = seconds + " 秒後可重新發送";
//     button.classList.add("clicked"); // 添加類別
//     emailInput.disabled = true;
    

//     setTimeout(function() {
//       button.classList.remove("clicked");
//       button.disabled = false;
//     }, seconds * 1000);

//     let countdown = setInterval(function () {
//       seconds--;
//       if (seconds > 0) {
//         button.innerText = seconds + " 秒後可重新發送";
//       } else {
//         clearInterval(countdown);
//         button.innerText = "重新發送驗證信";
//         button.disabled = false; // 啟用按鈕
//         emailInput.disabled = false;
//       }
//     }, 1000);
//   }


function startCountdown(seconds, buttonId) {

  let button = document.getElementById(buttonId);
  // let emailInput = document.getElementById('email-input');

  button.disabled = true;
  button.classList.add("clicked");
  // emailInput.disabled = true;

  // 更新按鈕顯示的文字，顯示剩餘秒數
  button.innerHTML = seconds + ' 秒後可再次發送';

  // 開始倒數計時
  let countdownInterval = setInterval(function() {
    seconds--;

    // 更新按鈕顯示的文字，顯示剩餘秒數
    button.innerHTML = seconds + ' 秒後可再次發送';

    // 若倒數計時結束，還原按鈕狀態並停止計時
    if (seconds <= 0) {
      clearInterval(countdownInterval);
      button.innerHTML = '重發送驗證信';
      button.disabled = false;
      button.classList.remove("clicked");
      // emailInput.disabled = false;
    }
  }, 1000); // 每秒更新一次
}



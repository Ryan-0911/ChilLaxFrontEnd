using System.ComponentModel.DataAnnotations;

namespace ChilLaxFrontEnd.ViewModels
{
    public class LoginViewModel
    {
        //登入

        public int Id { get; set; }

        [Required(ErrorMessage = "請輸入帳號")]
        public string txtAccount { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        public string txtPassword { get; set; }

        //註冊
        [Required(ErrorMessage = "請輸入帳號")]
        public string txtRegisterAccount { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [Compare("txtRegisterPasswordChk", ErrorMessage = "確認密碼是否相同")]
        [DataType(DataType.Password)]
        public string txtRegisterPassword { get; set; }
        [Required(ErrorMessage = "請輸入相同密碼")]
        [DataType(DataType.Password)]
        public string txtRegisterPasswordChk { get; set; }

        [Required(ErrorMessage = "請輸入姓名")]
        public string memberName { get; set; }

        [Required(ErrorMessage = "請輸入手機")]
        public string memberPhone { get; set; }

        [Required(ErrorMessage = "請輸入信箱")]
        public string memberEmail { get; set; }

        public bool memberGender { get; set; }

        [Required(ErrorMessage = "請輸入生日")]
        [Range(typeof(DateTime), "1/1/1900", "1/1/2024", ErrorMessage = "日期區間，只能在1950年以後~2025年之前")]
        public DateTime memberBirth { get; set; }
        public string memberAddress { get; set; }

    }
}

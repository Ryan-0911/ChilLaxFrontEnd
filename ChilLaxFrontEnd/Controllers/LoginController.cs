using ChilLaxFrontEnd.Models;
using ChilLaxFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using System.Diagnostics;
using System.Text.Json;

namespace ChilLaxFrontEnd.Controllers
{
    public class LoginController : Controller
    {

        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
        {
            MemberCredential membercredential = (new ChilLaxContext()).MemberCredentials.FirstOrDefault(
                t => t.MemberAccount.Equals(vm.txtAccount) && t.MemberPassword.Equals(vm.txtPassword));
            Member member = (new ChilLaxContext()).Members.FirstOrDefault(
                t => t.MemberId.Equals(membercredential.MemberId) && t.Available == true);
            LoginViewModel user = new LoginViewModel
            {
                txtAccount = membercredential.MemberAccount,
                txtPassword = membercredential.MemberPassword
            };
            if (membercredential != null && membercredential.MemberPassword.Equals(vm.txtPassword) && member.Available == true)
            {
                string json = JsonSerializer.Serialize(user);
                HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, json);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Register()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Register(LoginViewModel vm)
        {
            MemberCredential membercredential = (new ChilLaxContext()).MemberCredentials.FirstOrDefault(
t => t.MemberAccount.Equals(vm.txtRegisterAccount));
            //MemberCredential membercredential=new MemberCredential();
            //Member member = new Member();
            MemberCredential mc = new MemberCredential
            {
                MemberAccount = vm.txtRegisterAccount,
                MemberPassword = vm.txtRegisterPassword
            };
            if (membercredential == null && vm.txtRegisterPassword != null && vm.txtRegisterPasswordChk == vm.txtRegisterPassword)
            {
                string json = JsonSerializer.Serialize(mc);
                HttpContext.Session.SetString(CDictionary.SK_REGISTER_USER, json);
                return RedirectToAction("registerProfile");
            }
            return View();
        }

        public IActionResult registerProfile()
        {

            return View();
        }
        [HttpPost]
        public IActionResult registerProfile(RegisterViewModel rvm)
        {
            Member member = new Member
            {
                MemberName = rvm.memberName,
                MemberTel = rvm.memberPhone,
                MemberEmail = rvm.memberEmail,
                MemberSex = rvm.memberGender,
                MemberBirthday = rvm.memberBirth,
                MemberAddress = rvm.memberAddress,
                MemberPoint = 0,
                MemberJoinTime = DateTime.Now,
                Available = true
            };

            string json = HttpContext.Session.GetString(CDictionary.SK_REGISTER_USER);  //取session註冊的帳號密碼資料
            MemberCredential mc = JsonSerializer.Deserialize<MemberCredential>(json);  //將json字串轉成物件lvm
            Console.WriteLine(mc);

            if (rvm.memberName != null && rvm.memberPhone != null && rvm.memberEmail!= null && rvm.memberBirth != null)
            {

                ChilLaxContext db = new ChilLaxContext();
                db.Members.Add(member);
                db.MemberCredentials.Add(mc);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public IActionResult forgetPassword()
        {

            return View();
        }
        [HttpPost]
        public IActionResult forgetPassword(RegisterViewModel rvm)
        {
            
            return View();
        }


    }
}

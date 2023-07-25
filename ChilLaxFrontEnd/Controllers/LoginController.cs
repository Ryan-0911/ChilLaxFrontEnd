using ChilLaxFrontEnd.Models;
using ChilLaxFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text.Encodings.Web;

namespace ChilLaxFrontEnd.Controllers
{
    public class LoginController : Controller
    {

        //private readonly ILogger<LoginController> _logger;

        //public LoginController(ILogger<LoginController> logger)
        //{
        //    _logger = logger;
        //}
        private readonly ChilLaxContext _context;

        public LoginController(ChilLaxContext context)
        {
            _context = context;
        }


        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
        {
            MemberCredential membercredential = (new ChilLaxContext()).MemberCredential.FirstOrDefault(
                t => t.MemberAccount.Equals(vm.txtAccount) && t.MemberPassword.Equals(vm.txtPassword));

            bool accountExists = _context.MemberCredential.Any(mc => mc.MemberAccount.Equals(vm.txtAccount) && mc.MemberPassword.Equals(vm.txtPassword));
            Member member = (new ChilLaxContext()).Member.FirstOrDefault(
                t => t.MemberId.Equals(membercredential.MemberId) && t.Available == true);
            LoginViewModel user = new LoginViewModel
            {
                txtAccount = membercredential.MemberAccount,
                txtPassword = membercredential.MemberPassword
            };
            
            if (accountExists == true && membercredential.MemberPassword.Equals(vm.txtPassword) && member.Available == true)   
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
            MemberCredential membercredential = (new ChilLaxContext()).MemberCredential.FirstOrDefault(
t => t.MemberAccount.Equals(vm.txtRegisterAccount));

            bool accountExists = _context.MemberCredential.Any(mc => mc.MemberAccount.Equals(vm.txtAccount));
            //MemberCredential membercredential=new MemberCredential();
            //Member member = new Member();
            MemberCredential mc = new MemberCredential
            {
                MemberAccount = vm.txtRegisterAccount,
                MemberPassword = vm.txtRegisterPassword
            };
            if (accountExists == false && vm.txtRegisterPassword != null && vm.txtRegisterPasswordChk == vm.txtRegisterPassword)
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
        public IActionResult registerProfile(LoginViewModel vm)
        {
            Member member = new Member
            {
                MemberName = vm.memberName,
                MemberTel = vm.memberPhone,
                MemberEmail = vm.memberEmail,
                MemberSex = vm.memberGender,
                MemberBirthday = vm.memberBirth,
                MemberAddress = vm.memberAddress,
                MemberPoint = 0,
                MemberJoinTime = DateTime.Now,
                Available = true
            };


            string json = HttpContext.Session.GetString(CDictionary.SK_REGISTER_USER);  //取session註冊的帳號密碼資料
            MemberCredential mc = JsonSerializer.Deserialize<MemberCredential>(json);  //將json字串轉成物件lvm
            Console.WriteLine(mc);

            if (vm.memberName != null && vm.memberPhone != null && vm.memberEmail!= null && vm.memberBirth != null)
            {

                ChilLaxContext db = new ChilLaxContext();
                db.Member.Add(member);
                db.MemberCredential.Add(mc);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //Google第三方登入

        public IActionResult ValidGoogleLogin()
        {
            string? formCredential = Request.Form["credential"]; //回傳憑證
            string? formToken = Request.Form["g_csrf_token"]; //回傳令牌
            string? cookiesToken = Request.Cookies["g_csrf_token"]; //Cookie 令牌

            // 驗證 Google Token
            GoogleJsonWebSignature.Payload? payload = VerifyGoogleToken(formCredential, formToken, cookiesToken).Result;


            if (payload == null)
            {
                // 驗證失敗
                ViewData["Msg"] = "驗證 Google 授權失敗";
            }
            else
            {
                PropertyInfo[] properties = payload.GetType().GetProperties();
                //Member member = new Member();
                bool emailExists = _context.Member.Any(m => m.MemberEmail.Equals(payload.Email));
                var memberData = new
                {
                    MemberEmail = payload.Email,
                    MemberName = payload.Name
                };
                LoginViewModel lvm = new LoginViewModel
                {
                    memberEmail = payload.Email,
                    memberName = payload.Name
                };
                if (emailExists == false)
                {
                    
                    string json = JsonSerializer.Serialize(memberData, new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = true
                    });

                    HttpContext.Session.SetString(CDictionary.SK_EXTERNALLOGIN_USER, json);
                    //string test = HttpContext.Session.GetString(CDictionary.SK_EXTERNALLOGIN_USER);
                    //Member mem = JsonSerializer.Deserialize<Member>(test);
                    //Console.WriteLine(test);
                    //return RedirectToAction("registerProfile");
               
                    return View(lvm);

                }

            }

            //return View();
            return RedirectToAction("Index", "Home");
        }

        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string? formCredential, string? formToken, string? cookiesToken)
        {
            // 檢查空值
            if (formCredential == null || formToken == null && cookiesToken == null)
            {
                return null;
            }

            GoogleJsonWebSignature.Payload? payload;
            try
            {
                // 驗證 token
                if (formToken != cookiesToken)
                {
                    return null;
                }

                // 驗證憑證
                IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
                string GoogleApiClientId = Config.GetSection("GoogleApiClientId").Value;
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { GoogleApiClientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(formCredential, settings);
                if (!payload.Issuer.Equals("accounts.google.com") && !payload.Issuer.Equals("https://accounts.google.com"))
                {
                    return null;
                }
                if (payload.ExpirationTimeSeconds == null)
                {
                    return null;
                }
                else
                {
                    DateTime now = DateTime.Now.ToUniversalTime();
                    DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds).DateTime;
                    if (now > expiration)
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
            return payload;
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

﻿using ChilLaxFrontEnd.Models;
using ChilLaxFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Text.Encodings.Web;
using Microsoft.CodeAnalysis.Scripting;

namespace ChilLaxFrontEnd.Controllers
{
    public class LoginController : Controller
    {

        //private readonly ILogger<LoginController> _logger;

        //public LoginController(ILogger<LoginController> logger)
        //{
        //    _logger = logger;
        //}
        ChilLaxContext db = new ChilLaxContext();

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
            // 確認使用者登入的帳密是否存在，並取出該資料列
            MemberCredential membercredential = (new ChilLaxContext()).MemberCredentials.FirstOrDefault(
                t => t.MemberAccount.Equals(vm.txtAccount) && t.MemberPassword.Equals(vm.txtPassword));

            // 確認使用者登入的帳密是否存在，回傳布林值
            bool accountExists = _context.MemberCredentials.Any(mc => mc.MemberAccount.Equals(vm.txtAccount) && mc.MemberPassword.Equals(vm.txtPassword));
            
            // 利用id關聯到Member資料表，取出登入的會員資料
            Member member = (new ChilLaxContext()).Members.FirstOrDefault(
                t => t.MemberId.Equals(membercredential.MemberId) && t.Available == true);

            // 只有在會員帳密跟會員基本資料都存在的話，才會執行下列程式碼
            if (membercredential != null && member != null)
            {
                if (accountExists == true && membercredential.MemberPassword.Equals(vm.txtPassword) && member.Available == true)
                {
                    // 計算會員點數，並存入member物件
                    member.MemberPoint = _context.PointHistories.Where(ph => ph.MemberId == member.MemberId).Sum(ph => ph.ModifiedAmount);
                    // 將member物件傳換成JSON字串
                    string json = JsonSerializer.Serialize(member);
                    Console.WriteLine(json);
                    // 以key(SK_LOINGED_USER)儲存該JSON字串
                    HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, json);
                    return RedirectToAction("Index", "Home");
                }
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

            bool accountExists = _context.MemberCredentials.Any(mc => mc.MemberAccount.Equals(vm.txtAccount));
            //MemberCredential membercredential=new MemberCredential();
            //Member member = new Member();
            
            string password = vm.txtRegisterPassword;  // 假設這是使用者輸入的密碼
            string salt = BCrypt.Net.BCrypt.GenerateSalt();// 產生隨機的鹽值
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);// 將密碼和鹽值一起加密

            var memberData = new 
            {
                MemberAccount = vm.txtRegisterAccount,
                MemberPassword = hashedPassword

            };
            if (accountExists == false && vm.txtRegisterPassword != null && vm.txtRegisterPasswordChk == vm.txtRegisterPassword)
            {
                string json = JsonSerializer.Serialize(memberData);
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

            //if (vm.memberName != null && vm.memberPhone != null && vm.memberEmail!= null && vm.memberBirth != null && HttpContext.Session.Keys.Contains(CDictionary.SK_REGISTER_USER))

            //string json = HttpContext.Session.GetString(CDictionary.SK_REGISTER_USER);  //取session註冊的帳號密碼資料
            //MemberCredential mc = JsonSerializer.Deserialize<MemberCredential>(json);  //將json字串轉成物件lvm
            //Console.WriteLine(mc);

            if (vm.memberName != null && vm.memberPhone != null && vm.memberEmail != null && vm.memberBirth != null)
            {
                db.Members.Add(member);
                db.SaveChanges();

                string json = HttpContext.Session.GetString(CDictionary.SK_REGISTER_USER);  //取session註冊的帳號密碼資料
                MemberCredential mc = JsonSerializer.Deserialize<MemberCredential>(json);  //將json字串轉成物件lvm 
                MemberCredential credential = new MemberCredential 
                {
                    MemberId = member.MemberId,
                    MemberAccount = mc.MemberAccount,
                    MemberPassword = mc.MemberPassword
                };
                db.MemberCredentials.Add(credential);
                db.SaveChanges();

                

                return RedirectToAction("Login");

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
            Member member = (new ChilLaxContext()).Members.FirstOrDefault(
                t => t.MemberEmail.Equals(payload.Email) && t.Available == true);

            if (payload == null)
            {
                // 驗證失敗
                ViewData["Msg"] = "驗證 Google 授權失敗";
                return RedirectToAction("Login");
            }
            else
            {
                PropertyInfo[] properties = payload.GetType().GetProperties();
                
                bool emailExists = _context.Members.Any(m => m.MemberEmail.Equals(payload.Email));
                var memberData = new
                {
                    Provider = "Google",
                    ProviderUserId = payload.Subject
                };
                var view = new
                {
                    memberEmail = payload.Email,
                    memberName = payload.Name
                    
                };
                if (member == null)
                {

                    string json = JsonSerializer.Serialize(memberData, new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = true
                    });

                    HttpContext.Session.SetString(CDictionary.SK_EXTERNALLOGIN_USER, json);

                    return View(view);
                    //return RedirectToAction("registerProfile");


                }
                else 
                {
                    member.MemberPoint = _context.PointHistories.Where(ph => ph.MemberId == member.MemberId).Sum(ph => ph.ModifiedAmount);
                        
                    string json = JsonSerializer.Serialize(member);
                    //Console.WriteLine(json);
                    HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, json);
                    return RedirectToAction("Index", "Home");
                 
                }

            }
        }


        [HttpPost]
        public IActionResult ProcessGoogleLogin(LoginViewModel vm)
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_EXTERNALLOGIN_USER);
            Member mem = JsonSerializer.Deserialize<Member>(json);
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
                Available = true,
                Provider = mem.Provider,
                ProviderUserId = mem.ProviderUserId
            };

            if (member.MemberName != null && member.MemberTel != null && member.MemberEmail != null && member.MemberBirthday != null && member.Provider != null && member.ProviderUserId != null)
            {
                db.Members.Add(member);
                db.SaveChanges();

                string Memjson = JsonSerializer.Serialize(member);
                //Console.WriteLine(json);
                HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, Memjson);
                return RedirectToAction("Index", "Home");

                //return RedirectToAction("Index", "Home");
                //return RedirectToAction("Login");

            }
            return View();
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

        //登出
        public IActionResult Logout()
        {
            
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }


        public IActionResult forgetPassword()
        {

            return View();
        }
        [HttpPost]
        public IActionResult forgetPassword(LoginViewModel vm)
        {

            return View();
        }


    }
}

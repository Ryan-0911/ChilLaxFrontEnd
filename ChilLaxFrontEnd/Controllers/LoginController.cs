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
using System.Text.Encodings.Web;
using Microsoft.CodeAnalysis.Scripting;
//以下為新增的
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json.Linq;
using Microsoft.Data.SqlClient.Server;
using System.Diagnostics.Metrics;

namespace ChilLaxFrontEnd.Controllers
{
    public class LoginController : Controller
    {

        //private readonly ILogger<LoginController> _logger;

        //public LoginController(ILogger<LoginController> logger)
        //{
        //    _logger = logger;
        //}
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ChilLaxContext _context;

        public LoginController(IWebHostEnvironment hostingEnvironment, ChilLaxContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        ChilLaxContext db = new ChilLaxContext();

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
        {
            // 確認使用者登入的帳密是否存在，並取出該資料列
            MemberCredential membercredential = (new ChilLaxContext()).MemberCredential.FirstOrDefault(
                t => t.MemberAccount.Equals(vm.txtAccount) && t.MemberPassword.Equals(vm.txtPassword));

            // 確認使用者登入的帳密是否存在，回傳布林值
            bool accountExists = _context.MemberCredential.Any(mc => mc.MemberAccount.Equals(vm.txtAccount) && mc.MemberPassword.Equals(vm.txtPassword));
            
            // 利用id關聯到Member資料表，取出登入的會員資料
            Member member = (new ChilLaxContext()).Member.FirstOrDefault(
                t => t.MemberId.Equals(membercredential.MemberId) && t.Available == true);

            bool isPwdMatch = BCrypt.Net.BCrypt.Verify(vm.txtPassword, membercredential.MemberPassword);
            Console.WriteLine("驗證結果：" + isPwdMatch); // 印出 true



            if (membercredential != null && member != null && isPwdMatch == true)
            {
                if (accountExists == true && membercredential.MemberPassword.Equals(vm.txtPassword) && member.Available == true)
                {
                    // 計算會員點數，並存入member物件
                    member.MemberPoint = _context.PointHistory.Where(ph => ph.MemberId == member.MemberId).Sum(ph => ph.ModifiedAmount);
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
            MemberCredential membercredential = (new ChilLaxContext()).MemberCredential.FirstOrDefault(
t => t.MemberAccount.Equals(vm.txtRegisterAccount));

            bool accountExists = _context.MemberCredential.Any(mc => mc.MemberAccount.Equals(vm.txtAccount));
            
            string password = vm.txtRegisterPassword;  
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


            if (vm.memberName != null && vm.memberPhone != null && vm.memberEmail != null && vm.memberBirth != null)
            {

                string json = HttpContext.Session.GetString(CDictionary.SK_REGISTER_USER);  //取session註冊的帳號密碼資料
                MemberCredential mc = JsonSerializer.Deserialize<MemberCredential>(json);  //將json字串轉成物件lvm 
                if (mc != null)
                {
                    db.Member.Add(member);
                    db.SaveChanges();

                    MemberCredential credential = new MemberCredential
                    {
                        MemberId = member.MemberId,
                        MemberAccount = mc.MemberAccount,
                        MemberPassword = mc.MemberPassword
                    };
                    db.MemberCredential.Add(credential);
                    db.SaveChanges();

                    Member memberData = (new ChilLaxContext()).Member.FirstOrDefault(
                t => t.MemberId.Equals(member.MemberId));
                    string toVerifyEmail = JsonSerializer.Serialize(memberData);
                    HttpContext.Session.SetString(CDictionary.SK_REGISTER_USER, toVerifyEmail);
                    if (!HttpContext.Session.Keys.Contains(CDictionary.SK_REGISTER_USER)) 
                    {
                        return View();
                    }
                    return RedirectToAction("verifyEmail");
                }

            }
            return View();
        }

        //驗證信箱及註冊
        public IActionResult verifyEmail()
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_REGISTER_USER);
            Member mc = JsonSerializer.Deserialize<Member>(json);
            VerifyEmailViewModel VE = new VerifyEmailViewModel();
            VE.MemberId = mc.MemberId;
            VE.MemberEmail = mc.MemberEmail;
            return View(VE);
        }

        //[HttpPut("{id}")]
        //public async Task<string> SaveData(int id,LoginViewModel MemberVM)
        //{
        //    Member Mem = await _context.Members.FindAsync(id);
        //    if (Mem != null)
        //    {
        //        Mem.MemberId = MemberVM.Id;
        //        Mem.MemberEmail = MemberVM.memberEmail;
        //        Mem.IsValid = false;
        
        //        _context.Entry(Mem).State = EntityState.Modified;
        //        db.SaveChanges();
                

        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {//DbUpdateConcurrencyException資料庫因為是可以多人同時操作，如果有多個人同時作業或是有人搶先修改過了，就會做更新失敗。
        //            if (!MemberExists(id))
        //            {
        //                return "修改失敗";
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        BuildEmailTemplate(Mem.MemberId);

        //        return "修改成功";

        //    }
        //    else
        //    {
        //        return "修改失敗";
        //    }
                        
        //}


        //public void BuildEmailTemplate(int regID)
        //{
        //    string templatePath = Path.Combine(_hostingEnvironment.WebRootPath, "EmailTemplate", "Text.cshtml");
        //    string body = System.IO.File.ReadAllText(templatePath);
        //    var regInfo = db.Members.Where(x => x.MemberId == regID).FirstOrDefault();
        //    string UserName = regInfo.MemberName;
        //    body = body.Replace("@ViewBag.UserName", UserName);
        //    body = body.ToString();
        //    BuildEmailTemplate("驗證帳號", body, regInfo.MemberEmail);
        //}

        //public static void BuildEmailTemplate(string subjectText, string bodyText, string sendTo)
        //{
        //    string GoogleID = "chillax20230808@gmail.com"; //Google 發信帳號
        //    string TempPwd = "gzwmfcbpepypgikf"; //應用程式密碼

        //    string to, body, bcc, cc;
        //    to = sendTo.Trim();
        //    bcc = "";
        //    cc = "";

        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(bodyText);
        //    body = sb.ToString();
        //    MailMessage mail = new MailMessage();
        //    mail.From = new MailAddress(GoogleID);//發件人
        //    mail.To.Add(new MailAddress(to));//收件人
        //    if (!string.IsNullOrEmpty(bcc))
        //    {
        //        mail.Bcc.Add(new MailAddress(bcc));
        //    }
        //    if (!string.IsNullOrEmpty(cc))
        //    {
        //        mail.CC.Add(new MailAddress(cc));
        //    }
        //    mail.Subject = subjectText;//郵件主題
        //    mail.Body = body;//郵件內文
        //    mail.IsBodyHtml = true;
        //    mail.SubjectEncoding = Encoding.UTF8;

        //    string SmtpServer = "smtp.gmail.com";
        //    int SmtpPort = 587;
        //    using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))//使用郵件伺服器來發送這個郵件
        //    {
        //        client.EnableSsl = true;
        //        client.Credentials = new NetworkCredential(GoogleID, TempPwd);//寄信帳密 
        //        try
        //        {
        //            client.Send(mail);//寄出信件
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }

        //}




        //Google第三方登入

        public IActionResult ValidGoogleLogin()
        {
            string? formCredential = Request.Form["credential"]; //回傳憑證
            string? formToken = Request.Form["g_csrf_token"]; //回傳令牌
            string? cookiesToken = Request.Cookies["g_csrf_token"]; //Cookie 令牌

            // 驗證 Google Token
            GoogleJsonWebSignature.Payload? payload = VerifyGoogleToken(formCredential, formToken, cookiesToken).Result;
            Member member = (new ChilLaxContext()).Member.FirstOrDefault(
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
                //Member member = new Member();
                bool emailExists = _context.Member.Any(m => m.MemberEmail.Equals(payload.Email));
                var memberData = new
                {
                    Provider = "Google",
                    ProviderUserId = payload.Subject
                };
                var view = new LoginViewModel
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
                   
                }
                else 
                {
                    member.MemberPoint = _context.PointHistory.Where(ph => ph.MemberId == member.MemberId).Sum(ph => ph.ModifiedAmount);
                        
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
                db.Member.Add(member);
                db.SaveChanges();

                string Memjson = JsonSerializer.Serialize(member);
                HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, Memjson);
                return RedirectToAction("Index", "Home");
              
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

        public IActionResult editMemberProfile()
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            Member member = JsonSerializer.Deserialize<Member>(json);
           
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER)) 
            {
                return RedirectToAction("Login");
            }
            LoginViewModel vm = new LoginViewModel();
            vm.Id = member.MemberId;
            vm.memberName = member.MemberName;
            vm.memberPhone = member.MemberTel;
            vm.memberBirth = member.MemberBirthday;
            vm.memberEmail = member.MemberEmail;
            vm.memberGender = (bool)member.MemberSex;
            vm.memberAddress = member.MemberAddress;

            return View(vm);
        }
        [HttpPost]
        public IActionResult editMemberProfile(LoginViewModel vm)
        {
            Member member = (new ChilLaxContext()).Member.FirstOrDefault(
               t => t.MemberId.Equals(vm.Id));
            if (member != null)
            {
                if (vm.memberName != null && vm.memberPhone != null && vm.memberEmail != null && vm.memberBirth != null)
                {
                    member.MemberName = vm.memberName;
                    member.MemberTel = vm.memberPhone;
                    member.MemberEmail = vm.memberEmail;
                    member.MemberSex = vm.memberGender;
                    member.MemberBirthday = vm.memberBirth;
                    member.MemberAddress = vm.memberAddress;
                   
                    db.Entry(member).State = EntityState.Modified;  //以防更新未被檢測到
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.MemberId == id);
        }



    }
}

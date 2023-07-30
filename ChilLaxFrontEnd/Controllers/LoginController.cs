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
using Microsoft.AspNetCore.Cors;



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

		public IActionResult Register()
		{

			return View();
		}

		public IActionResult registerProfile()
		{

			return View();
		}

		//驗證信箱及註冊
		public IActionResult verifyEmail()
		{
			string json = HttpContext.Session.GetString(CDictionary.SK_REGISTER_USER);
			VerifyEmailViewModel VE = JsonSerializer.Deserialize<VerifyEmailViewModel>(json);
			return View(VE);
		}

		public async Task<IActionResult> Verify(int regID)
		{
			int id = Convert.ToInt32(regID);
			var user = db.Member.FirstOrDefault(x => x.MemberId == regID);
			if (user != null)
			{
				user.IsValid = true;
				await db.SaveChangesAsync();
				return View();
			}

			var errorResponse = new { message = "驗證失敗！" };
			return NotFound(errorResponse);
		}


		//Google第三方登入
		//註冊畫面

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
				return RedirectToAction("Login");
			}
			else
			{
				PropertyInfo[] properties = payload.GetType().GetProperties();
				Member member = _context.Member.FirstOrDefault(
				t => t.MemberEmail.Equals(payload.Email) && t.Available == true);

				if (member == null)
				{
					var memberData = new
					{
						Provider = "Google",
						ProviderUserId = payload.Subject
					};
					var view = new registerViewModel
					{
						memberEmail = payload.Email,
						memberName = payload.Name

					};
					string memjson = JsonSerializer.Serialize(memberData, new JsonSerializerOptions
					{
						Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
						WriteIndented = true
					});

					HttpContext.Session.SetString(CDictionary.SK_EXTERNALLOGIN_USER, memjson);

					return View(view);


				}

				member.MemberPoint = _context.PointHistory.Where(ph => ph.MemberId == member.MemberId).Sum(ph => ph.ModifiedAmount);


				var LoginData = new
				{
					MemberId = member.MemberId,
					MemberName = member.MemberName,
					MemberPoint = member.MemberPoint
				};

				string Memjson = JsonSerializer.Serialize(LoginData);
				HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, Memjson);
				if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
				{
					return RedirectToAction("Login");

				}

				return RedirectToAction("Index", "Home");



			}
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
		public IActionResult forgetPassword(registerViewModel rvm)
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
			Member memProfile = _context.Member.SingleOrDefault(m => m.MemberId.Equals(member.MemberId));
			registerViewModel rvm = new registerViewModel();
			if (member != null)
			{
				rvm.memberId = memProfile.MemberId;
				rvm.memberName = memProfile.MemberName;
				rvm.memberTel = memProfile.MemberTel;
				rvm.memberEmail = memProfile.MemberEmail;

				rvm.memberBirthday = memProfile.MemberBirthday;
				rvm.memberSex = (bool)memProfile.MemberSex;
				rvm.memberAddress = memProfile.MemberAddress;

        //新增會員訂單資料
        List<MemberOrder> memberOrder = _context.ProductOrder
            .Where(od => od.MemberId == member.MemberId)
            .Join(_context.OrderDetail,
                po => po.OrderId,
                od => od.OrderId,
                (po, od) => new MemberOrder
                {
                    ProductOrder = new List<ProductOrder> { po },
                    orderDetails = new List<OrderDetail> { od }
                })
            .ToList();
        rvm.memberOrder = memberOrder;


				return View(rvm);
			}
			return RedirectToAction("Index", "Home");

		}
		//[HttpPost]
		//public IActionResult editMemberProfile(LoginViewModel vm)
		//{
		//    Member member = _context.Member.FirstOrDefault(
		//       t => t.MemberId.Equals(vm.memberId));
		//    if (member != null)
		//    {
		//        if (vm.memberName != null && vm.memberTel != null && vm.memberEmail != null && vm.memberBirthday != null)
		//        {
		//            member.MemberName = vm.memberName;
		//            member.MemberTel = vm.memberTel;
		//            member.MemberEmail = vm.memberEmail;
		//            member.MemberSex = vm.memberSex;
		//            member.MemberBirthday = vm.memberBirthday;
		//            member.MemberAddress = vm.memberAddress;

		//            db.Entry(member).State = EntityState.Modified;  //以防更新未被檢測到
		//            db.SaveChanges();
		//            return RedirectToAction("Index", "Home");
		//        }
		//    }
		//    return View();
		//}



	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChilLaxFrontEnd.Models;
using ChilLaxFrontEnd.ViewModels;
using System.Net.Mail;
using System.Net;
using System.Text;
using Google;
using System.Web;
using Newtonsoft.Json.Linq;
using Microsoft.Data.SqlClient.Server;
using Microsoft.AspNetCore.Cors;
using System.Text.Json;
using System.Text.Json.Serialization;
using BCrypt.Net;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using System.Diagnostics.Metrics;



namespace ChilLaxFrontEnd.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MembersController : ControllerBase
	{
		private readonly ChilLaxContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public MembersController(ChilLaxContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

		// PUT: api/Members/5
		[HttpPut("{id}")]
		public async Task<string> PutMember(int id, VerifyEmailViewModel VE)
		{
			if (id != VE.MemberId)
			{
				return "驗證失敗";

			}
			Member Mem = await _context.Member.FindAsync(VE.MemberId);
			if (Mem != null)
			{

				try
				{
					await BuildEmailTemplate(VE.MemberId);

					return "已發送驗證信，請前往Email查看!";
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!MemberExists(id))
					{
						return "系統忙碌中，請稍後再試!";
					}
					else
					{
						throw;
					}
				}

			}
			return "驗證失敗!";
		}

		public async Task BuildEmailTemplate(int regID)
		{
			string body;
			try
			{
				body = System.IO.File.ReadAllText(Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplate", "Text.cshtml"));
			}
			catch (Exception ex)
			{
				throw ex;
			}

			//string body = $@"
			//            <html>
			//            <head>
			//                <title>驗證帳號</title>
			//            </head>
			//            <body>
			//                <h1>歡迎您，@ViewBag.UserName</h1>
			//                <p>感謝您註冊成為我們的會員。</p>
			//                <p>請點擊以下連結來驗證您的帳號：</p>
			//                <a href=""https://localhost:5000/api/Members/Verify/{regID}"">點擊這裡</a>
			//            </body>
			//            </html>
			//        ";
			var regInfo = _context.Member.Where(x => x.MemberId == regID).FirstOrDefault();
			//var url = "https://localhost:5000/Login/Verify?regID=" + regID;
			//body = body.Replace("@ViewBag.ConfirmationLink", url);
			string UserName = regInfo.MemberName;
			body = body.Replace("@ViewBag.UserName", UserName);
			body = body.Replace("@ViewBag.regID", regID.ToString());

			body = body.ToString();

			await BuildEmailAsync("驗證帳號", body, regInfo.MemberEmail);
		}

		public async Task BuildEmailAsync(string subjectText, string bodyText, string sendTo)
		{
			string GoogleID = "chillax20230808@gmail.com"; //Google 發信帳號
			string TempPwd = "gzwmfcbpepypgikf"; //應用程式密碼

			string to, body, bcc, cc;
			to = sendTo.Trim();
			bcc = "";
			cc = "";

			StringBuilder sb = new StringBuilder();
			sb.Append(bodyText);
			body = sb.ToString();
			MailMessage mail = new MailMessage();
			mail.From = new MailAddress(GoogleID);//發件人
			mail.To.Add(new MailAddress(to));//收件人
			if (!string.IsNullOrEmpty(bcc))
			{
				mail.Bcc.Add(new MailAddress(bcc));
			}
			if (!string.IsNullOrEmpty(cc))
			{
				mail.CC.Add(new MailAddress(cc));
			}
			mail.Subject = subjectText;//郵件主題
			mail.Body = body;//郵件內文
			mail.IsBodyHtml = true;
			mail.SubjectEncoding = Encoding.UTF8;

			string SmtpServer = "smtp.gmail.com";
			int SmtpPort = 587;
			using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))//使用郵件伺服器來發送這個郵件
			{
				client.EnableSsl = true; //啟用安全連線（SSL/TLS）
				client.Credentials = new NetworkCredential(GoogleID, TempPwd);//寄信帳密 
				try
				{
					await client.SendMailAsync(mail);//寄出信件，使用SendMailAsync進行非同步寄信
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
		}



		//POST: api/Login
		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromBody] SigninViewModel formData)
		{
			string memberAccount = formData.memberAccount;
			var query = from mc in _context.MemberCredential
						join m in _context.Member on mc.MemberId equals m.MemberId
						where mc.MemberAccount == memberAccount && m.Available == true
						select new { MemberCredential = mc, Member = m };

			var result = await query.FirstOrDefaultAsync();

			if (result == null)
			{
				return BadRequest(new { success = false, message = "無此帳號，請重新輸入!" });
			}

			MemberCredential membercredential = result.MemberCredential;
			Member member = result.Member;

			bool isPwdMatch = BCrypt.Net.BCrypt.Verify(formData.memberPassword, membercredential.MemberPassword);

			Console.WriteLine("驗證結果：" + isPwdMatch);
			try
			{
				if (!isPwdMatch)
				{
					return BadRequest(new { success = false, message = "帳號或密碼錯誤，請重新輸入。" });

				}

				member.MemberPoint = await _context.PointHistory
					.Where(ph => ph.MemberId == member.MemberId)
					.SumAsync(ph => ph.ModifiedAmount);
                _context.Entry(member).State = EntityState.Modified;//新增兩行，修改點數
                await _context.SaveChangesAsync();

                var LoginData = new
				{
					MemberId = member.MemberId,
					MemberName = member.MemberName,
					MemberPoint = member.MemberPoint
				};
				string json = JsonSerializer.Serialize(LoginData);//使用LoginData避免循環參考
				Console.WriteLine(json);
				HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, json);
				if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
				{
					return BadRequest(new { success = false, message = "伺服器錯誤，請稍後再試!" });
				}
				return Ok(new { success = true, message = "登入成功" });
			}
			catch (Exception ex)
			{
				Console.WriteLine("序列化錯誤：" + ex.Message);
				return BadRequest(new { success = false, message = "伺服器錯誤，請稍後再試。", error = ex.Message });

			}

		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register([FromBody] SigninViewModel formData)
		{
			if (formData.memberAccount != null && formData.memberPassword != null)
			{
				bool accountExists =await _context.MemberCredential.AnyAsync(mc => mc.MemberAccount.Equals(formData.memberAccount));
				
				if (accountExists == false)
				{
					string password = formData.memberPassword;
					string salt = BCrypt.Net.BCrypt.GenerateSalt();// 產生隨機的鹽值
					string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);// 將密碼和鹽值一起加密

					formData.memberAccount = formData.memberAccount;
					formData.memberPassword = hashedPassword;
					string json = JsonSerializer.Serialize(formData);
					HttpContext.Session.SetString(CDictionary.SK_REGISTER_USER, json);
					//return RedirectToAction("registerProfile");
					return Ok(new { success = true });
				}
				return BadRequest(new { success = false, message = "此帳號已註冊過，請重新登入!" });
			}
			return BadRequest(new { success = false, message = "請輸入帳號及密碼!" });

		}


		[HttpPost("registerProfile")]
		public async Task<IActionResult> registerProfile([FromBody] registerViewModel formData)
		{
			try
			{
				if (ModelState.IsValid)
				{
					Member member = new Member
					{
						MemberName = formData.memberName,
						MemberTel = formData.memberTel,
						MemberEmail = formData.memberEmail,
						MemberSex = formData.memberSex,
						MemberBirthday = formData.memberBirthday,
						MemberAddress = formData.memberAddress,
						MemberPoint = 0,
						MemberJoinTime = DateTime.Now,
						Available = true,
						IsValid = false
					};

					string json = HttpContext.Session.GetString(CDictionary.SK_REGISTER_USER);  //取session註冊的帳號密碼資料
					SigninViewModel svm = JsonSerializer.Deserialize<SigninViewModel>(json);
					if (svm != null)
					{
						_context.Member.Add(member);
						await _context.SaveChangesAsync();

						MemberCredential mc = new MemberCredential();
						mc.MemberId = member.MemberId;
						mc.MemberAccount = svm.memberAccount;
						mc.MemberPassword = svm.memberPassword;

						_context.MemberCredential.Add(mc);
						await _context.SaveChangesAsync();

						VerifyEmailViewModel emailvm = new VerifyEmailViewModel();
						emailvm.MemberId = member.MemberId;
						emailvm.MemberEmail = member.MemberEmail;

						string toVerifyEmail = JsonSerializer.Serialize(emailvm);
						HttpContext.Session.SetString(CDictionary.SK_REGISTER_USER, toVerifyEmail);
						if (!HttpContext.Session.Keys.Contains(CDictionary.SK_REGISTER_USER))
						{
							return BadRequest(new { success = false, message = "伺服器錯誤，請稍後再試!" });
						}
						return Ok(new { success = true });
					}
				}
			}
			catch (Exception ex)
			{

				Console.WriteLine("發生異常：" + ex.Message);
				return BadRequest(new { success = false, message = "伺服器錯誤，請稍後再試!" });

			}
			return BadRequest(new { success = false, message = "請輸入必填欄位!" });


		}

		[HttpPost("ProcessGoogleLogin")]
		public async Task<IActionResult> ProcessGoogleLogin([FromBody] registerViewModel rvm)
		{
			try
			{
				if (ModelState.IsValid)
				{
					string json = HttpContext.Session.GetString(CDictionary.SK_EXTERNALLOGIN_USER);
					Member mem = JsonSerializer.Deserialize<Member>(json);
					mem.MemberName = rvm.memberName;
					mem.MemberTel = rvm.memberTel;
					mem.MemberEmail = rvm.memberEmail;
					mem.MemberSex = rvm.memberSex;
					mem.MemberBirthday = rvm.memberBirthday;
					mem.MemberAddress = rvm.memberAddress;
					mem.MemberPoint = 0;
					mem.MemberJoinTime = DateTime.Now;
					mem.Available = true;
					mem.IsValid = true;
					//Provider = mem.Provider,
					//ProviderUserId = mem.ProviderUserId
					_context.Member.Add(mem);
					await _context.SaveChangesAsync();
					var LoginData = new
					{
						MemberId = mem.MemberId,
						MemberName = mem.MemberName,
						MemberPoint = mem.MemberPoint
					};

					string Memjson = JsonSerializer.Serialize(LoginData);
					HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, Memjson);

					if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
					{
						return BadRequest(new { success = false, message = "伺服器錯誤，請稍後再試!" });
					}
					return Ok(new { success = true });

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("發生異常：" + ex.Message);
				return BadRequest(new { success = false, message = "伺服器錯誤，請稍後再試!" });
			}
			return BadRequest(new { success = false, message = "請輸入必填欄位!" });

		}


		[HttpPost("editMemberProfile")]
		public async Task<IActionResult> editMemberProfile([FromBody] registerViewModel rvm)
		{
			if (ModelState.IsValid)
			{
				Member member = _context.Member.FirstOrDefault(
			   t => t.MemberId.Equals(rvm.memberId));
				if (member != null)
				{
					member.MemberName = rvm.memberName;
					member.MemberTel = rvm.memberTel;
					member.MemberEmail = rvm.memberEmail;
					member.MemberSex = rvm.memberSex;
					member.MemberBirthday = rvm.memberBirthday;
					member.MemberAddress = rvm.memberAddress;

					_context.Entry(member).State = EntityState.Modified;
					await _context.SaveChangesAsync();

                    var LoginData = new
                    {
                        MemberId = member.MemberId,
                        MemberName = member.MemberName,
                        MemberPoint = member.MemberPoint
                    };

                    string Memjson = JsonSerializer.Serialize(LoginData);
                    HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, Memjson);
                    return Ok(new { success = true, message = "修改成功!" });


                }
                return BadRequest(new { success = false, message = "伺服器錯誤，請稍後再試!" });
			}
			return BadRequest(new { success = false, message = "請輸入必填欄位!" });

		}




		private bool MemberExists(int id)
		{
			return (_context.Member?.Any(e => e.MemberId == id)).GetValueOrDefault();
		}


		[HttpPost("forgetPassword")]
		public async Task<IActionResult> forgetPassword([FromBody] SigninViewModel formData)
		{
			MemberCredential mc = await _context.MemberCredential.FirstOrDefaultAsync(mc => mc.MemberAccount.Equals(formData.memberAccount));
			if (mc != null)
			{
				Member m = await _context.Member.FirstOrDefaultAsync(m => m.MemberId.Equals(mc.MemberId));

				var data = new
				{
					MemberId= mc.MemberId
				};
				string json = JsonSerializer.Serialize(data);
				HttpContext.Session.SetString(CDictionary.SK_RESETPWD_USER, json);
				if (!HttpContext.Session.Keys.Contains(CDictionary.SK_RESETPWD_USER))
				{
					return BadRequest(new { success = false, message = "伺服器錯誤，請稍後再試!" });
				}
				string body = System.IO.File.ReadAllText(Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplate", "resendPassword.cshtml"));
				var url = "https://localhost:5000/Login/ResetPwd";
				body = body.Replace("@ViewBag.ResetPwdLink", url);
				body = body.Replace("@ViewBag.UserName", m.MemberName);
				body = body.ToString();
				await BuildEmailAsync("驗證帳號", body, m.MemberEmail);
				return Ok(new { success = true, message = "已發送修改密碼驗證信，請至信箱查看!" });
			}
			return BadRequest(new { success = false, message = "請輸入正確帳號!" });
		}

		[HttpPost("ResetPwd")]
		public async Task<IActionResult> ResetPwd([FromBody] SigninViewModel formData)
		{
			if (ModelState.IsValid)
			{
				string json = HttpContext.Session.GetString(CDictionary.SK_RESETPWD_USER);
				MemberCredential credential = JsonSerializer.Deserialize<MemberCredential>(json);


				string password = formData.memberPassword;
				string salt = BCrypt.Net.BCrypt.GenerateSalt();
				string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
				MemberCredential mc = await _context.MemberCredential.FirstOrDefaultAsync(mc => mc.MemberId.Equals(Convert.ToInt32(credential.MemberId)));
				if (mc != null)
				{
					mc.MemberPassword = hashedPassword;
					_context.Entry(mc).State = EntityState.Modified;
					await _context.SaveChangesAsync();
					return Ok(new { success = true, message = "新密碼修改完成，請重新登入!" });
				}
				return BadRequest(new { success = false, message = "伺服器問題，請稍後再試!" });


			}
			return BadRequest(new { success = false, message = "請輸入密碼與確認密碼!" });

		}

        [HttpPost("editPwd")]
        public async Task<IActionResult> editPwd([FromBody] PwdViewModel formData)
        {
            if (ModelState.IsValid)
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
                Member member = JsonSerializer.Deserialize<Member>(json);
				MemberCredential mc = await _context.MemberCredential.FirstOrDefaultAsync(mc=>mc.MemberId.Equals(member.MemberId));
				if (mc != null)
				{
                    string password = formData.memberPassword;
                    string newPassword = formData.memberNewPassword;
                    bool isPwdMatch = BCrypt.Net.BCrypt.Verify(formData.memberPassword, mc.MemberPassword);
					if (isPwdMatch) {
                        string salt = BCrypt.Net.BCrypt.GenerateSalt();
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword, salt);
                        mc.MemberPassword = hashedPassword;
                        _context.Entry(mc).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        return Ok(new { success = true, message = "新密碼修改完成!" });
                    }
                    return BadRequest(new { success = false, message = "密碼不正確，請再試一次!" });
                }
                return BadRequest(new { success = false, message = "請重新登入!", login = false });

            }
            return BadRequest(new { success = false, message = "請輸入密碼與確認密碼!" });

        }

    }
}

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
                return RedirectToAction("Index","Home");
            }
            return View();
        }

        



    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Generators;
using TopLearn.Core.Security;
using TopLearn.Core.Senders;
using TopLearn.Core.Services.Interfaces;
using TopLearnWeb.DataLayer.Entities.User;

namespace TopLearn.Web.Controllers
{
    public class AccountController : Controller
    {
        IUserService _userService;
        IViewRenderService _viewRenderService;

        public AccountController(IUserService userService, IViewRenderService viewRenderService)
        {
            _userService = userService;
            _viewRenderService = viewRenderService;
        }

        #region Register

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }
            if (_userService.UserNameExists(register.UserName))
            {
                ModelState.AddModelError("UserName", "نام کاربری معتبر نمی باشد");
                return View(register);
            }
            if (_userService.EmailExists(FixedText.FixEmail(register.Email)))
            {
                ModelState.AddModelError("Email", "ایمیل معتبر نمی باشد");
                return View(register);
            }

            User user = new User()
            {
                ActivationCode = NameGenerator.GenerateUniqeCode(),
                Email = FixedText.FixEmail(register.Email),
                IsActive = false,
                PassWord = PasswordHelper.EncodePasswordMd5(register.PassWord),
                RegisterDate = DateTime.Now,
                Avatar = "Default Avatar.jpg",
                UserName = register.UserName
            };

            _userService.AddUser(user);

            #region Send Activation Email

            string body = _viewRenderService.RenderToStringAsync("Shared/Emails/_ActivationEmail", user);
            SendEmail.SendAsync(user.Email, "فعالسازی حساب", body);

            #endregion

            return View("RegisterSuccess", user);
        }

        #endregion

        #region Login

        [Route("/Login")]
        public IActionResult Login(bool EditProfile=false)
        {
            ViewBag.EditProfile = EditProfile;
            return View();
        }

        [HttpPost]
        [Route("/Login")]
        public IActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var user = _userService.LoginUser(login);

            if (user != null)
            {
                if (user.IsActive)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = login.RememberMe
                    };

                    HttpContext.SignInAsync(principal, properties);

                    ViewBag.IsSuccess = true;
                    return View();
                }
                else
                {
                    ModelState.AddModelError("Email", "حساب کابری شما فعال نمیباشد");
                    return View(login);
                }
            }
            if (user == null)
            {
                ModelState.AddModelError("Email", "کاربری با این مشخصات یافت نشد");
                return View(login);
            }

            return View();
        }

        #endregion

        #region Logout

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login");
        }

        #endregion

        #region Account Activation

        public IActionResult ActivateAccount(string Id)
        {
            ViewBag.IsActive = _userService.ActivateAccount(Id);
            return View("ActivateAccount");
        }

        #endregion

        #region Forgot PassWord

        [Route("ForgotPassword")]
        public IActionResult ForgotPassWord()
        {
            return View();
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassWord(ForgotPassWordViewModel forgot)
        {
            if (!ModelState.IsValid)
            {
                return View(forgot);
            }

            string fixedemail = FixedText.FixEmail(forgot.Email);
            User user = _userService.GetUserByEmail(fixedemail);

            if (user == null)
            {
                ModelState.AddModelError("Email", "کاربری با این مشخصات یافت نشد");
                return View(forgot);
            }

            string emailBody = _viewRenderService.RenderToStringAsync("Shared/Emails/_ResetPassWordEmail", user);
            SendEmail.SendAsync(user.Email, "بازیابی کلمه عبور", emailBody);
            ViewBag.IsSuccess = true;

            return View();
        }

        #endregion

        #region Reset PassWord

        public IActionResult ResetPassWord(string id)
        {
            return View(new ResetPassWordViewModel()
            {
                ActivationCode = id
            });
        }

        [HttpPost]
        public IActionResult ResetPassWord(ResetPassWordViewModel reset)
        {
            if (!ModelState.IsValid)
            {
                return View(reset);
            }

            User user = _userService.GetUserByActicationCode(reset.ActivationCode);
            if (user == null)
            {
                return NotFound();
            }

            string hashNewPassWord = PasswordHelper.EncodePasswordMd5(reset.PassWord);
            user.PassWord = hashNewPassWord;
            _userService.UpdateUser(user);
            ViewBag.IsSuccess = true;
            return View();
        }

        #endregion

    }
}

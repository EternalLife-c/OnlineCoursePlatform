using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;
using TopLearn.Core.DTOs;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class HomeController : Controller
    {
        private IUserService _userService;
        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View(_userService.GetUserInformation(User.Identity.Name));
        }

        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile()
        {
            return View(_userService.GetDataForUserProfileEdit(User.Identity.Name));
        }

        [HttpPost]
        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile(EditProfileViewModel profile)
        {
            ModelState.Remove("Avatar");
            if (!ModelState.IsValid)
            {
                return View(profile);
            }

            _userService.EditProfile(User.Identity.Name, profile);

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account", new { EditProfile = true });
        }

        [Route("UserPanel/ChangePassWord")]
        public IActionResult ChangePassWord()
        {
            return View();
        }

        [HttpPost]
        [Route("UserPanel/ChangePassWord")]
        public IActionResult ChangePassWord(ChangePassWordViewModel change)
        {
            string currentUserName = User.Identity.Name;
            if (!ModelState.IsValid)
            {
                return View(change);
            }
            if (!_userService.CompareOldPassWord(change.OldPassWord, currentUserName))
            {
                ModelState.AddModelError("OldPassWord", "کلمه عبور قدیمی صحیح نمی باشد");
                return View(change);
            }

            _userService.ChangeUserPassWord(currentUserName, change.PassWord);
            ViewBag.IsSuccess = true;
            return View();
        }
    }
}

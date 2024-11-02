using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.ManageUsers
{
    public class IndexModel : PageModel
    {
        private IUserService _userService;
        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public UsersForAdminViewModel UserForAdminViewModel { get; set; }

        public void OnGet(int pageId = 1 ,string emailFilter = "", string userNameFilter = "")
        {
            UserForAdminViewModel = _userService.GetUsers(pageId, emailFilter, userNameFilter);
        }
    }
}

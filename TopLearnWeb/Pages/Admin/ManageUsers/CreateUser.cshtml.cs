using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.ManageUsers
{
    public class CreateUserModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;

        public CreateUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }



        [BindProperty]
        public CreateUserViewModel CreateUserViewModel { get; set; }
        public void OnGet()
        {
            ViewData["Roles"] = _permissionService.GetRoles();
        }

        public IActionResult OnPost(List<int> SelectedRoles)
        {
            ViewData["Roles"] = _permissionService.GetRoles();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            int userId = _userService.AddUserFromAdminPanel(CreateUserViewModel);

            _permissionService.AddRolesToUser(SelectedRoles, userId);

            return Redirect("/Admin/ManageUsers");

        }

    }
}

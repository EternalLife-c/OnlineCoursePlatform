using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.ManageUsers
{
    public class EditUserModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;
        public EditUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        [BindProperty]
        public EditUserViewModel EditUserViewModel { get; set; }
        public void OnGet(int id)
        {
            EditUserViewModel = _userService.GetUserForEdit(id);
            EditUserViewModel.PassWord = null; // Ensure password is not populated
            ViewData["Roles"] = _permissionService.GetRoles();
        }

        public IActionResult OnPost(List<int> SelectedRoles)
        {

            ViewData["Roles"] = _permissionService.GetRoles();

            if (!ModelState.IsValid)
            {
                // to retreive the data if the valiations fail!
                EditUserViewModel = _userService.GetUserForEdit(EditUserViewModel.UserId);
                EditUserViewModel.PassWord = null;
                return Page();
            }

            _userService.EditUserFromAdminPanel(EditUserViewModel);
            _permissionService.EditUserRoles(EditUserViewModel.UserId, SelectedRoles);

            return Redirect("/Admin/Manageusers");
        }
    }
}

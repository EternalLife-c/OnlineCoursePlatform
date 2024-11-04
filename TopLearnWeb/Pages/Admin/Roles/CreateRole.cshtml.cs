using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.NetworkInformation;
using System.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearnWeb.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Admin.Roles
{
    public class CreateRoleModel : PageModel
    {

        private IPermissionService _permissionService;

        public CreateRoleModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [BindProperty]
        public Role Role { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Add newly made role
            Role.Deleted = false;
            int roleId = _permissionService.AddRole(Role);

            //ToDo add permission


            return Redirect("/Admin/Roles");
        }
    }
}

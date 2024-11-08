using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;
using TopLearnWeb.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Admin.Roles
{
    public class IndexModel : PageModel
    {
        private IPermissionService _permissionService;
        public IndexModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public List<Role>  RolesList { get; set; }

        public void OnGet()
        {
            RolesList = _permissionService.GetRoles();
        }
    }
}

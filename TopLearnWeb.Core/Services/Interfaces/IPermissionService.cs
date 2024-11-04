using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearnWeb.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        #region Roles

        int AddRole(Role role);
        List<Role> GetRoles();
        Role GetRoleById(int RoleId);
        void UpdateRole(Role role);
        void DeleteRole(Role role);
        void AddRolesToUser(List<int> roleIds,int userId);
        void EditUserRoles(int userId, List<int> RolesId);
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearnWeb.DataLayer.Context;
using TopLearnWeb.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public class PermissionService : IPermissionService
    {
        private TopLearnContext _context;
        public PermissionService(TopLearnContext context)
        {
            _context = context;
        }

        public int AddRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role.RoleId;
        }

        public void AddRolesToUser(List<int> roleIds, int userId)
        {
            foreach (int roleId in roleIds)
            {
                _context.UserRoles.Add(new UserRole()
                {
                    RoleId = roleId,
                    UserId = userId
                });
            }
            _context.SaveChanges();
        }

        public void DeleteRole(Role role)
        {
            role.Deleted = true;
            UpdateRole(role);
        }

        public void EditUserRoles(int userId, List<int> RolesId)
        {
            //Remove all User Roles
            _context.UserRoles.Where(r=> r.UserId==userId).ToList().ForEach(r=> _context.UserRoles.Remove(r));

            //Add New Roles to User
            AddRolesToUser(RolesId, userId);
        }

        public Role GetRoleById(int RoleId)
        {
            return _context.Roles.Find(RoleId);
        }

        public List<Role> GetRoles()
        {
            return _context.Roles.ToList();
        }

        public void UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
        }
    }
}

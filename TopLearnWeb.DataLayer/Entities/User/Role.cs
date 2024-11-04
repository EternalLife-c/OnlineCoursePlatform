using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TopLearnWeb.DataLayer.Entities.User
{
    public class Role
    {
        public Role()
        {
            
        }

        [Key]
        public int RoleId { get; set; }

        [Display(Name = "عنوان نقش")]
        [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string RoleTitle { get; set; }

        public bool Deleted { get; set; }

        #region Relations

        [ValidateNever]
        public virtual List<UserRole> UserRoles { get; set; }

        #endregion
    }
}

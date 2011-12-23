using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MockUserAdmin.Models
{
    public class UserViewModel : IUserViewModel
    {
        public IList<SysUser> sysUsers { get; set; }


    }
}
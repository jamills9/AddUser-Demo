using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockUserAdmin.Models
{
    public interface IUserViewModel
    {
        IList<SysUser> sysUsers { get; set; }
    }
}

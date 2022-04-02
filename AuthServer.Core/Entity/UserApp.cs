using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Entity
{
    public class UserApp: IdentityUser
    {
        public string City { get; set; }
    }


    //bu kod bloğu ile identity kütüphanesinde olmayıp kendi eklemek istediğiniz özellikleri yazabiliriz.
    //public class UserRole: IdentityRole
    //{
    //    public int MyProperty { get; set; }
    //}
}

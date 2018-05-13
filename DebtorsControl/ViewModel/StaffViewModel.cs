using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebtorsControl.ViewModel
{
    public class StaffViewModel
    {
        public List<StaffRole> StaffRoles { get; set; }
    }

    public class StaffRole
    {
        public string Role { get; set; }
    }
}
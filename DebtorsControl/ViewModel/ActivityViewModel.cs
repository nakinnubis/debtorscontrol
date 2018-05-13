using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebtorsControl.ViewModel
{
    public class ActivityViewModel
    {
        public List<Activity> Activities { get; set; }
    }

    public class Activity
    {
        public long Id { get; set; }
        public string StaffName { get; set; }
        public string ActivityType { get; set; }
        public string TimeCreated { get; set; }
    }
}
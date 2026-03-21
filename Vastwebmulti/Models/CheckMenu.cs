using System.Collections.Generic;

namespace Vastwebmulti.Models
{
    public class CheckMenu
    {
        public int Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public bool Checked
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
        public string Role
        {
            get;
            set;
        }
        public string UserId
        {
            get;
            set;
        }
    }
    public class showmenu
    {
        public List<CheckMenu> check { get; set; }
    }
}
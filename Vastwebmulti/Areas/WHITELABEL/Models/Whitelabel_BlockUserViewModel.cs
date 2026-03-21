using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class BlockUserlistmodel
    {
        public int uid { get; set; }
        public string Userid { set; get; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Frmname { get; set; }
        public string sts { get; set; }
        public string apploginsts { get; set; }
        public string ROLES { get; set; }
        public string MOBILE { get; set; }
        public string Block { get; set; }
        public IEnumerable<string> userlocationlistbyid { get; set; }
    }
    public class Whitelabel_BlockUserViewModel
    {
        public IEnumerable<spWhitelabel_Failed_login_history_Result> Apploginhistory { get; set; }
        public IEnumerable<spWhitelabel_login_history_Result> WLoginHistory { get; set; }
        public IEnumerable<spWhitelabel_UsersDetails_Result> Whitelabel_UsersList { get; set; }
        public IEnumerable<spWhitelabel_Login_Blocked_List_Result> Blocked_UsersList { get; set; }
    }
}
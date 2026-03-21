using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model representing a user entry in the blocked/suspended users list.
    /// </summary>
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
        public string NetOFFR { get; set; }
        public IEnumerable<string> userlocationlistbyid { get; set; }
    }
    public class BlockUserViewModel
    {

      //  public IEnumerable<SelectListItem> Userlist { get; set; }
        public IEnumerable<User> userslist { get; set; }
        public IEnumerable<Failed_login_history_Result> Apploginhistory { get; set; }
        public IEnumerable<login_history_Result> WLoginHistory { get; set; }
        public string LoginSearchUserRole { get; set; }
        public string LoginType { get; set; }
        public string searchfrmname { get; set; }
        public string searchEmailID { get; set; }
        public string CompanyURL { get; set; }
        public string RegisteredMobile { get; set; }
        public DateTime Fromdatesearch { get; set; }
        public DateTime Todatesearch { get; set; }
    }
}
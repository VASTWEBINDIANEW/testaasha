using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.ViewModel
{
    public class RegisternewEmployeeViewModel
    {
      
      
        public string Employee_name { get; set; }
        public string Address { get; set; }
        public int Pincode { get; set; }
        public string Mobile { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime DOB { get; set; }
        public string E_mail { get; set; }
        public string Password { get; set; }
        public string AadharCard { get; set; }
        public string PanCard { get; set; }
        public int State1 { get; set; }
        public int District1 { get; set; }
        public string City { get; set; }
        public bool Status { get; set; }
    }

    public class GeltALLEmployee
    {

        public IEnumerable<Select_all_employee_Result> Emplist { get; set; }

    }
    public class EmployeePermissionViewmodel
    {
      

        public string EmployeenameID { get; set; }
     
        public string EmployeeIDforpart{ get; set; }

        public IEnumerable<tbl_permission_Services> ServicelistALL { get; set; }
        public List<SelectListItem> Employeenames { get; set; }
    }

    public class EmployeeAdvancePermissionViewmodel
    {


        public int stateid { get; set; }

        public string userID { get; set; }
        public string user_statename { get; set; }
        public string usertype { get; set; }

        public bool isaccess { get; set; }

     
    }

    public class EmployeeAdvancePermissionViewmodellist
    {
       public string Employeeid { get; set; }
        public string usertype { get; set; }
        public string permissiontype { get; set; }
        public  List<EmployeeAdvancePermissionViewmodel> statewiselist { get; set; }
      public  List<EmployeeAdvancePermissionViewmodel> userwiselist { get; set; }
    }


    }
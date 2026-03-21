using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.Employee.ViewModel
{
    public class Registernewemployeeviewmodel
    {
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string Employee_name { get; set; }
        public string Address { get; set; }
        public int Pincode { get; set; }
        public string Mobile { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime DOB { get; set; }
        public string E_mail  { get; set; }
        public string AadharCard { get; set; }
        public string PanCard { get; set; }
        public int State { get; set; }
        public int District { get; set; }
        public string City { get; set; }
        public bool Status { get; set; }


    }
}
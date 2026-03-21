using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vastwebmulti.Models;

namespace Vastwebmulti
{
    public partial class ServerRenewalDateUpdate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                try
                {
                    //db.Connection.Open();
                    var outpt = HttpContext.Current.Request.Url.AbsoluteUri;
                    var date = Request.QueryString["RenewalDate"];
                    string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                    DateTime dt = DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);

                    var entry = db.Admin_details.FirstOrDefault();
                    if (entry == null)
                    {
                        Admin_details newEntry = new Admin_details();
                        newEntry.RenivalDate = dt;
                        db.Admin_details.Add(newEntry);
                        db.SaveChanges();
                    }
                    else
                    {
                        entry.RenivalDate = dt;
                        db.SaveChanges();
                    }

                }
                catch 
                {
                    throw;
                }
                finally
                {
                    //db.Close();
                }
            }
        }
    }
}
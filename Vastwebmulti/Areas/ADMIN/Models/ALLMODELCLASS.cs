using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
   
    public class ALLMODELCLASS
    {
       
        public static string returnstatus { get; set; }


        //VastwebmultiEntities db = new VastwebmultiEntities();
        public static string Update_MicroATMLimit(int maxvalues)
        {
             
            using (VastwebmultiEntities db=new VastwebmultiEntities()) {
                var result = db.micro_atm_Purchase_MaxtransectionLimit.SingleOrDefault();
                if (result != null)
                {
                    result.maxvalues = maxvalues;

                }
                else
                {
                    micro_atm_Purchase_MaxtransectionLimit model = new micro_atm_Purchase_MaxtransectionLimit();
                    model.maxvalues = 9999;
                    db.micro_atm_Purchase_MaxtransectionLimit.Add(model);
                }
                
              var status=  db.SaveChanges()>0;
                if (status == true)
                {
                    returnstatus = "SUCCESS";
                }
                else
                {
                    returnstatus = "FAIL";
                }
               

            }
            return returnstatus;


        }

        public static string Update_MPOSLimit(int maxvalues)
        {

            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var result = db.MPOS_MaxtransectionLimit.SingleOrDefault();
                if (result != null)
                {
                    result.maxvalues = maxvalues;

                }
                else
                {
                    MPOS_MaxtransectionLimit model = new MPOS_MaxtransectionLimit();
                    model.maxvalues = 9999;
                    db.MPOS_MaxtransectionLimit.Add(model);
                }

                var status = db.SaveChanges() > 0;
                if (status == true)
                {
                    returnstatus = "SUCCESS";
                }
                else
                {
                    returnstatus = "FAIL";
                }


            }
            return returnstatus;


        }
    }
}
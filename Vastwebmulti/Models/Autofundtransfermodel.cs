using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Vastwebmulti.Models
{
    public class Autofundtransfermodel
    {

        public int idno { get; set; }

        public string Name { get; set; }
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Please enter miniumamount")]
        public decimal? minimiumamount { get; set; }
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Please enter maxmium amount")]
        public decimal? transferamount { get; set; }
        [DataType(DataType.Date)]
        public DateTime? transferdatetime { get; set; }

        public string status { get; set; }
        [Required(ErrorMessage = "Please enter maxmium amount")]
        public string types { get; set; }
        public int? totaltransfer { get; set; }

        public decimal? MaxCredit { get; set; }


        //      public static Autofundtransfermodel Autofundtransfermethode(T todoItem) =>
        //new Autofundtransfermodel
        //{
        //    Id = todoItem.idno,
        //    Name = todoItem.,
        //    IsComplete = todoItem.IsComplete
        //};

    }
    public class Autofundtransfermodelviewmodel
    {

        public int idno { get; set; }

        public string Name { get; set; }
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Please enter miniumamount")]
        public decimal? minimiumamount { get; set; }
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Please enter maxmium amount")]
        public decimal? transferamount { get; set; }
        [DataType(DataType.Date)]
        public DateTime? transferdatetime { get; set; }

        public string status { get; set; }
        [Required(ErrorMessage = "Please enter maxmium amount")]
        public string types { get; set; }
        public int? totaltransfer { get; set; }

        public decimal? MaxCredit { get; set; }
        public IEnumerable<Autofundtransfermodel> autofundtransfermodel { get; set; }
    }

    public class MethodeClsForAuto
    {

        public async Task Autofundtransfer_Master_to_dealer(Autofundtransfermodelviewmodel modes, string currenuserid)
        {
            using (var db = new VastwebmultiEntities())
            {
                modes.autofundtransfermodel = await (from tbl in db.autofundtransfer_master_to_dealer
                                                     join tbl1 in db.Dealer_Details
                                                     on tbl.dlmid equals tbl1.DealerId
                                                     where tbl.masterid.Equals(currenuserid)
                                                     //join tbl2 in db.Admin_details
                                                     //on tbl.masterid equals tbl2.userid
                                                     select new Autofundtransfermodel
                                                     {
                                                         idno = tbl.idno,
                                                         Name = tbl1.FarmName + " " + tbl1.Mobile,
                                                         status = tbl.status,
                                                         minimiumamount = tbl.minamount,
                                                         transferamount = tbl.Transferamount,
                                                         transferdatetime = tbl.updatedatetime,
                                                         types = tbl.types

                                                     }).ToListAsync();
            }

        }





    }

}
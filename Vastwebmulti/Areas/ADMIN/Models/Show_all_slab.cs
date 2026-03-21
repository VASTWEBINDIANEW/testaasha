using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class DTH
    {
        public int idno { get; set; }
        public string  slabname { get; set; }
        public string slabfor { get; set; }
        [Required(ErrorMessage = "Enter Commission")]
        public decimal commission { get; set; }
        public string OperatorName { get; set; }
        public decimal? DefaultComm { set; get; }
    }
    public class ELE
    {
        public int idno { get; set; }
        public string slabname { get; set; }
        public decimal? comm1 { get; set; }
        public decimal? comm2 { get; set; }
        public decimal? comm3 { get; set; }
        public decimal? comm4 { get; set; }
        public decimal? cashback1 { get; set; }
        public decimal? cashback2 { get; set; }
        public decimal? cashback3 { get; set; }
        public string OperatorName { get; set; }
        public string slabfor { get; set; }
        public decimal? DefaultComm1 { set; get; }
        public decimal? DefaultComm2 { set; get; }
        public decimal? DefaultComm3 { set; get; }
        public string DefaultComm4 { set; get; }
    }
    public class pan
    {
        public int idno { get; set; }
        public string slabname { get; set; }
        public string OperatorName { get; set; }
        public decimal? tokenval { get; set; }
        public decimal? Commission { get; set; }
        public decimal? cashback { get; set; }
        public string slabfor { get; set; }
        public decimal? DefaultComm { set; get; }

    }
    public class money
    {
        public int idno {get; set;}
        public string  slabname { get; set; }
        public string type { get; set; }
        public string OperatorName { get; set; }
        public int? min { get; set; }
        public int? max { get; set; }
        public decimal? comm { get; set; }
        public decimal? CashBack { get; set; }
        public string slabfor { get; set; }
        public string value { set; get; }
    }

    public class Show_all_slab
    {
        public DTH DTH { get; set; }
        public List<DTH> Slab_DTH { get; set; }
        public List<DTH> Slab_prepaid { get; set; }
        public List<DTH> Slab_DthBooking { get; set; }
        public List<ELE> Slab_Electricity { get; set; }
        public List<ELE> Slab_Gas { get; set; }
        public List<ELE> Slab_Broadband { get; set; }
        public List<ELE> slab_insurance { get; set; }
        public List<ELE> slab_landline { get; set; }
        public List<ELE> slab_Postpaid { get; set; }
        public List<ELE> Slab_Water { get; set; }
        public List<pan> Slab_PanCard { get; set; }
        public List<money> Slab_Money { get; set; }
        public List<money> Slab_IndoNepal { get; set; }
    }

}
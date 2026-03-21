using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    public class DMTSlabModelPPI
    {
        public List<PPI_common_comm_new> commonnew { get; set; }
        public List<MasterCommPPI> Materuser { get; set; }
        public List<DealerCommPPI> Dealeruser { get; set; }
        public List<RetailerCommPPI> Retaileruser { get; set; }
        public List<UpdateDMTMasterCommPPI> UpdateMaster { get; set; }
        public List<UpdateDMTDealerCommPPI> UpdateDealer { get; set; }
        public List<UpdateDMTRetailerCommPPI> UpdateRetailer { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> Retailerid { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class MasterCommPPI
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? comm_6000 { get; set; }
        public decimal? comm_7000 { get; set; }
        public decimal? comm_8000 { get; set; }
        public decimal? comm_9000 { get; set; }
        public decimal? comm_10000 { get; set; }
        public decimal? comm_11000 { get; set; }
        public decimal? comm_12000 { get; set; }
        public decimal? comm_13000 { get; set; }
        public decimal? comm_14000 { get; set; }
        public decimal? comm_15000 { get; set; }
        public decimal? comm_16000 { get; set; }
        public decimal? comm_17000 { get; set; }
        public decimal? comm_18000 { get; set; }
        public decimal? comm_19000 { get; set; }
        public decimal? comm_20000 { get; set; }
        public decimal? comm_21000 { get; set; }
        public decimal? comm_22000 { get; set; }
        public decimal? comm_23000 { get; set; }
        public decimal? comm_24000 { get; set; }
        public decimal? comm_25000 { get; set; }
    }
    public class UpdateDMTMasterCommPPI
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? comm_6000 { get; set; }
        public decimal? comm_7000 { get; set; }
        public decimal? comm_8000 { get; set; }
        public decimal? comm_9000 { get; set; }
        public decimal? comm_10000 { get; set; }
        public decimal? comm_11000 { get; set; }
        public decimal? comm_12000 { get; set; }
        public decimal? comm_13000 { get; set; }
        public decimal? comm_14000 { get; set; }
        public decimal? comm_15000 { get; set; }
        public decimal? comm_16000 { get; set; }
        public decimal? comm_17000 { get; set; }
        public decimal? comm_18000 { get; set; }
        public decimal? comm_19000 { get; set; }
        public decimal? comm_20000 { get; set; }
        public decimal? comm_21000 { get; set; }
        public decimal? comm_22000 { get; set; }
        public decimal? comm_23000 { get; set; }
        public decimal? comm_24000 { get; set; }
        public decimal? comm_25000 { get; set; }
    }
    public class DealerCommPPI
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? comm_6000 { get; set; }
        public decimal? comm_7000 { get; set; }
        public decimal? comm_8000 { get; set; }
        public decimal? comm_9000 { get; set; }
        public decimal? comm_10000 { get; set; }
        public decimal? comm_11000 { get; set; }
        public decimal? comm_12000 { get; set; }
        public decimal? comm_13000 { get; set; }
        public decimal? comm_14000 { get; set; }
        public decimal? comm_15000 { get; set; }
        public decimal? comm_16000 { get; set; }
        public decimal? comm_17000 { get; set; }
        public decimal? comm_18000 { get; set; }
        public decimal? comm_19000 { get; set; }
        public decimal? comm_20000 { get; set; }
        public decimal? comm_21000 { get; set; }
        public decimal? comm_22000 { get; set; }
        public decimal? comm_23000 { get; set; }
        public decimal? comm_24000 { get; set; }
        public decimal? comm_25000 { get; set; }
    }
    public class UpdateDMTDealerCommPPI
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? comm_6000 { get; set; }
        public decimal? comm_7000 { get; set; }
        public decimal? comm_8000 { get; set; }
        public decimal? comm_9000 { get; set; }
        public decimal? comm_10000 { get; set; }
        public decimal? comm_11000 { get; set; }
        public decimal? comm_12000 { get; set; }
        public decimal? comm_13000 { get; set; }
        public decimal? comm_14000 { get; set; }
        public decimal? comm_15000 { get; set; }
        public decimal? comm_16000 { get; set; }
        public decimal? comm_17000 { get; set; }
        public decimal? comm_18000 { get; set; }
        public decimal? comm_19000 { get; set; }
        public decimal? comm_20000 { get; set; }
        public decimal? comm_21000 { get; set; }
        public decimal? comm_22000 { get; set; }
        public decimal? comm_23000 { get; set; }
        public decimal? comm_24000 { get; set; }
        public decimal? comm_25000 { get; set; }
    }
    public class RetailerCommPPI
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? comm_6000 { get; set; }
        public decimal? comm_7000 { get; set; }
        public decimal? comm_8000 { get; set; }
        public decimal? comm_9000 { get; set; }
        public decimal? comm_10000 { get; set; }
        public decimal? comm_11000 { get; set; }
        public decimal? comm_12000 { get; set; }
        public decimal? comm_13000 { get; set; }
        public decimal? comm_14000 { get; set; }
        public decimal? comm_15000 { get; set; }
        public decimal? comm_16000 { get; set; }
        public decimal? comm_17000 { get; set; }
        public decimal? comm_18000 { get; set; }
        public decimal? comm_19000 { get; set; }
        public decimal? comm_20000 { get; set; }
        public decimal? comm_21000 { get; set; }
        public decimal? comm_22000 { get; set; }
        public decimal? comm_23000 { get; set; }
        public decimal? comm_24000 { get; set; }
        public decimal? comm_25000 { get; set; }
    }
    public class UpdateDMTRetailerCommPPI
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? comm_6000 { get; set; }
        public decimal? comm_7000 { get; set; }
        public decimal? comm_8000 { get; set; }
        public decimal? comm_9000 { get; set; }
        public decimal? comm_10000 { get; set; }
        public decimal? comm_11000 { get; set; }
        public decimal? comm_12000 { get; set; }
        public decimal? comm_13000 { get; set; }
        public decimal? comm_14000 { get; set; }
        public decimal? comm_15000 { get; set; }
        public decimal? comm_16000 { get; set; }
        public decimal? comm_17000 { get; set; }
        public decimal? comm_18000 { get; set; }
        public decimal? comm_19000 { get; set; }
        public decimal? comm_20000 { get; set; }
        public decimal? comm_21000 { get; set; }
        public decimal? comm_22000 { get; set; }
        public decimal? comm_23000 { get; set; }
        public decimal? comm_24000 { get; set; }
        public decimal? comm_25000 { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;
namespace Vastwebmulti.Areas.ADMIN.Models
{
    #region DMT1
    public class DMTSlabModel
    {
        public List<imps_common_comm> common { get; set; }
        public List<MasterComm> Materuser { get; set; }
        public List<DealerComm> Dealeruser { get; set; }
        public List<RetailerComm> Retaileruser { get; set; }
        public List<APIComm> APIuser { get; set; }
        public List<WhitelabelComm> Whitelabeluser { get; set; }
        public List<UpdateDMTMasterComm> UpdateMaster { get; set; }
        public List<UpdateDMTDealerComm> UpdateDealer { get; set; }
        public List<UpdateDMTRetailerComm> UpdateRetailer { get; set; }
        public List<UpdateDMTAPIComm> UpdateAPI { get; set; }

        public List<UpdateDMTWhitelabelComm> UpdateWhitelabel { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public string role { get; set; }
    }
    public class MasterComm
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
        public decimal? gst { get; set; }

    }
    public class UpdateDMTMasterComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? gst { get; set; }

    }
    public class DealerComm
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
        public decimal? gst { get; set; }

    }
    public class UpdateDMTDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? gst { get; set; }

    }
    public class RetailerComm
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
        public decimal? gst { get; set; }

    }
    public class UpdateDMTRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? gst { get; set; }

    }
    public class APIComm
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
        public decimal? gst { get; set; }

    }
    public class UpdateDMTAPIComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? gst { get; set; }

    }
    public class WhitelabelComm
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
        public decimal? gst { get; set; }

    }
    public class UpdateDMTWhitelabelComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? gst { get; set; }

    }
    #endregion
    #region DMT2
    public class DMT2SlabModel
    {
        public List<paytm_imps_common_comm> common { get; set; }
        public List<imps_common_comm_new> commonnew { get; set; }
        public List<MasterComm2> Materuser { get; set; }
        public List<DealerComm2> Dealeruser { get; set; }
        public List<RetailerComm2> Retaileruser { get; set; }
        public List<dlmremComm2> DlmRemuser { get; set; }
        public List<APIComm2> APIuser { get; set; }
        public List<WhitelabelComm2> Whitelabeluser { get; set; }
        public List<UpdateDMTMasterComm2> UpdateMaster { get; set; }
        public List<UpdateDMTDealerComm2> UpdateDealer { get; set; }
        public List<UpdateDMTRetailerComm2> UpdateRetailer { get; set; }
        public List<UpdateDMTAPIComm2> UpdateAPI { get; set; }

        public List<UpdateDMTWhitelabelComm2> UpdateWhitelabel { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> Retailerid { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class MasterComm2
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }
    }
    public class UpdateDMTMasterComm2
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }

    }
    public class DealerComm2
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }
    }
    public class UpdateDMTDealerComm2
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }

    }
    public class RetailerComm2
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }

    }
    public class dlmremComm2
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }
    }

    public class UpdateDMTRetailerComm2
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }

    }
    public class APIComm2
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }
    }
    public class UpdateDMTAPIComm2
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }

    }
    public class WhitelabelComm2
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }
    }
    public class UpdateDMTWhitelabelComm2
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }
    }
    #endregion
    #region PAYOUT2
    public class DMT2SlabModel1
    {
        public List<paytm_imps_common_comm1> common { get; set; }
        public List<paytm_imps_common_commUPI> commonUPI { get; set; }
        public List<imps_common_comm_new> commonnew { get; set; }
        public List<MasterComm21> Materuser { get; set; }
        public List<DealerComm21> Dealeruser { get; set; }
        public List<RetailerComm21> Retaileruser { get; set; }
        public List<dlmremComm21> DlmRemuser { get; set; }
        public List<APIComm21> APIuser { get; set; }
        public List<WhitelabelComm21> Whitelabeluser { get; set; }
        public List<UpdateDMTMasterComm21> UpdateMaster { get; set; }
        public List<UpdateDMTDealerComm21> UpdateDealer { get; set; }
        public List<UpdateDMTRetailerComm21> UpdateRetailer { get; set; }
        public List<UpdateDMTAPIComm21> UpdateAPI { get; set; }

        public List<UpdateDMTWhitelabelComm21> UpdateWhitelabel { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> Retailerid { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class MasterComm21
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }
    }
    public class UpdateDMTMasterComm21
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }

    }
    public class DealerComm21
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }
    }
    public class UpdateDMTDealerComm21
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }

    }
    public class RetailerComm21
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }

    }
    public class dlmremComm21
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }
    }

    public class UpdateDMTRetailerComm21
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }

    }
    public class APIComm21
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }
    }
    public class UpdateDMTAPIComm21
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }

    }
    public class WhitelabelComm21
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string Email { get; set; }
        public string RolesName { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }
    }
    public class UpdateDMTWhitelabelComm21
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public decimal? verify_comm { get; set; } = 0;
        public decimal? comm_1000 { get; set; } = 0;
        public decimal? comm_2000 { get; set; } = 0;
        public decimal? comm_3000 { get; set; } = 0;
        public decimal? comm_4000 { get; set; } = 0;
        public decimal? comm_5000 { get; set; } = 0;
        public decimal? comm_6000 { get; set; } = 0;
        public decimal? comm_7000 { get; set; } = 0;
        public decimal? comm_8000 { get; set; } = 0;
        public decimal? comm_9000 { get; set; } = 0;
        public decimal? comm_10000 { get; set; } = 0;

        public decimal? comm_11000 { get; set; } = 0;
        public decimal? comm_12000 { get; set; } = 0;
        public decimal? comm_13000 { get; set; } = 0;
        public decimal? comm_14000 { get; set; } = 0;
        public decimal? comm_15000 { get; set; } = 0;
        public decimal? comm_16000 { get; set; } = 0;
        public decimal? comm_17000 { get; set; } = 0;
        public decimal? comm_18000 { get; set; } = 0;
        public decimal? comm_19000 { get; set; } = 0;
        public decimal? comm_20000 { get; set; } = 0;

        public decimal? comm_21000 { get; set; } = 0;
        public decimal? comm_22000 { get; set; } = 0;
        public decimal? comm_23000 { get; set; } = 0;
        public decimal? comm_24000 { get; set; } = 0;
        public decimal? comm_25000 { get; set; } = 0;
        public decimal? comm_26000 { get; set; } = 0;
        public decimal? comm_27000 { get; set; } = 0;
        public decimal? comm_28000 { get; set; } = 0;
        public decimal? comm_29000 { get; set; } = 0;
        public decimal? comm_30000 { get; set; } = 0;

        public decimal? comm_31000 { get; set; } = 0;
        public decimal? comm_32000 { get; set; } = 0;
        public decimal? comm_33000 { get; set; } = 0;
        public decimal? comm_34000 { get; set; } = 0;
        public decimal? comm_35000 { get; set; } = 0;
        public decimal? comm_36000 { get; set; } = 0;
        public decimal? comm_37000 { get; set; } = 0;
        public decimal? comm_38000 { get; set; } = 0;
        public decimal? comm_39000 { get; set; } = 0;
        public decimal? comm_40000 { get; set; } = 0;

        public decimal? comm_41000 { get; set; } = 0;
        public decimal? comm_42000 { get; set; } = 0;
        public decimal? comm_43000 { get; set; } = 0;
        public decimal? comm_44000 { get; set; } = 0;
        public decimal? comm_45000 { get; set; } = 0;
        public decimal? comm_46000 { get; set; } = 0;
        public decimal? comm_47000 { get; set; } = 0;
        public decimal? comm_48000 { get; set; } = 0;
        public decimal? comm_49000 { get; set; } = 0;
        public decimal? comm_50000 { get; set; } = 0;
        public decimal? gst { get; set; } = 0;
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }
        public decimal? per_26000 { get; set; } = 0;
        public decimal? maxrs_26000 { get; set; } = 0;
        public string Type_26000 { get; set; }
        public string Type_1000 { get; set; }
        public string Type_2000 { get; set; }
        public string Type_3000 { get; set; }
        public string Type_4000 { get; set; }
        public string Type_5000 { get; set; }
        public string Type_6000 { get; set; }
        public string Type_7000 { get; set; }
        public string Type_8000 { get; set; }
        public string Type_9000 { get; set; }
        public string Type_10000 { get; set; }
        public string Type_11000 { get; set; }
        public string Type_12000 { get; set; }
        public string Type_13000 { get; set; }
        public string Type_14000 { get; set; }
        public string Type_15000 { get; set; }
        public string Type_16000 { get; set; }
        public string Type_17000 { get; set; }
        public string Type_18000 { get; set; }
        public string Type_19000 { get; set; }
        public string Type_20000 { get; set; }
        public string Type_21000 { get; set; }
        public string Type_22000 { get; set; }
        public string Type_23000 { get; set; }
        public string Type_24000 { get; set; }
        public string Type_25000 { get; set; }
    }
    #endregion
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;
namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    #region DMT1
    public class DMTSlabModel
    {
        public List<imps_whitelabel_common_comm> common { get; set; }
        public List<imps_whitelabel_comm> common1 { get; set; }
        public List<WDealerComm> WDealeruser { get; set; }
        public List<WRetailerComm> WRetaileruser { get; set; }

        public List<WUpdateDealercomm> WUpdateDealer { get; set; }
        public List<WUpdateRetailercomm> WUpdateRetailer { get; set; }
        public List<SelectListItem> UserId { get; set; }

        public string role { get; set; }
    }

    public class WDealerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string whitelabelid { get; set; }
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
    public class WRetailerComm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string whitelabelid { get; set; }
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

    public class WUpdateDealercomm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string whitelabelid { get; set; }
        public string Email { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? gst { get; set; }

    }

    public class WUpdateRetailercomm
    {
        public int idno { get; set; }
        public string userid { get; set; }
        public string whitelabelid { get; set; }
        public string Email { get; set; }
        public decimal? verify_comm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? gst { get; set; }

    }

    public class DeleteWhitelabelUser
    {
        public IEnumerable<Vastwebmulti.Models.Whitelabel_DeleteUserHistory> DeleteWhitelabelUserAll { get; set; }
    }
    #endregion

    #region DMT2
    public class DMT2SlabModel
    {
        public List<Whitelabel_paytm_imps_common_comm> common { get; set; }
        public List<MasterComm2> Materuser { get; set; }
        public List<DealerComm2> Dealeruser { get; set; }
        public List<RetailerComm2> Retaileruser { get; set; }
        public List<dlmremComm2> DlmRemuser { get; set; }
        public List<UpdateDMTMasterComm2> UpdateMaster { get; set; }
        public List<UpdateDMTDealerComm2> UpdateDealer { get; set; }
        public List<UpdateDMTRetailerComm2> UpdateRetailer { get; set; }
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
        public string whitelabelid { get; set; }
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
        public decimal? comm_26000 { get; set; }
        public decimal? comm_27000 { get; set; }
        public decimal? comm_28000 { get; set; }
        public decimal? comm_29000 { get; set; }
        public decimal? comm_30000 { get; set; }

        public decimal? comm_31000 { get; set; }
        public decimal? comm_32000 { get; set; }
        public decimal? comm_33000 { get; set; }
        public decimal? comm_34000 { get; set; }
        public decimal? comm_35000 { get; set; }
        public decimal? comm_36000 { get; set; }
        public decimal? comm_37000 { get; set; }
        public decimal? comm_38000 { get; set; }
        public decimal? comm_39000 { get; set; }
        public decimal? comm_40000 { get; set; }

        public decimal? comm_41000 { get; set; }
        public decimal? comm_42000 { get; set; }
        public decimal? comm_43000 { get; set; }
        public decimal? comm_44000 { get; set; }
        public decimal? comm_45000 { get; set; }
        public decimal? comm_46000 { get; set; }
        public decimal? comm_47000 { get; set; }
        public decimal? comm_48000 { get; set; }
        public decimal? comm_49000 { get; set; }
        public decimal? comm_50000 { get; set; }
        public decimal? gst { get; set; }
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }

    }
    public class UpdateDMTMasterComm2
    {
        public int idno { get; set; }
        public string whitelabelid { get; set; }
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
        public decimal? comm_26000 { get; set; }
        public decimal? comm_27000 { get; set; }
        public decimal? comm_28000 { get; set; }
        public decimal? comm_29000 { get; set; }
        public decimal? comm_30000 { get; set; }

        public decimal? comm_31000 { get; set; }
        public decimal? comm_32000 { get; set; }
        public decimal? comm_33000 { get; set; }
        public decimal? comm_34000 { get; set; }
        public decimal? comm_35000 { get; set; }
        public decimal? comm_36000 { get; set; }
        public decimal? comm_37000 { get; set; }
        public decimal? comm_38000 { get; set; }
        public decimal? comm_39000 { get; set; }
        public decimal? comm_40000 { get; set; }

        public decimal? comm_41000 { get; set; }
        public decimal? comm_42000 { get; set; }
        public decimal? comm_43000 { get; set; }
        public decimal? comm_44000 { get; set; }
        public decimal? comm_45000 { get; set; }
        public decimal? comm_46000 { get; set; }
        public decimal? comm_47000 { get; set; }
        public decimal? comm_48000 { get; set; }
        public decimal? comm_49000 { get; set; }
        public decimal? comm_50000 { get; set; }
        public decimal? gst { get; set; }
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }

    }
    public class DealerComm2
    {
        public int idno { get; set; }
        public string whitelabelid { get; set; }
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
        public decimal? comm_26000 { get; set; }
        public decimal? comm_27000 { get; set; }
        public decimal? comm_28000 { get; set; }
        public decimal? comm_29000 { get; set; }
        public decimal? comm_30000 { get; set; }

        public decimal? comm_31000 { get; set; }
        public decimal? comm_32000 { get; set; }
        public decimal? comm_33000 { get; set; }
        public decimal? comm_34000 { get; set; }
        public decimal? comm_35000 { get; set; }
        public decimal? comm_36000 { get; set; }
        public decimal? comm_37000 { get; set; }
        public decimal? comm_38000 { get; set; }
        public decimal? comm_39000 { get; set; }
        public decimal? comm_40000 { get; set; }

        public decimal? comm_41000 { get; set; }
        public decimal? comm_42000 { get; set; }
        public decimal? comm_43000 { get; set; }
        public decimal? comm_44000 { get; set; }
        public decimal? comm_45000 { get; set; }
        public decimal? comm_46000 { get; set; }
        public decimal? comm_47000 { get; set; }
        public decimal? comm_48000 { get; set; }
        public decimal? comm_49000 { get; set; }
        public decimal? comm_50000 { get; set; }
        public decimal? gst { get; set; }
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }

    }
    public class UpdateDMTDealerComm2
    {
        public int idno { get; set; }
        public string whitelabelid { get; set; }
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
        public decimal? comm_26000 { get; set; }
        public decimal? comm_27000 { get; set; }
        public decimal? comm_28000 { get; set; }
        public decimal? comm_29000 { get; set; }
        public decimal? comm_30000 { get; set; }

        public decimal? comm_31000 { get; set; }
        public decimal? comm_32000 { get; set; }
        public decimal? comm_33000 { get; set; }
        public decimal? comm_34000 { get; set; }
        public decimal? comm_35000 { get; set; }
        public decimal? comm_36000 { get; set; }
        public decimal? comm_37000 { get; set; }
        public decimal? comm_38000 { get; set; }
        public decimal? comm_39000 { get; set; }
        public decimal? comm_40000 { get; set; }

        public decimal? comm_41000 { get; set; }
        public decimal? comm_42000 { get; set; }
        public decimal? comm_43000 { get; set; }
        public decimal? comm_44000 { get; set; }
        public decimal? comm_45000 { get; set; }
        public decimal? comm_46000 { get; set; }
        public decimal? comm_47000 { get; set; }
        public decimal? comm_48000 { get; set; }
        public decimal? comm_49000 { get; set; }
        public decimal? comm_50000 { get; set; }
        public decimal? gst { get; set; }
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }

    }
    public class RetailerComm2
    {
        public int idno { get; set; }
        public string whitelabelid { get; set; }
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
        public decimal? comm_26000 { get; set; }
        public decimal? comm_27000 { get; set; }
        public decimal? comm_28000 { get; set; }
        public decimal? comm_29000 { get; set; }
        public decimal? comm_30000 { get; set; }

        public decimal? comm_31000 { get; set; }
        public decimal? comm_32000 { get; set; }
        public decimal? comm_33000 { get; set; }
        public decimal? comm_34000 { get; set; }
        public decimal? comm_35000 { get; set; }
        public decimal? comm_36000 { get; set; }
        public decimal? comm_37000 { get; set; }
        public decimal? comm_38000 { get; set; }
        public decimal? comm_39000 { get; set; }
        public decimal? comm_40000 { get; set; }

        public decimal? comm_41000 { get; set; }
        public decimal? comm_42000 { get; set; }
        public decimal? comm_43000 { get; set; }
        public decimal? comm_44000 { get; set; }
        public decimal? comm_45000 { get; set; }
        public decimal? comm_46000 { get; set; }
        public decimal? comm_47000 { get; set; }
        public decimal? comm_48000 { get; set; }
        public decimal? comm_49000 { get; set; }
        public decimal? comm_50000 { get; set; }
        public decimal? gst { get; set; }
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }

    }
    public class dlmremComm2
    {
        public int idno { get; set; }
        public string whitelabelid { get; set; }
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
        public decimal? comm_26000 { get; set; }
        public decimal? comm_27000 { get; set; }
        public decimal? comm_28000 { get; set; }
        public decimal? comm_29000 { get; set; }
        public decimal? comm_30000 { get; set; }

        public decimal? comm_31000 { get; set; }
        public decimal? comm_32000 { get; set; }
        public decimal? comm_33000 { get; set; }
        public decimal? comm_34000 { get; set; }
        public decimal? comm_35000 { get; set; }
        public decimal? comm_36000 { get; set; }
        public decimal? comm_37000 { get; set; }
        public decimal? comm_38000 { get; set; }
        public decimal? comm_39000 { get; set; }
        public decimal? comm_40000 { get; set; }

        public decimal? comm_41000 { get; set; }
        public decimal? comm_42000 { get; set; }
        public decimal? comm_43000 { get; set; }
        public decimal? comm_44000 { get; set; }
        public decimal? comm_45000 { get; set; }
        public decimal? comm_46000 { get; set; }
        public decimal? comm_47000 { get; set; }
        public decimal? comm_48000 { get; set; }
        public decimal? comm_49000 { get; set; }
        public decimal? comm_50000 { get; set; }
        public decimal? gst { get; set; }
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }

    }
    public class UpdateDMTRetailerComm2
    {
        public int idno { get; set; }
        public string whitelabelid { get; set; }
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
        public decimal? comm_26000 { get; set; }
        public decimal? comm_27000 { get; set; }
        public decimal? comm_28000 { get; set; }
        public decimal? comm_29000 { get; set; }
        public decimal? comm_30000 { get; set; }

        public decimal? comm_31000 { get; set; }
        public decimal? comm_32000 { get; set; }
        public decimal? comm_33000 { get; set; }
        public decimal? comm_34000 { get; set; }
        public decimal? comm_35000 { get; set; }
        public decimal? comm_36000 { get; set; }
        public decimal? comm_37000 { get; set; }
        public decimal? comm_38000 { get; set; }
        public decimal? comm_39000 { get; set; }
        public decimal? comm_40000 { get; set; }

        public decimal? comm_41000 { get; set; }
        public decimal? comm_42000 { get; set; }
        public decimal? comm_43000 { get; set; }
        public decimal? comm_44000 { get; set; }
        public decimal? comm_45000 { get; set; }
        public decimal? comm_46000 { get; set; }
        public decimal? comm_47000 { get; set; }
        public decimal? comm_48000 { get; set; }
        public decimal? comm_49000 { get; set; }
        public decimal? comm_50000 { get; set; }
        public decimal? gst { get; set; }
        public bool? gst_sts { get; set; }
        public bool? tds_sts { get; set; }

    }
    #endregion
}
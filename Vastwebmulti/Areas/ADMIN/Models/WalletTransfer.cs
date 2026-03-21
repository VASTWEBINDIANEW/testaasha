using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for initiating and tracking wallet-to-wallet or wallet-to-bank fund transfers.
    /// </summary>
    public class WalletTransfer
    {
        public List<wallet_imps_common_comm> common { get; set; }
        public List<WalletMasterComm> Materuser { get; set; }
        public List<WalletDealerComm> Dealeruser { get; set; }
        public List<WalletRetailerComm> Retaileruser { get; set; }
        public List<WalletRetailerComm> DlmRemuser { get; set; }
        public List<UpdateWalletMasterComm> UpdateMaster { get; set; }
        public List<UpdateWalletDealerComm> UpdateDealer { get; set; }
        public List<UpdateWalletRetailerComm> UpdateRetailer { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> Retailerid { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
    public class WalletMasterComm
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
    public class WalletDealerComm
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
    public class WalletRetailerComm
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
    public class UpdateWalletMasterComm
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
    public class UpdateWalletDealerComm
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
    public class UpdateWalletRetailerComm
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
}
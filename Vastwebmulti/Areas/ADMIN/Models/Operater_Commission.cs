using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Represents operator-level commission configuration data.
    /// </summary>
    public class Operater_Commission
    {
        public List<Recharge_Amount_range_info> AmountRange { get; set; }
        public List<Prepaid_Comm> Prepaid { set; get; }
        public List<Electricity> Electricity_comm { set; get; }
        public List<Pencard> Pencard_comm { set; get; }
        public List<money_comm> Money { set; get; }
        public List<money2_comm> Money2 { set; get; }
        public List<money2_comm> Wallet { set; get; }
        public List<AEPS_Comm> AEPS { set; get; }
        public List<MPOS_Comm> MPOS { set; get; }

        public List<INDONEPAL_Comm> INDONEPAL { get; set; }

        public List<FLIGHT_Comm> FLIGHT { get; set; }
        public List<HOTEL_Comm> HOTEL { get; set; }

        public List<BUS_Comm> BUS { get; set; }
        public List<Giftcard_Comm> Giftcard { get; set; }
        public List<Broadband_Comm> Broadband { get; set; }
        public List<Loan_Comm> Loan { get; set; }
        public List<Water_Comm> Water { get; set; }
        public List<Insurance_Comm> Insurance { get; set; }
        public List<MicroAtmCash_comm> microatmcash { get; set; }
        public List<MicroAtm> microatmpurchase { get; set; }


    }
    public class Recharge_Amount_range_info
    {
        public int idno { get; set; }
        public decimal? maximum1 { get; set; }
        public decimal? maximum2 { get; set; }
    }
    public class MicroAtm
    {
        public string cardtype { get; set; }
        public string cardcategorytype { get; set; }
        public string cardname { get; set; }
        public decimal? minvalue { get; set; }
        public decimal? maxvalue { get; set; }
        public string chargetype { get; set; }
        public decimal? charge { get; set; }
    }
    public class MicroAtmCash_comm
    {
        public decimal? cash_0_1000_per { get; set; }
        public decimal? cash_1001_2000_per { get; set; }
        public decimal? cash_0_1000_rs { get; set; }
        public decimal? cash_1001_2000_rs { get; set; }
        public decimal? cash_0_1000_maxrs { get; set; }
        public decimal? cash_1001_2000_maxrs { get; set; }
        public string cash_0_1000_Type { get; set; }
        public string cash_1001_2000_Type { get; set; }
        public decimal? per_500_999 { get; set; }
        public decimal? rs_500_999 { get; set; }
        public decimal? maxrs_500_999 { get; set; }
        public string Type_500_999 { get; set; }
        public decimal? per_1000_1499 { get; set; }
        public decimal? rs_1000_1499 { get; set; }
        public decimal? maxrs_1000_1499 { get; set; }
        public string Type_1000_1499 { get; set; }
        public decimal? per_1500_1999 { get; set; }
        public decimal? rs_1500_1999 { get; set; }
        public decimal? maxrs_1500_1999 { get; set; }
        public string Type_1500_1999 { get; set; }
        public decimal? per_2000_2499 { get; set; }
        public decimal? rs_2000_2499 { get; set; }
        public decimal? maxrs_2000_2499 { get; set; }
        public string Type_2000_2499 { get; set; }
        public decimal? per_2500_2999 { get; set; }
        public decimal? rs_2500_2999 { get; set; }
        public decimal? maxrs_2500_2999 { get; set; }
        public string Type_2500_2999 { get; set; }
        public decimal? per_3000_3499 { get; set; }
        public decimal? rs_3000_3499 { get; set; }
        public decimal? maxrs_3000_3499 { get; set; }
        public string Type_3000_3499 { get; set; }
        public decimal? per_3500_5000 { get; set; }
        public decimal? rs_3500_5000 { get; set; }
        public decimal? maxrs_3500_5000 { get; set; }
        public string Type_3500_5000 { get; set; }
        public decimal? per_5001_10000 { get; set; }
        public decimal? rs_5001_10000 { get; set; }
        public decimal? maxrs_5001_10000 { get; set; }
        public string Type_5001_10000 { get; set; }
    }


    public class Insurance_Comm
    {
        public string OperatorName { get; set; }
        public string OperatorType { get; set; }
        public string OperatorCode { get; set; }
        public string Status { get; set; }
        public DateTime? BlockTime { get; set; }
        public decimal? Commission { get; set; }
        public decimal? Gst { set; get; }
    }
    public class Prepaid_Comm
    {
        public string OperatorName { get; set; }
        public string OperatorType { get; set; }
        public string OperatorCode { get; set; }
        public string Status { get; set; }
        public DateTime? BlockTime { get; set; }
        public decimal? Commission { get; set; }
        public decimal? Commission1 { get; set; }
        public decimal? Commission2 { get; set; }
    }
    public class Electricity
    {
        public string OperatorName { get; set; }
        public string OperatorType { get; set; }
        public string OperatorCode { get; set; }
        public string Status { get; set; }
        public DateTime? BlockTime { get; set; }
        public decimal? Commission { get; set; }
        public decimal? Commission1 { get; set; }
        public decimal? Commission2 { get; set; }
        public decimal? Commission3 { get; set; }
        public decimal? Commission4 { get; set; }
        public decimal? Gst { set; get; }
        public bool? CommIn { set; get; }
    }
    public class Pencard
    {
        public decimal? TokenValueDigital { get; set; }
        public decimal? TokenValuePhysical { get; set; }
        public decimal? CommissionDigital { get; set; }
        public decimal? CommissionPhysical { get; set; }
        public decimal? Gst { set; get; }
        public string panSerSts { set; get; }
        public DateTime? panPaidSts { set; get; }

    }
    public class money_comm
    {
        public decimal? Verifycomm { get; set; }
        public decimal? comm_1000 { get; set; }
        public decimal? comm_2000 { get; set; }
        public decimal? comm_3000 { get; set; }
        public decimal? comm_4000 { get; set; }
        public decimal? comm_5000 { get; set; }
        public decimal? gst { set; get; }
    }
    public class money2_comm
    {
        public decimal? Verifycomm { get; set; }
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
        public decimal? gst { set; get; }
        public decimal? per_26000 { set; get; }
        public decimal? maxrs_26000 { set; get; }
        public string Type_26000 { set; get; }
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

        public string panSerSts { set; get; }
        public DateTime? panPaidSts { set; get; }
    }
    public class AEPS_Comm
    {

        public decimal? aadharpay { get; set; }
        public decimal? ministatement { get; set; }
        public decimal? per_500_999 { get; set; }
        public decimal? rs_500_999 { get; set; }
        public decimal? maxrs_500_999 { get; set; }
        public string Type_500_999 { get; set; }
        public decimal? per_1000_1499 { get; set; }
        public decimal? rs_1000_1499 { get; set; }
        public decimal? maxrs_1000_1499 { get; set; }
        public string Type_1000_1499 { get; set; }
        public decimal? per_1500_1999 { get; set; }
        public decimal? rs_1500_1999 { get; set; }
        public decimal? maxrs_1500_1999 { get; set; }
        public string Type_1500_1999 { get; set; }
        public decimal? per_2000_2499 { get; set; }
        public decimal? rs_2000_2499 { get; set; }
        public decimal? maxrs_2000_2499 { get; set; }
        public string Type_2000_2499 { get; set; }
        public decimal? per_2500_2999 { get; set; }
        public decimal? rs_2500_2999 { get; set; }
        public decimal? maxrs_2500_2999 { get; set; }
        public string Type_2500_2999 { get; set; }
        public decimal? per_3000_3499 { get; set; }
        public decimal? rs_3000_3499 { get; set; }
        public decimal? maxrs_3000_3499 { get; set; }
        public string Type_3000_3499 { get; set; }
        public decimal? per_3500_5000 { get; set; }
        public decimal? rs_3500_5000 { get; set; }
        public decimal? maxrs_3500_5000 { get; set; }
        public string Type_3500_5000 { get; set; }
        public decimal? per_5001_10000 { get; set; }
        public decimal? rs_5001_10000 { get; set; }
        public decimal? maxrs_5001_10000 { get; set; }
        public string Type_5001_10000 { get; set; }
    }

    public class MPOS_Comm
    {
        public decimal? maxcashcomm { get; set; }
        public decimal? prepaidcard { get; set; }
        public decimal? CashWithdraw { get; set; }
        public decimal? Salesdebitupto2000 { get; set; }
        public decimal? Salesdebitabove2000 { get; set; }
        public decimal? SalescreditNormal { get; set; }
        public decimal? Salescreditgrocery { get; set; }
        public decimal? SalescreditEduandInsu { get; set; }
        public decimal? gst { get; set; }
        public string credit_type { get; set; }

        public string mposSerSts { set; get; }
        public DateTime? mposPaidSts { set; get; }

    }

    public class INDONEPAL_Comm
    {
        public decimal? min { get; set; }
        public decimal? max { get; set; }
        public decimal? charge { get; set; }
        public decimal? margin { get; set; }


    }

    public class FLIGHT_Comm
    {
        public bool IsDomestic { get; set; }
        public decimal? marginPercentage { get; set; }
        public decimal? RetailerMarkupCharge { get; set; }
        public decimal? gst { get; set; }
        public decimal? tds { get; set; }

        public string FLTSerSts { set; get; }
        public DateTime? FLTPaidSts { set; get; }

    }
    public class HOTEL_Comm
    {
        public bool IsDomestic { get; set; }
        public decimal? marginPercentage { get; set; }
        public decimal? RetailerMarkupCharge { get; set; }
        public decimal? gst { get; set; }
        public decimal? tds { get; set; }

        public string HTLSerSts { set; get; }
        public DateTime? HTLPaidSts { set; get; }

    }
    public class BUS_Comm
    {
        public decimal? marginPercentage { get; set; }
        public decimal? RetailerMarkupCharge { get; set; }
        public decimal? gst { get; set; }
        public decimal? tds { get; set; }

        public string busSerSts { set; get; }
        public DateTime? busPaidSts { set; get; }

    }
    public class Giftcard_Comm
    {
        public string OperatorName { get; set; }
        public string OperatorType { get; set; }
        public string OperatorCode { get; set; }
        public string Status { get; set; }
        public decimal? Commission { get; set; }

        public string GiftSerSts { set; get; }
        public DateTime? GiftPaidSts { set; get; }
    }
    public class Broadband_Comm
    {
        public string OperatorName { get; set; }
        public string OperatorType { get; set; }
        public string OperatorCode { get; set; }
        public string Status { get; set; }
        public DateTime? BlockTime { get; set; }
        public decimal? Commission { get; set; }

    }

    public class Loan_Comm
    {
        public string OperatorName { get; set; }
        public string OperatorType { get; set; }
        public string OperatorCode { get; set; }
        public string Status { get; set; }
        public DateTime? BlockTime { get; set; }
        public decimal? Commission { get; set; }
    }
    public class Water_Comm
    {
        public string OperatorName { get; set; }
        public string OperatorType { get; set; }
        public string OperatorCode { get; set; }
        public string Status { get; set; }
        public DateTime? BlockTime { get; set; }
        public decimal? Commission { get; set; }
    }
}
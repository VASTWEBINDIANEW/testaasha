using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.WHITELABEL.Models
{
    public class vmWhitelabel_OperatorIndex
    {
        public List<OperatorList> OptCodeList { get; set; }
        public List<OperatorList> UitlityOptList { get; set; }
        public List<OperatorList> FinancialOptList { get; set; }
        public List<OperatorList> TravelsOptList { get; set; }
        public List<OperatorList> OthersOptList { get; set; }
    }

    public class OperatorList
    {
        public string Operator_Id { get; set; }
        public string Operator_Name { get; set; }
        public string Operator_type { get; set; }
        public Nullable<bool> status { get; set; }
        public string opt_code { get; set; }
    }
}
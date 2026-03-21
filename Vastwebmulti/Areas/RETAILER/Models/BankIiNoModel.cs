using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.Models
{
    public  class BankIiNoModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public List<BankIiNoModelData> Data { get; set; }
        public long StatusCode { get; set; }
    }

    public  class BankIiNoModelData
    {
        public long Id { get; set; }
        public long ActiveFlag { get; set; }
        public string BankName { get; set; }
        public string Details { get; set; }
        public string IInNo { get; set; }
        public string Remarks { get; set; }
        public string Timestamp { get; set; }
    }
}
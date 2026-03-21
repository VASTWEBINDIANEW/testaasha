using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Model for DTH new connection service request data.
    /// </summary>
    public class dthconnectionModel
    {
        public int Id { get; set; }
        public int MappingId { get; set; }
        public string Title { get; set; }
        public string PlanName { get; set; }
        public Nullable<decimal> OfferePrice { get; set; }
        public Nullable<decimal> PublishePrice { get; set; }
        public string Specification { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ImageUrl { get; set; }
        //public IEnumerable<Vastwebmulti.Models.DTH_info> DTH_info { get; set; }
    }
}
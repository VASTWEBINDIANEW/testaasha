using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.ADMIN.Models
{
    /// <summary>
    /// Represents a prepaid card configuration or transaction entry.
    /// </summary>
    public class prepaid_card1
    {
        public List<prepaid_card_rem_comm> REMcommon { get; set; }
        public List<prepaid_card_dlm_comm> dlmcomm { get; set; }
        public List<prepaid_card_dlm_rem_comm> REMdlmcommon { get; set; }
        public List<prepaid_card_master_comm> Mastercommon { get; set; }
        public List<prepaid_card_common_comm> common_common { get; set; }
        public List<SelectListItem> UserId { get; set; }
        public List<SelectListItem> RetailerId { get; set; }
        public List<commmm> usersd { get; set; }

        public List<FinoDealer> Dealeruser { get; set; }
        public List<update_set> updateset { get; set; }
        public string role { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }


        public class commmm
        {
            public int idno { get; set; }
            public string Email { get; set; }
            public decimal? Comm { get; set; }
        }

        public class update_set
        {
            public int idno { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public decimal? Comm { get; set; }


        }


    }
}
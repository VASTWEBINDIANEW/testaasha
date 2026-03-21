using System.Linq;
using System.Net;

namespace Vastwebmulti.Models.Scheduling
{
    public class IPAddressCheck
    {
        public string GetComputer_InternetIP()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var urlinfo = db.Admin_details.SingleOrDefault().WebsiteUrl;
                IPAddress[] addresses = Dns.GetHostAddresses(urlinfo);
                return addresses[0].ToString();
            }
        }
    }
}
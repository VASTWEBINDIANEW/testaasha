using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Routing;
using Vastwebmulti.Hubs;
using Vastwebmulti.Models;

namespace Vastwebmulti.Controllers
{

    [RoutePrefix("api/Notification")]
    public class NotificationController : ApiController
    {
        private VastwebmultiEntities context = new VastwebmultiEntities();

        [HttpPost]
        [AllowAnonymous]
        [Route("SendNotification")]
        public HttpResponseMessage SendNotification(NotifModels obj)
        {
            NotificationHub objNotifHub = new NotificationHub();
            Notification objNotif = new Notification();
            objNotif.SentTo = obj.UserID;
            objNotif.Date = DateTime.Now;
            objNotif.IsRead = false;
            objNotif.IsDeleted = false;
            objNotif.DetailsURL = obj.DetaileUrl ?? "";
            objNotif.Details = obj.Message ?? "";
            objNotif.Title = obj.Title ?? "";

            context.Configuration.ProxyCreationEnabled = false;
            context.Notifications.Add(objNotif);
            context.SaveChanges();

            objNotifHub.SendNotification(objNotif.SentTo);

            var query = (from t in context.Notifications
                         select t).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, new { query });
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("UpdateReadProperty")]
        public HttpResponseMessage UpdateReadProperty(UpdateNotifReadProp obj)
        {
            var entry = context.Notifications.FirstOrDefault(a => a.Id == obj.Id);
            context.Configuration.ProxyCreationEnabled = false;
            entry.IsRead = true;
            context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("TEST")]
        public IHttpActionResult test()
        {
            return Ok("");
        }
    }
}

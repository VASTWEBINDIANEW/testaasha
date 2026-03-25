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
    /// <summary>
    /// Handles in-app notifications — provides endpoints to send new notifications to users
    /// and to update their read status via SignalR hub integration.
    /// </summary>
    public class NotificationController : ApiController
    {
        private VastwebmultiEntities context = new VastwebmultiEntities();

        /// <summary>
        /// Creates and sends a new notification to a specified user. Persists the notification
        /// to the database and broadcasts it in real time via the SignalR NotificationHub.
        /// </summary>
        /// <param name="obj">Notification model containing the target user ID, title, message, and optional detail URL.</param>
        /// <returns>An HTTP 200 response containing the full list of all notifications.</returns>
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

        /// <summary>
        /// Marks a specific notification as read by updating its <c>IsRead</c> property to <c>true</c>.
        /// </summary>
        /// <param name="obj">Model containing the ID of the notification to mark as read.</param>
        /// <returns>An HTTP 200 response indicating the update was successful.</returns>
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

        /// <summary>
        /// A simple test endpoint that returns an empty OK response to verify the controller is reachable.
        /// </summary>
        /// <returns>An HTTP 200 OK result with an empty string body.</returns>
        [Route("TEST")]
        public IHttpActionResult test()
        {
            return Ok("");
        }
    }
}

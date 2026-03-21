using Microsoft.AspNet.Identity;
using RestSharp;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Vastwebmulti.Models
{

    public class Low_Bal_CustomFilter : ActionFilterAttribute, IActionFilter
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ALLSMSSend smssend = new ALLSMSSend();
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                try
                {
                    var userid = filterContext.HttpContext.User.Identity.GetUserId();
                    var role = (from rol in db.Roles join user in db.UserRoles on rol.RoleId equals user.RoleId where user.UserId == userid select rol.Name).SingleOrDefault().ToString();
                    switch (role)
                    {
                        case "Retailer":
                            var remain = (from nm in db.Remain_reteller_balance where nm.RetellerId == userid select nm).Single().Remainamount;
                            var userfor = db.lowbalances.Where(a => a.Role == "Retailer" && a.status == true && a.amount > remain).FirstOrDefault();
                            if (userfor != null)
                            {
                                var retailer = db.Retailer_Details.Where(a => a.RetailerId == userid && a.ISDeleteuser == false).SingleOrDefault();
                                var chkmsg = db.lowbalancemsgs.Where(a => a.userid == userid).SingleOrDefault();
                                if (chkmsg != null)
                                {
                                    if (chkmsg.status == false)
                                    {
                                        try
                                        {
                                            string apiurls = "";
                                            var smsapi = db.apisms.Where(x => x.sts == "Y").ToList();
                                            if (smsapi.Any())
                                            {

                                                var smsapionsts = smsapi.Where(s => s.api_type == "whatsapp").SingleOrDefault();
                                                if (smsapionsts != null)
                                                {
                                                    apiurls = smsapionsts.smsapi;
                                                    string text = "Your Wallet Balance is Low, Please Add Amount, Your Current Balance is" + remain.ToString();
                                                    text = string.Format(text, "1230");

                                                    var apinamechange = apiurls.Replace("tttt", retailer.Mobile).Replace("mmmm", text);

                                                    var client = new RestClient(apinamechange);
                                                    var request = new RestRequest(Method.GET);

                                                    VastBazaartoken Responsetoken = new VastBazaartoken();
                                                    var whatsts = db.Email_show_passcode.SingleOrDefault();
                                                    if (apinamechange.ToUpper().Contains("API.VASTBAZAAR.COM/API/WEB/WHATSAPPMSG") && whatsts.whatsappapists == true)
                                                    {
                                                        var token = Responsetoken.gettoken();
                                                        request.AddHeader("authorization", "bearer " + token);
                                                        request.AddHeader("content-type", "application/json");
                                                    }
                                                    var task = Task.Run(() =>
                                                    {
                                                        return client.Execute(request).Content;
                                                    });
                                                    bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(10000));
                                                    var resp = "";
                                                    if (isCompletedSuccessfully == true)
                                                    {
                                                        resp = task.Result;
                                                    }

                                                    sms_api_entry sms = new sms_api_entry();
                                                    sms.apiname = apinamechange;
                                                    sms.msg = text;
                                                    sms.m_date = System.DateTime.Now;
                                                    sms.response = resp;
                                                    sms.messagefor = userid;
                                                    db.sms_api_entry.Add(sms);
                                                    db.SaveChanges();

                                                    chkmsg.status = true;
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                        catch
                                        { }

                                    }

                                }
                                else
                                {
                                    lowbalancemsg lowmsg = new lowbalancemsg();
                                    lowmsg.userid = userid;
                                    lowmsg.message = "Message Sent";
                                    lowmsg.status = false;
                                    db.lowbalancemsgs.Add(lowmsg);
                                    db.SaveChanges();
                                }
                            }

                            break;


                        case "API":
                            var remainbalance = db.api_remain_amount.Where(p => p.apiid == userid).SingleOrDefault().balance;
                            var userfor1 = db.lowbalances.Where(a => a.Role == "API" && a.status == true && a.amount > remainbalance).FirstOrDefault();
                            if (userfor1 != null)
                            {
                                var apiuser = db.api_user_details.Where(a => a.apiid == userid).SingleOrDefault();
                                var chkmsg = db.lowbalancemsgs.Where(a => a.userid == userid).SingleOrDefault();
                                if (chkmsg != null)
                                {
                                    if (chkmsg.status == false)
                                    {
                                        try
                                        {
                                            string apiurls = "";
                                            var smsapi = db.apisms.Where(x => x.sts == "Y").ToList();
                                            if (smsapi.Any())
                                            {

                                                var smsapionsts = smsapi.Where(s => s.api_type == "whatsapp").SingleOrDefault();
                                                if (smsapionsts != null)
                                                {
                                                    apiurls = smsapionsts.smsapi;
                                                    string text = "Your Wallet Balance is Low, Please Add Amount";
                                                    text = string.Format(text, "1230");

                                                    var apinamechange = apiurls.Replace("tttt", apiuser.mobile).Replace("mmmm", text);

                                                    var client = new RestClient(apinamechange);
                                                    var request = new RestRequest(Method.GET);

                                                    VastBazaartoken Responsetoken = new VastBazaartoken();
                                                    var whatsts = db.Email_show_passcode.SingleOrDefault();
                                                    if (apinamechange.ToUpper().Contains("API.VASTBAZAAR.COM/API/WEB/WHATSAPPMSG") && whatsts.whatsappapists == true)
                                                    {
                                                        var token = Responsetoken.gettoken();
                                                        request.AddHeader("authorization", "bearer " + token);
                                                        request.AddHeader("content-type", "application/json");
                                                    }
                                                    var task = Task.Run(() =>
                                                    {
                                                        return client.Execute(request).Content;
                                                    });
                                                    bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(10000));
                                                    var resp = "";
                                                    if (isCompletedSuccessfully == true)
                                                    {
                                                        resp = task.Result;
                                                    }

                                                    sms_api_entry sms = new sms_api_entry();
                                                    sms.apiname = apinamechange;
                                                    sms.msg = text;
                                                    sms.m_date = System.DateTime.Now;
                                                    sms.response = resp;
                                                    sms.messagefor = userid;
                                                    db.sms_api_entry.Add(sms);
                                                    db.SaveChanges();

                                                    chkmsg.status = true;
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                        catch
                                        { }
                                    }

                                }
                                else
                                {
                                    lowbalancemsg lowmsg = new lowbalancemsg();
                                    lowmsg.userid = userid;
                                    lowmsg.message = "Message Sent";
                                    lowmsg.status = false;
                                    db.lowbalancemsgs.Add(lowmsg);
                                    db.SaveChanges();
                                }
                            }
                            break;


                        case "Dealer":
                            var remainbalance2 = db.Remain_dealer_balance.SingleOrDefault(a => a.DealerID == userid)?.Remainamount;
                            var userfor2 = db.lowbalances.Where(a => a.Role == "Dealer" && a.status == true && a.amount > remainbalance2).FirstOrDefault();
                            if (userfor2 != null)
                            {
                                var dealer = db.Dealer_Details.Where(a => a.DealerId == userid).SingleOrDefault();
                                var chkmsg = db.lowbalancemsgs.Where(a => a.userid == userid).SingleOrDefault();
                                if (chkmsg != null)
                                {
                                    if (chkmsg.status == false)
                                    {
                                        try
                                        {
                                            string apiurls = "";
                                            var smsapi = db.apisms.Where(x => x.sts == "Y").ToList();
                                            if (smsapi.Any())
                                            {

                                                var smsapionsts = smsapi.Where(s => s.api_type == "whatsapp").SingleOrDefault();
                                                if (smsapionsts != null)
                                                {
                                                    apiurls = smsapionsts.smsapi;
                                                    string text = "Your Wallet Balance is Low, Please Add Amount";
                                                    text = string.Format(text, "1230");

                                                    var apinamechange = apiurls.Replace("tttt", dealer.Mobile).Replace("mmmm", text);

                                                    var client = new RestClient(apinamechange);
                                                    var request = new RestRequest(Method.GET);

                                                    VastBazaartoken Responsetoken = new VastBazaartoken();
                                                    var whatsts = db.Email_show_passcode.SingleOrDefault();
                                                    if (apinamechange.ToUpper().Contains("API.VASTBAZAAR.COM/API/WEB/WHATSAPPMSG") && whatsts.whatsappapists == true)
                                                    {
                                                        var token = Responsetoken.gettoken();
                                                        request.AddHeader("authorization", "bearer " + token);
                                                        request.AddHeader("content-type", "application/json");
                                                    }
                                                    var task = Task.Run(() =>
                                                    {
                                                        return client.Execute(request).Content;
                                                    });
                                                    bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(10000));
                                                    var resp = "";
                                                    if (isCompletedSuccessfully == true)
                                                    {
                                                        resp = task.Result;
                                                    }

                                                    sms_api_entry sms = new sms_api_entry();
                                                    sms.apiname = apinamechange;
                                                    sms.msg = text;
                                                    sms.m_date = System.DateTime.Now;
                                                    sms.response = resp;
                                                    sms.messagefor = userid;
                                                    db.sms_api_entry.Add(sms);
                                                    db.SaveChanges();

                                                    chkmsg.status = true;
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                        catch
                                        { }

                                    }

                                }
                                else
                                {
                                    lowbalancemsg lowmsg = new lowbalancemsg();
                                    lowmsg.userid = userid;
                                    lowmsg.message = "Message Sent";
                                    lowmsg.status = false;
                                    db.lowbalancemsgs.Add(lowmsg);
                                    db.SaveChanges();
                                }
                            }

                            break;

                        case "master":
                            var masterlivebal3 = db.Remain_superstokist_balance.Where(p => p.SuperStokistID == userid).SingleOrDefault().Remainamount;
                            var userfor3 = db.lowbalances.Where(a => a.Role == "master" && a.status == true && a.amount > masterlivebal3).FirstOrDefault();
                            if (userfor3 != null)
                            {
                                var master = db.Superstokist_details.Where(a => a.SSId == userid).SingleOrDefault();
                                var chkmsg = db.lowbalancemsgs.Where(a => a.userid == userid).SingleOrDefault();
                                if (chkmsg != null)
                                {
                                    if (chkmsg.status == false)
                                    {
                                        try
                                        {
                                            string apiurls = "";
                                            var smsapi = db.apisms.Where(x => x.sts == "Y").ToList();
                                            if (smsapi.Any())
                                            {

                                                var smsapionsts = smsapi.Where(s => s.api_type == "whatsapp").SingleOrDefault();
                                                if (smsapionsts != null)
                                                {
                                                    apiurls = smsapionsts.smsapi;
                                                    string text = "Your Wallet Balance is Low, Please Add Amount";
                                                    text = string.Format(text, "1230");

                                                    var apinamechange = apiurls.Replace("tttt", master.Mobile).Replace("mmmm", text);

                                                    var client = new RestClient(apinamechange);
                                                    var request = new RestRequest(Method.GET);

                                                    VastBazaartoken Responsetoken = new VastBazaartoken();
                                                    var whatsts = db.Email_show_passcode.SingleOrDefault();
                                                    if (apinamechange.ToUpper().Contains("API.VASTBAZAAR.COM/API/WEB/WHATSAPPMSG") && whatsts.whatsappapists == true)
                                                    {
                                                        var token = Responsetoken.gettoken();
                                                        request.AddHeader("authorization", "bearer " + token);
                                                        request.AddHeader("content-type", "application/json");
                                                    }
                                                    var task = Task.Run(() =>
                                                    {
                                                        return client.Execute(request).Content;
                                                    });
                                                    bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(10000));
                                                    var resp = "";
                                                    if (isCompletedSuccessfully == true)
                                                    {
                                                        resp = task.Result;
                                                    }

                                                    sms_api_entry sms = new sms_api_entry();
                                                    sms.apiname = apinamechange;
                                                    sms.msg = text;
                                                    sms.m_date = System.DateTime.Now;
                                                    sms.response = resp;
                                                    sms.messagefor = userid;
                                                    db.sms_api_entry.Add(sms);
                                                    db.SaveChanges();

                                                    chkmsg.status = true;
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                        catch
                                        { }

                                    }

                                }
                                else
                                {
                                    lowbalancemsg lowmsg = new lowbalancemsg();
                                    lowmsg.userid = userid;
                                    lowmsg.message = "Message Sent";
                                    lowmsg.status = false;
                                    db.lowbalancemsgs.Add(lowmsg);
                                    db.SaveChanges();
                                }
                            }

                            break;
                    }
                }
                catch
                { }
            }
        }
    }
}
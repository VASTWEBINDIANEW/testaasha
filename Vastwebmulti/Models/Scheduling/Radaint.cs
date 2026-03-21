using Newtonsoft.Json;
using Quartz;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Models.Scheduling
{
    public class Radaint : IJob
    {
        public static string radianttoken;
        public static string radianagentid;
        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {

                    var infochk = db.Rem_UPI_REQUEST.Where(aa => aa.status == "PENDING" && (aa.Bankrrn == "RADIANTQR" || aa.Bankrrn== "RADIANTCOLLECTION")).ToList();
                    foreach (var item in infochk)
                    {
                        Radiantdmt radi = new Radiantdmt();
                        var tokenchk = db.radianttokens.SingleOrDefault();
                        var radiantauthchk = db.radiantauths.SingleOrDefault();
                        var radiantresponse = db.rediantremtresponses.Where(aa => aa.userid == item.retailerid).SingleOrDefault();
                        if (tokenchk == null)
                        {
                            radi.Token(out radianttoken, out radianagentid, radiantauthchk.clientID, radiantauthchk.clientSecret, radiantauthchk.APIKey, radiantresponse.username, radiantresponse.password);
                        }
                        else
                        {
                            radianttoken = tokenchk.accessToken;
                            radianagentid = tokenchk.agentID;
                        }
                        if (item.Bankrrn == "RADIANTCOLLECTION")
                        {
                            var respchk = radi.UPICollectionStatusCheck(radianagentid, radiantauthchk.clientID, radiantauthchk.clientSecret, radiantauthchk.APIKey, radianttoken, item.UPITXNID);
                            if (respchk.StatusCode == HttpStatusCode.NotAcceptable)
                            {
                                radi.Token(out radianttoken, out radianagentid, radiantauthchk.clientID, radiantauthchk.clientSecret, radiantauthchk.APIKey, radiantresponse.username, radiantresponse.password);
                                respchk = radi.UPICollectionStatusCheck(radianagentid, radiantauthchk.clientID, radiantauthchk.clientSecret, radiantauthchk.APIKey, radianttoken, item.UPITXNID);
                            }
                            if(respchk.StatusCode==HttpStatusCode.OK)
                            {
                                dynamic dyrespchk = JsonConvert.DeserializeObject<dynamic>(respchk.ToString());
                                var sts = dyrespchk.success;
                                if(sts==true)
                                {
                                    var sts1 = dyrespchk.result.status;
                                    if(sts1==true )
                                    {
                                        string txnStatus = dyrespchk.result.status;
                                        string rrn = dyrespchk.result.rrn;
                                        if(txnStatus=="APPROVED")
                                        {
                                            System.Data.Entity.Core.Objects.ObjectParameter output = new
                                          System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                            db.Update_UPI_REQ(item.id, "APPROVED", output);
                                            try
                                            {
                                                var reminfochk=db.Rem_UPI_REQUEST.Where(aa=>aa.id==item.id).FirstOrDefault();
                                                var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == reminfochk.retailerid).SingleOrDefault();
                                                
                                                var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == reminfochk.retailerid).SingleOrDefault();
                                               
                                                var admininfo = db.Admin_details.SingleOrDefault();
                                                Backupinfo back = new Backupinfo();
                                                var model = new Backupinfo.Addinfo
                                                {
                                                    Websitename = admininfo.WebsiteUrl,
                                                    RetailerID = reminfochk.retailerid,
                                                    Email = retailerdetails.Email,
                                                    Mobile = retailerdetails.Mobile,
                                                    Details = "Fund Recived From Radiant Collection ",
                                                    RemainBalance = (decimal)remdetails.Remainamount,
                                                    Usertype = "Retailer"
                                                };
                                                back.Fundtransfer(model);

                                               
                                            }
                                            catch { }
                                        }
                                        else
                                        {
                                            System.Data.Entity.Core.Objects.ObjectParameter output = new
                                         System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                            db.Update_UPI_REQ(item.id, "REJECTED", output);
                                        }
                                    }
                                    else
                                    {
                                        string message = dyrespchk.message;
                                        System.Data.Entity.Core.Objects.ObjectParameter output = new
                                           System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                        db.Update_UPI_REQ(item.id, "REJECTED", output);
                                    }
                                }
                                else
                                {
                                    string message = dyrespchk.message;
                                    System.Data.Entity.Core.Objects.ObjectParameter output = new
                                       System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                    db.Update_UPI_REQ(item.id, "REJECTED", output);
                                }
                            }
                            else
                            {
                                dynamic dyrespchk=JsonConvert.DeserializeObject<dynamic>(respchk.ToString());
                                var sts = dyrespchk.success;
                                string message = dyrespchk.message;
                                System.Data.Entity.Core.Objects.ObjectParameter output = new
                                   System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                                db.Update_UPI_REQ(item.id, "REJECTED", output);
                            }

                        }
                        else
                        {
                            var respchk = radi.UPIATMstatusCheck(radianagentid, radiantauthchk.clientID, radiantauthchk.clientSecret, radiantauthchk.APIKey, radianttoken, item.UPITXNID);
                            if (respchk.StatusCode == HttpStatusCode.NotAcceptable)
                            {
                                radi.Token(out radianttoken, out radianagentid, radiantauthchk.clientID, radiantauthchk.clientSecret, radiantauthchk.APIKey, radiantresponse.username, radiantresponse.password);
                                respchk = radi.UPIATMstatusCheck(radianagentid, radiantauthchk.clientID, radiantauthchk.clientSecret, radiantauthchk.APIKey, radianttoken, item.UPITXNID);
                            }
                        }

                    }
                }
            });
            return task;
        }
    }

}
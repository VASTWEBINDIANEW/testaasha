using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti
{
    public class VastBazaartoken
    {
        
        
        VastwebmultiEntities db = new VastwebmultiEntities();
        public string gettoken()
        {
            var resptoken = "";
            var chk = db.vastbazzartokens.SingleOrDefault();
            if (chk == null)
            {
                var apidetails = db.Money_API_URLS.Where(aa => aa.API_Name == "VASTWEB").SingleOrDefault();
                var token = apidetails == null ? "" : apidetails.Token;
                var apiid = apidetails == null ? "" : apidetails.API_ID;
                var apiidpwd = apidetails == null ? "" : apidetails.Api_pwd;
                var client = new RestClient("http://api.vastbazaar.com/token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("iptoken", token);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", "UserName=" + apiid + "&Password=" + apiidpwd + "&grant_type=password", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var responsechk = response.Content;
                dynamic json = JsonConvert.DeserializeObject(responsechk);
                var token1 = json.access_token.ToString();
                var expire = json[".expires"].ToString();
                DateTime exp = Convert.ToDateTime(expire);
                vastbazzartoken vast = new vastbazzartoken();
                vast.apitoken = token1;
                vast.exptime = exp;
                db.vastbazzartokens.Add(vast);
                db.SaveChanges();
                resptoken = token1;
            }
            else
            {
                if (chk.exptime >= DateTime.Now)
                {
                    resptoken = chk.apitoken;
                }
                else if (chk.exptime <= DateTime.Now)
                {
                    var apidetails = db.Money_API_URLS.Where(aa => aa.API_Name == "VASTWEB").SingleOrDefault();
                    var token = apidetails == null ? "" : apidetails.Token;
                    var apiid = apidetails == null ? "" : apidetails.API_ID;
                    var apiidpwd = apidetails == null ? "" : apidetails.Api_pwd;
                    var client = new RestClient("http://api.vastbazaar.com/token");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("iptoken", token);
                    request.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request.AddParameter("application/x-www-form-urlencoded", "UserName=" + apiid + "&Password=" + apiidpwd + "&grant_type=password", ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    var responsechk = response.Content;
                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                    var token1 = json.access_token.ToString();
                    var expire = json[".expires"].ToString();
                    DateTime exp = Convert.ToDateTime(expire);
                    var vast = db.vastbazzartokens.SingleOrDefault();
                    //vastbazzartoken vast = new vastbazzartoken();
                    vast.apitoken = token1;
                    vast.exptime = exp;
                    db.SaveChanges();
                    resptoken = token1;
                }
                else
                {
                    var apidetails = db.Money_API_URLS.Where(aa => aa.API_Name == "VASTWEB").SingleOrDefault();
                    var token = apidetails == null ? "" : apidetails.Token;
                    var apiid = apidetails == null ? "" : apidetails.API_ID;
                    var apiidpwd = apidetails == null ? "" : apidetails.Api_pwd;
                    var client = new RestClient("http://api.vastbazaar.com/");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("iptoken", token);
                    request.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request.AddParameter("application/x-www-form-urlencoded", "UserName=" + apiid + "&Password=" + apiidpwd + "&grant_type=password", ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    var responsechk = response.Content;
                    try
                    {
                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                        var token1 = json.access_token.ToString();
                        var expire = json[".expires"].ToString();
                        DateTime exp = Convert.ToDateTime(expire);
                        chk.apitoken = token1;
                        chk.exptime = exp;
                        db.SaveChanges();
                        resptoken = token1;
                    }
                    catch
                    {

                    }
                }
            }
            return resptoken;
        }
    }
}
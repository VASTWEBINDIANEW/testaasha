using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Models
{
    public class Backupinfo
    {
        public void info(Addinfo Model) 
        {
            var clientn = new RestClient("http://backup.vastwebindia.com/api/Values/info");
            var requestn = new RestRequest(Method.POST);
            requestn.AddHeader("Content-Type", "application/json");
            var body = new
            {
                Websitename = Model.Websitename,
                RetailerID = Model.RetailerID,
                Email = Model.Email,
                Mobile = Model.Mobile,
                Details = Model.Details,
                RemainBalance = Model.RemainBalance,
                Usertype = Model.Usertype
            };
            var bodystring = JsonConvert.SerializeObject(body);
            requestn.AddParameter("application/json", bodystring, ParameterType.RequestBody);
            IRestResponse responsen = clientn.Execute(requestn);
        }
        public void Aeps(Addinfo Model)
        {
            var clientn = new RestClient("http://backup.vastwebindia.com/api/Values/Aeps");
            var requestn = new RestRequest(Method.POST);
            requestn.AddHeader("Content-Type", "application/json");
            var body = new
            {
                Websitename = Model.Websitename,
                RetailerID = Model.RetailerID,
                Email = Model.Email,
                Mobile = Model.Mobile,
                Details = Model.Details,
                RemainBalance = Model.RemainBalance,
                Usertype = Model.Usertype
            };
            var bodystring = JsonConvert.SerializeObject(body);
            requestn.AddParameter("application/json", bodystring, ParameterType.RequestBody);
            IRestResponse responsen = clientn.Execute(requestn);
        }
        public void Rechargeandutility(Addinfo Model)
        {
            var clientn = new RestClient("http://backup.vastwebindia.com/api/Values/Rechargeandutility");
            var requestn = new RestRequest(Method.POST);
            requestn.AddHeader("Content-Type", "application/json");
            var body = new
            {
                Websitename = Model.Websitename,
                RetailerID = Model.RetailerID,
                Email = Model.Email,
                Mobile = Model.Mobile,
                Details = Model.Details,
                RemainBalance = Model.RemainBalance,
                Usertype = Model.Usertype
            };
            var bodystring = JsonConvert.SerializeObject(body);
            requestn.AddParameter("application/json", bodystring, ParameterType.RequestBody);
            IRestResponse responsen = clientn.Execute(requestn);
        }
        public void MoneyTransfer(Addinfo Model)
        {
            var clientn = new RestClient("http://backup.vastwebindia.com/api/Values/MoneyTransfer");
            var requestn = new RestRequest(Method.POST);
            requestn.AddHeader("Content-Type", "application/json");
            var body = new
            {
                Websitename = Model.Websitename,
                RetailerID = Model.RetailerID,
                Email = Model.Email,
                Mobile = Model.Mobile,
                Details = Model.Details,
                RemainBalance = Model.RemainBalance,
                Usertype = Model.Usertype
            };
            var bodystring = JsonConvert.SerializeObject(body);
            requestn.AddParameter("application/json", bodystring, ParameterType.RequestBody);
            IRestResponse responsen = clientn.Execute(requestn);
        }
        public void Pancard(Addinfo Model)
        {
            var clientn = new RestClient("http://backup.vastwebindia.com/api/Values/Pancard");
            var requestn = new RestRequest(Method.POST);
            requestn.AddHeader("Content-Type", "application/json");
            var body = new
            {
                Websitename = Model.Websitename,
                RetailerID = Model.RetailerID,
                Email = Model.Email,
                Mobile = Model.Mobile,
                Details = Model.Details,
                RemainBalance = Model.RemainBalance,
                Usertype = Model.Usertype
            };
            var bodystring = JsonConvert.SerializeObject(body);
            requestn.AddParameter("application/json", bodystring, ParameterType.RequestBody);
            IRestResponse responsen = clientn.Execute(requestn);
        }
        public void Fundtransfer(Addinfo Model)
        {
            var clientn = new RestClient("http://backup.vastwebindia.com/api/Values/Fundtransfer");
            var requestn = new RestRequest(Method.POST);
            requestn.AddHeader("Content-Type", "application/json");
            var body = new
            {
                Websitename = Model.Websitename,
                RetailerID = Model.RetailerID,
                Email = Model.Email,
                Mobile = Model.Mobile,
                Details = Model.Details,
                RemainBalance = Model.RemainBalance,
                Usertype = Model.Usertype
            };
            var bodystring = JsonConvert.SerializeObject(body);
            requestn.AddParameter("application/json", bodystring, ParameterType.RequestBody);
            IRestResponse responsen = clientn.Execute(requestn);
        }
        public void Microatm(Addinfo Model)
        {
            var clientn = new RestClient("http://backup.vastwebindia.com/api/Values/Microatm");
            var requestn = new RestRequest(Method.POST);
            requestn.AddHeader("Content-Type", "application/json");
            var body = new
            {
                Websitename = Model.Websitename,
                RetailerID = Model.RetailerID,
                Email = Model.Email,
                Mobile = Model.Mobile,
                Details = Model.Details,
                RemainBalance = Model.RemainBalance,
                Usertype = Model.Usertype
            };
            var bodystring = JsonConvert.SerializeObject(body);
            requestn.AddParameter("application/json", bodystring, ParameterType.RequestBody);
            IRestResponse responsen = clientn.Execute(requestn);
        }
        public class Addinfo
        {
            public string Websitename { get; set; }
            public string RetailerID { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public string Details { get; set; }
            public decimal RemainBalance { get; set; }
            public string Usertype { get; set; }
        }
    }
}
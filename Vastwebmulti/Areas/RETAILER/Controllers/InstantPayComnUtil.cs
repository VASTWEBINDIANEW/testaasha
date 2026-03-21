using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Vastwebmulti.Areas.WRetailer.Models;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Controllers
{
    public class InstantPayComnUtil
    {
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        // string VastbazaarBaseUrl = "http://localhost:65209/";
        HttpClient client = new HttpClient();
        string token = string.Empty;
        public InstantPayComnUtil()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                try
                {
                    token = db.Money_API_URLS.Where(a => a.API_Name.Contains("INSTANTPAY")).Single().Token;
                    //token = "c4955a10bc97d33d9c64e09690af03ed";
                    if (HttpContext.Current.Request.IsLocal)
                    {
                        token = "ca23f55293bc9c2a51fe06d2d07e5ea4";
                    }
                }
                catch
                {
                    token = null;
                }

            }

        }

        #region Indo_NEPAL
        public JObject GetBankBranchList()
        {
            try
            {
                //////////////// Create Request Object ///////////////////////
                InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                InstantPayReq r = new InstantPayReq();
                r.token = token;
                ////////////////////////////////
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/pmt/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("bank_branch_list", r).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var contents = response.Content.ReadAsStringAsync();
                    dynamic jsonObeject = JsonConvert.DeserializeObject(contents.Result);
                    return jsonObeject;
                }
                else
                {
                    dynamic jsonObject = new JObject();
                    jsonObject.statuscode = "ERR";
                    jsonObject.status = "Some thing went wrong!";
                    return jsonObject;
                }
            }
            catch (Exception ex)
            {

                dynamic jsonObject = new JObject();
                jsonObject.statuscode = "ERR";
                jsonObject.status = ex.Message;
                return jsonObject;
            }

        }
        /// <summary>
        ///modes supported(Pass numeric value for respective mode) -
        ///
        ///1 - Transfer cash to receiver
        ///
        ///2 - Transfer into receiver account
        ///
        ///3 - Top-Up receiver Remittance Card
        ///
        ///branchid - Required in case of mode value 3
        ///
        ///amount - min:1, max: 49800
        ///
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="amount"></param>
        /// <param name="branchid"></param>
        /// <returns></returns>
        public JObject GetServiceCharge(string mode, string amount, string branchid)
        {
            try
            {
                //////////////// Create Request Object ///////////////////////
                InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                o.mode = mode;
                o.amount = amount;
                o.branchid = branchid;

                InstantPayReq r = new InstantPayReq();
                r.token = token;
                r.format = "json";
                r.request = o;
                ////////////////////////////////
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/pmt/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("service_charge", r).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var contents = response.Content.ReadAsStringAsync();
                    dynamic jsonObeject = JsonConvert.DeserializeObject(contents.Result);
                    return jsonObeject;
                }
                else
                {
                    dynamic jsonObject = new JObject();
                    jsonObject.statuscode = "ERR";
                    jsonObject.status = "Some thing went wrong!";
                    return jsonObject;
                }
            }
            catch (Exception ex)
            {

                dynamic jsonObject = new JObject();
                jsonObject.statuscode = "ERR";
                jsonObject.status = ex.Message;
                return jsonObject;
            }

        }
        public JObject ValidateBankAccount(string accountnumber)
        {
            try
            {
                //////////////// Create Request Object ///////////////////////
                InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                o.accountnumber = accountnumber;

                InstantPayReq r = new InstantPayReq();
                r.token = token;
                //r.format = "json";
                r.request = o;
                ////////////////////////////////
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/pmt/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("validate_bank_account", r).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var contents = response.Content.ReadAsStringAsync();
                    dynamic jsonObeject = JsonConvert.DeserializeObject(contents.Result);
                    return jsonObeject;
                }
                else
                {
                    dynamic jsonObject = new JObject();
                    jsonObject.statuscode = "ERR";
                    jsonObject.status = "Some thing went wrong!";
                    return jsonObject;
                }
            }
            catch (Exception ex)
            {

                dynamic jsonObject = new JObject();
                jsonObject.statuscode = "ERR";
                jsonObject.status = ex.Message;
                return jsonObject;
            }

        }
        public JObject ValidateCardAccount(string cardnumber)
        {
            try
            {
                //////////////// Create Request Object ///////////////////////
                InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                o.cardnumber = cardnumber;

                InstantPayReq r = new InstantPayReq();
                r.token = token;
                //r.format = "json";
                r.request = o;
                ////////////////////////////////
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/pmt/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("validate_card_number", r).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var contents = response.Content.ReadAsStringAsync();
                    dynamic jsonObeject = JsonConvert.DeserializeObject(contents.Result);
                    return jsonObeject;
                }
                else
                {
                    dynamic jsonObject = new JObject();
                    jsonObject.statuscode = "ERR";
                    jsonObject.status = "Some thing went wrong!";
                    return jsonObject;
                }
            }
            catch (Exception ex)
            {

                dynamic jsonObject = new JObject();
                jsonObject.statuscode = "ERR";
                jsonObject.status = ex.Message;
                return jsonObject;
            }

        }
        public JObject CashBranchList()
        {
            try
            {
                //////////////// Create Request Object ///////////////////////
                //InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                //o.cardnumber = cardnumber;

                InstantPayReq r = new InstantPayReq();
                r.token = token;
                //r.format = "json";
                //r.request = o;
                ////////////////////////////////
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/pmt/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("cash_branch_list", r).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var contents = response.Content.ReadAsStringAsync();
                    dynamic jsonObeject = JsonConvert.DeserializeObject(contents.Result);
                    return jsonObeject;
                }
                else
                {
                    dynamic jsonObject = new JObject();
                    jsonObject.statuscode = "ERR";
                    jsonObject.status = "Some thing went wrong!";
                    return jsonObject;
                }
            }
            catch (Exception ex)
            {

                dynamic jsonObject = new JObject();
                jsonObject.statuscode = "ERR";
                jsonObject.status = ex.Message;
                return jsonObject;
            }

        }
        public JObject SearchTxn(string pin, string partnerpin, string fromdate, string todate)
        {
            try
            {
                //////////////// Create Request Object ///////////////////////
                InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                o.pin = pin;
                o.partnerpin = partnerpin;
                o.fromdate = fromdate;
                o.todate = todate;
                InstantPayReq r = new InstantPayReq();
                r.token = token;
                //r.format = "json";
                r.request = o;
                ////////////////////////////////
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/pmt/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("search_txn", r).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var contents = response.Content.ReadAsStringAsync();
                    dynamic jsonObeject = JsonConvert.DeserializeObject(contents.Result);
                    return jsonObeject;
                }
                else
                {
                    dynamic jsonObject = new JObject();
                    jsonObject.statuscode = "ERR";
                    jsonObject.status = "Some thing went wrong!";
                    return jsonObject;
                }
            }
            catch (Exception ex)
            {

                dynamic jsonObject = new JObject();
                jsonObject.statuscode = "ERR";
                jsonObject.status = ex.Message;
                return jsonObject;
            }

        }
        public JObject Send_txn(string senderidtype, string sendername, string sendergender, string employer, string senderaddress, string sendermobile, string mode, string senderidnumber, string receivername, string receivergender, string receiveraddress, string doctype, string filename, string image_url)
        {
            try
            {
                //////////////// Create Request Object ///////////////////////
                InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                o.senderidtype = senderidtype;
                o.sendername = sendername;
                o.sendergender = sendergender;
                o.employer = employer;
                o.senderaddress = senderaddress;
                o.sendermobile = sendermobile;
                o.mode = mode;
                o.senderidnumber = senderidnumber;
                o.receivername = receivername;
                o.receivergender = receivergender;
                o.receiveraddress = receiveraddress;
                o.doctype = doctype;
                o.filename = filename;
                o.image_url = image_url;
                InstantPayReq r = new InstantPayReq();
                r.token = token;
                //r.format = "json";
                r.request = o;
                ////////////////////////////////
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/pmt/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("send_txn", r).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var contents = response.Content.ReadAsStringAsync();
                    dynamic jsonObeject = JsonConvert.DeserializeObject(contents.Result);
                    return jsonObeject;
                }
                else
                {
                    dynamic jsonObject = new JObject();
                    jsonObject.statuscode = "ERR";
                    jsonObject.status = "Some thing went wrong!";
                    return jsonObject;
                }
            }
            catch (Exception ex)
            {

                dynamic jsonObject = new JObject();
                jsonObject.statuscode = "ERR";
                jsonObject.status = ex.Message;
                return jsonObject;
            }

        }
        #endregion

        #region OutLetAPI
        public JObject VerifyOutletMobile(string Mobile)
        {
            JObject jj = new JObject();
            try
            {
                ////////////  Request ///////////
                InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                o.mobile = Mobile;


                InstantPayReq req = new InstantPayReq();
                req.token = token;
                req.request = o;
                ////////////////////////////////
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/outlet/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("sendOTP", req).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync();
                    JObject responseJson = JObject.Parse(responseString.Result.ToString());
                    JToken value;
                    dynamic dynJson = JsonConvert.DeserializeObject(responseString.Result.ToString());
                    if (responseJson.TryGetValue("statuscode", out value) && value.ToString() == "TXN")
                    {
                        jj.Add("RESULT", "0");
                        jj.Add("ADDINFO", dynJson.status);

                    }
                    else
                    {
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", dynJson.status);
                    }
                }


                return jj;
            }
            catch (Exception ex)
            {
                jj.Add("RESULT", "1");
                jj.Add("ADDINFO", ex.Message);
                return jj;
            }
        }
        public JObject RegisterOutlet(string RetailerId, string Mobile, string OTP, string email, string store_type, string company, string name, string pincode, string address)
        {
            JObject jj = new JObject();
            try
            {
                ////////////  Request ///////////
                InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                o.mobile = Mobile;
                o.otp = OTP;
                o.email = email;
                o.store_type = store_type;
                o.company = company;
                o.name = name;
                o.pincode = pincode;
                o.address = address;

                InstantPayReq req = new InstantPayReq();
                req.token = token;
                req.request = o;
                ////////////////////////////////
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/outlet/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("register", req).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync();
                    JObject responseJson = JObject.Parse(responseString.Result.ToString());
                    JToken value;
                    if (responseJson.TryGetValue("statuscode", out value) && value.ToString() == "TXN")
                    {
                        dynamic dynJson = JsonConvert.DeserializeObject(responseString.Result.ToString());
                        using (VastwebmultiEntities db = new VastwebmultiEntities())
                        {
                            var entry = db.RetailerOutlets.Where(a => a.RetailerId.ToLower() == RetailerId.ToLower()).SingleOrDefault();
                            if (entry != null)
                            {
                                entry.RetailerId = RetailerId.ToUpper();
                                entry.kyc_status = false;
                                entry.outlet_id = dynJson.data.outlet_id;
                                entry.outlet_status = true;
                                entry.store_type = string.IsNullOrWhiteSpace(store_type) ? "Others" : store_type;
                                entry.UpdatedOn = DateTime.Now;
                                db.SaveChanges();
                            }
                            else
                            {
                                entry = new RetailerOutlet();
                                entry.RetailerId = RetailerId.ToUpper();
                                entry.CreatedOn = DateTime.Now;
                                entry.kyc_status = false;
                                entry.outlet_id = dynJson.data.outlet_id;
                                entry.outlet_status = true;
                                entry.store_type = string.IsNullOrWhiteSpace(store_type) ? "Others" : store_type;
                                entry.IsKycUploaded = false;
                                entry.UpdatedOn = DateTime.Now;
                                db.RetailerOutlets.Add(entry);
                                db.SaveChanges();
                            }

                        }
                        jj.Add("RESULT", "0");
                        jj.Add("ADDINFO", dynJson.data.outlet_id);

                    }
                    else
                    {
                        responseJson.TryGetValue("status", out value);
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", value.ToString());
                    }
                }
                else
                {
                    jj.Add("RESULT", "1");
                    jj.Add("ADDINFO", "Somthing went wrong");

                }
                return jj;

            }
            catch (Exception ex)
            {

                jj.Add("RESULT", "1");
                jj.Add("ADDINFO", ex.Message);
                return jj;
            }
        }
        public JObject Upload_KYC_Doc(string RetailerID, string docId, string pan_no, string base64Content, string filename)
        {
            JObject jj = new JObject();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var entry = db.RetailerOutlets.Where(a => a.RetailerId.Equals(RetailerID, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                    if (entry == null)
                    {
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", "Outlet Not Created");
                        return jj;
                    }
                    else if (entry.IsPanConfirmed == null || entry.IsPanConfirmed == false)
                    {
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", "Please Update Pan Card First.");
                        return jj;
                    }
                    ////////////  Request ///////////
                    documentUpload document = new documentUpload();
                    document.id = docId;
                    document.base64 = base64Content;
                    document.filename = filename;
                    InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                    o.outletid = entry.outlet_id;
                    o.pan_no = pan_no;
                    o.document = document;

                    InstantPayReq req = new InstantPayReq();
                    req.token = token;
                    req.request = o;
                    ////////////////////////////////
                    client.BaseAddress = new Uri("https://www.instantpay.in/ws/outlet/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.PostAsJsonAsync("uploadDocs", req).Result;
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = response.Content.ReadAsStringAsync();
                        JObject responseJson = JObject.Parse(responseString.Result.ToString());
                        JToken value;
                        if (responseJson.TryGetValue("statuscode", out value) && value.ToString() == "TXN")
                        {
                            dynamic dynJson = JsonConvert.DeserializeObject(responseString.Result.ToString());
                            try
                            {
                                if (entry != null)
                                {

                                    entry.kyc_status = true;
                                    entry.UpdatedOn = DateTime.Now;
                                    db.SaveChanges();
                                }

                            }
                            catch { }
                            jj.Add("RESULT", "0");
                            jj.Add("ADDINFO", dynJson.status);

                        }
                        else
                        {
                            responseJson.TryGetValue("status", out value);
                            jj.Add("RESULT", "1");
                            jj.Add("ADDINFO", value.ToString());
                        }
                    }
                    else
                    {
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", "Somthing went wrong");

                    }
                    return jj;
                }

            }
            catch (Exception ex)
            {

                jj.Add("RESULT", "1");
                jj.Add("ADDINFO", ex.Message);
                return jj;
            }
        }
        public JObject UpdatePAN(string RetailerId, string pan_no)
        {
            JObject jj = new JObject();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var outlet = db.RetailerOutlets.Where(a => a.RetailerId.Equals(RetailerId, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                    if (outlet == null)
                    {
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", "Outlet Id Not Registered.");
                        return jj;
                    }
                    else
                    {
                        ////////////  Request ///////////
                        InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                        o.outletid = outlet.outlet_id;
                        o.pan_no = pan_no;

                        InstantPayReq req = new InstantPayReq();
                        req.token = token;
                        req.request = o;
                        ////////////////////////////////
                        client.BaseAddress = new Uri("https://www.instantpay.in/ws/outlet/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = client.PostAsJsonAsync("updatePan", req).Result;
                        response.EnsureSuccessStatusCode();
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = response.Content.ReadAsStringAsync();
                            JObject responseJson = JObject.Parse(responseString.Result.ToString());
                            JToken value;
                            if (responseJson.TryGetValue("statuscode", out value) && value.ToString() == "TXN")
                            {
                                dynamic dynJson = JsonConvert.DeserializeObject(responseString.Result.ToString());
                                try
                                {

                                    if (outlet != null)
                                    {

                                        outlet.IsPanConfirmed = true;
                                        outlet.UpdatedOn = DateTime.Now;
                                        db.SaveChanges();
                                    }

                                }
                                catch { }
                                jj.Add("RESULT", "0");
                                jj.Add("ADDINFO", dynJson.status);

                            }
                            else
                            {
                                responseJson.TryGetValue("status", out value);
                                jj.Add("RESULT", "1");
                                jj.Add("ADDINFO", value.ToString());
                            }
                        }
                        else
                        {
                            jj.Add("RESULT", "1");
                            jj.Add("ADDINFO", "Somthing went wrong");

                        }
                        return jj;
                    }
                }


            }
            catch (Exception ex)
            {

                jj.Add("RESULT", "1");
                jj.Add("ADDINFO", ex.Message);
                return jj;
            }
        }
        public JObject GetKycDoc(string RetailerId, string pan_no)
        {
            JObject jj = new JObject();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var outlet = db.RetailerOutlets.Where(a => a.RetailerId.Equals(RetailerId, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                    if (outlet == null)
                    {
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", "Outlet Id Not Registered.");
                        return jj;
                    }
                    else
                    {
                        ////////////  Request ///////////
                        InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                        o.outletid = outlet.outlet_id;
                        o.pan_no = pan_no;

                        InstantPayReq req = new InstantPayReq();
                        req.token = token;
                        req.request = o;
                        ////////////////////////////////
                        client.BaseAddress = new Uri("https://www.instantpay.in/ws/outlet/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = client.PostAsJsonAsync("requiredDocs", req).Result;
                        response.EnsureSuccessStatusCode();
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = response.Content.ReadAsStringAsync();
                            JObject responseJson = JObject.Parse(responseString.Result.ToString());
                            JToken value;
                            if (responseJson.TryGetValue("statuscode", out value) && value.ToString() == "TXN")
                            {
                                dynamic dynJson = JsonConvert.DeserializeObject(responseString.Result.ToString());
                                //try
                                //{
                                //    using (VastwebmultiEntities db = new VastwebmultiEntities())
                                //    {
                                //        var entry = db.RetailerOutlets.Where(a => a.outlet_id.Equals(outletid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                                //        if (entry != null)
                                //        {

                                //            entry.kyc_status = true;
                                //            entry.UpdatedOn = DateTime.Now;
                                //            db.SaveChanges();
                                //        }
                                //    }
                                //}
                                //catch { }
                                jj.Add("RESULT", "0");
                                jj.Add("ADDINFO", dynJson.data);

                            }
                            else
                            {
                                responseJson.TryGetValue("status", out value);
                                jj.Add("RESULT", "1");
                                jj.Add("ADDINFO", value.ToString());
                            }
                        }
                        else
                        {
                            jj.Add("RESULT", "1");
                            jj.Add("ADDINFO", "Somthing went wrong");

                        }
                        return jj;
                    }
                }


            }
            catch (Exception ex)
            {

                jj.Add("RESULT", "1");
                jj.Add("ADDINFO", ex.Message);
                return jj;
            }
        }
        #endregion

        #region PANCARD_API
        public JObject GetUTILoginCredentials(string RetailerId)
        {
            JObject jj = new JObject();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var outlet = db.RetailerOutlets.Where(a => a.RetailerId.Equals(RetailerId, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                    if (outlet == null)
                    {
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", "Outlet Id Not Registered.");
                        return jj;
                    }
                    else
                    {
                        ////////////  Request ///////////
                        InstantPayParamsPOCO o = new InstantPayParamsPOCO();
                        o.outletid = outlet.outlet_id;


                        InstantPayReq req = new InstantPayReq();
                        req.token = token;
                        if (HttpContext.Current.Request.IsLocal)
                        {
                            token = "60cd8f584d92c97468bfc505deed48eb";
                        }
                        req.request = o;
                        ////////////////////////////////
                        client.BaseAddress = new Uri("https://www.instantpay.in/ws/utipan/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = client.PostAsJsonAsync("psa_registration", req).Result;
                        response.EnsureSuccessStatusCode();
                        if (response.IsSuccessStatusCode)
                        {
                            var responseString = response.Content.ReadAsStringAsync();
                            JObject responseJson = JObject.Parse(responseString.Result.ToString());
                            JToken value;
                            if (responseJson.TryGetValue("statuscode", out value) && value.ToString() == "TXN")
                            {
                                dynamic dynJson = JsonConvert.DeserializeObject(responseString.Result.ToString());
                                //try
                                //{
                                //    using (VastwebmultiEntities db = new VastwebmultiEntities())
                                //    {
                                //        var entry = db.RetailerOutlets.Where(a => a.outlet_id.Equals(outletid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                                //        if (entry != null)
                                //        {

                                //            entry.kyc_status = true;
                                //            entry.UpdatedOn = DateTime.Now;
                                //            db.SaveChanges();
                                //        }
                                //    }
                                //}
                                //catch { }
                                jj.Add("RESULT", "0");
                                jj.Add("ADDINFO", dynJson.data.psa_uti_login_id);

                            }
                            else
                            {
                                responseJson.TryGetValue("status", out value);
                                jj.Add("RESULT", "1");
                                jj.Add("ADDINFO", value.ToString());
                            }
                        }
                        else
                        {
                            jj.Add("RESULT", "1");
                            jj.Add("ADDINFO", "Somthing went wrong");

                        }
                        return jj;
                    }
                }


            }
            catch (Exception ex)
            {

                jj.Add("RESULT", "1");
                jj.Add("ADDINFO", ex.Message);
                return jj;
            }
        }
        public JObject GetUTIToken(string RetailerId, string token, string digitalCount, string physicalCount, string merchantTxnId, string websitename, string ip, string latss, string longloc, string city, string address)
        {
            JObject jj = new JObject();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var rem = db.Retailer_Details.Where(aa => aa.RetailerId == RetailerId).SingleOrDefault();
                    var outlet = db.VastBazaarRetailerOutlets.Where(a => a.RetailerId.Equals(RetailerId, StringComparison.InvariantCultureIgnoreCase) && a.IsPanConfirmed == true).SingleOrDefault();
                    if (outlet == null)
                    {
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", "Outlet Id Not Registered.");
                        return jj;
                    }
                    else
                    {
                        ////  charge Retailer for Pan Token Request

                        System.Data.Entity.Core.Objects.ObjectParameter output = new
               System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
                        System.Data.Entity.Core.Objects.ObjectParameter Idno = new
                 System.Data.Entity.Core.Objects.ObjectParameter("Idno", typeof(string));
                        //var dealerid = db.Retailer_Details.Where(p => p.RetailerId == RetailerId).Single().DealerId;
                        //var slabname = db.Retailer_Details.Where(p => p.RetailerId == RetailerId).Single().slab_name;
                        //var retailertokanvalue = db.Slab_PanCard.SingleOrDefault(p => p.createdby == dealerid && p.clienttype == "Retailer" && p.slabname == slabname).tokenval;
                        var dbresponse = db.proc_insert_PAN_CARD_IPAY_Request(RetailerId, "none", Convert.ToInt32(digitalCount ?? "0"), Convert.ToInt32(physicalCount), merchantTxnId, Idno, output).SingleOrDefault();
                        try
                        {
                            var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == RetailerId).SingleOrDefault();
                            var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                            var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                            var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == RetailerId).SingleOrDefault();
                            var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                            var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                            var admininfo = db.Admin_details.SingleOrDefault();
                            Backupinfo back = new Backupinfo();
                            var modeln = new Backupinfo.Addinfo
                            {
                                Websitename = admininfo.WebsiteUrl,
                                RetailerID = RetailerId,
                                Email = retailerdetails.Email,
                                Mobile = retailerdetails.Mobile,
                                Details = "Pan Card Booking ",
                                RemainBalance = (decimal)remdetails.Remainamount,
                                Usertype = "Retailer"
                            };
                            back.Pancard(modeln);

                            var model1 = new Backupinfo.Addinfo
                            {
                                Websitename = admininfo.WebsiteUrl,
                                RetailerID = dealerdetails.DealerId,
                                Email = dealerdetails.Email,
                                Mobile = dealerdetails.Mobile,
                                Details = "Pan Card Booking ",

                                RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                Usertype = "Dealer"
                            };
                            back.Pancard(model1);

                            var model2 = new Backupinfo.Addinfo
                            {
                                Websitename = admininfo.WebsiteUrl,
                                RetailerID = masterdetails.SSId,
                                Email = masterdetails.Email,
                                Mobile = masterdetails.Mobile,
                                Details = "Pan Card Booking ",
                                RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                Usertype = "Master"
                            };
                            back.Pancard(model2);
                        }
                        catch { }

                        var isSuccess = dbresponse.msg;
                        int idnoo = (int)dbresponse.Idno;

                        if (isSuccess == "Success")
                        {



                            var rchinforsss = db.PAN_CARD_IPAY.Where(x => x.Idno == idnoo).FirstOrDefault();
                            if (rchinforsss != null)
                            {

                                rchinforsss.Devicetoken = null;
                                rchinforsss.Latitude = latss;
                                rchinforsss.Longitude = longloc;
                                rchinforsss.ModelNo = null;
                                rchinforsss.City = city;
                                rchinforsss.PostalCode = null;
                                rchinforsss.InternetTYPE = null;
                                rchinforsss.IPaddress = ip;
                                rchinforsss.Address = address;
                                db.SaveChanges();
                            }

                            var tXNdATE = DateTime.Now.ToString("MM/dd/yyyy hh:MM:ss tt"); //"09/17/2018 01:40:50 PM";//
                            var RequestObject = new
                            {
                                totalcoupon_physical = physicalCount,
                                totalcoupon_digital = digitalCount,
                                psaid = outlet.outlet_id,
                                transactionid = merchantTxnId,
                                remfirm = rem.Frm_Name,
                                rememail = rem.Email
                            };

                            var client = new RestClient(VastbazaarBaseUrl + "api/UTI/buyToken_New_Update");
                            var request = new RestRequest(Method.POST);
                            request.RequestFormat = DataFormat.Json;
                            request.AddHeader("authorization", "bearer " + token + "");
                            request.AddHeader("content-type", "application/json");
                            //request.AddParameter("application/json", "{\r\n\"securityKey\":\"Ma******3*****\",\r\n\"createdby\":\"M******1\",\r\n\"totalcoupon_physical\":\"1\",\r\n\"psaid\":\"TEST7960517\",\r\n\"transactionid\":\"Maha321\",\r\n\"transactiondate\":\"05/25/1991 11:42:39 PM\",\r\n\"totalcoupon_digital\":\"2\",\r\n\"udf1\":\"test1\",\r\n\"udf2\":\"test2\",\r\n\"udf3\":\"test3\",\r\n\"udf4\":\"test4\",\r\n\"udf5\":\"test5\"\r\n}", ParameterType.RequestBody);
                            request.AddBody(RequestObject);
                            IRestResponse response = client.Execute(request);
                            if (response.StatusCode == HttpStatusCode.BadRequest)
                            {
                                var xx = UpdateToken();
                                db.proc_PAN_CARD_Refund(idnoo.ToString(), "Failed", "Rejected", "");
                                try
                                {
                                    var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == RetailerId).SingleOrDefault();
                                    var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                                    var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                                    var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == RetailerId).SingleOrDefault();
                                    var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                                    var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                                    var admininfo = db.Admin_details.SingleOrDefault();
                                    Backupinfo back = new Backupinfo();
                                    var modeln = new Backupinfo.Addinfo
                                    {
                                        Websitename = admininfo.WebsiteUrl,
                                        RetailerID = RetailerId,
                                        Email = retailerdetails.Email,
                                        Mobile = retailerdetails.Mobile,
                                        Details = "Pan Card booking Refund",
                                        RemainBalance = (decimal)remdetails.Remainamount,
                                        Usertype = "Retailer"
                                    };
                                    back.Pancard(modeln);

                                    var model1 = new Backupinfo.Addinfo
                                    {
                                        Websitename = admininfo.WebsiteUrl,
                                        RetailerID = dealerdetails.DealerId,
                                        Email = dealerdetails.Email,
                                        Mobile = dealerdetails.Mobile,
                                        Details = "Pan Card booking Refund",

                                        RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                        Usertype = "Dealer"
                                    };
                                    back.Pancard(model1);

                                    var model2 = new Backupinfo.Addinfo
                                    {
                                        Websitename = admininfo.WebsiteUrl,
                                        RetailerID = masterdetails.SSId,
                                        Email = masterdetails.Email,
                                        Mobile = masterdetails.Mobile,
                                        Details = "Pan Card booking Refund",
                                        RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                        Usertype = "Master"
                                    };
                                    back.Pancard(model2);
                                }
                                catch { }
                                jj.Add("RESULT", "1");
                                jj.Add("ADDINFO", "Unknown Error");
                                return jj;
                            }
                            //IRestResponse response = client.Execute(request);
                            dynamic respo = JsonConvert.DeserializeObject(response.Content);
                            if (string.IsNullOrWhiteSpace(response.Content))
                            {
                                jj.Add("RESULT", "1");
                                jj.Add("ADDINFO", "Unknown Error");
                                return jj;
                            }
                            string sts = respo.Content.ADDINFO[0].Txnstatus;
                            if (sts.ToUpper() == "SUCCESS")
                            {
                                try
                                {
                                    string TXNID = respo.Content.ADDINFO[0].Orderid;
                                    int id = (int)idnoo;
                                    db.proc_PAN_CARD_Refund(idnoo.ToString(), "Success", "Pending", Convert.ToString(TXNID));
                                }
                                catch { }
                                jj.Add("RESULT", "0");
                                jj.Add("ADDINFO", respo.Content.ADDINFO[0].Msg);
                            }
                            else if (sts.ToUpper() == "FAILED")
                            {
                                string msg = respo.Content.ADDINFO[0].Msg;
                                db.proc_PAN_CARD_Refund(idnoo.ToString(), "Failed", "Rejected", "");
                                try
                                {
                                    var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == RetailerId).SingleOrDefault();
                                    var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                                    var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                                    var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == RetailerId).SingleOrDefault();
                                    var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                                    var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                                    var admininfo = db.Admin_details.SingleOrDefault();
                                    Backupinfo back = new Backupinfo();
                                    var modeln = new Backupinfo.Addinfo
                                    {
                                        Websitename = admininfo.WebsiteUrl,
                                        RetailerID = RetailerId,
                                        Email = retailerdetails.Email,
                                        Mobile = retailerdetails.Mobile,
                                        Details = "Pan Card booking Refund",
                                        RemainBalance = (decimal)remdetails.Remainamount,
                                        Usertype = "Retailer"
                                    };
                                    back.Pancard(modeln);

                                    var model1 = new Backupinfo.Addinfo
                                    {
                                        Websitename = admininfo.WebsiteUrl,
                                        RetailerID = dealerdetails.DealerId,
                                        Email = dealerdetails.Email,
                                        Mobile = dealerdetails.Mobile,
                                        Details = "Pan Card booking Refund",

                                        RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                        Usertype = "Dealer"
                                    };
                                    back.Pancard(model1);

                                    var model2 = new Backupinfo.Addinfo
                                    {
                                        Websitename = admininfo.WebsiteUrl,
                                        RetailerID = masterdetails.SSId,
                                        Email = masterdetails.Email,
                                        Mobile = masterdetails.Mobile,
                                        Details = "Pan Card booking Refund",
                                        RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                        Usertype = "Master"
                                    };
                                    back.Pancard(model2);
                                }
                                catch { }
                                jj.Add("RESULT", "1");
                                jj.Add("ADDINFO", msg);
                            }
                            else
                            {
                                string msg = respo.Content.ADDINFO[0].Msg;
                                jj.Add("RESULT", "1");
                                jj.Add("ADDINFO", msg);
                            }

                            return jj;
                        }
                        else
                        {
                            jj.Add("RESULT", "1");
                            jj.Add("ADDINFO", isSuccess);
                            return jj;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                jj.Add("RESULT", "1");
                jj.Add("ADDINFO", ex.Message);
                return jj;
            }
        }
        public JObject getUtiTokenStatus(string id, string token)
        {

            JObject jj = new JObject();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    int idnoINT = Convert.ToInt32(id);
                    var retailerid = db.PAN_CARD_IPAY.SingleOrDefault(a => a.Idno == idnoINT).RetailerId;
                    var userType = db.PAN_CARD_IPAY.SingleOrDefault(a => a.Idno == idnoINT).Usertype;
                    var merchantTxnId = db.PAN_CARD_IPAY.SingleOrDefault(a => a.Idno == idnoINT).MerchantTxnId;
                    string outletid = string.Empty;
                    if (userType == "Retailer")
                    {
                        outletid = db.VastBazaarRetailerOutlets.Where(a => a.RetailerId.Equals(retailerid, StringComparison.InvariantCultureIgnoreCase) && a.IsPanConfirmed == true).SingleOrDefault().outlet_id;

                    }
                    else
                    {
                        outletid = db.Whitelabel_VastBazaarRetailerOutlets.Where(a => a.RetailerId.Equals(retailerid, StringComparison.InvariantCultureIgnoreCase) && a.IsPanConfirmed == true).SingleOrDefault().outlet_id;

                    }
                    var RequestObject = new
                    {
                        requestid = merchantTxnId,
                        psaid = outletid
                    };

                    var client = new RestClient(VastbazaarBaseUrl + "api/UTI/tokenStatus_New");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token + "");
                    request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", "{\r\n\"securityKey\":\"Ma******3*****\",\r\n\"createdby\":\"M******1\",\r\n\"totalcoupon_physical\":\"1\",\r\n\"psaid\":\"TEST7960517\",\r\n\"transactionid\":\"Maha321\",\r\n\"transactiondate\":\"05/25/1991 11:42:39 PM\",\r\n\"totalcoupon_digital\":\"2\",\r\n\"udf1\":\"test1\",\r\n\"udf2\":\"test2\",\r\n\"udf3\":\"test3\",\r\n\"udf4\":\"test4\",\r\n\"udf5\":\"test5\"\r\n}", ParameterType.RequestBody);
                    request.AddBody(RequestObject);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var xx = UpdateToken();
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", "Unknown Error");
                        return jj;
                    }
                    //IRestResponse response = client.Execute(request);
                    dynamic respo = JsonConvert.DeserializeObject(response.Content);
                    if (string.IsNullOrWhiteSpace(response.Content))
                    {
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", "Unknown Error");
                        return jj;
                    }
                    string sts = respo.Content.ADDINFO[0].Txnstatus;
                    if (sts == null)
                    {
                        sts = "Failed";
                    }
                    if (sts.ToUpper() == "SUCCESS")
                    {
                        var TXNID = respo.Content.ADDINFO[0].Orderid.ToString();
                        db.proc_PAN_CARD_Refund(id, "Success", "Approved", TXNID);
                        jj.Add("RESULT", "0");
                        jj.Add("ADDINFO", respo.Content.ADDINFO[0].Msg);
                    }
                    else if (sts.ToUpper() == "FAILED")
                    {
                        var msg = respo.Content.ADDINFO[0].Msg;
                        if (msg == null)
                        {
                            msg = "Not Found";
                        }
                        db.proc_PAN_CARD_Refund(id.ToString(), "Failed", "Rejected", Convert.ToString(respo.Content.ADDINFO[0].Msg));
                        try
                        {
                            var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == retailerid).SingleOrDefault();
                            var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                            var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                            var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == retailerid).SingleOrDefault();
                            var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                            var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                            var admininfo = db.Admin_details.SingleOrDefault();
                            Backupinfo back = new Backupinfo();
                            var modeln = new Backupinfo.Addinfo
                            {
                                Websitename = admininfo.WebsiteUrl,
                                RetailerID = retailerid,
                                Email = retailerdetails.Email,
                                Mobile = retailerdetails.Mobile,
                                Details = "Pan Card booking Refund",
                                RemainBalance = (decimal)remdetails.Remainamount,
                                Usertype = "Retailer"
                            };
                            back.Pancard(modeln);

                            var model1 = new Backupinfo.Addinfo
                            {
                                Websitename = admininfo.WebsiteUrl,
                                RetailerID = dealerdetails.DealerId,
                                Email = dealerdetails.Email,
                                Mobile = dealerdetails.Mobile,
                                Details = "Pan Card booking Refund",

                                RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                Usertype = "Dealer"
                            };
                            back.Pancard(model1);

                            var model2 = new Backupinfo.Addinfo
                            {
                                Websitename = admininfo.WebsiteUrl,
                                RetailerID = masterdetails.SSId,
                                Email = masterdetails.Email,
                                Mobile = masterdetails.Mobile,
                                Details = "Pan Card booking Refund",
                                RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                Usertype = "Master"
                            };
                            back.Pancard(model2);
                        }
                        catch { }
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", msg);
                    }
                    else
                    {
                        jj.Add("RESULT", "2");
                        jj.Add("ADDINFO", respo.Content.ADDINFO[0].Msg);
                    }
                    return jj;
                }

            }
            catch (Exception ex)
            {
                jj.Add("RESULT", "1");
                jj.Add("ADDINFO", ex.Message);
                return jj;
            }
        }
        public void PanManualFailed(string id, string remark)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    int IDNO= Convert.ToInt32(id);
                    var retailerid = db.PAN_CARD_IPAY.Where(aa => aa.Idno == IDNO).SingleOrDefault().RetailerId;
                    db.proc_PAN_CARD_Refund_Manual(id.ToString(), "Manual Failed", remark);
                    try
                    {
                        var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == retailerid).SingleOrDefault();
                        var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                        var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                        var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == retailerid).SingleOrDefault();
                        var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                        var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                        var admininfo = db.Admin_details.SingleOrDefault();
                        Backupinfo back = new Backupinfo();
                        var modeln = new Backupinfo.Addinfo
                        {
                            Websitename = admininfo.WebsiteUrl,
                            RetailerID = retailerid,
                            Email = retailerdetails.Email,
                            Mobile = retailerdetails.Mobile,
                            Details = "Pan Card booking Refund",
                            RemainBalance = (decimal)remdetails.Remainamount,
                            Usertype = "Retailer"
                        };
                        back.Pancard(modeln);

                        var model1 = new Backupinfo.Addinfo
                        {
                            Websitename = admininfo.WebsiteUrl,
                            RetailerID = dealerdetails.DealerId,
                            Email = dealerdetails.Email,
                            Mobile = dealerdetails.Mobile,
                            Details = "Pan Card booking Refund",

                            RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                            Usertype = "Dealer"
                        };
                        back.Pancard(model1);

                        var model2 = new Backupinfo.Addinfo
                        {
                            Websitename = admininfo.WebsiteUrl,
                            RetailerID = masterdetails.SSId,
                            Email = masterdetails.Email,
                            Mobile = masterdetails.Mobile,
                            Details = "Pan Card booking Refund",
                            RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                            Usertype = "Master"
                        };
                        back.Pancard(model2);
                    }
                    catch { }
                }

            }
            catch
            {

            }

        }
        #endregion
        public string UpdateToken()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                /////////////////
                var apidetails = db.Money_API_URLS.Where(aa => aa.API_Name == "VASTWEB").SingleOrDefault();
                var token = apidetails == null ? "" : apidetails.Token;
                var apiid = apidetails == null ? "" : apidetails.API_ID;
                var apiidpwd = apidetails == null ? "" : apidetails.Api_pwd;

                var client = new RestClient(VastbazaarBaseUrl + "token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("iptoken", token);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", "UserName=" + apiid + "&Password=" + apiidpwd + "&grant_type=password", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                //////////////////
                var responsechk = response.Content.ToString();
                var responsecode = response.StatusCode.ToString();
                if (responsecode == "OK")
                {

                    var tokn = db.vastbazzartokens.SingleOrDefault();
                    dynamic json = JsonConvert.DeserializeObject(responsechk);
                    var newtoken = json.access_token.ToString();
                    var expire = json[".expires"].ToString();
                    DateTime exp = Convert.ToDateTime(expire);
                    tokn.apitoken = newtoken;
                    tokn.exptime = exp;
                    db.SaveChanges();
                    return newtoken;

                }
                else
                {
                    return null;
                }
            }
        }
        #region MoneyTransfer
        public string getBankList(string AccNo)
        {

            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/dmi/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //string acc = db.bank_info.FirstOrDefault().acno;

                var data = new
                {
                    token = token,
                    request = new { account = AccNo }
                };
                HttpResponseMessage response = client.PostAsJsonAsync("bank_details", data).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync();
                    JObject responseJson = JObject.Parse(responseString.Result.ToString());
                    JToken value;
                    if (responseJson.TryGetValue("statuscode", out value) && value.ToString() == "TXN")
                    {
                        //dynamic dynJson = JsonConvert.DeserializeObject(responseString.Result.ToString());
                        //try
                        //{
                        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
                        //    {
                        //        var entry = db.RetailerOutlets.Where(a => a.outlet_id.Equals(outletid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                        //        if (entry != null)
                        //        {

                        //            entry.kyc_status = true;
                        //            entry.UpdatedOn = DateTime.Now;
                        //            db.SaveChanges();
                        //        }
                        //    }
                        //}
                        //catch { }
                        return responseString.Result.ToString();

                    }
                    else
                    {
                        return "ERROR";
                    }
                }
                else
                {
                    return "ERROR";

                }

            }


        }
        #endregion
        #region GIFT_CARD
        public string getGiftCardList(string flag)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/ppcbv/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //string acc = db.bank_info.FirstOrDefault().acno;
                var data = new
                {
                    token = token,
                    request = new { type = flag } //pass type = ALL to get all gift cards.
                };
                HttpResponseMessage response = client.PostAsJsonAsync("brand_voucher", data).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync();
                    JObject responseJson = JObject.Parse(responseString.Result.ToString());
                    JToken value;
                    if (responseJson.TryGetValue("statuscode", out value) && value.ToString() == "TXN")
                    {
                        //dynamic dynJson = JsonConvert.DeserializeObject(responseString.Result.ToString());
                        //try
                        //{
                        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
                        //    {
                        //        var entry = db.RetailerOutlets.Where(a => a.outlet_id.Equals(outletid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                        //        if (entry != null)
                        //        {

                        //            entry.kyc_status = true;
                        //            entry.UpdatedOn = DateTime.Now;
                        //            db.SaveChanges();
                        //        }
                        //    }
                        //}
                        //catch { }
                        return responseString.Result.ToString();

                    }
                    else
                    {
                        return "ERROR";
                    }
                }
                else
                {
                    return "ERROR";

                }

            }


        }
        public string getProductByKey(string spKey)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/ppcbv/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //string acc = db.bank_info.FirstOrDefault().acno;
                var data = new
                {
                    token = token,
                    request = new { sp_key = spKey }
                };
                HttpResponseMessage response = client.PostAsJsonAsync("product_detail", data).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync();
                    JObject responseJson = JObject.Parse(responseString.Result.ToString());
                    JToken value;
                    if (responseJson.TryGetValue("statuscode", out value) && value.ToString() == "TXN")
                    {
                        //dynamic dynJson = JsonConvert.DeserializeObject(responseString.Result.ToString());
                        //try
                        //{
                        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
                        //    {
                        //        var entry = db.RetailerOutlets.Where(a => a.outlet_id.Equals(outletid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                        //        if (entry != null)
                        //        {

                        //            entry.kyc_status = true;
                        //            entry.UpdatedOn = DateTime.Now;
                        //            db.SaveChanges();
                        //        }
                        //    }
                        //}
                        //catch { }
                        return responseString.Result.ToString();

                    }
                    else
                    {
                        return "ERROR";
                    }
                }
                else
                {
                    return "ERROR";

                }

            }


        }
        #endregion
        #region DTH Booking
        public string getOperatorDetails(string flag)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://www.instantpay.in/ws/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //string acc = db.bank_info.FirstOrDefault().acno;
                var data = new
                {
                    token = token,
                    spkey = flag, //passcode here.
                    format = "json",
                };
                HttpResponseMessage response = client.PostAsJsonAsync("serviceproviders", data).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync();
                    JObject responseJson = JObject.Parse(responseString.Result.ToString());
                    JToken value;
                    if (responseJson.TryGetValue("ipay_errorcode", out value) && value.ToString() == "TXN")
                    {
                        //dynamic dynJson = JsonConvert.DeserializeObject(responseString.Result.ToString());
                        //try
                        //{
                        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
                        //    {
                        //        var entry = db.RetailerOutlets.Where(a => a.outlet_id.Equals(outletid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                        //        if (entry != null)
                        //        {

                        //            entry.kyc_status = true;
                        //            entry.UpdatedOn = DateTime.Now;
                        //            db.SaveChanges();
                        //        }
                        //    }
                        //}
                        //catch { }
                        return responseString.Result.ToString();

                    }
                    else
                    {
                        return "ERROR";
                    }
                }
                else
                {
                    return "ERROR";

                }

            }


        }
        public string doPayment(string userid, string STB, string ConOpt, string ddlPackage, string packageAmt, string txtName, string txtMobile, string customerAddress, string txtPIN)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {

                var myoptCode = "";
                var myoptName = "";

                if (ConOpt == "ATK")
                {
                    var item = db.Operator_Code.Where(aa => aa.Operator_type == "DTH-Booking" && aa.operator_Name.Contains("Airtel DTH-Booking")).SingleOrDefault();
                    myoptCode = item.new_opt_code;
                    myoptName = item.operator_Name;
                }
                else if (ConOpt == "DTK")
                {
                    var item = db.Operator_Code.Where(aa => aa.Operator_type == "DTH-Booking" && aa.operator_Name.Contains("Dish TV DTH-Booking")).SingleOrDefault();
                    myoptCode = item.new_opt_code;
                    myoptName = item.operator_Name;
                }
                else if (ConOpt == "TTK")
                {
                    var item = db.Operator_Code.Where(aa => aa.Operator_type == "DTH-Booking" && aa.operator_Name.Contains("Tata Sky DTH-Booking")).SingleOrDefault();
                    myoptCode = item.new_opt_code;
                    myoptName = item.operator_Name;
                }
                else if (ConOpt == "ATK")
                {
                    var item = db.Operator_Code.Where(aa => aa.Operator_type == "DTH-Booking" && aa.operator_Name.Contains("Videocon DTH-Booking")).SingleOrDefault();
                    myoptCode = item.new_opt_code;
                    myoptName = item.operator_Name;
                }
                var url = (db.api_name.Where(aa => aa.api_url.ToUpper().Contains("INSTANTPAY.IN"))).Single().api_url;
                url = url.Replace("ooo", ConOpt);
                url = url.Replace("iii", DateTime.Now.ToString());
                url = url.Replace("aaa", packageAmt);
                url = url.Replace("mmm", txtMobile);
                url = url.Replace("ppp", txtPIN);//optional2
                url = url.Replace("bbb", ddlPackage.Split(new char[] { '|' }).LastOrDefault().ToString()); //Optional1
                url = url.Replace("nnn", HttpUtility.UrlEncode(txtName));//optional3
                url = url.Replace("lll", HttpUtility.UrlEncode(customerAddress));//optional3
                if (HttpContext.Current.Request.IsLocal)
                {
                    url = url.Replace("token=26298f852d2c909cdd4fa2fcab8ac554", "token=ab8ac554");
                }

                System.Data.Entity.Core.Objects.ObjectParameter output = new
                 System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                var respo = db.IPAY_DTH_Booking_Retailer(userid, txtName, txtMobile, customerAddress, DateTime.Now, myoptName, myoptCode, Convert.ToDecimal(1), ddlPackage, STB, output).SingleOrDefault().msg;
                try
                {
                    var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
                    var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                    var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                    var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == userid).SingleOrDefault();
                    var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                    var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                    var admininfo = db.Admin_details.SingleOrDefault();
                    Backupinfo back = new Backupinfo();
                    var modeln = new Backupinfo.Addinfo
                    {
                        Websitename = admininfo.WebsiteUrl,
                        RetailerID = userid,
                        Email = retailerdetails.Email,
                        Mobile = retailerdetails.Mobile,
                        Details = "DTH Booking ",
                        RemainBalance = (decimal)remdetails.Remainamount,
                        Usertype = "Retailer"
                    };
                    back.info(modeln);

                    var model1 = new Backupinfo.Addinfo
                    {
                        Websitename = admininfo.WebsiteUrl,
                        RetailerID = dealerdetails.DealerId,
                        Email = dealerdetails.Email,
                        Mobile = dealerdetails.Mobile,
                        Details = "DTH Booking ",

                        RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                        Usertype = "Dealer"
                    };
                    back.info(model1);

                    var model2 = new Backupinfo.Addinfo
                    {
                        Websitename = admininfo.WebsiteUrl,
                        RetailerID = masterdetails.SSId,
                        Email = masterdetails.Email,
                        Mobile = masterdetails.Mobile,
                        Details = "DTH Booking ",
                        RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                        Usertype = "Master"
                    };
                    back.info(model2);
                }
                catch { }
                if (respo.Contains("OK"))
                {
                    HttpClient client = new HttpClient();

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = response.Content.ReadAsStringAsync();
                        JObject responseJson = JObject.Parse(responseString.Result.ToString());
                        JToken value;
                        if (responseJson.TryGetValue("ipay_errorcode", out value) && value.ToString() == "TXN")
                        {
                            //dynamic dynJson = JsonConvert.DeserializeObject(responseString.Result.ToString());
                            //try
                            //{
                            //    using (VastwebmultiEntities db = new VastwebmultiEntities())
                            //    {
                            //        var entry = db.RetailerOutlets.Where(a => a.outlet_id.Equals(outletid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                            //        if (entry != null)
                            //        {

                            //            entry.kyc_status = true;
                            //            entry.UpdatedOn = DateTime.Now;
                            //            db.SaveChanges();
                            //        }
                            //    }
                            //}
                            //catch { }
                            return responseString.Result.ToString();

                        }
                        else
                        {
                            var idno = respo.Replace("OK|", "");
                            var refundresponse = db.IPAY_Refund_DTH_Booking_Retailer(idno, userid, myoptName, "DTH", myoptCode, output).SingleOrDefault().msg;
                            try
                            {
                                var retailerdetails = db.Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
                                var dealerdetails = db.Dealer_Details.Where(aa => aa.DealerId == retailerdetails.DealerId).SingleOrDefault();
                                var masterdetails = db.Superstokist_details.Where(aa => aa.SSId == dealerdetails.SSId).SingleOrDefault();

                                var remdetails = db.Remain_reteller_balance.Where(aa => aa.RetellerId == userid).SingleOrDefault();
                                var dlmdetails = db.Remain_dealer_balance.Where(aa => aa.DealerID == retailerdetails.DealerId).SingleOrDefault();
                                var Masterdetails = db.Remain_superstokist_balance.Where(aa => aa.SuperStokistID == dealerdetails.SSId).SingleOrDefault();

                                var admininfo = db.Admin_details.SingleOrDefault();
                                Backupinfo back = new Backupinfo();
                                var modeln = new Backupinfo.Addinfo
                                {
                                    Websitename = admininfo.WebsiteUrl,
                                    RetailerID = userid,
                                    Email = retailerdetails.Email,
                                    Mobile = retailerdetails.Mobile,
                                    Details = "DTH Boking Refund",
                                    RemainBalance = (decimal)remdetails.Remainamount,
                                    Usertype = "Retailer"
                                };
                                back.info(modeln);

                                var model1 = new Backupinfo.Addinfo
                                {
                                    Websitename = admininfo.WebsiteUrl,
                                    RetailerID = dealerdetails.DealerId,
                                    Email = dealerdetails.Email,
                                    Mobile = dealerdetails.Mobile,
                                    Details = "DTH Boking Refund",

                                    RemainBalance = Convert.ToDecimal(dlmdetails.Remainamount),
                                    Usertype = "Dealer"
                                };
                                back.info(model1);

                                var model2 = new Backupinfo.Addinfo
                                {
                                    Websitename = admininfo.WebsiteUrl,
                                    RetailerID = masterdetails.SSId,
                                    Email = masterdetails.Email,
                                    Mobile = masterdetails.Mobile,
                                    Details = "DTH Boking Refund",
                                    RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                    Usertype = "Master"
                                };
                                back.info(model2);
                            }
                            catch { }
                            return "ERROR";
                        }
                    }
                    else
                    {
                        return "ERROR";

                    }
                }
                else
                {
                    return "ERROR";
                }


            }


        }

        #endregion


    }
}
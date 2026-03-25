using Vastwebmulti.Areas.WRetailer.Models;
using Vastwebmulti.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Net;
using RestSharp;

namespace Vastwebmulti.Areas.WRetailer.Controllers
{
    /// <summary>
    /// Utility class encapsulating all InstantPay API integration methods.
    /// Covers Indo-Nepal money transfer, outlet KYC registration, PAN card token purchase,
    /// gift card operations, bank list retrieval, and DTH booking payments.
    /// </summary>
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
        /// <summary>
        /// Retrieves the list of available bank branches supported by InstantPay for Indo-Nepal money transfer.
        /// </summary>
        /// <returns>A <see cref="JObject"/> containing bank branch data, or an error status object on failure.</returns>
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
        /// <summary>
        /// Validates a bank account number against the InstantPay service.
        /// </summary>
        /// <param name="accountnumber">The bank account number to validate.</param>
        /// <returns>A <see cref="JObject"/> containing the validation result, or an error status object on failure.</returns>
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
        /// <summary>
        /// Validates a remittance card account number against the InstantPay service.
        /// </summary>
        /// <param name="cardnumber">The card number to validate.</param>
        /// <returns>A <see cref="JObject"/> containing the validation result, or an error status object on failure.</returns>
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
        /// <summary>
        /// Retrieves the list of cash branch locations available for InstantPay Indo-Nepal cash transfer.
        /// </summary>
        /// <returns>A <see cref="JObject"/> containing the cash branch list, or an error status object on failure.</returns>
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
        /// <summary>
        /// Searches for Indo-Nepal money transfer transactions within a specified date range.
        /// </summary>
        /// <param name="pin">The retailer PIN for authentication.</param>
        /// <param name="partnerpin">The partner PIN for authentication.</param>
        /// <param name="fromdate">The start date for the search range.</param>
        /// <param name="todate">The end date for the search range.</param>
        /// <returns>A <see cref="JObject"/> containing matching transaction records, or an error status object on failure.</returns>
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
        /// <summary>
        /// Initiates an Indo-Nepal money transfer transaction via the InstantPay API.
        /// </summary>
        /// <param name="senderidtype">ID type of the sender (e.g., Passport, Citizenship).</param>
        /// <param name="sendername">Full name of the sender.</param>
        /// <param name="sendergender">Gender of the sender.</param>
        /// <param name="employer">Employer name of the sender.</param>
        /// <param name="senderaddress">Residential address of the sender.</param>
        /// <param name="sendermobile">Mobile number of the sender.</param>
        /// <param name="mode">Transfer mode (e.g., cash, bank, card).</param>
        /// <param name="senderidnumber">ID number of the sender.</param>
        /// <param name="receivername">Full name of the receiver.</param>
        /// <param name="receivergender">Gender of the receiver.</param>
        /// <param name="receiveraddress">Address of the receiver.</param>
        /// <param name="doctype">Document type attached for this transaction.</param>
        /// <param name="filename">Name of the uploaded document file.</param>
        /// <param name="image_url">URL or base64 content of the uploaded document.</param>
        /// <returns>A <see cref="JObject"/> with the transaction result, or an error status object on failure.</returns>
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
        /// <summary>
        /// Verifies whether a mobile number is already registered as an outlet with InstantPay.
        /// Sends an OTP to the mobile number as part of the outlet registration process.
        /// </summary>
        /// <param name="Mobile">The mobile number to verify for outlet registration.</param>
        /// <returns>A <see cref="JObject"/> with verification status and OTP details, or an error object on failure.</returns>
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
                    if (responseJson.TryGetValue("statuscode", out value) && value.ToString() == "TXN")
                    {
                        jj.Add("RESULT", "0");
                        jj.Add("ADDINFO", "OTP Sent");

                    }
                    else
                    {
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", "INVALID MOBILE");
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
        /// <summary>
        /// Registers a retailer as a new outlet with InstantPay for AEPS/KYC services.
        /// </summary>
        /// <param name="RetailerId">The unique retailer ID in the system.</param>
        /// <param name="Mobile">The mobile number for the outlet.</param>
        /// <param name="OTP">The OTP received via <see cref="VerifyOutletMobile"/> for verification.</param>
        /// <param name="email">The email address of the outlet owner.</param>
        /// <param name="store_type">The type/category of the retail store.</param>
        /// <param name="company">The company or firm name of the outlet.</param>
        /// <param name="name">The full name of the outlet owner.</param>
        /// <param name="pincode">The PIN code of the outlet location.</param>
        /// <param name="address">The full address of the outlet.</param>
        /// <returns>A <see cref="JObject"/> with outlet registration status, or an error object on failure.</returns>
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
        /// <summary>
        /// Uploads a KYC (Know Your Customer) document for the retailer outlet to InstantPay.
        /// </summary>
        /// <param name="RetailerID">The unique retailer ID whose KYC document is being uploaded.</param>
        /// <param name="docId">The document type identifier (e.g., Aadhaar, PAN, voter ID).</param>
        /// <param name="pan_no">The PAN card number of the retailer for verification.</param>
        /// <param name="base64Content">The base64-encoded content of the document image/file.</param>
        /// <param name="filename">The original filename of the uploaded document.</param>
        /// <returns>A <see cref="JObject"/> with upload status from InstantPay, or an error object on failure.</returns>
        public JObject Upload_KYC_Doc(string RetailerID, string docId, string pan_no, string base64Content, string filename)
        {
            JObject jj = new JObject();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var entry = db.Whitelabel_RetailerOutlets.Where(a => a.RetailerId.Equals(RetailerID, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
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
        /// <summary>
        /// Updates the PAN card information for a retailer in the InstantPay outlet system.
        /// </summary>
        /// <param name="RetailerId">The unique retailer ID whose PAN is being updated.</param>
        /// <param name="pan_no">The PAN card number to register or update for the retailer.</param>
        /// <returns>A <see cref="JObject"/> with the update operation result, or an error object on failure.</returns>
        public JObject UpdatePAN(string RetailerId, string pan_no)
        {
            JObject jj = new JObject();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var outlet = db.Whitelabel_RetailerOutlets.Where(a => a.RetailerId.Equals(RetailerId, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
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
        /// <summary>
        /// Retrieves the KYC document status and details for a retailer from InstantPay.
        /// </summary>
        /// <param name="RetailerId">The unique retailer ID to query KYC documents for.</param>
        /// <param name="pan_no">The PAN card number of the retailer used for identification.</param>
        /// <returns>A <see cref="JObject"/> containing KYC document status and details, or an error object on failure.</returns>
        public JObject GetKycDoc(string RetailerId, string pan_no)
        {
            JObject jj = new JObject();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var outlet = db.Whitelabel_RetailerOutlets.Where(a => a.RetailerId.Equals(RetailerId, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
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
        /// <summary>
        /// Retrieves UTI ITSL login credentials for the retailer to access the PAN card token purchase portal.
        /// </summary>
        /// <param name="RetailerId">The unique retailer ID for which to retrieve UTI login credentials.</param>
        /// <returns>A <see cref="JObject"/> containing UTI login credentials, or an error object on failure.</returns>
        public JObject GetUTILoginCredentials(string RetailerId)
        {
            JObject jj = new JObject();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var outlet = db.Whitelabel_RetailerOutlets.Where(a => a.RetailerId.Equals(RetailerId, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
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

        /// <summary>
        /// Purchases UTI ITSL PAN card tokens (digital or physical) on behalf of a retailer.
        /// </summary>
        /// <param name="RetailerId">The unique retailer ID purchasing the tokens.</param>
        /// <param name="token">The authentication token obtained from UTI for this session.</param>
        /// <param name="digitalCount">The number of digital PAN card tokens to purchase.</param>
        /// <param name="physicalCount">The number of physical PAN card tokens to purchase.</param>
        /// <param name="merchantTxnId">The merchant-side transaction ID for this purchase.</param>
        /// <param name="websitename">The website/channel name for the token purchase.</param>
        /// <returns>A <see cref="JObject"/> with the purchase result and token details, or an error object on failure.</returns>
        public JObject GetUTIToken(string RetailerId, string token, string digitalCount, string physicalCount, string merchantTxnId, string websitename)
        {
            JObject jj = new JObject();
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var outlet = db.Whitelabel_VastBazaarRetailerOutlets.Where(a => a.RetailerId.Equals(RetailerId, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
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
                        var dbresponse = db.proc_whitelabel_insert_PAN_CARD_IPAY_Request(RetailerId, "none", Convert.ToInt32(digitalCount ?? "0"), Convert.ToInt32(physicalCount), merchantTxnId, Idno, output).SingleOrDefault();
                        var isSuccess = dbresponse.msg;
                        int idnoo = (int)dbresponse.Idno;
                        if (isSuccess == "Success")
                        {
                            var tXNdATE = DateTime.Now.ToString("MM/dd/yyyy hh:MM:ss tt"); //"09/17/2018 01:40:50 PM";//
                            var RequestObject = new
                            {
                                totalcoupon_physical = physicalCount,
                                psaid = outlet.outlet_id,
                                transactionid = merchantTxnId,
                                transactiondate = tXNdATE,
                                totalcoupon_digital = digitalCount,
                                udf1 = websitename,
                                udf2 = "test2",
                                udf3 = "test3",
                                udf4 = "test4",
                                udf5 = "test5"
                            };

                            var client = new RestClient(VastbazaarBaseUrl + "api/UTI/buyToken");
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
                            if (respo.Content.ADDINFO[0].StatusCode == "000")
                            {
                                try
                                {
                                    var TXNID = respo.Content.ADDINFO[0].RequestId;
                                    int id = (int)idnoo;
                                    db.proc_PAN_CARD_Refund(idnoo.ToString(), "Success", "Pending", Convert.ToString(TXNID));
                                }
                                catch { }
                                jj.Add("RESULT", "0");
                                jj.Add("ADDINFO", respo.Content.ADDINFO[0].Message);
                            }
                            else if (respo.Content.ADDINFO[0].StatusCode == "008")
                            {
                                db.proc_PAN_CARD_Refund(idnoo.ToString(), "Failed", "Rejected", "");
                                jj.Add("RESULT", "1");
                                jj.Add("ADDINFO", "UTI server is down please try after some time.");
                            }
                            else
                            {
                                db.proc_PAN_CARD_Refund(idnoo.ToString(), "Failed", "Rejected", "");
                                jj.Add("RESULT", "1");
                                jj.Add("ADDINFO", respo.Content.ADDINFO[0].Message);
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

        //public JObject GetUTIToken(string RetailerId, string token, string digitalCount, string physicalCount)
        //{
        //    JObject jj = new JObject();
        //    try
        //    {
        //        using (VastwebmultiEntities db = new VastwebmultiEntities())
        //        {
        //            var outlet = db.Whitelabel_VastBazaarRetailerOutlets.Where(a => a.RetailerId.Equals(RetailerId, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
        //            if (outlet == null)
        //            {
        //                jj.Add("RESULT", "1");
        //                jj.Add("ADDINFO", "Outlet Id Not Registered.");
        //                return jj;
        //            }
        //            else
        //            {
        //                ////  charge Retailer for Pan Token Request
        //                System.Data.Entity.Core.Objects.ObjectParameter output = new
        //       System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
        //                System.Data.Entity.Core.Objects.ObjectParameter Idno = new
        //         System.Data.Entity.Core.Objects.ObjectParameter("Idno", typeof(string));
        //                //var dealerid = db.Retailer_Details.Where(p => p.RetailerId == RetailerId).Single().DealerId;
        //                //var slabname = db.Retailer_Details.Where(p => p.RetailerId == RetailerId).Single().slab_name;
        //                //var retailertokanvalue = db.Slab_PanCard.SingleOrDefault(p => p.createdby == dealerid && p.clienttype == "Retailer" && p.slabname == slabname).tokenval;
        //                //var dbresponse = db.proc_whitelabel_insert_PAN_CARD_IPAY_Request(RetailerId, "none", Idno, output).SingleOrDefault();
        //                var dbresponse = db.proc_whitelabel_insert_PAN_CARD_IPAY_Request(RetailerId, "none", Convert.ToInt32(digitalCount ?? "0"), Convert.ToInt32(physicalCount), Idno, output).SingleOrDefault();
        //                var isSuccess = dbresponse.msg;
        //                int idnoo = (int)dbresponse.Idno;
        //                if (isSuccess == "Success")
        //                {
        //                    var tXNdATE = DateTime.Now.ToString("MM/dd/yyyy hh:MM:ss tt"); //"09/17/2018 01:40:50 PM";//
        //                    var RequestObject = new
        //                    {
        //                        totalcoupon_physical = physicalCount,
        //                        psaid = outlet.outlet_id,
        //                        transactionid = idnoo.ToString(),
        //                        transactiondate = tXNdATE,
        //                        totalcoupon_digital = digitalCount,
        //                        udf1 = "test1",
        //                        udf2 = "test2",
        //                        udf3 = "test3",
        //                        udf4 = "test4",
        //                        udf5 = "test5"
        //                    };

        //                    var client = new RestClient(VastbazaarBaseUrl + "api/UTI/buyToken");
        //                    var request = new RestRequest(Method.POST);
        //                    request.RequestFormat = DataFormat.Json;
        //                    request.AddHeader("authorization", "bearer " + token + "");
        //                    request.AddHeader("content-type", "application/json");
        //                    //request.AddParameter("application/json", "{\r\n\"securityKey\":\"Ma******3*****\",\r\n\"createdby\":\"M******1\",\r\n\"totalcoupon_physical\":\"1\",\r\n\"psaid\":\"TEST7960517\",\r\n\"transactionid\":\"Maha321\",\r\n\"transactiondate\":\"05/25/1991 11:42:39 PM\",\r\n\"totalcoupon_digital\":\"2\",\r\n\"udf1\":\"test1\",\r\n\"udf2\":\"test2\",\r\n\"udf3\":\"test3\",\r\n\"udf4\":\"test4\",\r\n\"udf5\":\"test5\"\r\n}", ParameterType.RequestBody);
        //                    request.AddBody(RequestObject);
        //                    IRestResponse response = client.Execute(request);
        //                    if (response.StatusCode == HttpStatusCode.BadRequest)
        //                    {
        //                        var xx = UpdateToken();
        //                        db.proc_PAN_CARD_Refund(idnoo.ToString(), "Failed", "Rejected", "");
        //                        jj.Add("RESULT", "1");
        //                        jj.Add("ADDINFO", "Unknown Error");
        //                        return jj;
        //                    }
        //                    //IRestResponse response = client.Execute(request);
        //                    dynamic respo = JsonConvert.DeserializeObject(response.Content);
        //                    if (string.IsNullOrWhiteSpace(response.Content))
        //                    {
        //                        jj.Add("RESULT", "1");
        //                        jj.Add("ADDINFO", "Unknown Error");
        //                        return jj;
        //                    }
        //                    if (respo.Content.ADDINFO[0].StatusCode == "000")
        //                    {
        //                        try
        //                        {
        //                            var TXNID = respo.Content.ADDINFO[0].RequestId;
        //                            int id = (int)idnoo;
        //                            db.proc_PAN_CARD_Refund(idnoo.ToString(), "Success", "Pending", Convert.ToString(TXNID));
        //                        }
        //                        catch { }
        //                        jj.Add("RESULT", "0");
        //                        jj.Add("ADDINFO", respo.Content.ADDINFO[0].Message);
        //                    }
        //                    else
        //                    {
        //                        db.proc_PAN_CARD_Refund(idnoo.ToString(), "Failed", "Rejected", "");
        //                        jj.Add("RESULT", "1");
        //                        jj.Add("ADDINFO", respo.Content.ADDINFO[0].Message);
        //                    }
        //                    return jj;
        //                }
        //                else
        //                {
        //                    jj.Add("RESULT", "1");
        //                    jj.Add("ADDINFO", isSuccess);
        //                    return jj;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        jj.Add("RESULT", "1");
        //        jj.Add("ADDINFO", ex.Message);
        //        return jj;
        //    }
        //}


        /// <summary>
        /// Checks the status of a UTI ITSL PAN card token purchase transaction.
        /// </summary>
        /// <param name="id">The internal transaction ID to check status for.</param>
        /// <param name="token">The UTI authentication token for the current session.</param>
        /// <returns>A <see cref="JObject"/> containing the token purchase transaction status, or an error object on failure.</returns>
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
                        outletid = db.VastBazaarRetailerOutlets.Where(a => a.RetailerId.Equals(retailerid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault().outlet_id;

                    }
                    else
                    {
                        outletid = db.Whitelabel_VastBazaarRetailerOutlets.Where(a => a.RetailerId.Equals(retailerid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault().outlet_id;

                    }
                    var RequestObject = new
                    {
                        requestid = merchantTxnId,
                        psaid = outletid
                    };

                    var client = new RestClient(VastbazaarBaseUrl + "api/UTI/tokenStatus");
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
                    if (respo.Content.ADDINFO[0].StatusCode == "000")
                    {
                        #region RefundIfTokenFaild
                        try
                        {
                            if (respo.Content.ADDINFO[0].status == "Rejected")
                            {

                                db.proc_PAN_CARD_Refund(id.ToString(), "Failed", "Rejected", Convert.ToString(respo.Content.ADDINFO[0].Message));

                            }
                            else if (respo.Content.ADDINFO[0].status == "Approved")
                            {

                                var TXNID = respo.Content.ADDINFO[0].RequestId;

                                db.proc_PAN_CARD_Refund(id, "Success", "Approved", Convert.ToString(TXNID));

                            }
                        }
                        catch
                        {

                        }
                        #endregion
                        jj.Add("RESULT", "0");
                        jj.Add("ADDINFO", respo.Content.ADDINFO[0].status);
                    }
                    else if (respo.Content.ADDINFO[0].Message == "Invalid Merchant Id")
                    {
                        //#region RefundIfTokenFaild
                        //try
                        //{
                        //        using (VastwebmultiEntities db = new VastwebmultiEntities())
                        //        {
                        //            db.proc_PAN_CARD_Refund(id.ToString(), "Failed", Convert.ToString(respo.Content.ADDINFO[0].Message));
                        //        }
                        //}
                        //catch (Exception ex)
                        //{

                        //}
                        //#endregion
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", respo.Content.ADDINFO[0].Message);
                    }
                    else
                    {
                        jj.Add("RESULT", "1");
                        jj.Add("ADDINFO", respo.Content.ADDINFO[0].Message);
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


      
        /// <summary>
        /// Marks a PAN card token transaction as manually failed with an admin-provided remark.
        /// Updates the transaction status in the database.
        /// </summary>
        /// <param name="id">The transaction ID to mark as failed.</param>
        /// <param name="remark">The reason or remark describing why the transaction is being marked as failed.</param>
        public void PanManualFailed(string id, string remark)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    db.proc_PAN_CARD_Refund_Manual(id.ToString(), "Manual Failed", remark);
                }

            }
            catch
            {

            }

        }
        #endregion

        //#region PANCARD_API
        //public JObject GetUTILoginCredentials(string RetailerId)
        //{
        //    JObject jj = new JObject();
        //    try
        //    {
        //        using (VastwebmultiEntities db = new VastwebmultiEntities())
        //        {
        //            var outlet = db.Whitelabel_RetailerOutlets.Where(a => a.RetailerId.Equals(RetailerId, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
        //            if (outlet == null)
        //            {
        //                jj.Add("RESULT", "1");
        //                jj.Add("ADDINFO", "Outlet Id Not Registered.");
        //                return jj;
        //            }
        //            else
        //            {
        //                ////////////  Request ///////////
        //                InstantPayParamsPOCO o = new InstantPayParamsPOCO();
        //                o.outletid = outlet.outlet_id;


        //                InstantPayReq req = new InstantPayReq();
        //                req.token = token;
        //                if (HttpContext.Current.Request.IsLocal)
        //                {
        //                    token = "60cd8f584d92c97468bfc505deed48eb";
        //                }
        //                req.request = o;
        //                ////////////////////////////////
        //                client.BaseAddress = new Uri("https://www.instantpay.in/ws/utipan/");
        //                client.DefaultRequestHeaders.Accept.Clear();
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                HttpResponseMessage response = client.PostAsJsonAsync("psa_registration", req).Result;
        //                response.EnsureSuccessStatusCode();
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var responseString = response.Content.ReadAsStringAsync();
        //                    JObject responseJson = JObject.Parse(responseString.Result.ToString());
        //                    JToken value;
        //                    if (responseJson.TryGetValue("statuscode", out value) && value.ToString() == "TXN")
        //                    {
        //                        dynamic dynJson = JsonConvert.DeserializeObject(responseString.Result.ToString());
        //                        //try
        //                        //{
        //                        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
        //                        //    {
        //                        //        var entry = db.RetailerOutlets.Where(a => a.outlet_id.Equals(outletid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
        //                        //        if (entry != null)
        //                        //        {

        //                        //            entry.kyc_status = true;
        //                        //            entry.UpdatedOn = DateTime.Now;
        //                        //            db.SaveChanges();
        //                        //        }
        //                        //    }
        //                        //}
        //                        //catch { }
        //                        jj.Add("RESULT", "0");
        //                        jj.Add("ADDINFO", dynJson.data.psa_uti_login_id);

        //                    }
        //                    else
        //                    {
        //                        responseJson.TryGetValue("status", out value);
        //                        jj.Add("RESULT", "1");
        //                        jj.Add("ADDINFO", value.ToString());
        //                    }
        //                }
        //                else
        //                {
        //                    jj.Add("RESULT", "1");
        //                    jj.Add("ADDINFO", "Somthing went wrong");

        //                }
        //                return jj;
        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        jj.Add("RESULT", "1");
        //        jj.Add("ADDINFO", ex.Message);
        //        return jj;
        //    }
        //}
        //public JObject GetUTIToken(string RetailerId, string token)
        //{
        //    JObject jj = new JObject();
        //    try
        //    {
        //        using (VastwebmultiEntities db = new VastwebmultiEntities())
        //        {
        //            var outlet = db.Whitelabel_VastBazaarRetailerOutlets.Where(a => a.RetailerId.Equals(RetailerId, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
        //            if (outlet == null)
        //            {
        //                jj.Add("RESULT", "1");
        //                jj.Add("ADDINFO", "Outlet Id Not Registered.");
        //                return jj;
        //            }
        //            else
        //            {
        //                ////  charge Retailer for Pan Token Request
        //                System.Data.Entity.Core.Objects.ObjectParameter output = new
        //       System.Data.Entity.Core.Objects.ObjectParameter("output", typeof(string));
        //                System.Data.Entity.Core.Objects.ObjectParameter Idno = new
        //         System.Data.Entity.Core.Objects.ObjectParameter("Idno", typeof(string));
        //                //var dealerid = db.Retailer_Details.Where(p => p.RetailerId == RetailerId).Single().DealerId;
        //                //var slabname = db.Retailer_Details.Where(p => p.RetailerId == RetailerId).Single().slab_name;
        //                //var retailertokanvalue = db.Slab_PanCard.SingleOrDefault(p => p.createdby == dealerid && p.clienttype == "Retailer" && p.slabname == slabname).tokenval;
        //                var dbresponse = db.proc_whitelabel_insert_PAN_CARD_IPAY_Request(RetailerId, "none", Idno, output).SingleOrDefault();
        //                var isSuccess = dbresponse.msg;
        //                int idnoo = (int)dbresponse.Idno;
        //                if (isSuccess == "Success")
        //                {
        //                    var tXNdATE = DateTime.Now.ToString("MM/dd/yyyy hh:MM:ss tt"); //"09/17/2018 01:40:50 PM";//
        //                    var RequestObject = new
        //                    {
        //                        totalcoupon_physical = "1",
        //                        psaid = outlet.outlet_id,
        //                        transactionid = idnoo.ToString(),
        //                        transactiondate = tXNdATE,
        //                        totalcoupon_digital = "1",
        //                        udf1 = "NA",
        //                        udf2 = "NA",
        //                        udf3 = "NA",
        //                        udf4 = "NA",
        //                        udf5 = "NA"
        //                    };

        //                    var client = new RestClient(VastbazaarBaseUrl + "api/UTI/buyToken");
        //                    var request = new RestRequest(Method.POST);
        //                    request.RequestFormat = DataFormat.Json;
        //                    request.AddHeader("authorization", "bearer " + token + "");
        //                    request.AddHeader("content-type", "application/json");
        //                    //request.AddParameter("application/json", "{\r\n\"securityKey\":\"Ma******3*****\",\r\n\"createdby\":\"M******1\",\r\n\"totalcoupon_physical\":\"1\",\r\n\"psaid\":\"TEST7960517\",\r\n\"transactionid\":\"Maha321\",\r\n\"transactiondate\":\"05/25/1991 11:42:39 PM\",\r\n\"totalcoupon_digital\":\"2\",\r\n\"udf1\":\"test1\",\r\n\"udf2\":\"test2\",\r\n\"udf3\":\"test3\",\r\n\"udf4\":\"test4\",\r\n\"udf5\":\"test5\"\r\n}", ParameterType.RequestBody);
        //                    request.AddBody(RequestObject);
        //                    IRestResponse response = client.Execute(request);
        //                    if (response.StatusCode == HttpStatusCode.BadRequest)
        //                    {
        //                        var xx = UpdateToken();
        //                        db.proc_PAN_CARD_Refund(idnoo.ToString(), "Failed", "");
        //                        jj.Add("RESULT", "1");
        //                        jj.Add("ADDINFO", "Unknown Error");
        //                        return jj;
        //                    }
        //                    //IRestResponse response = client.Execute(request);
        //                    dynamic respo = JsonConvert.DeserializeObject(response.Content);
        //                    if (string.IsNullOrWhiteSpace(response.Content))
        //                    {
        //                        jj.Add("RESULT", "1");
        //                        jj.Add("ADDINFO", "Unknown Error");
        //                        return jj;
        //                    }
        //                    if (respo.Content.ADDINFO[0].StatusCode == "000")
        //                    {
        //                        try
        //                        {
        //                            var TXNID = respo.Content.ADDINFO[0].RequestId;
        //                            int id = (int)idnoo;
        //                            db.proc_PAN_CARD_Refund(idnoo.ToString(), "Success", Convert.ToString(TXNID));
        //                        }
        //                        catch { }
        //                        jj.Add("RESULT", "0");
        //                        jj.Add("ADDINFO", respo.Content.ADDINFO[0].Message);
        //                    }
        //                    else
        //                    {
        //                        db.proc_PAN_CARD_Refund(idnoo.ToString(), "Failed", "");
        //                        jj.Add("RESULT", "1");
        //                        jj.Add("ADDINFO", respo.Content.ADDINFO[0].Message);
        //                    }
        //                    return jj;
        //                }
        //                else
        //                {
        //                    jj.Add("RESULT", "1");
        //                    jj.Add("ADDINFO", isSuccess);
        //                    return jj;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        jj.Add("RESULT", "1");
        //        jj.Add("ADDINFO", ex.Message);
        //        return jj;
        //    }
        //}
        //public JObject getUtiTokenStatus(string id, string token)
        //{
        //    JObject jj = new JObject();
        //    try
        //    {
        //        var RequestObject = new
        //        {
        //            requestid = id,
        //        };

        //        var client = new RestClient(VastbazaarBaseUrl + "api/UTI/tokenStatus");
        //        var request = new RestRequest(Method.POST);
        //        request.RequestFormat = DataFormat.Json;
        //        request.AddHeader("authorization", "bearer " + token + "");
        //        request.AddHeader("content-type", "application/json");
        //        //request.AddParameter("application/json", "{\r\n\"securityKey\":\"Ma******3*****\",\r\n\"createdby\":\"M******1\",\r\n\"totalcoupon_physical\":\"1\",\r\n\"psaid\":\"TEST7960517\",\r\n\"transactionid\":\"Maha321\",\r\n\"transactiondate\":\"05/25/1991 11:42:39 PM\",\r\n\"totalcoupon_digital\":\"2\",\r\n\"udf1\":\"test1\",\r\n\"udf2\":\"test2\",\r\n\"udf3\":\"test3\",\r\n\"udf4\":\"test4\",\r\n\"udf5\":\"test5\"\r\n}", ParameterType.RequestBody);
        //        request.AddBody(RequestObject);
        //        IRestResponse response = client.Execute(request);
        //        if (response.StatusCode == HttpStatusCode.BadRequest)
        //        {
        //            var xx = UpdateToken();
        //            jj.Add("RESULT", "1");
        //            jj.Add("ADDINFO", "Unknown Error");
        //            return jj;
        //        }
        //        //IRestResponse response = client.Execute(request);
        //        dynamic respo = JsonConvert.DeserializeObject(response.Content);
        //        if (string.IsNullOrWhiteSpace(response.Content))
        //        {
        //            jj.Add("RESULT", "1");
        //            jj.Add("ADDINFO", "Unknown Error");
        //            return jj;
        //        }
        //        if (respo.Content.ADDINFO[0].StatusCode == "000")
        //        {
        //            #region RefundIfTokenFaild
        //            try
        //            {
        //                if (respo.Content.ADDINFO[0].status == "Rejected")
        //                {
        //                    using (VastwebmultiEntities db = new VastwebmultiEntities())
        //                    {
        //                        db.proc_PAN_CARD_Refund(id.ToString(), "Failed", Convert.ToString(respo.Content.ADDINFO[0].Message));
        //                    }
        //                }
        //                else if (respo.Content.ADDINFO[0].status == "Approved")
        //                {
        //                    using (VastwebmultiEntities db = new VastwebmultiEntities())
        //                    {
        //                        var TXNID = respo.Content.ADDINFO[0].RequestId;

        //                        db.proc_PAN_CARD_Refund(id, "Success", Convert.ToString(TXNID));
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //            #endregion
        //            jj.Add("RESULT", "0");
        //            jj.Add("ADDINFO", respo.Content.ADDINFO[0].status);
        //        }
        //        else if (respo.Content.ADDINFO[0].Message == "Invalid Merchant Id")
        //        {
        //            //#region RefundIfTokenFaild
        //            //try
        //            //{
        //            //        using (VastwebmultiEntities db = new VastwebmultiEntities())
        //            //        {
        //            //            db.proc_PAN_CARD_Refund(id.ToString(), "Failed", Convert.ToString(respo.Content.ADDINFO[0].Message));
        //            //        }
        //            //}
        //            //catch (Exception ex)
        //            //{

        //            //}
        //            //#endregion
        //            jj.Add("RESULT", "1");
        //            jj.Add("ADDINFO", respo.Content.ADDINFO[0].Message);
        //        }
        //        else
        //        {
        //            jj.Add("RESULT", "1");
        //            jj.Add("ADDINFO", respo.Content.ADDINFO[0].Message);
        //        }
        //        return jj;

        //    }
        //    catch (Exception ex)
        //    {
        //        jj.Add("RESULT", "1");
        //        jj.Add("ADDINFO", ex.Message);
        //        return jj;
        //    }
        //}
        //public void PanManualFailed(string id, string remark)
        //{
        //    try
        //    {
        //        using (VastwebmultiEntities db = new VastwebmultiEntities())
        //        {
        //            db.proc_PAN_CARD_Refund_Manual(id.ToString(), "Manual Failed", remark);
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //}
        //#endregion

        /// <summary>
        /// Refreshes and updates the InstantPay API authentication token stored in the database.
        /// </summary>
        /// <returns>The new access token string, or an error message if the refresh fails.</returns>
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
        /// <summary>
        /// Retrieves the list of banks supported for IMPS/NEFT money transfer for the given account number.
        /// </summary>
        /// <param name="AccNo">The account number to look up supported banks for.</param>
        /// <returns>A JSON string containing the bank list, or "ERROR" if the request fails.</returns>
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
        /// <summary>
        /// Retrieves the list of available gift cards from InstantPay for the specified category flag.
        /// </summary>
        /// <param name="flag">A category or filter flag to narrow down the gift card listing.</param>
        /// <returns>A JSON string containing the gift card list, or an error indicator if the request fails.</returns>
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
        /// <summary>
        /// Retrieves detailed product information for a specific gift card product by its key.
        /// </summary>
        /// <param name="spKey">The unique product key identifying the gift card product.</param>
        /// <returns>A JSON string containing the product details, or an error indicator if the request fails.</returns>
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
        /// <summary>
        /// Retrieves DTH operator package details from InstantPay for a specified operator code.
        /// </summary>
        /// <param name="flag">The operator code or flag identifying the DTH operator.</param>
        /// <returns>A JSON string with operator/package details, or "ERROR" if the request fails.</returns>
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
                    spkey = flag , //passcode here.
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
        /// <summary>
        /// Processes a DTH (Direct-to-Home) recharge or new connection payment via InstantPay.
        /// </summary>
        /// <param name="userid">The retailer user ID initiating the payment.</param>
        /// <param name="STB">The Set-Top Box (STB) number or subscriber ID for the DTH connection.</param>
        /// <param name="ConOpt">Connection option indicating new connection or recharge.</param>
        /// <param name="ddlPackage">The selected DTH package or plan code.</param>
        /// <param name="packageAmt">The amount to be paid for the selected package.</param>
        /// <param name="txtName">The customer name for the DTH connection.</param>
        /// <param name="txtMobile">The customer mobile number for the DTH connection.</param>
        /// <param name="customerAddress">The customer address for new DTH connection setup.</param>
        /// <param name="txtPIN">The retailer transaction PIN for authorization.</param>
        /// <returns>A JSON string containing the payment result, or "ERROR" if the transaction fails.</returns>
        public string doPayment(string userid,string STB, string ConOpt, string ddlPackage, string packageAmt, string txtName, string txtMobile, string customerAddress,string txtPIN)
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
                if(HttpContext.Current.Request.IsLocal)
                {
                    url = url.Replace("token=26298f852d2c909cdd4fa2fcab8ac554", "token=ab8ac554");
                }

                System.Data.Entity.Core.Objects.ObjectParameter output = new
                 System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
                var respo = db.IPAY_DTH_Booking_Retailer(userid, txtName, txtMobile, customerAddress, DateTime.Now, myoptName, myoptCode, Convert.ToDecimal(1), ddlPackage, STB, output).SingleOrDefault().msg;
                if(respo.Contains("OK"))
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
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
    /// <summary>
    /// Utility class that wraps the InstantPay API, providing methods for Indo-Nepal money transfer,
    /// outlet registration, KYC document upload, PAN card token purchase, gift card lookup, and DTH booking.
    /// </summary>
    public class InstantPayComnUtil
    {
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        // string VastbazaarBaseUrl = "http://localhost:65209/";
        HttpClient client = new HttpClient();
        string token = string.Empty;
        /// <summary>
        /// Initializes a new instance of <see cref="InstantPayComnUtil"/>, loading the InstantPay API token from the database.
        /// </summary>
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
        /// Retrieves the list of bank branches available for Indo-Nepal money transfers from the InstantPay API.
        /// </summary>
        /// <returns>A <see cref="JObject"/> containing the branch list or an error status.</returns>
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
        /// Validates a bank account number against the InstantPay API for Indo-Nepal transfers.
        /// </summary>
        /// <param name="accountnumber">The bank account number to validate.</param>
        /// <returns>A <see cref="JObject"/> with the validation result or an error status.</returns>
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
        /// Validates a remittance card number against the InstantPay API.
        /// </summary>
        /// <param name="cardnumber">The card number to validate.</param>
        /// <returns>A <see cref="JObject"/> with the validation result or an error status.</returns>
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
        /// Retrieves the list of cash payout branches from the InstantPay API for Indo-Nepal transfers.
        /// </summary>
        /// <returns>A <see cref="JObject"/> containing cash branch data or an error status.</returns>
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
        /// Searches for Indo-Nepal money transfer transactions within a date range using sender and partner PINs.
        /// </summary>
        /// <param name="pin">The sender's PIN.</param>
        /// <param name="partnerpin">The partner agent PIN.</param>
        /// <param name="fromdate">The start date for the transaction search.</param>
        /// <param name="todate">The end date for the transaction search.</param>
        /// <returns>A <see cref="JObject"/> containing matching transaction records or an error status.</returns>
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
        /// Initiates an Indo-Nepal money transfer transaction by submitting sender and receiver details to the InstantPay API.
        /// </summary>
        /// <param name="senderidtype">The type of identity document provided by the sender.</param>
        /// <param name="sendername">The full name of the sender.</param>
        /// <param name="sendergender">The gender of the sender.</param>
        /// <param name="employer">The sender's employer name.</param>
        /// <param name="senderaddress">The sender's residential address.</param>
        /// <param name="sendermobile">The sender's mobile phone number.</param>
        /// <param name="mode">The transfer mode (1=Cash, 2=Account, 3=Card).</param>
        /// <param name="senderidnumber">The sender's identity document number.</param>
        /// <param name="receivername">The full name of the receiver.</param>
        /// <param name="receivergender">The gender of the receiver.</param>
        /// <param name="receiveraddress">The receiver's address.</param>
        /// <param name="doctype">The document type for verification.</param>
        /// <param name="filename">The name of the uploaded document file.</param>
        /// <param name="image_url">The URL of the sender's identity image.</param>
        /// <returns>A <see cref="JObject"/> containing the transaction result or an error status.</returns>
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
        /// Sends an OTP to the specified mobile number to initiate outlet verification with InstantPay.
        /// </summary>
        /// <param name="Mobile">The retailer's mobile number to send the OTP to.</param>
        /// <returns>A <see cref="JObject"/> with RESULT "0" on success or "1" on failure, plus an ADDINFO message.</returns>
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
        /// <summary>
        /// Registers a retailer outlet on the InstantPay platform using the provided OTP and store details, saving the outlet ID on success.
        /// </summary>
        /// <param name="RetailerId">The unique retailer identifier.</param>
        /// <param name="Mobile">The retailer's mobile number.</param>
        /// <param name="OTP">The one-time password received during mobile verification.</param>
        /// <param name="email">The retailer's email address.</param>
        /// <param name="store_type">The category/type of the store.</param>
        /// <param name="company">The firm/company name.</param>
        /// <param name="name">The owner's name.</param>
        /// <param name="pincode">The store's postal/PIN code.</param>
        /// <param name="address">The store's address.</param>
        /// <returns>A <see cref="JObject"/> with RESULT "0" and outlet ID on success, or "1" on failure.</returns>
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
        /// Uploads a KYC document in base64 format to the InstantPay outlet API for the specified retailer outlet.
        /// </summary>
        /// <param name="RetailerID">The unique retailer identifier.</param>
        /// <param name="docId">The document type identifier required by InstantPay.</param>
        /// <param name="pan_no">The retailer's PAN card number for verification.</param>
        /// <param name="base64Content">The document file content encoded as a base64 string.</param>
        /// <param name="filename">The original filename of the uploaded document.</param>
        /// <returns>A <see cref="JObject"/> with RESULT "0" on success or "1" on failure with an ADDINFO message.</returns>
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
        /// <summary>
        /// Updates the PAN card number for the retailer's registered outlet and marks the PAN as confirmed on success.
        /// </summary>
        /// <param name="RetailerId">The unique retailer identifier whose outlet PAN should be updated.</param>
        /// <param name="pan_no">The new PAN card number to associate with the outlet.</param>
        /// <returns>A <see cref="JObject"/> with RESULT "0" on success or "1" on failure with an ADDINFO message.</returns>
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
        /// <summary>
        /// Retrieves the list of required KYC documents and their current approval status from the InstantPay outlet API.
        /// </summary>
        /// <param name="RetailerId">The unique retailer identifier.</param>
        /// <param name="pan_no">The retailer's PAN card number for verification.</param>
        /// <returns>A <see cref="JObject"/> with document data on success or an error message on failure.</returns>
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
        /// <summary>
        /// Registers the retailer outlet for UTI PAN card services and retrieves the UTI PSA login credentials.
        /// </summary>
        /// <param name="RetailerId">The unique retailer identifier whose outlet should be registered for UTI PAN services.</param>
        /// <returns>A <see cref="JObject"/> with the UTI login ID on success or an error message on failure.</returns>
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
        /// <summary>
        /// Purchases UTI PAN card tokens (digital and/or physical) for the specified retailer outlet, charging the retailer balance and recording the transaction.
        /// </summary>
        /// <param name="RetailerId">The unique retailer identifier purchasing the tokens.</param>
        /// <param name="token">The Vastbazaar bearer token for API authentication.</param>
        /// <param name="digitalCount">The number of digital PAN tokens to purchase.</param>
        /// <param name="physicalCount">The number of physical PAN tokens to purchase.</param>
        /// <param name="merchantTxnId">The unique merchant transaction identifier for this purchase.</param>
        /// <param name="websitename">The website URL for backup records.</param>
        /// <param name="ip">The client IP address for audit logging.</param>
        /// <param name="latss">The client latitude for geo-logging.</param>
        /// <param name="longloc">The client longitude for geo-logging.</param>
        /// <param name="city">The client city for geo-logging.</param>
        /// <param name="address">The client address for geo-logging.</param>
        /// <returns>A <see cref="JObject"/> with RESULT "0" and token info on success or an error message on failure.</returns>
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
        /// <summary>
        /// Checks the status of a UTI PAN token purchase request and updates the database record accordingly, issuing a refund if the request failed.
        /// </summary>
        /// <param name="id">The internal database ID of the PAN card request record.</param>
        /// <param name="token">The Vastbazaar bearer token for API authentication.</param>
        /// <returns>A <see cref="JObject"/> with RESULT "0" on success, "1" on failure, or "2" for a pending status.</returns>
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
        /// <summary>
        /// Manually marks a PAN card token request as failed, triggers a refund, and records balance backup info for retailer, dealer and master.
        /// </summary>
        /// <param name="id">The internal database ID of the PAN card request to fail.</param>
        /// <param name="remark">The reason or remark for the manual failure.</param>
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
        /// <summary>
        /// Refreshes the stored Vastbazaar API bearer token by requesting a new one and updating the database record.
        /// </summary>
        /// <returns>The new bearer token string, or null if the request failed.</returns>
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
        /// Retrieves bank details for the given account number from the InstantPay DMI API.
        /// </summary>
        /// <param name="AccNo">The bank account number to look up.</param>
        /// <returns>A JSON string with bank details on success, or "ERROR" on failure.</returns>
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
        /// Retrieves the list of available gift card / brand voucher products from the InstantPay API.
        /// </summary>
        /// <param name="flag">The product type filter; pass "ALL" to retrieve all available gift cards.</param>
        /// <returns>A JSON string with the gift card list on success, or "ERROR" on failure.</returns>
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
        /// Retrieves the detail of a specific gift card product using its service provider key from the InstantPay API.
        /// </summary>
        /// <param name="spKey">The service provider key identifying the specific gift card product.</param>
        /// <returns>A JSON string with the product detail on success, or "ERROR" on failure.</returns>
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
        /// Retrieves the available DTH operator package details from the InstantPay API using the operator service provider code.
        /// </summary>
        /// <param name="flag">The operator service provider code (spkey) identifying the DTH operator.</param>
        /// <returns>A JSON string with operator package details on success, or "ERROR" on failure.</returns>
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
        /// <summary>
        /// Processes a DTH new connection booking payment via the InstantPay API, charging the retailer balance and recording backup info.
        /// </summary>
        /// <param name="userid">The unique identifier of the retailer performing the booking.</param>
        /// <param name="STB">The set-top box serial number for the new DTH connection.</param>
        /// <param name="ConOpt">The DTH operator code (e.g., ATK for Airtel, DTK for Dish TV, TTK for Tata Sky).</param>
        /// <param name="ddlPackage">The selected DTH package identifier.</param>
        /// <param name="packageAmt">The amount for the selected package.</param>
        /// <param name="txtName">The customer's name.</param>
        /// <param name="txtMobile">The customer's mobile number.</param>
        /// <param name="customerAddress">The customer's installation address.</param>
        /// <param name="txtPIN">The customer's PIN code.</param>
        /// <returns>A JSON string with the booking confirmation on success, or "ERROR" on failure.</returns>
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
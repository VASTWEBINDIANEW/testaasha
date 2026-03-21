using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER
{
    public class ExternalMoneyApi
    {
        Dictionary<string, string> MoneyUrls =
         new Dictionary<string, string>();

        public String customer_verify(string senderMobile)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebClient client = new WebClient();
                //Stream data = client.OpenRead(MoneyUrls["GetBeneficiarList"].Replace("sendernumber", senderMobile));
                string url = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().SenderVerification;
                Stream data = client.OpenRead(url.Replace("sno", senderMobile));

                StreamReader reader = new StreamReader(data);

                string result = reader.ReadToEnd();


                return result;
            }
        }

        public String add_customer(string number, string name)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebClient client = new WebClient();
                string url = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().AddNewSenderNumber;
                Stream data = client.OpenRead(url.Replace("sno", number).Replace("senderName", name));
                StreamReader reader = new StreamReader(data);

                string result = reader.ReadToEnd();

                return result;
            }
        }
        public String otp_verify(string number, string otp, string requestid)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebClient client = new WebClient();
                string url = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().OTPVerify;
                Stream data = client.OpenRead(url.Replace("sno", number).Replace("ocode", otp).Replace("RequestNo", requestid));
                StreamReader reader = new StreamReader(data);

                string result = reader.ReadToEnd();

                return result;
            }
        }
        public String resend_otp(string number, string requestno)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebClient client = new WebClient();
                string url = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().ResendOTP;
                Stream data = client.OpenRead(url.Replace("sno", number).Replace("RequestNo", requestno));
                StreamReader reader = new StreamReader(data);

                string result = reader.ReadToEnd();

                return result;
            }
        }
        public String add_benificy(string benAccount, string senderno, string NUMBER, string benIFSC, string name)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebClient client = new WebClient();
                string baseurl = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().AddBeneficiary;
                string url = baseurl.Replace("sno", senderno).Replace("actno", benAccount).Replace("rno", NUMBER).Replace("icode", benIFSC).Replace("recname", name);
                Stream data = client.OpenRead(url);
                StreamReader reader = new StreamReader(data);
                string result = reader.ReadToEnd();

                return result;
            }
        }

        public String delete_benificy(string mobile, string ifsc, string code)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebClient client = new WebClient();
                string baseurl = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().DeleteBeneficiary;
                string ulr = baseurl.Replace("sno", mobile).Replace("icode", ifsc).Replace("bcode", code);
                Stream data = client.OpenRead(ulr);
                StreamReader reader = new StreamReader(data);
                string result = reader.ReadToEnd();

                return result;
            }
        }

        public String updatepancard(string senderno, string fname, string lname, string pancard)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebClient client = new WebClient();
                string baseurl = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().updatepancard;
                string url = baseurl.Replace("sno", senderno).Replace("fnm", fname).Replace("lnm", lname).Replace("pcard", pancard);
                Stream data = client.OpenRead(url);
                StreamReader reader = new StreamReader(data);
                string result = reader.ReadToEnd();

                return result;
            }
        }


        public String Verify_benificy(string mobile, string ifsc, string code, string idn, string apiid, string Password, string tokennumber)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebClient client = new WebClient();
                string baseurl = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().VerifyBeneficiary;
                string url = baseurl.Replace("ID", apiid).Replace("pwd", Password).Replace("tkn", tokennumber).Replace("sno", mobile).Replace("icode", ifsc).Replace("bcode", code);
                Stream data = client.OpenRead(url);
                StreamReader reader = new StreamReader(data);
                string result = reader.ReadToEnd();
                try
                {
                    IMPS_transtion_detsils objCourse = (from p in db.IMPS_transtion_detsils
                                                         where p.trans_id == idn
                                                         select p).SingleOrDefault();
                    objCourse.Response_output = result;
                    db.SaveChanges();
                }
                catch
                {

                }

                return result;
            }
        }
        public String fundtransfer(string apiid, string Password, string tokennumber, string NUMBER, string benIFSC, string benCode, string transamount, string bank_account, string transtype, string idno)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebClient client = new WebClient();
                string baseurl = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().SendMoney;
                string url = baseurl.Replace("ID", apiid).Replace("pwd", Password).Replace("tkn", tokennumber).Replace("sno", NUMBER).Replace("icode", benIFSC).Replace("bcode", benCode).Replace("amt", transamount).Replace("actno", bank_account).Replace("tsnt", transtype);
                Stream data = client.OpenRead(url);
                StreamReader reader = new StreamReader(data);

                string result = reader.ReadToEnd();
                IMPS_transtion_detsils objCourse = (from p in db.IMPS_transtion_detsils
                                                     where p.trans_id == idno
                                                     select p).SingleOrDefault();
                objCourse.Response_output = result;
                db.SaveChanges();
                return result;
            }
        }

        public String history(string mobile)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebClient client = new WebClient();
                string baseurl = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().TransitionDetails;
                string url = baseurl.Replace("sno", mobile);
                Stream data = client.OpenRead(url);
                StreamReader reader = new StreamReader(data);

                string result = reader.ReadToEnd();

                return result;
            }
        }

        public String reintilized(string refid, string mobile, string ifsc, string code, string idn, string transtype, string apiid, string Password, string tokennumber, string refacc, string refdate, string amount, string account)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                WebClient client = new WebClient();
                string baseurl = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().Reinitialize;

                string url = baseurl.Replace("ID", apiid).Replace("pwd", Password).Replace("tkn", tokennumber).Replace("rfid", refid).Replace("sno", mobile).Replace("icode", ifsc).Replace("bcode", code).Replace("idn", idn).Replace("tsnt", transtype).Replace("refaccnt", refacc).Replace("rdate", refdate).Replace("amt", amount).Replace("actno", account);
                Stream data = client.OpenRead(url);
                StreamReader reader = new StreamReader(data);

                string result = reader.ReadToEnd();

                return result;
            }
        }

        //public String ViewBill(string apiid, string Password, string tokennumber, string billnumber, string billunit, string ProcessingCycle, string optmobelectricity1)
        //{
        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
        //    {
        //        WebClient client = new WebClient();
        //        string baseurl = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault();
        //        string url = baseurl.Replace("ID", apiid).Replace("pwd", Password).Replace("tkn", tokennumber).Replace("bno", billnumber).Replace("bunit", billunit).Replace("pro_cycle", ProcessingCycle).Replace("EOPT", optmobelectricity1);
        //        Stream data = client.OpenRead(url);
        //        StreamReader reader = new StreamReader(data);

        //        string result = reader.ReadToEnd();

        //        return result;
        //    }
        //}
        //public String PayBillElectricity(string apiid, string Password, string tokennumber, string billnumber, string billunit, string ProcessingCycle, string optmobelectricity1,string amount)
        //{
        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
        //    {
        //        WebClient client = new WebClient();
        //        string baseurl = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().PayBillElectricity;
        //        string url = baseurl.Replace("ID", apiid).Replace("pwd", Password).Replace("tkn", tokennumber).Replace("bno", billnumber).Replace("bunit", billunit).Replace("pro_cycle", ProcessingCycle).Replace("EOPT", optmobelectricity1).Replace("amt", amount);
        //        Stream data = client.OpenRead(url);
        //        StreamReader reader = new StreamReader(data);

        //        string result = reader.ReadToEnd();

        //        return result;
        //    }
        //}
        //public String PayBillElectricity(string apiid, string Password, string tokennumber, string billnumber, string amount, string optmobelectricity1, string ProcessingCycle, string billunit)
        //{
        //    var sts = "";
        //    var traid = "";


        //    try
        //    {
        //        using (VastwebmultiEntities db = new VastwebmultiEntities())
        //        {
        //            string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
        //            //var sess = ssl.SessionNo;

        //            //var verifi = _strRequest("331564","331706","331707", SessionNo, billnumber, billunit, processcycle, amount, txtCERTNo,"");
        //            //var url = (from gg in db.Cyber_plate_code where gg.Own_code == optmobelectricity1 select gg).Single().Cyber_code_verify.ToString();
        //            //var urlpayment = (from gg in db.Cyber_plate_code where gg.Own_code == optmobelectricity1 select gg).Single().Cyber_code_Payment.ToString();
        //            //var checkverifiy = Verification(verifi, url);
        //            //int start = checkverifiy.IndexOf("RESULT=") + 7;
        //            //int end = checkverifiy.IndexOf("\r\n", start);
        //            //string result = checkverifiy.Substring(start, end - start);
        //            var idno = (from rch in db.recharge_info where rch.RechargeTo == billnumber where rch.Amount == amount where rch.Status == "Request Send" || rch.Status == "Request Sent" select rch.IdNo).SingleOrDefault().ToString();
        //            int idnn = Convert.ToInt32(idno);
        //            //if (result == "0")
        //            //{
        //            WebClient client = new WebClient();
        //            string baseurl = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault().PayBillElectricity;
        //            string url = baseurl.Replace("ID", apiid).Replace("pwd", Password).Replace("tkn", tokennumber).Replace("bno", billnumber).Replace("bunit", billunit).Replace("pro_cycle", ProcessingCycle).Replace("EOPT", optmobelectricity1).Replace("amt", amount);
        //            Stream data = client.OpenRead(url);
        //            StreamReader reader = new StreamReader(data);

        //            string result = reader.ReadToEnd();
        //            var paycheck = result;
        //            Thread.Sleep(10000);

        //            recharge_info objCourse = (from p in db.recharge_info where p.IdNo == idnn select p).Single();
        //            objCourse.response = paycheck.ToString();
        //            objCourse.session_name = SessionNo;
        //            db.SaveChanges();

        //            int start1 = paycheck.IndexOf("RESULT=") + 7;
        //            int end1 = paycheck.IndexOf("\r\n", start1);
        //            string result1 = paycheck.Substring(start1, end1 - start1);

        //            if (result1 == "0")
        //            {
        //                sts = "SUCCESS";
        //                int start2 = paycheck.IndexOf("AUTHCODE=") + 9;
        //                int end2 = paycheck.IndexOf("\r\n", start2);
        //                string result2 = paycheck.Substring(start2, end2 - start2);
        //                traid = result2;

        //                idno = (from rch in db.recharge_info where rch.RechargeTo == billnumber where rch.Amount == amount where rch.Status == "Request Send" || rch.Status == "Request Sent" select rch.IdNo).SingleOrDefault().ToString();
        //                var port = (from rch in db.recharge_info where rch.RechargeTo == billnumber where rch.Amount == amount where rch.Status == "Request Send" || rch.Status == "Request Sent" select rch.PortNo).SingleOrDefault().ToString();
        //                db.update_success_fail(idno, billnumber, Convert.ToDecimal(amount), optmobelectricity1, traid, sts, port, "0");
        //            }

        //            else
        //            {
        //                start1 = paycheck.IndexOf("ERROR=") + 6;
        //                end1 = paycheck.IndexOf("\r\n", start1);
        //                result1 = paycheck.Substring(start1, end1 - start1);

        //                sts = "FAILED";
        //                var errordetails = (from gg in db.Cyber_error_name_list where gg.error_code == result1 select gg).Single().Error_name.ToString();
        //                traid = errordetails;

        //                idno = (from rch in db.recharge_info where rch.RechargeTo == billnumber where rch.Amount == amount where rch.Status == "Request Send" || rch.Status == "Request Sent" select rch.IdNo).SingleOrDefault().ToString();
        //                var port = (from rch in db.recharge_info where rch.RechargeTo == billnumber where rch.Amount == amount where rch.Status == "Request Send" || rch.Status == "Request Sent" select rch.PortNo).SingleOrDefault().ToString();
        //                db.update_success_fail(idno, billnumber, Convert.ToDecimal(amount), optmobelectricity1, traid, sts, port, "0");
        //            }
        //            //}
        //            //else
        //            //{
        //            //    recharge_info objCourse = (from p in db.recharge_info where p.IdNo == idnn select p).Single();
        //            //    objCourse.response = checkverifiy.ToString();
        //            //    objCourse.session_name = SessionNo;
        //            //    db.SaveChanges();

        //            //    start = checkverifiy.IndexOf("ERROR=") + 6;
        //            //    end = checkverifiy.IndexOf("\r\n", start);
        //            //    result = checkverifiy.Substring(start, end - start);

        //            //    sts = "FAILED";
        //            //    var errordetails = (from gg in db.Cyber_error_name_list where gg.error_code == result select gg).Single().Error_name.ToString();
        //            //    traid = errordetails;

        //            //    idno = (from rch in db.recharge_info where rch.RechargeTo == billnumber where rch.Amount == amount where rch.Status == "Request Send" || rch.Status == "Request Sent" select rch.IdNo).SingleOrDefault().ToString();
        //            //    var port = (from rch in db.recharge_info where rch.RechargeTo == billnumber where rch.Amount == amount where rch.Status == "Request Send" || rch.Status == "Request Sent" select rch.PortNo).SingleOrDefault().ToString();
        //            //    db.update_success_fail(idno, billnumber, Convert.ToDecimal(amount), optmobelectricity1, traid, sts, port, "0");
        //            //}
        //            return sts;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {

        //    }
        //    return sts;

        //}
        //public String ViewBillGas(string billnumber, string optmobgas1)
        //{
        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
        //    {
        //        WebClient client = new WebClient();
        //        var baseurl = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault();
        //        string url = baseurl.ViewBillGas.Replace("ID", baseurl.API_ID).Replace("pwd", baseurl.Api_pwd).Replace("tkn", baseurl.Token).Replace("bno", billnumber).Replace("optgas", optmobgas1);
        //        Stream data = client.OpenRead(url);
        //        StreamReader reader = new StreamReader(data);

        //        string result = reader.ReadToEnd();

        //        return result;
        //    }
        //}
        //public String payBillGas(string billnumber, string optmobgas1,string mobileamountgas)
        //{
        //    using (VastwebmultiEntities db = new VastwebmultiEntities())
        //    {
        //        WebClient client = new WebClient();
        //        var baseurl = db.Money_API_URLS.Where(a => a.Status == "Y").SingleOrDefault();
        //        string url = baseurl.PayBillGas.Replace("ID", baseurl.API_ID).Replace("pwd", baseurl.Api_pwd).Replace("tkn", baseurl.Token).Replace("bno", billnumber).Replace("optgas", optmobgas1).Replace("amt", mobileamountgas);
        //        Stream data = client.OpenRead(url);
        //        StreamReader reader = new StreamReader(data);

        //        string result = reader.ReadToEnd();

        //        return result;
        //    }
        //}
    }
}
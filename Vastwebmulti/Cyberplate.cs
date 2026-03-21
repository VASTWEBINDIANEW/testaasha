using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Vastwebmulti.Models;
using CyberPlatOpenSSL;

namespace Vastwebmulti
{
    public class Cyberplate
    {
        OpenSSL ssl = new OpenSSL();
        StringBuilder str = new StringBuilder();
        VastwebmultiEntities db = new VastwebmultiEntities();
        public String fatchplan(string mobile, string amount, string code)
        {
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            //var sts = "";
            //var traid = "";
            string result = "";
            var verifi = _strRequest(SessionNo, mobile, "", "", "1", "1");
            var url = "https://in.cyberplat.com/cgi-bin/rjio/rjio_pay_check.cgi";
            Thread.Sleep(500);
            var checkverifiy = Verification(verifi, url);
            int start1 = checkverifiy.IndexOf("RESULT=") + 7;
            int end1 = checkverifiy.IndexOf("\r\n", start1);
            string result1 = checkverifiy.Substring(start1, end1 - start1);
            if (result1 == "0")
            {
                int start = checkverifiy.IndexOf("ADDINFO=") + 8;
                int end = checkverifiy.IndexOf("\r\n", start);
                result = checkverifiy.Substring(start, end - start);
            }
            else
            {
                start1 = checkverifiy.IndexOf("ERROR=") + 6;
                end1 = checkverifiy.IndexOf("\r\n", start1);
                result1 = checkverifiy.Substring(start1, end1 - start1);
                var errordetails = (from gg in db.Cyber_error_name_list where gg.error_code == result1 select gg).Single().Error_name.ToString();
                result = "ERROR#" + errordetails;
            }
            return result;
        }
        public string landline(string number, string account, string autecate, string amount, string code)
        {
            amount = amount.Replace(".00", "");
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            var sts = "";
            var traid = "";
            var verifi = _strRequest(SessionNo, number, account, autecate, amount, "");
            var url = (from gg in db.Cyber_plate_code where gg.Own_code == code select gg.Cyber_code_verify).FirstOrDefault() ?? "NULL";
            var urlpayment = (from gg in db.Cyber_plate_code where gg.Own_code == code select gg.Cyber_code_Payment).FirstOrDefault() ?? "NULL";
            if (url != "NULL" && urlpayment != "NULL")
            {
                var checkverifiy = Verification(verifi, url);
                int start = checkverifiy.IndexOf("RESULT=") + 7;
                int end = checkverifiy.IndexOf("\r\n", start);
                string result = checkverifiy.Substring(start, end - start);
                var idno = (from rch in db.Recharge_info where rch.Mobile == number where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                int idnn = Convert.ToInt32(idno);
                if (result == "0")
                {
                    var paycheck = payment(verifi, urlpayment);
                    Thread.Sleep(5000);
                    int start1 = paycheck.IndexOf("RESULT=") + 7;
                    int end1 = paycheck.IndexOf("\r\n", start1);
                    string result1 = paycheck.Substring(start1, end1 - start1);
                    try
                    {
                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn select p).Single();
                        objCourse.response_output = paycheck.ToString();
                        //objCourse.session_name = SessionNo;
                        db.SaveChanges();
                    }
                    catch
                    { }

                    if (result1 == "0")
                    {
                        sts = "SUCCESS";
                        int start2 = paycheck.IndexOf("TRANSID=") + 8;
                        int end2 = paycheck.IndexOf("\r\n", start2);
                        string result2 = paycheck.Substring(start2, end2 - start2);
                        traid = result2;
                        idno = (from rch in db.Recharge_info where rch.Mobile == number where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                        var port = (from rch in db.Recharge_info where rch.Mobile == number where rch.amount ==Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                        db.update_success_fail(idno, number, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                    }
                    else
                    {
                        start = paycheck.IndexOf("ERROR=") + 6;
                        end = paycheck.IndexOf("\r\n", start);
                        result = paycheck.Substring(start, end - start);
                        sts = "FAILED";
                        var errordetails = (from gg in db.Cyber_error_name_list where gg.error_code == result select gg).Single().Error_name.ToString();
                        traid = errordetails;
                        idno = (from rch in db.Recharge_info where rch.Mobile == number where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                        var port = (from rch in db.Recharge_info where rch.Mobile == number where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                        db.update_success_fail(idno, number, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                    }
                }
                else
                {
                    try
                    {
                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn select p).Single();
                        objCourse.response_output = checkverifiy.ToString();
                        objCourse.Order_id = SessionNo;
                        db.SaveChanges();
                    }
                    catch
                    { }

                    start = checkverifiy.IndexOf("ERROR=") + 6;
                    end = checkverifiy.IndexOf("\r\n", start);
                    result = checkverifiy.Substring(start, end - start);
                    sts = "FAILED";
                    var errordetails = (from gg in db.Cyber_error_name_list where gg.error_code == result select gg).Single().Error_name.ToString();
                    traid = errordetails;
                    idno = (from rch in db.Recharge_info where rch.Mobile == number where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                    var port = (from rch in db.Recharge_info where rch.Mobile == number where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                    db.update_success_fail(idno, number, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                }
            }
            else
            {
                sts = "FAILED";

                traid = "Not Active";
                var idno = (from rch in db.Recharge_info where rch.Mobile == number where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                var port = (from rch in db.Recharge_info where rch.Mobile == number where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                db.update_success_fail(idno, number, Convert.ToDecimal(amount), code, traid, sts, port, "0");
            }
            return sts;
        }
        public String bill_verify(string mobile, string amount, string code, string processcycle, string billunit)
        {
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            //var sts = "";
            //var traid = "";
            string result = "";
            var verifi = _strRequest(SessionNo, mobile, billunit, processcycle, amount, "");
            var urlcommon = db.Cyber_plate_code.Where(aa => aa.Own_code == code).SingleOrDefault();
            var urlpayment = urlcommon == null ? "NO" : urlcommon.Cyber_code_Payment;
            var url = urlcommon == null ? "NO" : urlcommon.Cyber_code_verify;
            if (url != "NO" && urlpayment != "NO")
            {
                Thread.Sleep(500);
                var checkverifiy = Verification(verifi, url);
                if (checkverifiy.Contains("PRICE="))
                {
                    int start = checkverifiy.IndexOf("PRICE=") + 6;
                    int end = checkverifiy.IndexOf("\r\n", start);
                    result = checkverifiy.Substring(start, end - start);
                    result = "PRICE" + "||" + result;
                }
                else
                {
                    int start = checkverifiy.IndexOf("ERROR=") + 6;
                    int end = checkverifiy.IndexOf("\r\n", start);
                    result = checkverifiy.Substring(start, end - start);
                    var errordetails = (from gg in db.Cyber_error_name_list where gg.error_code == result select gg).Single().Error_name.ToString();
                    result = "ERROR" + "||" + errordetails;
                }
            }
            else
            {
                var errordetails = "Not Active";
                result = "ERROR" + "||" + errordetails;
            }
            return result;
        }
        public String bill_pay(string mobile, string amount, string code, string processcycle, string billunit)
        {
            amount = amount.Replace(".00", "");
            var sts = "";
            var traid = "";
            try
            {
                string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
                var sess = ssl.SessionNo;
                var verifi = _strRequest(SessionNo, mobile, billunit, processcycle, amount, "");
                var url = (from gg in db.Cyber_plate_code where gg.Own_code == code select gg.Cyber_code_verify).FirstOrDefault() ?? "NULL";
                var urlpayment = (from gg in db.Cyber_plate_code where gg.Own_code == code select gg.Cyber_code_Payment).FirstOrDefault() ?? "NULL";
                if (url != "NULL" && urlpayment != "NULL")
                {
                    var checkverifiy = Verification(verifi, url);
                    int start = checkverifiy.IndexOf("RESULT=") + 7;
                    int end = checkverifiy.IndexOf("\r\n", start);
                    string result = checkverifiy.Substring(start, end - start);
                    var idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                    int idnn = Convert.ToInt32(idno);
                    if (result == "0")
                    {
                        var paycheck = payment(verifi, urlpayment);
                        Thread.Sleep(10000);
                        try
                        {
                            Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn select p).Single();
                            objCourse.response_output = paycheck.ToString();
                            objCourse.Order_id = SessionNo;
                            db.SaveChanges();
                        }
                        catch
                        { }

                        int start1 = paycheck.IndexOf("RESULT=") + 7;
                        int end1 = paycheck.IndexOf("\r\n", start1);
                        string result1 = paycheck.Substring(start1, end1 - start1);
                        if (result1 == "0")
                        {
                            sts = "SUCCESS";
                            int start2 = paycheck.IndexOf("AUTHCODE=") + 9;
                            int end2 = paycheck.IndexOf("\r\n", start2);
                            string result2 = paycheck.Substring(start2, end2 - start2);
                            traid = result2;
                            idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                            var port = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                            db.update_success_fail(idno, mobile, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                        }
                        else
                        {
                            start1 = paycheck.IndexOf("ERROR=") + 6;
                            end1 = paycheck.IndexOf("\r\n", start1);
                            result1 = paycheck.Substring(start1, end1 - start1);
                            sts = "FAILED";
                            var errordetails = (from gg in db.Cyber_error_name_list where gg.error_code == result1 select gg).Single().Error_name.ToString();
                            traid = errordetails;
                            idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                            var port = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                            db.update_success_fail(idno, mobile, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                        }
                    }
                    else
                    {
                        try
                        {
                            Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn select p).Single();
                            objCourse.response_output = checkverifiy.ToString();
                            objCourse.Order_id = SessionNo;
                            db.SaveChanges();
                        }
                        catch { }

                        start = checkverifiy.IndexOf("ERROR=") + 6;
                        end = checkverifiy.IndexOf("\r\n", start);
                        result = checkverifiy.Substring(start, end - start);
                        sts = "FAILED";
                        var errordetails = (from gg in db.Cyber_error_name_list where gg.error_code == result select gg).Single().Error_name.ToString();
                        traid = errordetails;
                        idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                        var port = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                        db.update_success_fail(idno, mobile, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                    }
                }
                else
                {
                    sts = "FAILED";
                    var errordetails = "Not Active";
                    traid = errordetails;
                    var idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                    var port = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                    db.update_success_fail(idno, mobile, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                }
                return sts;
            }
            catch
            {
            }
            finally
            {
            }
            return sts;
        }
        public String rechargeJIO(string mobile, string amount, string code, string JioId)
        {
            amount = amount.Replace(".00", "");
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            var sts = "";
            var traid = "";
           // var account = "";

            var url = (from gg in db.Cyber_plate_code where gg.Own_code == code select gg.Cyber_code_verify).FirstOrDefault() ?? "NULL";
            var urlpayment = (from gg in db.Cyber_plate_code where gg.Own_code == code select gg.Cyber_code_Payment).Single() ?? "NULL";
            if (url != "NULL" && urlpayment != "NULL")
            {
                var verifi = _strRequestJIO(SessionNo, mobile, JioId, amount);
                var checkverifiy = Verification(verifi, url);
                int start = checkverifiy.IndexOf("RESULT=") + 7;
                int end = checkverifiy.IndexOf("\r\n", start);
                string result = checkverifiy.Substring(start, end - start);
                amount = amount.Replace(".00", "");
                var idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                int idnn = Convert.ToInt32(idno);
                if (result == "0")
                {
                    var paycheck = payment(verifi, urlpayment);
                    Thread.Sleep(5000);
                    int start1 = paycheck.IndexOf("RESULT=") + 7;
                    int end1 = paycheck.IndexOf("\r\n", start1);
                    string result1 = paycheck.Substring(start1, end1 - start1);
                    try
                    {
                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn select p).Single();
                        objCourse.response_output = paycheck.ToString();
                        objCourse.Order_id = SessionNo;
                        db.SaveChanges();
                    }
                    catch { }

                    if (result1 == "0")
                    {
                        sts = "SUCCESS";
                        int start2 = paycheck.IndexOf("TRANSID=") + 8;
                        int end2 = paycheck.IndexOf("\r\n", start2);
                        string result2 = paycheck.Substring(start2, end2 - start2);
                        traid = result2;
                        idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                        var port = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                        db.update_success_fail(idno, mobile, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                    }
                    else
                    {
                        start = paycheck.IndexOf("ERROR=") + 6;
                        end = paycheck.IndexOf("\r\n", start);
                        result = paycheck.Substring(start, end - start);
                        sts = "FAILED";
                        var errordetails = (from gg in db.Cyber_error_name_list where gg.error_code == result select gg).Single().Error_name.ToString();
                        traid = errordetails;
                        idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                        var port = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                        db.update_success_fail(idno, mobile, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                    }
                }
                else
                {
                    try
                    {
                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn select p).Single();
                        objCourse.response_output = checkverifiy.ToString();
                        objCourse.Order_id = SessionNo;
                        db.SaveChanges();
                    }
                    catch
                    { }

                    start = checkverifiy.IndexOf("ERROR=") + 6;
                    end = checkverifiy.IndexOf("\r\n", start);
                    result = checkverifiy.Substring(start, end - start);
                    sts = "FAILED";
                    var errordetails = (from gg in db.Cyber_error_name_list where gg.error_code == result select gg).Single().Error_name.ToString();
                    traid = errordetails;
                    idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                    var port = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                    db.update_success_fail(idno, mobile, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                }
            }
            else
            {
                sts = "FAILED";
                var errordetails = "Not Active";
                traid = errordetails;
                var idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                var port = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                db.update_success_fail(idno, mobile, Convert.ToDecimal(amount), code, traid, sts, port, "0");
            }
            return sts;
        }
        public String recharge(string mobile, string amount, string code)
        {
            amount = amount.Replace(".00", "");
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            var sts = "";
            var traid = "";
            var account = "";

            var url = (from gg in db.Cyber_plate_code where gg.Own_code == code select gg.Cyber_code_verify).FirstOrDefault() ?? "NULL";
            var urlpayment = (from gg in db.Cyber_plate_code where gg.Own_code == code select gg.Cyber_code_Payment).FirstOrDefault() ?? "NULL";
            if (url != "NULL" && urlpayment != "NULL")
            {
                var verifi = _strRequest(SessionNo, mobile, account, "", amount, "");
                var checkverifiy = Verification(verifi, url);
                int start = checkverifiy.IndexOf("RESULT=") + 7;
                int end = checkverifiy.IndexOf("\r\n", start);
                string result = checkverifiy.Substring(start, end - start);
                var idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                int idnn = Convert.ToInt32(idno);
                if (result == "0")
                {
                    var paycheck = payment(verifi, urlpayment);
                    Thread.Sleep(5000);
                    int start1 = paycheck.IndexOf("RESULT=") + 7;
                    int end1 = paycheck.IndexOf("\r\n", start1);
                    string result1 = paycheck.Substring(start1, end1 - start1);
                    try
                    {

                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn select p).Single();
                        objCourse.response_output = paycheck.ToString();
                        objCourse.Order_id = SessionNo;
                        db.SaveChanges();
                    }
                    catch { }

                    if (result1 == "0")
                    {
                        sts = "SUCCESS";
                        int start2 = paycheck.IndexOf("TRANSID=") + 8;
                        int end2 = paycheck.IndexOf("\r\n", start2);
                        string result2 = paycheck.Substring(start2, end2 - start2);
                        traid = result2;
                        idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                        var port = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                        db.update_success_fail(idno, mobile, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                    }
                    else
                    {
                        start = paycheck.IndexOf("ERROR=") + 6;
                        end = paycheck.IndexOf("\r\n", start);
                        result = paycheck.Substring(start, end - start);
                        sts = "FAILED";
                        var errordetails = (from gg in db.Cyber_error_name_list where gg.error_code == result select gg).Single().Error_name.ToString();
                        traid = errordetails;
                        idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                        var port = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                        db.update_success_fail(idno, mobile, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                    }
                }
                else
                {
                    try
                    {
                        Recharge_info objCourse = (from p in db.Recharge_info where p.idno == idnn select p).Single();
                        objCourse.response_output = checkverifiy.ToString();
                        objCourse.Order_id = SessionNo;
                        db.SaveChanges();
                    }
                    catch { }

                    start = checkverifiy.IndexOf("ERROR=") + 6;
                    end = checkverifiy.IndexOf("\r\n", start);
                    result = checkverifiy.Substring(start, end - start);
                    sts = "FAILED";
                    var errordetails = (from gg in db.Cyber_error_name_list where gg.error_code == result select gg).Single().Error_name.ToString();
                    traid = errordetails;
                    idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                    var port = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                    db.update_success_fail(idno, mobile, Convert.ToDecimal(amount), code, traid, sts, port, "0");
                }
            }
            else
            {
                sts = "FAILED";
                var errordetails = "Not Active";
                traid = errordetails;
                var idno = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.idno).SingleOrDefault().ToString();
                var port = (from rch in db.Recharge_info where rch.Mobile == mobile where rch.amount == Convert.ToDecimal(amount) where rch.Rstaus == "Request Send" || rch.Rstaus == "Request Sent" select rch.portno).SingleOrDefault().ToString();
                db.update_success_fail(idno, mobile, Convert.ToDecimal(amount), code, traid, sts, port, "0");
            }
            return sts;
        }
        private String _strRequest(string SessionNo, string txtMobileNo, string account, string authenticator3, string amount, string REQ_TYPE)
        {
            StringBuilder _reqStr = new StringBuilder();
            string SDCode = db.cyberplate_info.Select(aa => aa.sdcode).FirstOrDefault() ?? "";
            string APCode = db.cyberplate_info.Select(aa => aa.apcode).FirstOrDefault() ?? "";
            string OPCode = db.cyberplate_info.Select(aa => aa.opcode).FirstOrDefault() ?? "";
            string txtCERTNo = db.cyberplate_info.Select(aa => aa.certcode).FirstOrDefault() ?? "";
            #region Create Request
            _reqStr.Append("CERT=" + txtCERTNo + Environment.NewLine);
            _reqStr.Append("SD=" + SDCode + Environment.NewLine);
            _reqStr.Append("AP=" + APCode + Environment.NewLine);
            _reqStr.Append("OP=" + OPCode + Environment.NewLine);
            _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
            _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
            _reqStr.Append("ACCOUNT=" + account + Environment.NewLine);
            _reqStr.Append("Authenticator3=" + authenticator3 + Environment.NewLine);
            _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
            _reqStr.Append("REQ_TYPE=" + REQ_TYPE + Environment.NewLine);
            _reqStr.Append("TERM_ID=" + APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
            _reqStr.Append("COMMENT=test");
            #endregion
            return _reqStr.ToString();
        }
        private String _strRequestJIO(string SessionNo, string txtMobileNo, string planoffer, string amount)
        {
            StringBuilder _reqStr = new StringBuilder();
            string SDCode = db.cyberplate_info.Select(aa => aa.sdcode).FirstOrDefault() ?? "";
            string APCode = db.cyberplate_info.Select(aa => aa.apcode).FirstOrDefault() ?? "";
            string OPCode = db.cyberplate_info.Select(aa => aa.opcode).FirstOrDefault() ?? "";
            string txtCERTNo = db.cyberplate_info.Select(aa => aa.certcode).FirstOrDefault() ?? "";
            #region Create Request
            _reqStr.Append("CERT=" + txtCERTNo + Environment.NewLine);
            _reqStr.Append("SD=" + SDCode + Environment.NewLine);
            _reqStr.Append("AP=" + APCode + Environment.NewLine);
            _reqStr.Append("OP=" + OPCode + Environment.NewLine);
            _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
            _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
            _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
            _reqStr.Append("PlanOffer=" + planoffer + Environment.NewLine);
            _reqStr.Append("TERM_ID=" + APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
            _reqStr.Append("COMMENT=test");
            #endregion
            return _reqStr.ToString();
        }
        private String Verification(String txtRequest, String txtURL)
        {
            var response = "";
            try
            {
                var keypathchk = (db.cyberplate_info.Select(aa => aa.keypath)).FirstOrDefault() ?? "";
                var password = (db.cyberplate_info.Select(aa => aa.password)).FirstOrDefault() ?? "";
                string pathchk = System.Web.Hosting.HostingEnvironment.MapPath("~/CERT/");
                string keyPath = pathchk + "" + keypathchk;
                ssl.message = ssl.Sign_With_PFX(txtRequest, keyPath, password);
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, txtURL);
                response = "URL:\r\n" + txtURL + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
            }
            catch (Exception epx)
            {
                response = epx.ToString();
            }
            return response;
        }
        private String Verificationagain(String txtRequest, String txtURL)
        {
            var response = "";
            try
            {
                var keypathchk = (db.cyberplate_info.Select(aa => aa.keypath)).FirstOrDefault() ?? "";
                var password = (db.cyberplate_info.Select(aa => aa.password)).FirstOrDefault() ?? "";
                string pathchk = System.Web.Hosting.HostingEnvironment.MapPath("~/CERT/");
                string keyPath = pathchk + "" + keypathchk;
                ssl.message = ssl.Sign_With_PFX(txtRequest, keyPath, password);
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, txtURL);
                response = "URL:\r\n" + txtURL + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
            }
            catch (Exception epx)
            {
                response = epx.ToString();
            }
            return response;
        }
        private String payment(string txtRequest, string payURL)
        {
            var response = "";
            try
            {
                payURL = payURL.Replace("_check", "");
                var keypathchk = (db.cyberplate_info.Select(aa => aa.keypath)).FirstOrDefault() ?? "";
                var password = (db.cyberplate_info.Select(aa => aa.password)).FirstOrDefault() ?? "";
                string pathchk = System.Web.Hosting.HostingEnvironment.MapPath("~/CERT/");
                string keyPath = pathchk + "" + keypathchk;
                ssl.message = ssl.Sign_With_PFX(txtRequest, keyPath, password);
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, payURL);
                response = "URL:\r\n" + payURL + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
            }
            catch (Exception exp)
            {
                response = exp.ToString();
            }
            return response;
        }
        private String Status(string txtRequest, string payURL)
        {
            var response = "";
            try
            {
                payURL = payURL.Replace("_check", "");
                var keypathchk = (db.cyberplate_info.Select(aa => aa.keypath)).FirstOrDefault() ?? "";
                var password = (db.cyberplate_info.Select(aa => aa.password)).FirstOrDefault() ?? "";
                string pathchk = System.Web.Hosting.HostingEnvironment.MapPath("~/CERT/");
                string keyPath = pathchk + "" + keypathchk;
                ssl.message = ssl.Sign_With_PFX(txtRequest, keyPath, password);
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, payURL);
                response = "URL:\r\n" + payURL + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
            }
            catch (Exception exp)
            {
                response = exp.ToString();
            }
            return response;
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
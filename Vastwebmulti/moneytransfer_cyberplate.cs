using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using CyberPlatOpenSSL;
using System.Threading;
using Vastwebmulti.Models;

namespace A2ZMultiService
{
    public class moneytransfer_cyberplate
    {
        OpenSSL ssl = new OpenSSL(); StringBuilder str = new StringBuilder();
        VastwebmultiEntities db = new VastwebmultiEntities();
        static string Validationurl = "https://in.cyberplat.com/cgi-bin/instp/instp_pay_check.cgi";
        static string Paymenturl = "https://in.cyberplat.com/cgi-bin/instp/instp_pay.cgi";
     //   static string statusurl = "https://in.cyberplat.com/cgi-bin/yb/yb_pay.cgi";
        static string chkbal = "https://in.cyberplat.com/cgi-bin/mts_espp/mtspay_rest.cgi";

        private String _update(string SessionNo)
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
            //Mostly value of TERM_ID=AP Code, but value may be vary.
            _reqStr.Append("COMMENT=test");
            #endregion
            return _reqStr.ToString();
        }
        public string checkbal()
        {
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            //var sts = "";
            //var traid = "";
            //var account = "";
            string bal = "";
            var verifi = _strRequest(SessionNo, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            var checkverifiy = Verification(verifi, chkbal);
            int start = checkverifiy.IndexOf("ERROR=") + 6;
            int end = checkverifiy.IndexOf("\r\n", start);
            string result = checkverifiy.Substring(start, end - start);
            if (result == "0")
            {
                start = checkverifiy.IndexOf("REST=") + 5;
                end = checkverifiy.IndexOf("\r\n", start);
                bal = checkverifiy.Substring(start, end - start);
            }
            else
            {
                bal = "0";
            }
            return bal;
        }
        public string getbankname()
        {
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            var sts = "";
            //var traid = "";
            //var account = "";
            var output = "";

            var verifi = _strRequest(SessionNo, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "15", "", "1", "1", "");
            var checkverifiy = Verification(verifi, Validationurl);
            int start = checkverifiy.IndexOf("ERROR=") + 6;
            int end = checkverifiy.IndexOf("\r\n", start);
            string result = checkverifiy.Substring(start, end - start);
            if (result == "0")
            {
                sts = payment(verifi, Paymenturl);
                output = sts;
            }
            else
            {
                output = checkverifiy;
            }
            return output;
        }
        public String customer_verify(string number)
        {
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            var sts = "";
            //var traid = "";
            //var account = "";
            var output = "";
            var verifi = _strRequest(SessionNo, "", "", "", "", "", "", "", "", "", "", "", number, "", "", "", "", "", "", "", "5", "", "1", "1", "");

            var checkverifiy = Verification(verifi, Validationurl);
            int start = checkverifiy.IndexOf("ERROR=") + 6;
            int end = checkverifiy.IndexOf("\r\n", start);
            string result = checkverifiy.Substring(start, end - start);
            if (result == "0")
            {
                sts = payment(verifi, Paymenturl);
                output = sts;
            }
            else
            {
                output = checkverifiy;
            }
            return output;
        }
        public String add_customer(string number, string name)
        {
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            var sts = "";
            //var traid = "";
            //var account = "";
            var output = "";
            var verifi = _strRequest(SessionNo, "332311", "", name, "", "", "", "", "", "", "", "", number, "", "", "", "", "", "", "", "0", "", "1", "1", "");

            var checkverifiy = Verification(verifi, Validationurl);
            int start = checkverifiy.IndexOf("RESULT=") + 7;
            int end = checkverifiy.IndexOf("\r\n", start);
            string result = checkverifiy.Substring(start, end - start);
            if (result == "0")
            {
                sts = payment(verifi, Paymenturl);
                output = sts;
            }
            else
            {
                output = checkverifiy;
            }
            return output;
        }
        public String resend_otp(string remid, string benid, string senderno, string sendernm)
        {
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            var sts = "";
            //var traid = "";
            //var account = "";
            var output = "";
            var verifi = _strRequest(SessionNo, "", "", "", "", remid, "", "", "", "", "", "", senderno, "", benid, "", "", "", "", "", "9", "", "1", "1", "");
            var checkverifiy = Verification(verifi, Validationurl);
            int start = checkverifiy.IndexOf("ERROR=") + 6;
            int end = checkverifiy.IndexOf("\r\n", start);
            string result = checkverifiy.Substring(start, end - start);
            if (result == "0")
            {
                sts = payment(verifi, Paymenturl);
                output = sts;
            }
            else
            {
                output = checkverifiy;
            }
            return output;
        }
        public String otp_verify(string number, string otf, string requestid, string benid, string deletecode)
        {
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            var sts = "";
            //var traid = "";
            //var account = "";
            var output = "";
            if (deletecode != "23")
            {
                deletecode = "2";
            }
            var verifi = _strRequest(SessionNo, "", otf, "", "", requestid, "", "", "", "", "", "", number, "", benid, "", "", "", "", "", deletecode, "", "1", "1", "");

            var checkverifiy = Verification(verifi, Validationurl);
            int start = checkverifiy.IndexOf("ERROR=") + 6;
            int end = checkverifiy.IndexOf("\r\n", start);
            string result = checkverifiy.Substring(start, end - start);
            if (result == "0")
            {
                sts = payment(verifi, Paymenturl);
                output = sts;
            }
            else
            {
                output = checkverifiy;
            }
            return output;
        }
        public String add_benificy(string benAccount, string senderno, string benIFSC, string name, string remid)
        {
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            var sts = "";
            //var traid = "";
            //var account = "";
            var output = "";
            var verifi = _strRequest(SessionNo, "", "", name, "IMPS", remid, "", "", benAccount, "", "", "", senderno, "", "", "", benIFSC, "", "", "", "4", "", "1", "1", "");

            var checkverifiy = Verification(verifi, Validationurl);
            int start = checkverifiy.IndexOf("RESULT=") + 7;
            int end = checkverifiy.IndexOf("\r\n", start);
            string result = checkverifiy.Substring(start, end - start);
            if (result == "0")
            {
                sts = payment(verifi, Paymenturl);
                output = sts;
            }
            else
            {
                output = checkverifiy;
            }
            return output;
        }
        public String delete_benificy(string mobile, string ifsc, string code, string remid)
        {
            string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            var sts = "";
            //var traid = "";
            //var account = "";
            var output = "";
            var verifi = _strRequest(SessionNo, "", "", "", "IMPS", remid, "", "", "", "", "", "", mobile, "", code, "", ifsc, "", "", code, "6", "", "1", "1", "");
       
            var checkverifiy = Verification(verifi, Validationurl);
            int start = checkverifiy.IndexOf("ERROR=") + 6;
            int end = checkverifiy.IndexOf("\r\n", start);
            string result = checkverifiy.Substring(start, end - start);
            if (result == "0")
            {
              
                    sts = payment(verifi, Paymenturl);
                    output = sts;
            }
            else
            {
                  output = checkverifiy;
            }
            return output;
        }
        public String Verify_benificy(string mobile, string ifsc, string code,string account, string userid, string bankname, string kycsts, string Ipaddress, string macaddress,string CommonTranid)
        {
            //string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            var sess = ssl.SessionNo;
            var sts = "";
            //var traid = "";
            var output = "";
            var verifi = _strRequest(CommonTranid, "", "", "", "IMPS", "", "", "", account, "", "", "", mobile, "", "", "", ifsc, "", "", code, "10", "", "1.00", "2.5", "");
            System.Data.Entity.Core.Objects.ObjectParameter outputchk = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
            var checkverifiy = Verification(verifi, Validationurl);
            int start = checkverifiy.IndexOf("RESULT=") + 7;
            int end = checkverifiy.IndexOf("\r\n", start);
            string result = checkverifiy.Substring(start, end - start);
            if (result == "0")
            {
               var th = db.Money_transfer_new_new(userid, 0, 0, mobile, account, bankname, ifsc, CommonTranid, CommonTranid, "IMPS_VERIFY", "ONLINE", kycsts, verifi, "Cyber", Ipaddress, macaddress, "",0,0,"","", outputchk).Single().msg;
               if (th == "OK")
               {
                 sts = payment(verifi, Paymenturl);
                 output = sts;
              }
              else
              {
                 output = th;
              }
            }
            else
            {
                var th = db.Money_transfer_new_new(userid, 0, 0, mobile, account, bankname, ifsc, CommonTranid, CommonTranid, "IMPS_VERIFY", "ONLINE", kycsts, verifi, "Cyber", Ipaddress, macaddress, "",0,0,"","", outputchk).Single().msg;
                if (th == "OK")
                {
                    output = checkverifiy;
                }
                else
                {
                    output = th;
                }
            }
            return output;
        }

        public String fundtransfer(string mobile, string ifsc, string code, string money, string transtype,string userid,string finalamount,string accountno,string banknm,string commonid,string reqid,string kycstatus,string IPADDRESS,string MacAddress)
        {
           // string SessionNo = "W" + DateTime.Parse(DateTime.Now.ToString()).ToString("yyMMddHHmmss") + RandomString(4);
            //var sess = ssl.SessionNo;
            //var traid = "";
            //var account = "";
            var output = "";
            var verifi = _strRequest(reqid, "", "", "", transtype, "", "", "", "", "", "", "", mobile, "", code, "", "", "", "", "", "3", "", money, money + 5, "");

            var checkverifiy = Verification(verifi, Validationurl);
            int start = checkverifiy.IndexOf("RESULT=") + 7;
            int end = checkverifiy.IndexOf("\r\n", start);
            string result = checkverifiy.Substring(start, end - start);
            System.Data.Entity.Core.Objects.ObjectParameter outputchk = new System.Data.Entity.Core.Objects.ObjectParameter("Output", typeof(string));
            if (result == "0")
            {
                var th = db.Money_transfer_new_new(userid, Convert.ToDecimal(money), Convert.ToDecimal(finalamount), mobile, accountno, banknm, ifsc, commonid, reqid, "IMPS", "ONLINE", kycstatus, verifi, "CYBER", IPADDRESS, MacAddress, "",0,0,"","",outputchk).SingleOrDefault().msg;
                if (th == "OK")
                {
                    var sts1 = payment(verifi, Paymenturl);
                    output = sts1;
                }
                else
                {
                    output = th;
                }
            }
            else
            {
                var ch = db.Money_transfer_new_new(userid, Convert.ToDecimal(money), Convert.ToDecimal(finalamount), mobile, accountno, banknm, ifsc, commonid, reqid, "IMPS", "ONLINE", kycstatus, verifi, "CYBER", IPADDRESS, MacAddress, "",0,0,"","", outputchk).SingleOrDefault().msg;
                if (ch == "OK")
                {
                    output = checkverifiy;
                }
                else
                {
                    output = ch;
                }
            }
            return output;
        }

        private String _strRequest(string SessionNo, string txtpin,
            string otc, string fName, string routingType, string remid, string mothersMaidenName, string state,
            string benAccount, string benMobile, string address, string birthday, string NUMBER,
            string gender, string benId, string benNick, string benIFSC, string lName, string benName,
            string benCode, string TType, string ACCOUNT, string AMOUNT, string AMOUNT_ALL, string PanCardNo)
        {
            string SDCode = db.cyberplate_info.Select(aa => aa.sdcode).FirstOrDefault() ?? "";
            string APCode = db.cyberplate_info.Select(aa => aa.apcode).FirstOrDefault() ?? "";
            string OPCode = db.cyberplate_info.Select(aa => aa.opcode).FirstOrDefault() ?? "";
            string txtCERTNo = db.cyberplate_info.Select(aa => aa.certcode).FirstOrDefault() ?? "";

            StringBuilder _reqStr = new StringBuilder();
            #region Create Request
            _reqStr.Append("CERT=" + txtCERTNo + Environment.NewLine);
            _reqStr.Append("SD=" + SDCode + Environment.NewLine);
            _reqStr.Append("AP=" + APCode + Environment.NewLine);
            _reqStr.Append("OP=" + OPCode + Environment.NewLine);
            _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
            _reqStr.Append("Pin=" + txtpin + Environment.NewLine);
            _reqStr.Append("otc=" + otc + Environment.NewLine);
            _reqStr.Append("fName=" + fName + Environment.NewLine);
            _reqStr.Append("routingType=" + routingType + Environment.NewLine);
            _reqStr.Append("remId=" + remid + Environment.NewLine);
            _reqStr.Append("mothersMaidenName=" + mothersMaidenName + Environment.NewLine);
            _reqStr.Append("state=" + state + Environment.NewLine);
            _reqStr.Append("benAccount=" + benAccount + Environment.NewLine);
            _reqStr.Append("TERM_ID=" + APCode + Environment.NewLine);
            _reqStr.Append("benMobile=" + benMobile + Environment.NewLine);
            _reqStr.Append("address=" + address + Environment.NewLine);
            _reqStr.Append("birthday=" + birthday + Environment.NewLine);
            _reqStr.Append("NUMBER=" + NUMBER + Environment.NewLine);
            _reqStr.Append("gender=" + gender + Environment.NewLine);
            _reqStr.Append("benId=" + benId + Environment.NewLine);
            _reqStr.Append("benNick=" + benNick + Environment.NewLine);
            _reqStr.Append("benIFSC=" + benIFSC + Environment.NewLine);
            _reqStr.Append("lName=" + lName + Environment.NewLine);
            _reqStr.Append("benName=" + benName + Environment.NewLine);
            _reqStr.Append("benCode=" + benCode + Environment.NewLine);
            _reqStr.Append("Type=" + TType + Environment.NewLine);
            _reqStr.Append("ACCOUNT=" + ACCOUNT + Environment.NewLine);
            _reqStr.Append("AMOUNT=" + AMOUNT + Environment.NewLine);
            _reqStr.Append("AMOUNT_ALL=" + AMOUNT_ALL + Environment.NewLine);
            _reqStr.Append("PanCardNo=" + PanCardNo + Environment.NewLine);
            //Mostly value of TERM_ID=AP Code, but value may be vary.
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
        private static Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
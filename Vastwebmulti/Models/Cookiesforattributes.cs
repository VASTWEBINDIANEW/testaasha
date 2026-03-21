using System.Web;

namespace Vastwebmulti.Models
{
    public class Cookiesforattributes
    {
        public void setcookies(string pass, string userid)
        {
            HttpCookie myCookie = new HttpCookie("MyTestCookie");
            myCookie.Values.Add("exp", pass);
            myCookie.Values.Add("usr", userid);
            //   myCookie.Expires = DateTime.Now.AddYears(30);
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }
        public string getcookies(string userid)
        {
            try
            {
                string exp = HttpContext.Current.Request.Cookies["MyTestCookie"]["exp"];
                string usr = HttpContext.Current.Request.Cookies["MyTestCookie"]["usr"];
                if (exp != null && usr != null)
                {
                    if (userid == usr)
                    {
                        return exp;
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            catch { }
            return null;
        }
    }
}
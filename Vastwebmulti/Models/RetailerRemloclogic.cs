using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Net;

namespace Vastwebmulti.Models
{
    public class RetailerRemloclogic
    {

        public static void checklocationbyRem(string userids, string chksts, out string remlocationsts, string latiloc, string longloc, ref string city, ref string addrss)
        {
            string straddrss = null;
            string strlocation = null;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            VastwebmultiEntities db = new VastwebmultiEntities();

            try
            {
                var alllocations = db.latlongstores.Where(x => x.latitude == latiloc && x.longitude == longloc).FirstOrDefault();
                if (alllocations != null)
                {
                    city = alllocations.city;
                    addrss = alllocations.Address;
                }
                else
                {
                    try
                    {
                        var client = new RestClient("https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=" + latiloc + "&lon=" + longloc);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.GET);
                        IRestResponse response = client.Execute(request);

                        dynamic ssss = JsonConvert.DeserializeObject(response.Content);
                        dynamic addresss = ssss.address;

                        var county = Convert.ToString(addresss.county);
                        var state_district = Convert.ToString(addresss.state_district);
                        var state = Convert.ToString(addresss.state);
                        var postcode = Convert.ToString(addresss.postcode);
                        straddrss = Convert.ToString(ssss.display_name);
                        strlocation = Convert.ToString(addresss);
                        city = Convert.ToString(addresss.town);
                        dynamic village = Convert.ToString(addresss.village);
                        if (string.IsNullOrEmpty(city))
                        {
                            city = addresss.town;
                        }

                        else if (string.IsNullOrEmpty(city))
                        {
                            city = addresss.city;
                        }
                        else if (string.IsNullOrEmpty(city))
                        {
                            city = addresss.suburb;
                        }
                        else if (string.IsNullOrEmpty(city))
                        {
                            city = addresss.village;
                        }
                        else if (string.IsNullOrEmpty(city))
                        {
                            city = state;
                        }
                    }
                    catch
                    {
                        if (string.IsNullOrEmpty(city))
                        {
                            var client = new RestClient("https://api.opencagedata.com/geocode/v1/json?key=60baa3a9b6154ef3a3ab635c8bd06767&q=" + latiloc + "," + longloc + "&pretty=1&no_annotations=1");
                            client.Timeout = -1;
                            var request = new RestRequest(Method.GET);
                            IRestResponse response = client.Execute(request);
                            dynamic getrespons = response.Content;
                            dynamic cnvrtdessrilise = JsonConvert.DeserializeObject(getrespons);
                            dynamic getoriginaldata = cnvrtdessrilise.results[0];
                            city = getoriginaldata.components.village;
                            straddrss = getoriginaldata.formatted;
                            strlocation = Convert.ToString(getoriginaldata.components);
                            if (string.IsNullOrEmpty(city))
                            {
                                city = getoriginaldata.components.town;
                            }

                            else if (string.IsNullOrEmpty(city))
                            {
                                city = getoriginaldata.components.city;
                            }
                            else if (string.IsNullOrEmpty(city))
                            {
                                city = getoriginaldata.components.suburb;
                            }
                            else if (string.IsNullOrEmpty(city))
                            {
                                city = getoriginaldata.components.village;
                            }
                            else if (string.IsNullOrEmpty(city))
                            {
                                city = getoriginaldata.components.state_district;
                            }


                        }
                    }
                    addrss = straddrss;
                    latlongstore tblloc = new latlongstore();
                    tblloc.city = city;
                    tblloc.latitude = latiloc;
                    tblloc.longitude = longloc;
                    tblloc.Address = straddrss;

                    tblloc.locations = strlocation;
                    db.latlongstores.Add(tblloc);
                    db.SaveChanges();
                }

            }
            catch
            {
                try
                {
                    if (string.IsNullOrEmpty(city))
                    {
                        var client = new RestClient("https://geocode.xyz/" + latiloc + "," + longloc + "?json=1 ");
                        client.Timeout = -1;
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("Cookie", "__cflb=0H28vTE11mXeuU6nLEGMumyL4X6iAPhyuASYFgbKjuX; xyzh=xyzh");
                        IRestResponse response = client.Execute(request);
                        dynamic rescontent = response.Content;

                        dynamic ssss = JsonConvert.DeserializeObject(rescontent);
                        var po = Convert.ToString(ssss.poi);
                        if (string.IsNullOrEmpty(city))
                        {
                            city = Convert.ToString(ssss.city);
                        }

                        var state = Convert.ToString(ssss.state);
                        var reg = Convert.ToString(ssss.region);
                        addrss = city + state;
                    }
                }
                catch { }


            }


            remlocationsts = null;

            if (chksts == "CHECK")
            {

                var locationinformation = db.latlongstores.Select(x => x.city.ToUpper());
                var retailermanagelocation = db.Manage_rem_Location_by_Admin.Where(sss => sss.userid == userids).ToList();
                if (retailermanagelocation.Count() > 0)
                {

                    var findexectlocation = db.Manage_rem_Location_by_Admin.Where(sss => sss.userid == userids && locationinformation.Contains(sss.nameofcity.Trim().ToUpper())).FirstOrDefault();
                    if (findexectlocation == null)
                    {
                        remlocationsts = "NOTALLOW";

                    }
                    else
                    {
                        remlocationsts = "ALLOW";
                    }
                }
                else
                {
                    remlocationsts = "ALLOW";
                }
            }
            else
            {
                remlocationsts = "ALLOW";
            }
        }

    }
}
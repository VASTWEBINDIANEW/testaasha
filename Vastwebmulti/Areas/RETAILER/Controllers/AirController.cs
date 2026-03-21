using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vastwebmulti.Areas.RETAILER.Models;
using Vastwebmulti.Areas.RETAILER.ViewModels;
using Vastwebmulti.Models;

namespace Vastwebmulti.Areas.RETAILER.Controllers
{
    [Authorize(Roles = "Retailer")]
    [Low_Bal_CustomFilter()]
    public class AirController : Controller
    {
        //string VastbazaarBaseUrl = "http://localhost:62146/";
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        VastwebmultiEntities db = new VastwebmultiEntities();
        public AirController()
        {
        }
        public AirController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #region Travel
        [HttpGet]
        [OutputCache(Duration = 2000, VaryByParam = "term")]
        public JsonResult GetSourceName(string term)
        {
            List<string> planets = (from p in db.Airport_info
                                    where p.Airport_Name.StartsWith(term) || p.City_Name.StartsWith(term) || p.Aiport_Code.StartsWith(term)
                                    select p.Airport_Name + "," + p.City_Name + "," + p.Country_name + "(" + p.Aiport_Code + ")").ToList<string>();
            //select p.AirportName).ToList<string>() ;
            return Json(planets, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult _AirFilter(AirSearchResultVM model)
        {
            var FirmName = db.Admin_details.FirstOrDefault().Companyname;
            ViewBag.FirmName = FirmName;
            return PartialView(model);
        }
        public ActionResult Search()
        {
            return RedirectToAction("Travel", "Home");
        }
        [HttpGet]
        public ActionResult Search(string[] txtSource, string[] txtDestination, DateTime[] txt_frm_date, DateTime? txt_to_date, int txtAdultCount, int txtChildCount, int txtInfantCount, int ddlclass, int JourneyType, bool? calanderFare, string ReturnType, string[] prefferedAirlines)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                ViewBag.chk = "post";
                ViewBag.txtSource = txtSource[0];
                ViewBag.txtDestination = txtDestination[0];
                ViewBag.txt_frm_date = txt_frm_date;
                ViewBag.txt_to_date = txt_to_date;
                ViewBag.txtAdultCount = txtAdultCount;
                ViewBag.txtChildCount = txtChildCount;
                ViewBag.txtInfantCount = txtInfantCount;
                ViewBag.TotalPaxCount = txtAdultCount + txtChildCount + txtInfantCount;
                ViewBag.TotalPaxCount1 = txtAdultCount + txtChildCount;
                var charge = db.Slab_Flight.Where(aa => aa.UserId == userid).ToList();
                ViewBag.domestic = charge.Where(aa => aa.IsDomestic == true).SingleOrDefault().marginPercentage;
                ViewBag.international = charge.Where(aa => aa.IsDomestic == false).SingleOrDefault().marginPercentage;
                var sts = db.Retailer_Details.Where(a => a.RetailerId == userid && a.Flighsts == "Y" && a.Flighsts != null).Any();
                if (sts == true)
                {
                    var token = string.Empty;
                    token = getAuthToken();
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        UpdateAuthToken();
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Failed to connect With provider.please try again.";
                        return RedirectToAction("Travel", "Home");
                    }
                    ViewBag.JourneyType = JourneyType;
                    if (txtSource.Length > 0 && string.IsNullOrWhiteSpace(txtSource[0]))
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Source is required.";
                        return RedirectToAction("Travel", "Home");
                    }
                    if (txtDestination.Length > 0 && string.IsNullOrWhiteSpace(txtDestination[0]))
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Destination is required.";
                        return RedirectToAction("Travel", "Home");
                    }
                    if (txt_frm_date == null || txt_frm_date[0].Date < DateTime.Now.Date)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Invalid departure date.";
                        return RedirectToAction("Travel", "Home");
                    }
                    if (txtAdultCount <= 0)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Minimum one adult treveller is required";
                        return RedirectToAction("Travel", "Home");
                    }
                    if (ddlclass <= 0)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Choose a class.";
                        return RedirectToAction("Travel", "Home");
                    }
                    if (JourneyType <= 0)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Journey type is required.";
                        return RedirectToAction("Travel", "Home");
                    }
                    List<Models.Segment> segments = new List<Models.Segment>();
                    for (int i = 0; i < txtSource.Length; i++)
                    {
                        Models.Segment sgmt = new Models.Segment();
                        sgmt.Destination = txtDestination[i].Contains(',') ? txtDestination[i].Substring(txtDestination[i].Length - 4, 3) : txtDestination[i];
                        sgmt.FlightCabinClass = ddlclass;
                        sgmt.Origin = txtSource[i].Contains(',') ? txtSource[i].Substring(txtSource[i].Length - 4, 3) : txtSource[i];
                        sgmt.PreferredArrivalTime = txt_frm_date[i];
                        sgmt.PreferredDepartureTime = txt_frm_date[i];
                        segments.Add(sgmt);
                    }
                    if ((JourneyType == 2 || JourneyType == 5 || JourneyType == 4) && txt_to_date != null)
                    {
                        var returnSegment = new Models.Segment();
                        returnSegment.Destination = txtSource[0].Substring(txtSource[0].Length - 4, 3);
                        returnSegment.FlightCabinClass = ddlclass;
                        returnSegment.Origin = txtDestination[0].Substring(txtDestination[0].Length - 4, 3);
                        returnSegment.PreferredArrivalTime = Convert.ToDateTime(txt_to_date);
                        returnSegment.PreferredDepartureTime = Convert.ToDateTime(txt_to_date);
                        segments.Add(returnSegment);
                    }
                    var reqObject = new SearchAirRequestModel();
                    reqObject.AdultCount = txtAdultCount;
                    reqObject.ChildCount = txtChildCount;
                    reqObject.DirectFlight = false;
                    reqObject.InfantCount = txtInfantCount;
                    reqObject.JourneyType = JourneyType;
                    reqObject.OneStopFlight = false;
                    reqObject.Segments = segments;
                    if (JourneyType == 5)
                    {
                        reqObject.PreferredAirlines = prefferedAirlines;
                        reqObject.Sources = ReturnType == "GDS" ? new string[] { "GDS" } : prefferedAirlines;//optional
                    }
                    else
                    {
                        reqObject.PreferredAirlines = null;
                        reqObject.Sources = null;
                    }
                    var client1 = new RestClient(VastbazaarBaseUrl + "api/Air/Margin");
                    var request1 = new RestRequest(Method.POST);
                    request1.AddHeader("authorization", "bearer " + token); IRestResponse response = client1.Execute(request1);
                    var respchk = response.Content;
                    dynamic resp = JsonConvert.DeserializeObject(respchk);
                    ViewBag.vastdomestic = Convert.ToDecimal(resp.Content.ADDINFO.domestic);
                    ViewBag.vastinternational = Convert.ToDecimal(resp.Content.ADDINFO.international);


                    //var res =  TBO_Utils.GetResponse(JsonConvert.SerializeObject(reqObject), FlightURLS.BaseAddress+ FlightURLS.SearchFlight);
                    //  var client = new RestClient(VastbazaarBaseUrl + "api/AirTourista/Search");
                    var client = new RestClient(VastbazaarBaseUrl + "api/Air/Search");
                    var request = new RestRequest(Method.POST);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("accept-encoding", "gzip");

                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                    request.AddBody(reqObject);
                    Task<IRestResponse> task = Task.Run(() =>
                    {
                        return client.Execute(request);
                    });
                    bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                    if (isCompletedSuccessfully)
                    {
                        //IRestResponse response = client.Execute(request);
                        if (task.Result.StatusCode == HttpStatusCode.BadRequest)
                        {
                            UpdateAuthToken();
                            TempData["Status"] = "Failed";
                            TempData["Message"] = "Something went wrong,please later!";
                            return RedirectToAction("Travel", "Home");
                        }
                        var respo = JsonConvert.DeserializeObject<AirSearchResultVM>(task.Result.Content);

                        if (string.IsNullOrWhiteSpace(task.Result.Content))
                        {
                            TempData["Status"] = "Failed";
                            TempData["Message"] = "Server down please try later";
                            return RedirectToAction("Travel", "Home");
                        }
                        if (respo.Content == null || respo.Content.Addinfo == null || respo.Content.Addinfo.Error.ErrorCode != 0)
                        {
                            TempData["Status"] = "Failed";
                            TempData["Message"] = respo.Content.Addinfo.Error.ErrorMessage;
                            return RedirectToAction("Travel", "Home");
                        }
                        TempData["AdultCount"] = txtAdultCount;
                        TempData["ChildCount"] = txtChildCount;
                        TempData["InfantCount"] = txtInfantCount;
                        var sourceCountry = string.Empty;
                        if (JourneyType == 1)
                        {
                            sourceCountry = txtSource[0].Contains(',') ? txtSource[0].Substring(txtSource[0].LastIndexOf(','), txtSource[0].IndexOf('(') - txtSource[0].LastIndexOf(',')) : txtSource[0];
                            if (txtDestination[0].Contains(sourceCountry))
                            {
                                ViewBag.ShowCalanderFare = true;
                            }
                            else
                            {
                                ViewBag.ShowCalanderFare = false;
                            }
                        }
                        else
                        {
                            ViewBag.ShowCalanderFare = false;
                        }
                        if (calanderFare != null && calanderFare == true)
                        {
                            ViewBag.ShowCalanderFare = false;
                        }
                        ViewBag.txt_frm_date = txt_frm_date;
                        #region checkIsDomestic
                        sourceCountry = string.Empty;
                        sourceCountry = txtSource[0].Contains(',') ? txtSource[0].Substring(txtSource[0].LastIndexOf(','), txtSource[0].IndexOf('(') - txtSource[0].LastIndexOf(',')) : txtSource[0];
                        if (txtDestination[0].Contains(sourceCountry))
                        {
                            ViewBag.IsDomestic = true;
                            TempData["IsDomestic"] = "DomesticFlight";
                            var entry = db.Convence_Fees.FirstOrDefault(a => a.RetailerId == userid && a.Role == "DomesticFlight");
                            if (entry != null)
                            {
                                ViewBag.RetailerMarkup = entry.Amount;
                            }
                            else
                            {
                                ViewBag.RetailerMarkup = 0;
                            }
                        }
                        else
                        {
                            ViewBag.IsDomestic = false;
                            TempData["IsDomestic"] = "InternationalFlight";
                            var entry = db.Convence_Fees.FirstOrDefault(a => a.RetailerId == userid && a.Role == "InternationalFlight");
                            if (entry != null)
                            {
                                ViewBag.RetailerMarkup = entry.Amount;
                            }
                            else
                            {
                                ViewBag.RetailerMarkup = 0;
                            }

                        }
                        #endregion
                        return View(respo);
                    }
                    else
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Unable to get flight search result,try again.";
                        return RedirectToAction("Travel", "Home");
                    }
                }
                else
                {

                    TempData["msg"] = "Your Flight Status Inactive Please Contact Admin";
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            catch
            {

                TempData["Status"] = "Failed";
                TempData["Message"] = "Failed to connect with provider.please try again.";
                return RedirectToAction("Travel", "Home");
            }
        }
        [HttpGet]
        public PartialViewResult _CalanderFare(string txtSource, string txtDestination, DateTime txt_frm_date)
        {
            try
            {
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    return PartialView(new CalanderFareVM());
                }
                List<Models.Segment> segments = new List<Models.Segment> { new Models.Segment
            { Destination = txtDestination,
            FlightCabinClass = 1,
            Origin = txtSource,
            PreferredArrivalTime = txt_frm_date.AddDays(1),
            PreferredDepartureTime = txt_frm_date.AddDays(1)
            } };
                var reqObject = new
                {
                    JourneyType = 1,
                    //PreferredAirlines = null,
                    Segments = segments,
                    //Sources = null;//optional
                };
                //var res =  TBO_Utils.GetResponse(JsonConvert.SerializeObject(reqObject), FlightURLS.BaseAddress+ FlightURLS.SearchFlight);
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/CalendarFare");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("accept-encoding", "gzip");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(reqObject);
                Task<IRestResponse> task = Task.Run(() =>
                {
                    return client.Execute(request);
                });
                //IRestResponse response = client.Execute(request);
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                if (isCompletedSuccessfully)
                {
                    var respo = JsonConvert.DeserializeObject<CalanderFareVM>(task.Result.Content);
                    return PartialView(respo);
                }
                else
                {
                    return PartialView(new CalanderFareVM());
                }


            }
            catch
            {
                return PartialView(new CalanderFareVM());
            }
        }
        [HttpPost]
        public ActionResult CalanderFare(string txtSource, string txtDestination, string month, string type)
        {
            try
            {

                DateTime txt_frm_date = Convert.ToDateTime(month);
                DateTime datesend;
                if (type == "pre")
                {
                    datesend = txt_frm_date.AddMonths(-1);
                }
                else
                {
                    datesend = txt_frm_date.AddMonths(1);
                }
                if (datesend.Month == DateTime.Now.Month)
                {
                    if (datesend.Year == DateTime.Now.Year)
                    {
                        datesend = DateTime.Now.Date;
                    }
                }
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    return PartialView(new CalanderFareVM());
                }
                List<Models.Segment> segments = new List<Models.Segment> { new Models.Segment
            { Destination = txtDestination,
            FlightCabinClass = 1,
            Origin = txtSource,
            PreferredArrivalTime = datesend,
            PreferredDepartureTime = datesend
            }};
                var reqObject = new
                {
                    JourneyType = 1,
                    //PreferredAirlines = null,
                    Segments = segments,
                    //Sources = null;//optional
                };
                //var res =  TBO_Utils.GetResponse(JsonConvert.SerializeObject(reqObject), FlightURLS.BaseAddress+ FlightURLS.SearchFlight);
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/CalendarFare");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("accept-encoding", "gzip");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(reqObject);
                Task<IRestResponse> task = Task.Run(() =>
                {
                    return client.Execute(request);
                });
                //IRestResponse response = client.Execute(request);
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                if (isCompletedSuccessfully)
                {
                    var respo = JsonConvert.DeserializeObject<CalanderFareVM>(task.Result.Content);

                    //    var respo1=respo.Content.Addinfo.SearchResults.Single().Fare
                    //        var respo1 = respo.Content.Addinfo.SearchResults.Single().BaseFare
                    //           var respo1 = respo.Content.Addinfo.SearchResults.Single().IsLowestFareOfMonth
                    //       var respo1 = respo.Content.Addinfo.SearchResults.Single().AirlineName
                    return PartialView("_CalanderFare", respo);
                }
                else
                {
                    return PartialView("_CalanderFare", "notfound");
                }


            }
            catch
            {
                return PartialView("_CalanderFare", "notfound");
            }
        }
        [HttpGet]
        public PartialViewResult _TypeReturnAirTopSection(AirTopSectionModel model)
        {
            return PartialView(model);
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult ShowSelectedFlightItinarary(AirTopSectionModel model)
        {
            return PartialView("_TypeReturnAirTopSection", model);
        }
        [HttpGet]
        public ActionResult FlightDetails(string TraceId, string[] ResultIndex, int JourneyType = 0)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                string isdomestic = Convert.ToString(TempData.Peek("IsDomestic"));
                var adultcount = (int)TempData.Peek("AdultCount");
                var childcount = (int)TempData.Peek("ChildCount");
                var infantcount = (int)TempData.Peek("InfantCount");
                var totalcount = (adultcount + childcount + infantcount);
                var totalcount1 = (adultcount + childcount);
                var entry = db.Convence_Fees.FirstOrDefault(a => a.RetailerId == userid && a.Role == isdomestic);
                if (entry != null)
                {
                    ViewBag.RetailerMarkup = entry.Amount * totalcount1;
                }
                else
                {
                    ViewBag.RetailerMarkup = 0;
                }
                FlightDetailsVM model = new FlightDetailsVM();
                model.TraceId = TraceId;
                model.JourneyType = JourneyType;
                if (JourneyType == 5 && ResultIndex.Length > 1)
                {
                    model.InBoundResultIndexForSpacailReturn = ResultIndex[1];
                }
                FlightOnwardDetail Onward = new FlightOnwardDetail();
                Onward.ResultIndex = ResultIndex[0];
                var farerule = getFareRule(TraceId, (JourneyType == 5 && ResultIndex.Length == 2) ? ResultIndex[0] + "," + ResultIndex[1] : ResultIndex[0]);
                if (farerule != null && farerule.Content.Addinfo.Error.ErrorCode == 0)
                {
                    Onward.FareRuleVM = farerule;
                    TempData["FareRuleOB"] = Onward.FareRuleVM;
                    ViewBag.FareRuleOB = Onward.FareRuleVM;
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = farerule.Content.Addinfo.Error.ErrorMessage;
                    return RedirectToAction("Travel", "Home");
                }
                var farequote = getFareQuote(TraceId, (JourneyType == 5 && ResultIndex.Length == 2) ? ResultIndex[0] + "," + ResultIndex[1] : ResultIndex[0]);
                if (farequote != null && farequote.Content.Addinfo.Error.ErrorCode == 0)
                {
                    Onward.FareQuoteVM = farequote;
                    if (farequote.StatusCode == 200 && farequote.Content.Addinfo.Error.ErrorCode == 0)
                    {
                        if (farequote.Content.Addinfo.Results.IsLcc)
                        {
                            Onward.SSRLcc = getSSRLCC(TraceId, (JourneyType == 5 && ResultIndex.Length == 2) ? ResultIndex[0] + "," + ResultIndex[1] : ResultIndex[0]);
                        }
                        else
                        {
                            Onward.SSRNonLcc = getSSRNonLCC(TraceId, (JourneyType == 5 && ResultIndex.Length == 2) ? ResultIndex[0] + "," + ResultIndex[1] : ResultIndex[0]);
                        }
                    }
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = farequote.Content.Addinfo.Error.ErrorMessage;
                    return RedirectToAction("Travel", "Home");
                }
                var sourceCountryCode = farequote.Content.Addinfo.Results.Segments.First().First().Origin.Airport.CountryCode;
                var destinCountryCode = farequote.Content.Addinfo.Results.Segments.First().Last().Destination.Airport.CountryCode;
                if (sourceCountryCode == destinCountryCode)
                {
                    model.isDomastic = true;
                }
                else
                {
                    model.isDomastic = false;
                }
                model.Onward = Onward;

                TempData["FareQuoteOB"] = Onward.FareQuoteVM;
                if (model.isDomastic && ResultIndex.Length == 2 && JourneyType == 2 && JourneyType != 5)
                {
                    FlightInwardDetail Inward = new FlightInwardDetail();
                    Inward.ResultIndex = ResultIndex[1];
                    var farerule1 = getFareRule(TraceId, ResultIndex[1]);
                    if (farerule1 != null && farerule1.Content.Addinfo.Error.ErrorCode == 0)
                    {
                        Inward.FareRuleVM = farerule1;
                        TempData["FareRuleIB"] = Inward.FareRuleVM;
                        ViewBag.FareRuleIB = Inward.FareRuleVM;
                    }
                    else
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = farerule1.Content.Addinfo.Error.ErrorMessage;
                        return RedirectToAction("Travel", "Home");
                    }
                    var farequote1 = getFareQuote(TraceId, ResultIndex[1]);
                    if (farequote1 != null && farequote1.Content.Addinfo.Error.ErrorCode == 0)
                    {
                        Inward.FareQuoteVM = farequote1;
                        if (farequote1.StatusCode == 200 && farequote1.Content.Addinfo.Error.ErrorCode == 0)
                        {
                            if (farequote1.Content.Addinfo.Results.IsLcc)
                            {
                                Inward.SSRLcc = getSSRLCC(TraceId, ResultIndex[1]);
                                TempData["InwardSSRLcc"] = Inward.SSRLcc;
                            }
                            else
                            {
                                //Inward.SSRLcc = getSSRLCC(TraceId, ResultIndex[1]);
                                Inward.SSRNonLcc = getSSRNonLCC(TraceId, ResultIndex[1]);
                                TempData["InwardSSRNonLcc"] = Inward.SSRNonLcc;
                            }
                        }
                    }
                    else
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Failed to get Fare.\ncontact to admin.";
                        return RedirectToAction("Travel", "Home");
                    }
                    model.Inward = Inward;

                    TempData["FareQuoteIB"] = Inward.FareQuoteVM;
                }
                adultcount = (int)TempData.Peek("AdultCount");
                childcount = (int)TempData.Peek("ChildCount");
                infantcount = (int)TempData.Peek("InfantCount");
                var adultPaxList = Enumerable.Repeat(new Vastwebmulti.Areas.RETAILER.ViewModels.Passenger() { PaxType = 1.ToString() }, adultcount).ToList();
                model.AdultPax = adultPaxList;
                if (childcount > 0)
                {
                    var childPaxList = Enumerable.Repeat(new Vastwebmulti.Areas.RETAILER.ViewModels.Passenger() { PaxType = 2.ToString() }, childcount).ToList();
                    model.ChildPax = childPaxList;
                }
                if (infantcount > 0)
                {
                    var infantPaxList = Enumerable.Repeat(new Vastwebmulti.Areas.RETAILER.ViewModels.Passenger() { PaxType = 3.ToString() }, infantcount).ToList();
                    model.InfantPax = infantPaxList;
                }
                model.AdultPax[0].IsLeadPax = true;
                #region dropdownsSeting


                List<SelectListItem> contryCode = (from p in db.Airport_info
                                                   select new SelectListItem { Text = p.Country_name, Value = p.Country_Code, Selected = p.Country_Code == "IN" ? true : false }).ToList();



                ViewBag.contryCode = contryCode;

                List<SelectListItem> Nationality = (from p in db.Airport_info
                                                    select new SelectListItem { Text = p.Nationalty, Value = p.Country_Code, Selected = p.Nationalty == "Indian" ? true : false }).ToList();
                ViewBag.Nationality = Nationality;

                #endregion
                return View(model);
            }
            catch
            {
                return RedirectToAction("Travel", "Home");
            }
        }
        public FareRuleVM getFareRule(string TraceId, string ResultIndex)
        {
            try
            {
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    new FareRuleVM();
                }
                var reqObject = new
                {
                    TraceId = TraceId,
                    ResultIndex = ResultIndex
                };
                //var res =  TBO_Utils.GetResponse(JsonConvert.SerializeObject(reqObject), FlightURLS.BaseAddress+ FlightURLS.SearchFlight);
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/FareRule");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(reqObject);
                //IRestResponse response = client.Execute(request);
                Task<IRestResponse> task = Task.Run(() =>
                {
                    return client.Execute(request);
                });
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                if (isCompletedSuccessfully)
                {
                    var respo = JsonConvert.DeserializeObject<FareRuleVM>(task.Result.Content);

                    //respo.Content.Addinfo.FareRules

                    return respo;
                }
                else
                {
                    return new FareRuleVM();
                }

            }
            catch
            {
                return new FareRuleVM();
            }
        }

        public ActionResult getfarerules_search(string TraceId, string ResultIndex)
        {
            try
            {
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    new FareRuleVM();
                }
                var reqObject = new
                {
                    TraceId = TraceId,
                    ResultIndex = ResultIndex
                };
                //var res =  TBO_Utils.GetResponse(JsonConvert.SerializeObject(reqObject), FlightURLS.BaseAddress+ FlightURLS.SearchFlight);
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/FareRule");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(reqObject);
                //IRestResponse response = client.Execute(request);
                Task<IRestResponse> task = Task.Run(() =>
                {
                    return client.Execute(request);
                });
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                if (isCompletedSuccessfully)
                {
                    var respo = JsonConvert.DeserializeObject<FareRuleVM>(task.Result.Content);

                    //respo.Content.Addinfo.FareRules

                    return Json(respo.Content.Addinfo, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    return Json("", JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public FareQuoteVM getFareQuote(string TraceId, string ResultIndex)
        {

            var token = string.Empty;
            token = getAuthToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return new FareQuoteVM();
            }
            var reqObject = new
            {
                TraceId = TraceId,
                ResultIndex = ResultIndex
            };
            //var res =  TBO_Utils.GetResponse(JsonConvert.SerializeObject(reqObject), FlightURLS.BaseAddress+ FlightURLS.SearchFlight);
            var client = new RestClient(VastbazaarBaseUrl + "api/Air/FareQuote");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
            request.AddBody(reqObject);
            //IRestResponse response = client.Execute(request);
            Task<IRestResponse> task = Task.Run(() =>
            {
                return client.Execute(request);
            });
            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
            if (isCompletedSuccessfully)
            {
                var respo = JsonConvert.DeserializeObject<FareQuoteVM>(task.Result.Content);

                return respo;
            }
            else
            {
                return new FareQuoteVM();
            }

        }
        public SSRLcc getSSRLCC(string TraceId, string ResultIndex)
        {
            var token = string.Empty;
            token = getAuthToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return new SSRLcc();
            }
            var reqObject = new
            {
                TraceId = TraceId,
                ResultIndex = ResultIndex
            };
            //var res =  TBO_Utils.GetResponse(JsonConvert.SerializeObject(reqObject), FlightURLS.BaseAddress+ FlightURLS.SearchFlight);
            var client = new RestClient(VastbazaarBaseUrl + "api/Air/SSR");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
            request.AddBody(reqObject);
            //IRestResponse response = client.Execute(request);
            Task<IRestResponse> task = Task.Run(() =>
            {
                return client.Execute(request);
            });
            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
            if (isCompletedSuccessfully)
            {
                var respo = JsonConvert.DeserializeObject<SSRLcc>(task.Result.Content);


                return respo;
            }
            else
            {
                return new SSRLcc();
            }
        }
        public SSRNonLcc getSSRNonLCC(string TraceId, string ResultIndex)
        {
            var token = string.Empty;
            token = getAuthToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return new SSRNonLcc();
            }
            var reqObject = new
            {
                TraceId = TraceId,
                ResultIndex = ResultIndex
            };
            //var res =  TBO_Utils.GetResponse(JsonConvert.SerializeObject(reqObject), FlightURLS.BaseAddress+ FlightURLS.SearchFlight);
            var client = new RestClient(VastbazaarBaseUrl + "api/Air/SSR");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
            request.AddBody(reqObject);
            //IRestResponse response = client.Execute(request);
            Task<IRestResponse> task = Task.Run(() =>
            {
                return client.Execute(request);
            });
            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
            if (isCompletedSuccessfully)
            {
                var respo = JsonConvert.DeserializeObject<SSRNonLcc>(task.Result.Content);

                return respo;
            }
            else
            {
                return new SSRNonLcc();
            }
        }
        [HttpGet]
        public ActionResult BookingReview()
        {
            return RedirectToAction("Travel", "Home");
        }
        [HttpPost]
        public ActionResult BookingReview(FlightDetailsVM model)
        {
            try
            {
                // set Gender
                if (model.AdultPax != null)
                {
                    model.AdultPax.ForEach(a =>
                    {
                        if (a.Title == "Mr")
                        {
                            a.Gender = Gender.Male;
                        }
                        else if (a.Title == "Ms")
                        {
                            a.Gender = Gender.Female;
                        }
                        else if (a.Title == "Mrs")
                        {
                            a.Gender = Gender.Female;
                        }
                        else if (a.Title == "Miss")
                        {
                            a.Gender = Gender.Female;
                        }
                    });
                }
                if (model.ChildPax != null)
                {
                    model.ChildPax.ForEach(a =>
                    {
                        if (a.Title == "Mr")
                        {
                            a.Gender = Gender.Male;
                        }
                        else if (a.Title == "Ms")
                        {
                            a.Gender = Gender.Female;
                        }
                        else if (a.Title == "Mrs")
                        {
                            a.Gender = Gender.Female;
                        }
                        else if (a.Title == "Miss")
                        {
                            a.Gender = Gender.Female;
                        }
                    });
                }
                if (model.InfantPax != null)
                {
                    model.InfantPax.ForEach(a =>
                    {
                        if (a.Title == "Mr")
                        {
                            a.Gender = Gender.Male;
                        }
                        else if (a.Title == "Ms")
                        {
                            a.Gender = Gender.Female;
                        }
                        else if (a.Title == "Mrs")
                        {
                            a.Gender = Gender.Female;
                        }
                        else if (a.Title == "Miss")
                        {
                            a.Gender = Gender.Female;
                        }
                    });
                }
                var userid = User.Identity.GetUserId();
                string isdomestic = Convert.ToString(TempData.Peek("IsDomestic"));
                var entry = db.Convence_Fees.FirstOrDefault(a => a.RetailerId == userid && a.Role == isdomestic);
                var adultcount = (int)TempData.Peek("AdultCount");
                var childcount = (int)TempData.Peek("ChildCount");
                var infantcount = (int)TempData.Peek("InfantCount");
                var totalcount = (adultcount + childcount + infantcount);
                var totalcount1 = (adultcount + childcount);
                if (entry != null)
                {
                    ViewBag.RetailerMarkup = entry.Amount * totalcount1;
                }
                else
                {
                    ViewBag.RetailerMarkup = 0;
                }
                if (model.JourneyType == 0)
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
                var xx = TempData.Peek("FareRuleOB");
                var yy = TempData.Peek("FareQuoteOB");
                var OBFareRule = xx;//getFareRule(model.TraceId, (model.JourneyType == 5 && model.InBoundResultIndexForSpacailReturn != null) ? model.Onward.ResultIndex + "," + model.InBoundResultIndexForSpacailReturn : model.Onward.ResultIndex);
                                    //TempData["FareRuleOB"] = OBFareRule;
                ViewBag.FareRuleOB = OBFareRule;
                model.Onward.FareQuoteVM = (FareQuoteVM)yy;//getFareQuote(model.TraceId, (model.JourneyType == 5 && model.InBoundResultIndexForSpacailReturn != null) ? model.Onward.ResultIndex + "," + model.InBoundResultIndexForSpacailReturn : model.Onward.ResultIndex);
                if (model.Onward.FareQuoteVM == null || model.Onward.FareQuoteVM.Content == null || model.Onward.FareQuoteVM.Content.Addinfo.Results == null)
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Failed to get Fare.contact to admin.";
                    return RedirectToAction("Travel", "Home");
                }
                if (model.Onward.FareQuoteVM.Content.Addinfo.Error.ErrorCode > 0)
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = model.Onward.FareQuoteVM.Content.Addinfo.Error.ErrorMessage;
                    return RedirectToAction("Travel", "Home");
                }
                if (model.isDomastic && model.Inward != null && model.JourneyType == 2)
                {
                    var IBFareRule = TempData.Peek("FareRuleIB");//getFareRule(model.TraceId, model.Inward.ResultIndex);
                    var IBFareQuote = TempData.Peek("FareQuoteIB");
                    //TempData["FareRuleIB"] = IBFareRule;
                    ViewBag.FareRuleIB = IBFareRule;
                    model.Inward.FareQuoteVM = (FareQuoteVM)IBFareQuote;//getFareQuote(model.TraceId, model.Inward.ResultIndex);
                }
                #region SetCountry


                model.AdultPax.ForEach(a => a.CountryName = getCountryNameByCode(a.CountryCode));
                #endregion
                return View(model);
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Failed to connect with provider.please try again.";
                return RedirectToAction("Travel", "Home");
            }
        }
        public string getCountryNameByCode(string Code)
        {
            var name = string.Empty;
            return name = (from p in db.Airport_info
                           where p.Country_Code == Code
                           select p.Country_Code).FirstOrDefault();
        }
        public string Book(FlightDetailsVM model, List<PassengerNonLcc> lstPax, bool isForOnward, out string ResponseString)
        {
            string BookingResponses = string.Empty;
            ResponseString = string.Empty;
            lstPax.ForEach(a =>
            {
                if (a.PaxType == "3")
                {
                    a.Baggage = null;
                }
            });
            var token = string.Empty;
            try
            {
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    return null;
                }
                var userid = User.Identity.GetUserId();

                if (isForOnward)
                {
                    var reqObj = new
                    {
                        ResultIndex = model.Onward.FareQuoteVM.Content.Addinfo.Results.ResultIndex,
                        Passengers = lstPax,
                        TraceId = model.Onward.FareQuoteVM.Content.Addinfo.TraceId
                    };
                    System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                    System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                    db.proc_PNRBookingForNonLccFlight(userid, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare,
                    model.TraceId, JsonConvert.SerializeObject(reqObj), Idno, IsSuccess, Message);
                    var client = new RestClient(VastbazaarBaseUrl + "api/Air/Book");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                    request.AddBody(reqObj);
                    IRestResponse response = client.Execute(request);
                    dynamic respo = JsonConvert.DeserializeObject(response.Content);

                    ResponseString = response.Content;
                    //dynamic respo = JsonConvert.DeserializeObject(responseJs);
                    if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                    {
                        if (respo.Content.ADDINFO.Response.IsPriceChanged == true)
                        {
                            reqObj = new
                            {
                                ResultIndex = model.Onward.FareQuoteVM.Content.Addinfo.Results.ResultIndex,
                                Passengers = lstPax,
                                TraceId = model.Onward.FareQuoteVM.Content.Addinfo.TraceId
                            };
                            //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                            request.AddBody(reqObj);
                            response = client.Execute(request);
                            respo = JsonConvert.DeserializeObject(response.Content);
                            ResponseString = response.Content;

                            if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                            {
                                BookingResponses = "Success";
                                var idno = Convert.ToInt32(Idno.Value);
                                var entry = db.TBO_AirBookingForNonLccDetail.SingleOrDefault(a => a.idno == idno);
                                entry.PNR = respo.Content.ADDINFO.Response.PNR;
                                entry.BookingID = respo.Content.ADDINFO.Response.BookingId;
                                entry.TicketStatus = "Successful";
                                entry.TicketStatusCode = 1;
                                entry.ItinerarySource = respo.Content.ADDINFO.Response.FlightItinerary.Source;
                                entry.ResponseJson = response.Content;
                                entry.LeadPaxFirstName = lstPax.First().FirstName;
                                entry.LeadPaxLastName = lstPax.First().LastName;
                                entry.OfferedFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                                entry.PassengerName = string.Join(",", lstPax.Select(a => a.FirstName + " " + a.LastName));
                                entry.AirlineName = model.Onward.FareQuoteVM.Content.Addinfo.Results.Segments.First().First().Airline.AirlineName;
                                db.SaveChanges();
                            }
                            else
                            {
                                BookingResponses = "Failed";
                            }
                        }
                        else
                        {
                            BookingResponses = "Success";
                            var idno = Convert.ToInt32(Idno.Value);
                            var entry = db.TBO_AirBookingForNonLccDetail.SingleOrDefault(a => a.idno == idno);
                            entry.PNR = respo.Content.ADDINFO.Response.PNR;
                            entry.BookingID = respo.Content.ADDINFO.Response.BookingId;
                            entry.TicketStatus = "Successful";
                            entry.TicketStatusCode = 1;
                            entry.ItinerarySource = respo.Content.ADDINFO.Response.FlightItinerary.Source;
                            entry.ResponseJson = response.Content;
                            entry.LeadPaxFirstName = lstPax.First().FirstName;
                            entry.LeadPaxLastName = lstPax.First().LastName;
                            entry.OfferedFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                            entry.PassengerName = string.Join(",", lstPax.Select(a => a.FirstName + " " + a.LastName));
                            entry.AirlineName = model.Onward.FareQuoteVM.Content.Addinfo.Results.Segments.First().First().Airline.AirlineName;
                            db.SaveChanges();
                        }

                    }
                    else
                    {
                        BookingResponses = "Failed";
                    }
                }
                else
                {
                    var reqObj = new
                    {
                        ResultIndex = model.Inward.FareQuoteVM.Content.Addinfo.Results.ResultIndex,
                        Passengers = lstPax,
                        TraceId = model.Inward.FareQuoteVM.Content.Addinfo.TraceId
                    };
                    System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                    System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                    db.proc_PNRBookingForNonLccFlight(userid, model.Inward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare,
                    model.TraceId, JsonConvert.SerializeObject(reqObj), Idno, IsSuccess, Message);
                    var client = new RestClient(VastbazaarBaseUrl + "api/Air/Book");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                    request.AddBody(reqObj);
                    IRestResponse response = client.Execute(request);
                    dynamic respo = JsonConvert.DeserializeObject(response.Content);
                    ResponseString = response.Content;

                    if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                    {
                        if (Convert.ToBoolean(respo.Content.ADDINFO.Response.IsPriceChanged))
                        {
                            //  var Counter = 0;

                            reqObj = new
                            {
                                ResultIndex = model.Inward.FareQuoteVM.Content.Addinfo.Results.ResultIndex,
                                Passengers = lstPax,
                                TraceId = model.Inward.FareQuoteVM.Content.Addinfo.TraceId
                            };
                            //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                            request.AddBody(reqObj);
                            response = client.Execute(request);
                            respo = JsonConvert.DeserializeObject(response.Content);
                            ResponseString = response.Content;

                            if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                            {
                                BookingResponses = "Success";
                                var idno = Convert.ToInt32(Idno.Value);
                                var entry = db.TBO_AirBookingForNonLccDetail.SingleOrDefault(a => a.idno == idno);
                                entry.PNR = respo.Content.ADDINFO.Response.PNR;
                                entry.BookingID = respo.Content.ADDINFO.Response.BookingId;
                                entry.TicketStatus = "Successful";
                                entry.TicketStatusCode = 1;
                                entry.ItinerarySource = respo.Content.ADDINFO.Response.FlightItinerary.Source;
                                entry.ResponseJson = response.Content;
                                entry.LeadPaxFirstName = lstPax.First().FirstName;
                                entry.LeadPaxLastName = lstPax.First().LastName;
                                entry.OfferedFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                                entry.PassengerName = string.Join(",", lstPax.Select(a => a.FirstName + " " + a.LastName));
                                entry.AirlineName = model.Onward.FareQuoteVM.Content.Addinfo.Results.Segments.First().First().Airline.AirlineName;
                                db.SaveChanges();
                            }
                            else
                            {
                                BookingResponses = "Failed";
                            }
                        }
                        else
                        {
                            BookingResponses = "Success";
                            var idno = Convert.ToInt32(Idno.Value);
                            var entry = db.TBO_AirBookingForNonLccDetail.SingleOrDefault(a => a.idno == idno);
                            entry.PNR = respo.Content.ADDINFO.Response.PNR;
                            entry.BookingID = respo.Content.ADDINFO.Response.BookingId;
                            entry.TicketStatus = "Successful";
                            entry.TicketStatusCode = 1;
                            entry.ItinerarySource = respo.Content.ADDINFO.Response.FlightItinerary.Source;
                            entry.ResponseJson = response.Content;
                            entry.LeadPaxFirstName = lstPax.First().FirstName;
                            entry.LeadPaxLastName = lstPax.First().LastName;
                            entry.OfferedFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                            entry.PassengerName = string.Join(",", lstPax.Select(a => a.FirstName + " " + a.LastName));
                            entry.AirlineName = model.Onward.FareQuoteVM.Content.Addinfo.Results.Segments.First().First().Airline.AirlineName;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        BookingResponses = "Failed";
                    }
                }
                return BookingResponses;
            }
            catch
            {
                return null;
            }
        }
        [HttpGet]
        ActionResult BookingQueue()
        {//TODO : Showing Hold PNR List OR Release a PNR from History
            var lstQueue = db.TBO_AirBookingForNonLccDetail.ToList();
            return View(lstQueue);
        }
        public string TicketNonLCC(TicketNonLcc model, decimal opnlCharge, string LeadPaxFName, string LeadPaxLName, string PaxNames, string AirlineName, string EnduserIp, string TokenId, bool isDomestic)
        {
            var userid = User.Identity.GetUserId();
            var token = string.Empty;
            token = getAuthToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return "Failed";
            }
            decimal amtt_add = 0;
            decimal amtt_total = 0;
            var client1 = new RestClient(VastbazaarBaseUrl + "api/Air/Margin");
            var request1 = new RestRequest(Method.POST);
            request1.AddHeader("authorization", "bearer " + token);
            IRestResponse response1 = client1.Execute(request1);
            var respchk = response1.Content;
            dynamic resp = JsonConvert.DeserializeObject(respchk);
            if (isDomestic == true)
            {
                amtt_add = Convert.ToDecimal(resp.Content.ADDINFO.domestic);
            }
            else
            {
                amtt_add = Convert.ToDecimal(resp.Content.ADDINFO.international);
            }
            amtt_total = (Convert.ToDecimal(model.OfferedFare) * amtt_add) / 100;
            var client = new RestClient(VastbazaarBaseUrl + "api/Air/TicketNonLcc");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
            request.AddBody(model);

            System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
            System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
            System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
            //  db.proc_FlightBookingPayment(userid, model.TraceId, model.OfferedFare + opnlCharge, model.PublishedFare + opnlCharge, LeadPaxFName, LeadPaxLName, PaxNames, AirlineName, isDomestic, JsonConvert.SerializeObject(model), Idno,1,0,0, IsSuccess, Message);
            var hhhh = db.proc_FlightBookingPayment(userid, model.TraceId, model.OfferedFare - amtt_total, model.PublishedFare, LeadPaxFName, LeadPaxLName, PaxNames, AirlineName, isDomestic, JsonConvert.SerializeObject(model), Idno, 1, 0, 0, IsSuccess, Message).SingleOrDefault();
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
                    Details = "Flight Boking ",
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
                    Details = "Flight Boking ",

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
                    Details = "Flight Boking ",
                    RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                    Usertype = "Master"
                };
                back.info(model2);
            }
            catch { }

            if (hhhh.msg == "Success")
            {
                if (Convert.ToBoolean(IsSuccess.Value) == true)
                {
                    int TicketStatus = 0;
                    IRestResponse response = client.Execute(request);
                    dynamic respo = JsonConvert.DeserializeObject(response.Content);

                    if (respo.StatusCode == 200 && respo.Content.ADDINFO.Error.ErrorCode == 0 && respo.Content.ADDINFO.Response.TicketStatus == 1)
                    {

                        try { TicketStatus = respo.Content.ADDINFO.Response.TicketStatus; } catch { }
                        db.proc_UpdateFlightBooking(Idno.Value.ToString(), userid, model.OfferedFare + opnlCharge,
                            model.PublishedFare + opnlCharge, response.Content, 0, TicketStatus, EnduserIp, TokenId, model.PNR, model.BookingId, IsSuccess, Message);
                        return "Success";
                    }
                    else
                    {
                        //db.proc_UpdateFlightBooking(Idno.Value.ToString(), model.OfferedFare, responseJs, 1, IsSuccess, Message);
                        db.proc_UpdateFlightBooking(Idno.Value.ToString(), userid, model.OfferedFare + opnlCharge,
                            model.PublishedFare + opnlCharge, response.Content, 1, TicketStatus, EnduserIp, TokenId, model.PNR, model.BookingId, IsSuccess, Message);
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
                                Details = "Flight Booking Refund ",
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
                                Details = "Flight Booking Refund ",
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
                                Details = "Flight Booking Refund ",
                                RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                Usertype = "Master"
                            };
                            back.info(model2);
                        }
                        catch { }
                        return "Failed";
                    }
                }
                else
                {
                    //  string HardcodeJS = "{\"Version\":\"1.0\",\"StatusCode\":200,\"Content\":{\"ResponseCode\":0,\"ADDINFO\":{\"B2B2BStatus\":false,\"Error\":{\"ErrorCode\":0,\"ErrorMessage\":\"\"},\"ResponseStatus\":1,\"TraceId\":\"NA\",\"Response\":{\"PNR\":\"NA\",\"BookingId\":1383211,\"SSRDenied\":false,\"SSRMessage\":null,\"IsPriceChanged\":false,\"IsTimeChanged\":false,\"Message\":\"Unable to process your request please contact to admin. \",\"TicketStatus\":0}}}}";
                    return "Failed";
                }
            }
            else
            {
                return "Failed";
            }
        }
        [HttpPost]
        public ActionResult Ticket(FlightDetailsVM model)
        {

            dynamic TicketResponses = new JArray();
            try
            {
                var userid = User.Identity.GetUserId();

                string externalip = new WebClient().DownloadString("http://ipv4.icanhazip.com/");
                externalip = externalip.Replace("\n", "");
                //get only city using ip
                var clientforgetip = new RestClient("http://ip-api.com/json/" + externalip + "?fields=city");
                var requestforcurrntip = new RestRequest(Method.GET);
                var taskingforcurrntip = Task.Run(() =>
                {
                    return clientforgetip.Execute(requestforcurrntip).Content;
                });
                var responsessssforcurrentip = taskingforcurrntip.Result;
                dynamic presonse111ip = JsonConvert.DeserializeObject(responsessssforcurrentip);
                string city = presonse111ip.city;

                var retailermanagelocation = db.Manage_rem_Location_by_Admin.Where(sss => sss.userid == userid).ToList();
                if (retailermanagelocation.Count() > 0)
                {
                    var findexectlocation = db.Manage_rem_Location_by_Admin.Where(sss => sss.userid == userid && sss.nameofcity.Trim().ToUpper() == city.Trim().ToUpper()).FirstOrDefault();
                    if (findexectlocation == null)
                    {
                        var responseResult = new { IsSuccess = false, Message = "Bookticket not allowed at this location!!!" };
                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                        return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                    }
                }
                var admindetailschk = db.Retailer_Details.Where(aa => aa.RetailerId == userid).SingleOrDefault();
                var token = string.Empty;
                int TicketStatus = 0;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    var responseResult = new { IsSuccess = false, Message = "Empty token!!!" };
                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                    return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                }
                ViewModels.Fare adultfare = new ViewModels.Fare();
                ViewModels.Fare childfare = new ViewModels.Fare();
                ViewModels.Fare infantfare = new ViewModels.Fare();
                List<Passenger> lstPax = new List<Passenger>();
                #region setFarePerPax
                if (model.AdultPax != null && model.AdultPax.Count > 0)
                {
                    #region GST
                    if (model.Onward.FareQuoteVM.Content.Addinfo.Results.IsGstMandatory)
                    {
                        var adminGST = db.Admin_details.SingleOrDefault();
                        model.AdultPax[0].GSTCompanyAddress = adminGST.Address ?? "NA";
                        model.AdultPax[0].GSTCompanyContactNumber = adminGST.mobile ?? "NA";
                        model.AdultPax[0].GSTCompanyEmail = adminGST.email ?? "NA";
                        model.AdultPax[0].GSTCompanyName = "ALIA DRUG AGENCY";
                        model.AdultPax[0].GSTNumber = adminGST.Gstno ?? "NA";
                        model.AdultPax[0].Email = adminGST.email;
                        //}
                    }
                    if (model.Inward.FareQuoteVM != null && model.Inward.FareQuoteVM.Content.Addinfo.Results.IsGstMandatory)
                    {
                        var adminGST = db.Admin_details.SingleOrDefault();

                        model.AdultPax[0].GSTCompanyAddress = adminGST.Address ?? "NA";
                        model.AdultPax[0].GSTCompanyContactNumber = adminGST.mobile ?? "NA";
                        model.AdultPax[0].GSTCompanyEmail = adminGST.email ?? "NA";
                        model.AdultPax[0].GSTCompanyName = "Excel one stop solutions";
                        model.AdultPax[0].GSTNumber = adminGST.Gstno ?? "NA";
                        model.AdultPax[0].Email = adminGST.email;
                        //}
                    }
                    #endregion

                    adultfare.BaseFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[0].BaseFare / model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[0].PassengerCount;
                    adultfare.Tax = model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[0].Tax / model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[0].PassengerCount;
                    adultfare.PublishedFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare / (model.AdultPax.Count + model.ChildPax.Count + model.InfantPax.Count);
                    adultfare.TdsOnCommission = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.TdsOnCommission / (model.AdultPax.Count + model.ChildPax.Count + model.InfantPax.Count);

                    adultfare.YqTax = model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[0].YqTax;
                    model.AdultPax.ForEach(f => f.Fare = adultfare);
                    lstPax.AddRange(model.AdultPax);
                }
                if (model.ChildPax != null && model.ChildPax.Count > 0)
                {
                    childfare.BaseFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[1].BaseFare / model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[1].PassengerCount;
                    childfare.Tax = model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[1].Tax / model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[1].PassengerCount;
                    childfare.YqTax = model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[1].YqTax;
                    childfare.PublishedFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare / (model.AdultPax.Count + model.ChildPax.Count + model.InfantPax.Count);
                    childfare.TdsOnCommission = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.TdsOnCommission / (model.AdultPax.Count + model.ChildPax.Count + model.InfantPax.Count);

                    model.ChildPax.ForEach(f => f.Fare = childfare);
                    lstPax.AddRange(model.ChildPax);
                }
                if (model.InfantPax != null && model.InfantPax.Count > 0)
                {
                    infantfare.BaseFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[2].BaseFare / model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[2].PassengerCount;
                    infantfare.Tax = model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[2].Tax / model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[2].PassengerCount;
                    infantfare.YqTax = model.Onward.FareQuoteVM.Content.Addinfo.Results.FareBreakdown[2].YqTax;
                    infantfare.PublishedFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare / (model.AdultPax.Count + model.ChildPax.Count + model.InfantPax.Count);
                    infantfare.TdsOnCommission = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.TdsOnCommission / (model.AdultPax.Count + model.ChildPax.Count + model.InfantPax.Count);


                    model.InfantPax.ForEach(f => f.Fare = infantfare);
                    lstPax.AddRange(model.InfantPax);
                }
                #endregion

                #region setBaggageMeal
                decimal OptionalServicesCharge = 0;
                BaggageSSRLcc baggageOnward = new BaggageSSRLcc();
                BaggageSSRLcc baggageInward = new BaggageSSRLcc();
                BaggageSSRLcc mealDynamicOnward = new BaggageSSRLcc();
                BaggageSSRLcc mealDynamicInward = new BaggageSSRLcc();
                Meal meal = new Meal();
                ////////// Onward setting   ///////////
                if (model.Onward.SSRLcc != null)
                {
                    if (model.Onward.SSRLcc.Content.Addinfo.Baggage != null && model.Onward.SSRLcc.Content.Addinfo.Baggage.Count > 0)
                    {
                        var aa = model.Onward.SSRLcc.Content.Addinfo.Baggage.First().Where(a => a.Code == model.Onward.BaggageCode).FirstOrDefault();
                        if (aa != null)
                        {
                            if (aa.Code.ToLower().Contains("No Meal"))
                            {
                                mealDynamicOnward = null;
                            }
                            baggageOnward.AirlineDescription = aa.AirlineDescription;
                            baggageOnward.AirlineCode = aa.AirlineCode;
                            baggageOnward.Code = aa.Code;
                            baggageOnward.Currency = aa.Currency;
                            baggageOnward.Description = aa.Description;
                            baggageOnward.Destination = aa.Destination;
                            baggageOnward.FlightNumber = aa.FlightNumber;
                            baggageOnward.Origin = aa.Origin;
                            baggageOnward.Price = aa.Price;
                            baggageOnward.Quantity = aa.Quantity;
                            baggageOnward.WayType = aa.WayType;
                            baggageOnward.Weight = aa.Weight;
                            OptionalServicesCharge = OptionalServicesCharge + aa.Price;
                        }
                        else
                        {
                            baggageOnward = null;
                        }
                    }
                    else
                    {
                        baggageOnward = null;
                    }
                    var bb = model.Onward.SSRLcc.Content.Addinfo.MealDynamic.First().Where(a => a.Code == model.Onward.MealCode).FirstOrDefault();
                    if (bb != null)
                    {
                        if (bb.Code == "No Meal")
                        {
                            mealDynamicOnward = null;
                        }
                        else
                        {
                            mealDynamicOnward.AirlineDescription = bb.AirlineDescription;
                            mealDynamicOnward.AirlineCode = bb.AirlineCode;
                            mealDynamicOnward.Code = bb.Code;
                            mealDynamicOnward.Currency = bb.Currency;
                            mealDynamicOnward.Description = bb.Description;
                            mealDynamicOnward.Destination = bb.Destination;
                            mealDynamicOnward.FlightNumber = bb.FlightNumber;
                            mealDynamicOnward.Origin = bb.Origin;
                            mealDynamicOnward.Price = bb.Price;
                            mealDynamicOnward.Quantity = bb.Quantity;
                            mealDynamicOnward.WayType = bb.WayType;
                            mealDynamicOnward.Weight = bb.Weight;
                            OptionalServicesCharge = OptionalServicesCharge + bb.Price;
                        }

                    }
                }
                else if (model.Onward.SSRNonLcc != null)
                {
                    var bb = model.Onward.SSRNonLcc.Content.Addinfo.Meal.Where(a => a.Code == model.Onward.MealCode).SingleOrDefault();
                    if (bb != null)
                    {
                        meal.Code = bb.Code;
                        meal.Description = bb.Description;
                        //TODO
                        //OptionalServicesCharge = OptionalServicesCharge + bb.Price;
                    }
                }
                ////////// Inward setting   ///////////
                var InwardSSr = TempData.Peek("InwardSSRLcc");
                if (InwardSSr != null)
                {
                    model.Inward.SSRLcc = (SSRLcc)InwardSSr;
                }
                var InwardSSrNonLcc = TempData.Peek("InwardSSRNonLcc");
                if (InwardSSrNonLcc != null)
                {
                    model.Inward.SSRNonLcc = (SSRNonLcc)InwardSSrNonLcc;
                }
                if (model.Inward.SSRLcc != null)
                {
                    if (model.Inward.SSRLcc.Content.Addinfo.Baggage != null && model.Inward.SSRLcc.Content.Addinfo.Baggage.Count > 0)
                    {
                        var aa = model.Inward.SSRLcc.Content.Addinfo.Baggage.First().Where(a => a.Code == model.Onward.BaggageCode).FirstOrDefault();
                        if (aa != null)
                        {
                            if (aa.Code.ToLower().Contains("No Meal"))
                            {
                                mealDynamicInward = null;
                            }
                            baggageInward.AirlineDescription = aa.AirlineDescription;
                            baggageInward.AirlineCode = aa.AirlineCode;
                            baggageInward.Code = aa.Code;
                            baggageInward.Currency = aa.Currency;
                            baggageInward.Description = aa.Description;
                            baggageInward.Destination = aa.Destination;
                            baggageInward.FlightNumber = aa.FlightNumber;
                            baggageInward.Origin = aa.Origin;
                            baggageInward.Price = aa.Price;
                            baggageInward.Quantity = aa.Quantity;
                            baggageInward.WayType = aa.WayType;
                            baggageInward.Weight = aa.Weight;
                            OptionalServicesCharge = OptionalServicesCharge + aa.Price;
                        }
                        else
                        {
                            baggageInward = null;
                        }
                    }
                    else
                    {
                        baggageInward = null;
                    }
                    var bb = model.Inward.SSRLcc.Content.Addinfo.MealDynamic.First().Where(a => a.Code == model.Onward.MealCode).FirstOrDefault();
                    if (bb != null)
                    {
                        if (bb.Code == "No Meal")
                        {
                            mealDynamicInward = null;
                        }
                        else
                        {
                            mealDynamicInward.AirlineDescription = bb.AirlineDescription;
                            mealDynamicInward.AirlineCode = bb.AirlineCode;
                            mealDynamicInward.Code = bb.Code;
                            mealDynamicInward.Currency = bb.Currency;
                            mealDynamicInward.Description = bb.Description;
                            mealDynamicInward.Destination = bb.Destination;
                            mealDynamicInward.FlightNumber = bb.FlightNumber;
                            mealDynamicInward.Origin = bb.Origin;
                            mealDynamicInward.Price = bb.Price;
                            mealDynamicInward.Quantity = bb.Quantity;
                            mealDynamicInward.WayType = bb.WayType;
                            mealDynamicInward.Weight = bb.Weight;
                            OptionalServicesCharge = OptionalServicesCharge + bb.Price;
                        }

                    }
                }
                else if (model.Onward.SSRNonLcc != null)
                {
                    var bb = model.Onward.SSRNonLcc.Content.Addinfo.Meal.Where(a => a.Code == model.Onward.MealCode).SingleOrDefault();
                    if (bb != null)
                    {
                        meal.Code = bb.Code;
                        meal.Description = bb.Description;
                        //TODO
                        //OptionalServicesCharge = OptionalServicesCharge + bb.Price;
                    }
                }
                #endregion

                string isdomestic = Convert.ToString(TempData.Peek("IsDomestic"));
                var RetailerMarkupAsSercharge = db.Convence_Fees.FirstOrDefault(a => a.RetailerId == userid && a.Role == isdomestic).Amount;
                lstPax.ForEach(a => //Remove leading and trealing space
                {
                    a.FirstName = a.FirstName.Trim();
                    a.LastName = a.LastName.Trim();
                    a.Email = admindetailschk.Email;
                });
                var LeadPaxFirstName = lstPax.First().FirstName;
                var LeadPaxLastName = lstPax.First().LastName;
                var OfferedFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                var PassengerName = string.Join(",", lstPax.Select(a => a.FirstName + " " + a.LastName));
                var AirlineName = model.Onward.FareQuoteVM.Content.Addinfo.Results.Segments.First().First().Airline.AirlineName;
                if (model.isDomastic && model.JourneyType == 2) //Low bal Check for if two way.
                {
                    var entry = db.Remain_reteller_balance.FirstOrDefault(a => a.RetellerId == userid);
                    var OfferedFareIn = model.Inward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                    var OfferedFareOut = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                    var PublishedFareIn = model.Inward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                    var PublishedFareOut = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                    var SurchargeIn = (OfferedFareIn * getAirTicketSurcarge(OfferedFareIn, PublishedFareIn, model.isDomastic, userid)) / 100;
                    var SurchargeOut = (OfferedFareOut * getAirTicketSurcarge(OfferedFareOut, PublishedFareOut, model.isDomastic, userid)) / 100;
                    //SurchargeIn = SurchargeIn + RetailerMarkupAsSercharge;
                    // = SurchargeOut + RetailerMarkupAsSercharge;
                    if (model.isDomastic && entry.Remainamount < OfferedFareIn + OfferedFareOut + SurchargeIn + SurchargeOut + OptionalServicesCharge)
                    {
                        var responseResult = new { IsSuccess = false, Message = "Low Balance!!!" };
                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                        return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                    }
                    else if (!model.isDomastic && entry.Remainamount < OfferedFareIn + 0 + SurchargeIn + 0 + OptionalServicesCharge)
                    {
                        var responseResult = new { IsSuccess = false, Message = "Low Balance!!!" };
                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                        return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                    }
                }
                if (!model.isDomastic && model.JourneyType == 2) //Low bal Check for if two way.
                {
                    var entry = db.Remain_reteller_balance.FirstOrDefault(a => a.RetellerId == userid);
                    var OfferedFareIn = 0;
                    var OfferedFareOut = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                    //var PublishedFareIn = 0;
                    var PublishedFareOut = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                    var SurchargeIn = 0;
                    var SurchargeOut = (OfferedFareOut * getAirTicketSurcarge(OfferedFareOut, PublishedFareOut, model.isDomastic, userid)) / 100;
                    // SurchargeIn = Convert.ToInt32(SurchargeIn + RetailerMarkupAsSercharge);
                    // SurchargeOut = SurchargeOut + RetailerMarkupAsSercharge;
                    if (model.isDomastic && entry.Remainamount < OfferedFareIn + OfferedFareOut + SurchargeIn + SurchargeOut + OptionalServicesCharge)
                    {
                        var responseResult = new { IsSuccess = false, Message = "Low Balance!!!" };
                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                        return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                    }
                    else if (!model.isDomastic && entry.Remainamount < OfferedFareIn + 0 + SurchargeIn + 0 + OptionalServicesCharge)
                    {
                        var responseResult = new { IsSuccess = false, Message = "Low Balance!!!" };
                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                        return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                    }
                }
                if (model.JourneyType != 0)
                {
                    if (model.isDomastic && model.JourneyType == 2)//Domestic return flight booking
                    {
                        #region DometicReturnTypeTicket
                        #region OnwardBooking
                        var OnwardCounter = 0;
                        foreach (var pax in lstPax)
                        {
                            if (OnwardCounter == 0)
                            {
                                pax.IsLeadPax = true;
                            }
                            else
                            {
                                pax.IsLeadPax = false;
                            }
                            if (lstPax != null && lstPax.Count > 0)
                            {
                                lstPax[OnwardCounter].PassportExpiry = "0001-01-01T00:00:00"; //setting default value if domestic flight
                            }
                            OnwardCounter++;
                        }
                        if (model.Onward.FareQuoteVM.Content.Addinfo.Results.IsLcc) //LCC Flight ticket
                        {
                            lstPax.ForEach(a =>
                            {
                                if (a.IsLeadPax)
                                {
                                    if (baggageOnward != null)
                                    {
                                        a.Baggage.Add(baggageOnward);
                                    }
                                }
                                if (a.PaxType == "3")//Baggage should not pe bass in case of infant
                                {
                                    a.Baggage = null;
                                }
                            });
                            lstPax.ForEach(a =>
                            {
                                if (a.IsLeadPax)
                                {
                                    if (mealDynamicOnward != null)
                                    {
                                        a.MealDynamic.Add(mealDynamicOnward);
                                    }
                                }
                            });


                            decimal amtt_add = 0;
                            decimal amtt_total = 0;
                            var client1 = new RestClient(VastbazaarBaseUrl + "api/Air/Margin");
                            var request1 = new RestRequest(Method.POST);
                            request1.AddHeader("authorization", "bearer " + token);
                            IRestResponse response1 = client1.Execute(request1);
                            var respchk = response1.Content;
                            dynamic resp = JsonConvert.DeserializeObject(respchk);
                            if (model.isDomastic == true)
                            {
                                amtt_add = Convert.ToDecimal(resp.Content.ADDINFO.domestic);
                            }
                            else
                            {
                                amtt_add = Convert.ToDecimal(resp.Content.ADDINFO.international);
                            }
                            amtt_total = (Convert.ToDecimal(model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare) * amtt_add) / 100;

                            TicketLcc reqObj = new TicketLcc();
                            reqObj.OfferedFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                            reqObj.ResultIndex = model.Onward.FareQuoteVM.Content.Addinfo.Results.ResultIndex;
                            reqObj.Passengers = lstPax;
                            reqObj.TraceId = model.Onward.FareQuoteVM.Content.Addinfo.TraceId;
                            reqObj.PreferredCurrency = null;
                            int totalcount = (model.AdultPax.Count + model.ChildPax.Count + model.InfantPax.Count);
                            System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                            System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                            System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                            var respchk_new = db.proc_FlightBookingPayment(userid, model.TraceId, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare + OptionalServicesCharge - amtt_total, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge,
                            LeadPaxFirstName, LeadPaxLastName, PassengerName, AirlineName, model.isDomastic, JsonConvert.SerializeObject(reqObj), Idno, totalcount, 0, 0, IsSuccess, Message).SingleOrDefault();
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
                                    Details = "Flight Booking ",
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
                                    Details = "Flight Booking ",
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
                                    Details = "Flight Booking ",
                                    RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                    Usertype = "Master"
                                };
                                back.info(model2);
                            }
                            catch { }
                            if (respchk_new.msg == "Success")
                            {
                                if (Convert.ToBoolean(IsSuccess.Value) == true)
                                {

                                    int idvalrch = db.TBO_AirTicketingDetails.Where(aa => aa.RetailerId == userid).OrderByDescending(aa => aa.idno).Select(c => c.idno).FirstOrDefault();
                                    //int idvalrch = db.TBO_AirTicketingDetails.Max(x => x.idno);
                                    var rchinforsss = db.TBO_AirTicketingDetails.Where(x => x.idno == idvalrch).FirstOrDefault();
                                    if (rchinforsss != null)
                                    {

                                        rchinforsss.Devicetoken = null;
                                        rchinforsss.Latitude = null;
                                        rchinforsss.Longitude = null;
                                        rchinforsss.ModelNo = null;
                                        rchinforsss.City = city;
                                        rchinforsss.PostalCode = null;
                                        rchinforsss.InternetTYPE = null;
                                        rchinforsss.IPaddress = externalip;
                                        db.SaveChanges();
                                    }
                                    var client = new RestClient(VastbazaarBaseUrl + "api/Air/Ticket");
                                    var request = new RestRequest(Method.POST);
                                    request.RequestFormat = DataFormat.Json;
                                    request.AddHeader("authorization", "bearer " + token);
                                    request.AddHeader("cache-control", "no-cache");
                                    request.AddHeader("content-type", "application/json");
                                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                                    request.AddBody(reqObj);

                                    IRestResponse response = client.Execute(request);

                                    dynamic respo = JsonConvert.DeserializeObject(response.Content);


                                    if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                                    {
                                        //db.proc_UpdateFlightBooking(Idno.Value.ToString(), reqObj.OfferedFare,responseJs,0,IsSuccess,Message);
                                        string PNR, BookingId;
                                        try
                                        {
                                            PNR = respo.Content.ADDINFO.Response.PNR;
                                            BookingId = respo.Content.ADDINFO.Response.BookingId;
                                            TicketStatus = respo.Content.ADDINFO.Response.TicketStatus;
                                        }
                                        catch
                                        {
                                            PNR = "";
                                            BookingId = "";

                                        }
                                        //var PNRLCC = respo.Content.ADDINFO.Response.PNR;
                                        db.proc_UpdateFlightBooking(Idno.Value.ToString(), userid, reqObj.OfferedFare + OptionalServicesCharge, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge, response.Content, 0, TicketStatus, "", "", PNR, BookingId, IsSuccess, Message);
                                        var ResponseToView = new { IsSuccess = true, Message = "Success" };
                                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                        //return Json(JsonConvert.SerializeObject(ResponseToView), JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        //db.proc_UpdateFlightBooking(Idno.Value.ToString(), reqObj.OfferedFare, responseJs, 1, IsSuccess, Message);
                                        db.proc_UpdateFlightBooking(Idno.Value.ToString(), userid, reqObj.OfferedFare + OptionalServicesCharge, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge, response.Content, 1, TicketStatus, "", "", "", "", IsSuccess, Message);
                                        var ResponseToView = new { IsSuccess = false, Message = respo.Content.ADDINFO.Error.ErrorMessage ?? "Server Error!!" };
                                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                    }
                                }
                                else
                                {
                                    var ResponseToView = new { IsSuccess = false, Message = Convert.ToString(Message.Value) };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                }
                            }
                            else
                            {
                                var responseResult = new { IsSuccess = false, Message = respchk_new.msg };
                                TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (!model.Onward.FareQuoteVM.Content.Addinfo.Results.IsLcc) //Non LCC Flight ticket
                        {
                            // Mealdynamic and Seatdynamic should not pass in case of Non Lcc
                            List<PassengerNonLcc> lstPaxNonLcc = new List<PassengerNonLcc>();
                            lstPax.ForEach(a =>
                            {
                                PassengerNonLcc item = new PassengerNonLcc(a);
                                //item.AddressLine1 = a.AddressLine1;

                                lstPaxNonLcc.Add(item);
                            });
                            lstPax.ForEach(a =>
                            {
                                if (a.IsLeadPax)
                                {
                                    if (baggageInward != null)
                                    {
                                        a.Baggage.Add(baggageInward);
                                    }
                                }
                            });
                            string responseJson = string.Empty;
                            var bookingResponse = Book(model, lstPaxNonLcc, true, out responseJson);
                            if (bookingResponse != null && bookingResponse == "Success")
                            {
                                TicketNonLcc ticketnonlccmodel = new TicketNonLcc();
                                dynamic bookingResponseJs = JsonConvert.DeserializeObject(responseJson);
                                ticketnonlccmodel.BookingId = bookingResponseJs.Content.ADDINFO.Response.BookingId;
                                ticketnonlccmodel.PNR = bookingResponseJs.Content.ADDINFO.Response.PNR;
                                ticketnonlccmodel.TraceId = bookingResponseJs.Content.ADDINFO.TraceId;
                                ticketnonlccmodel.OfferedFare = bookingResponseJs.Content.ADDINFO.Response.FlightItinerary.Fare.OfferedFare;
                                ticketnonlccmodel.PublishedFare = bookingResponseJs.Content.ADDINFO.Response.FlightItinerary.Fare.PublishedFare;
                                ticketnonlccmodel.ResultIndex = model.Onward.ResultIndex;
                                var ticketResponse = TicketNonLCC(ticketnonlccmodel, OptionalServicesCharge, LeadPaxFirstName, LeadPaxLastName, PassengerName, AirlineName, "", "", model.isDomastic);
                                if (ticketResponse != null && ticketResponse == "Success")
                                {
                                    var responseResult = new { IsSuccess = true, Message = "OnwardBooking Suuccess!!!" };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                    //return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    try
                                    {
                                        RealsePNRModel REALSEPNRMODEL = new RealsePNRModel();
                                        REALSEPNRMODEL.TraceId = bookingResponseJs.Content.ADDINFO.Response.TraceId;
                                        REALSEPNRMODEL.PNR = bookingResponseJs.Content.ADDINFO.Response.PNR;
                                        REALSEPNRMODEL.BookingId = bookingResponseJs.Content.ADDINFO.Response.BookingId;
                                        REALSEPNRMODEL.Source = bookingResponseJs.Content.ADDINFO.Response.FlightItinerary.Source;
                                        string str = ReleasePNRNo(REALSEPNRMODEL);
                                    }
                                    catch { }
                                    var responseResult = new { IsSuccess = false, Message = "OnwardBooking Failed!!!" };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                    //return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                var responseResult = new { IsSuccess = false, Message = "OnwardBooking Failed!!!" };
                                TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                //return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var responseResult = new { IsSuccess = false, Message = "Onward Booking Failed!!!" };
                            TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                        }
                        #endregion
                        #region InwardBooking
                        var InwardCounter = 0;
                        foreach (var pax in lstPax)
                        {
                            if (InwardCounter == 0)
                            {
                                pax.IsLeadPax = true;
                            }
                            else
                            {
                                pax.IsLeadPax = false;
                            }
                            if (lstPax != null && lstPax.Count > 0)
                            {
                                lstPax[InwardCounter].PassportExpiry = "0001-01-01T00:00:00"; //setting default value if domestic flight
                            }
                            InwardCounter++;
                        }

                        if (model.Inward.FareQuoteVM.Content.Addinfo.Results.IsLcc) //LCC Flight ticket
                        {
                            lstPax.ForEach(a => a.Baggage.Clear());
                            lstPax.ForEach(a => a.MealDynamic.Clear());
                            lstPax.ForEach(a =>
                            {
                                if (a.IsLeadPax)
                                {
                                    if (baggageInward != null)
                                    {
                                        a.Baggage.Add(baggageInward);
                                    }
                                }
                                if (a.PaxType == "3")//Baggage should not pe bass in case of infant
                                {
                                    a.Baggage = null;
                                }
                            });
                            lstPax.ForEach(a =>
                            {
                                if (a.IsLeadPax)
                                {
                                    if (mealDynamicInward != null)
                                    {
                                        a.MealDynamic.Add(mealDynamicInward);
                                    }
                                }
                            });
                            TicketLcc reqObj = new TicketLcc();
                            reqObj.OfferedFare = model.Inward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                            reqObj.ResultIndex = model.Inward.FareQuoteVM.Content.Addinfo.Results.ResultIndex;
                            reqObj.Passengers = lstPax;
                            reqObj.TraceId = model.Inward.FareQuoteVM.Content.Addinfo.TraceId;
                            reqObj.PreferredCurrency = null;

                            int totalcount = (model.AdultPax.Count + model.ChildPax.Count + model.InfantPax.Count);
                            decimal amtt_add = 0;
                            decimal amtt_total = 0;
                            var client1 = new RestClient(VastbazaarBaseUrl + "api/Air/Margin");
                            var request1 = new RestRequest(Method.POST);
                            request1.AddHeader("authorization", "bearer " + token);
                            IRestResponse response1 = client1.Execute(request1);
                            var respchk = response1.Content;
                            dynamic resp = JsonConvert.DeserializeObject(respchk);
                            if (model.isDomastic == true)
                            {
                                amtt_add = Convert.ToDecimal(resp.Content.ADDINFO.domestic);
                            }
                            else
                            {
                                amtt_add = Convert.ToDecimal(resp.Content.ADDINFO.international);
                            }
                            amtt_total = (Convert.ToDecimal(model.Inward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare) * amtt_add) / 100;

                            System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                            System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                            System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                            var chk = db.proc_FlightBookingPayment(userid, model.TraceId, model.Inward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare + OptionalServicesCharge - amtt_total, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge,
                              LeadPaxFirstName, LeadPaxLastName, PassengerName, AirlineName, model.isDomastic, JsonConvert.SerializeObject(reqObj), Idno, totalcount, 0, 0, IsSuccess, Message).SingleOrDefault();
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
                                    Details = "Flight Booking ",
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
                                    Details = "Flight Booking ",
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
                                    Details = "Flight Booking ",
                                    RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                    Usertype = "Master"
                                };
                                back.info(model2);
                            }
                            catch { }
                            if (chk.msg == "Success")
                            {
                                if (Convert.ToBoolean(IsSuccess.Value) == true)
                                {

                                    int idvalrch = db.TBO_AirTicketingDetails.Where(aa => aa.RetailerId == userid).OrderByDescending(aa => aa.idno).Select(c => c.idno).FirstOrDefault();


                                    var rchinforsss = db.TBO_AirTicketingDetails.Where(x => x.idno == idvalrch).FirstOrDefault();
                                    if (rchinforsss != null)
                                    {
                                        rchinforsss.Devicetoken = null;
                                        rchinforsss.Latitude = null;
                                        rchinforsss.Longitude = null;
                                        rchinforsss.ModelNo = null;
                                        rchinforsss.City = city;
                                        rchinforsss.PostalCode = null;
                                        rchinforsss.InternetTYPE = null;
                                        rchinforsss.IPaddress = externalip;
                                        db.SaveChanges();
                                    }
                                    var client = new RestClient(VastbazaarBaseUrl + "api/Air/Ticket");
                                    var request = new RestRequest(Method.POST);
                                    request.RequestFormat = DataFormat.Json;
                                    request.AddHeader("authorization", "bearer " + token);
                                    request.AddHeader("cache-control", "no-cache");
                                    request.AddHeader("content-type", "application/json");
                                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                                    request.AddBody(reqObj);
                                    IRestResponse response = client.Execute(request);
                                    dynamic respo = JsonConvert.DeserializeObject(response.Content);
                                    if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                                    {
                                        string PNR, BookingId;
                                        try
                                        {
                                            PNR = respo.Content.ADDINFO.Response.PNR;
                                            BookingId = respo.Content.ADDINFO.Response.BookingId;
                                            TicketStatus = respo.Content.ADDINFO.Response.TicketStatus;
                                        }
                                        catch
                                        {
                                            PNR = "";
                                            BookingId = "";
                                        }
                                        db.proc_UpdateFlightBooking(Idno.Value.ToString(), userid, reqObj.OfferedFare + OptionalServicesCharge, model.Inward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge, response.Content, 0, TicketStatus, "", "", PNR, BookingId, IsSuccess, Message);
                                        var ResponseToView = new { IsSuccess = true, Message = "Success" };
                                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                    }
                                    else
                                    {
                                        db.proc_UpdateFlightBooking(Idno.Value.ToString(), userid, reqObj.OfferedFare + OptionalServicesCharge, model.Inward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge, response.Content, 1, TicketStatus, "", "", "", "", IsSuccess, Message);
                                        var ResponseToView = new { IsSuccess = false, Message = respo.Content.ADDINFO.Error.ErrorMessage ?? "Server Error!!" };
                                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                    }
                                }
                                else
                                {
                                    var ResponseToView = new { IsSuccess = false, Message = Convert.ToString(Message.Value) };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                }
                            }
                            else
                            {
                                var responseResult = new { IsSuccess = false, Message = chk.msg };
                                TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (!model.Inward.FareQuoteVM.Content.Addinfo.Results.IsLcc) //Non LCC Flight ticket
                        {

                            List<PassengerNonLcc> lstPaxNonLcc = new List<PassengerNonLcc>();
                            lstPax.ForEach(a =>
                            {
                                PassengerNonLcc item = new PassengerNonLcc(a);
                                //item.AddressLine1 = a.AddressLine1;
                                item.Email = admindetailschk.Email;
                                lstPaxNonLcc.Add(item);
                            });
                            lstPaxNonLcc.ForEach(a =>
                            {
                                if (a.IsLeadPax)
                                {
                                    if (meal != null)
                                    {
                                        a.Meal = new Meal { Code = meal.Code, Description = meal.Description };
                                    }
                                }
                            });
                            string responseJson = string.Empty;
                            var bookingResponse = Book(model, lstPaxNonLcc, false, out responseJson);
                            if (bookingResponse != null && bookingResponse == "Success")
                            {
                                TicketNonLcc ticketnonlccmodel = new TicketNonLcc();
                                dynamic bookingResponseJs = JsonConvert.DeserializeObject(responseJson);
                                ticketnonlccmodel.BookingId = bookingResponseJs.Content.ADDINFO.Response.BookingId;
                                ticketnonlccmodel.PNR = bookingResponseJs.Content.ADDINFO.Response.PNR;
                                ticketnonlccmodel.TraceId = bookingResponseJs.Content.ADDINFO.TraceId;
                                ticketnonlccmodel.ResultIndex = model.Inward.ResultIndex;
                                ticketnonlccmodel.OfferedFare = bookingResponseJs.Content.ADDINFO.Response.FlightItinerary.Fare.OfferedFare;
                                ticketnonlccmodel.PublishedFare = bookingResponseJs.Content.ADDINFO.Response.FlightItinerary.Fare.PublishedFare;
                                var ticketResponse = TicketNonLCC(ticketnonlccmodel, OptionalServicesCharge, LeadPaxFirstName, LeadPaxLastName, PassengerName, AirlineName, "", "", model.isDomastic);
                                if (ticketResponse != null && ticketResponse == "Success")
                                {
                                    var responseResult = new { IsSuccess = true, Message = "Inward Booking Success!!!" };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                }
                                else
                                {
                                    try
                                    {
                                        RealsePNRModel REALSEPNRMODEL = new RealsePNRModel();
                                        REALSEPNRMODEL.TraceId = bookingResponseJs.Content.ADDINFO.Response.TraceId;
                                        REALSEPNRMODEL.PNR = bookingResponseJs.Content.ADDINFO.Response.PNR;
                                        REALSEPNRMODEL.BookingId = bookingResponseJs.Content.ADDINFO.Response.BookingId;
                                        REALSEPNRMODEL.Source = bookingResponseJs.Content.ADDINFO.Response.FlightItinerary.Source;
                                        string str = ReleasePNRNo(REALSEPNRMODEL);
                                    }
                                    catch { }
                                    var responseResult = new { IsSuccess = false, Message = "OnwardBooking Failed!!!" };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                }
                            }
                            else
                            {
                                var responseResult = new { IsSuccess = false, Message = "Inward Booking Failed!!!" };
                                TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                            }
                        }
                        else
                        {
                            var responseResult = new { IsSuccess = false, Message = "Onward Booking Failed!!!" };
                            TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                        }
                        #endregion
                        #endregion
                    }
                    else if (!model.isDomastic && model.JourneyType == 2)//International return flight booking
                    {
                        #region InterNationalReturnTicket
                        var Counter = 0;
                        foreach (var pax in lstPax)
                        {
                            if (Counter == 0)
                            {
                                pax.IsLeadPax = true;
                            }
                            else
                            {
                                pax.IsLeadPax = false;
                            }
                            Counter++;

                        }
                        if (model.Onward.FareQuoteVM.Content.Addinfo.Results.IsLcc) //LCC Flight ticket
                        {
                            lstPax.ForEach(a =>
                            {
                                if (a.IsLeadPax)
                                {
                                    if (baggageOnward != null)
                                    {
                                        a.Baggage.Add(baggageOnward);
                                    }
                                }
                                if (a.PaxType == "3")//Baggage should not pe bass in case of infant
                                {
                                    a.Baggage = null;
                                }
                            });
                            lstPax.ForEach(a =>
                            {
                                if (a.IsLeadPax)
                                {
                                    if (mealDynamicOnward != null)
                                    {
                                        a.MealDynamic.Add(mealDynamicOnward);
                                    }
                                }
                            });
                            TicketLcc reqObj = new TicketLcc();
                            reqObj.OfferedFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                            reqObj.ResultIndex = model.Onward.FareQuoteVM.Content.Addinfo.Results.ResultIndex;
                            reqObj.Passengers = lstPax;
                            reqObj.TraceId = model.Onward.FareQuoteVM.Content.Addinfo.TraceId;
                            reqObj.PreferredCurrency = null;

                            int totalcount = (model.AdultPax.Count + model.ChildPax.Count + model.InfantPax.Count);
                            decimal amtt_add = 0;
                            decimal amtt_total = 0;
                            var client1 = new RestClient(VastbazaarBaseUrl + "api/Air/Margin");
                            var request1 = new RestRequest(Method.POST);
                            request1.AddHeader("authorization", "bearer " + token);
                            IRestResponse response1 = client1.Execute(request1);
                            var respchk = response1.Content;
                            dynamic resp = JsonConvert.DeserializeObject(respchk);
                            if (model.isDomastic == true)
                            {
                                amtt_add = Convert.ToDecimal(resp.Content.ADDINFO.domestic);
                            }
                            else
                            {
                                amtt_add = Convert.ToDecimal(resp.Content.ADDINFO.international);
                            }
                            amtt_total = (Convert.ToDecimal(model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare) * amtt_add) / 100;
                            System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                            System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                            System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                            var chkkk = db.proc_FlightBookingPayment(userid, model.TraceId, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare + OptionalServicesCharge - amtt_total, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge,
                             LeadPaxFirstName, LeadPaxLastName, PassengerName, AirlineName, model.isDomastic, JsonConvert.SerializeObject(reqObj), Idno, totalcount, 0, 0, IsSuccess, Message).SingleOrDefault();
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
                                    Details = "Flight Booking ",
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
                                    Details = "Flight Booking ",
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
                                    Details = "Flight Booking ",
                                    RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                    Usertype = "Master"
                                };
                                back.info(model2);
                            }
                            catch { }
                            if (chkkk.msg == "Success")
                            {
                                if (Convert.ToBoolean(IsSuccess.Value) == true)
                                {


                                    int idvalrch = db.TBO_AirTicketingDetails.Where(aa => aa.RetailerId == userid).OrderByDescending(aa => aa.idno).Select(c => c.idno).FirstOrDefault();
                                    //int idvalrch = db.TBO_AirTicketingDetails.Max(x => x.idno);

                                    var rchinforsss = db.TBO_AirTicketingDetails.Where(x => x.idno == idvalrch).FirstOrDefault();
                                    if (rchinforsss != null)
                                    {

                                        rchinforsss.Devicetoken = null;
                                        rchinforsss.Latitude = null;
                                        rchinforsss.Longitude = null;
                                        rchinforsss.ModelNo = null;
                                        rchinforsss.City = city;
                                        rchinforsss.PostalCode = null;
                                        rchinforsss.InternetTYPE = null;
                                        rchinforsss.IPaddress = externalip;
                                        db.SaveChanges();
                                    }




                                    var client = new RestClient(VastbazaarBaseUrl + "api/Air/Ticket");
                                    var request = new RestRequest(Method.POST);
                                    request.RequestFormat = DataFormat.Json;
                                    request.AddHeader("authorization", "bearer " + token);
                                    request.AddHeader("cache-control", "no-cache");
                                    request.AddHeader("content-type", "application/json");
                                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                                    request.AddBody(reqObj);

                                    IRestResponse response = client.Execute(request);

                                    dynamic respo = JsonConvert.DeserializeObject(response.Content);

                                    if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                                    {
                                        string PNR, BookingId;
                                        try
                                        {
                                            PNR = respo.Content.ADDINFO.Response.PNR;
                                            BookingId = respo.Content.ADDINFO.Response.BookingId;
                                            TicketStatus = respo.Content.ADDINFO.Response.TicketStatus;
                                        }
                                        catch
                                        {
                                            PNR = "";
                                            BookingId = "";
                                        }
                                        //db.proc_UpdateFlightBooking(Idno.Value.ToString(), reqObj.OfferedFare,responseJs,0,IsSuccess,Message);
                                        db.proc_UpdateFlightBooking(Idno.Value.ToString(), userid, reqObj.OfferedFare + OptionalServicesCharge, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge, response.Content, 0, TicketStatus, "", "", PNR, BookingId, IsSuccess, Message);
                                        var ResponseToView = new { IsSuccess = true, Message = "Success" };
                                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                        //return Json(JsonConvert.SerializeObject(ResponseToView), JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        //db.proc_UpdateFlightBooking(Idno.Value.ToString(), reqObj.OfferedFare, responseJs, 1, IsSuccess, Message);
                                        db.proc_UpdateFlightBooking(Idno.Value.ToString(), userid, reqObj.OfferedFare + OptionalServicesCharge, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge, response.Content, 1, TicketStatus, "", "", "", "", IsSuccess, Message);
                                        var ResponseToView = new { IsSuccess = false, Message = respo.Content.ADDINFO.Error.ErrorMessage ?? "Server Error!!" };
                                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                        //return Json(JsonConvert.SerializeObject(ResponseToView), JsonRequestBehavior.AllowGet);
                                    }

                                    //return respo;
                                }
                                else
                                {
                                    var ResponseToView = new { IsSuccess = false, Message = Convert.ToString(Message.Value) };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                    //return Json(JsonConvert.SerializeObject(ResponseToView), JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                var responseResult = new { IsSuccess = false, Message = chkkk.msg };
                                TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (!model.Onward.FareQuoteVM.Content.Addinfo.Results.IsLcc) //Non LCC Flight ticket
                        {
                            List<PassengerNonLcc> lstPaxNonLcc = new List<PassengerNonLcc>();
                            lstPax.ForEach(a =>
                            {
                                PassengerNonLcc item = new PassengerNonLcc(a);
                                //item.AddressLine1 = a.AddressLine1;
                                item.Email = admindetailschk.Email;
                                lstPaxNonLcc.Add(item);
                            });
                            lstPaxNonLcc.ForEach(a =>
                            {
                                if (a.IsLeadPax)
                                {
                                    if (meal != null)
                                    {
                                        a.Meal = new Meal { Code = meal.Code, Description = meal.Description };
                                    }
                                }
                            });
                            string responseJson = string.Empty;
                            var bookingResponse = Book(model, lstPaxNonLcc, true, out responseJson);
                            if (bookingResponse != null && bookingResponse == "Success")
                            {
                                TicketNonLcc ticketnonlccmodel = new TicketNonLcc();
                                dynamic bookingResponseJs = JsonConvert.DeserializeObject(responseJson);
                                ticketnonlccmodel.BookingId = bookingResponseJs.Content.ADDINFO.Response.BookingId;
                                ticketnonlccmodel.PNR = bookingResponseJs.Content.ADDINFO.Response.PNR;
                                ticketnonlccmodel.TraceId = bookingResponseJs.Content.ADDINFO.TraceId;
                                ticketnonlccmodel.ResultIndex = model.Onward.ResultIndex;
                                ticketnonlccmodel.OfferedFare = bookingResponseJs.Content.ADDINFO.Response.FlightItinerary.Fare.OfferedFare;
                                ticketnonlccmodel.PublishedFare = bookingResponseJs.Content.ADDINFO.Response.FlightItinerary.Fare.PublishedFare;
                                var ticketResponse = TicketNonLCC(ticketnonlccmodel, OptionalServicesCharge, LeadPaxFirstName, LeadPaxLastName, PassengerName, AirlineName, "", "", model.isDomastic);
                                if (ticketResponse != null && ticketResponse == "Success")
                                {
                                    var responseResult = new { IsSuccess = true, Message = "Booking Success!!!" };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                }
                                else
                                {
                                    try
                                    {
                                        RealsePNRModel REALSEPNRMODEL = new RealsePNRModel();
                                        REALSEPNRMODEL.TraceId = bookingResponseJs.Content.ADDINFO.Response.TraceId;
                                        REALSEPNRMODEL.PNR = bookingResponseJs.Content.ADDINFO.Response.PNR;
                                        REALSEPNRMODEL.BookingId = bookingResponseJs.Content.ADDINFO.Response.BookingId;
                                        REALSEPNRMODEL.Source = bookingResponseJs.Content.ADDINFO.Response.FlightItinerary.Source;
                                        string str = ReleasePNRNo(REALSEPNRMODEL);
                                    }
                                    catch { }
                                    var responseResult = new { IsSuccess = false, Message = "Booking Failed!!!" };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                }
                            }
                            else
                            {
                                try
                                {
                                    dynamic jObj = JsonConvert.DeserializeObject(responseJson);
                                    if (jObj != null && jObj.Content != null && jObj.ADDINFO != null && jObj.ADDINFO.Error.ErrorCode > 0)
                                    {
                                        var responseResult = new { IsSuccess = false, Message = jObj.ADDINFO.Error.ErrorMessage };
                                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                    }
                                    else
                                    {
                                        var responseResult = new { IsSuccess = false, Message = "Booking Failed!!!" };
                                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                    }
                                }
                                catch
                                {
                                    var responseResult = new { IsSuccess = false, Message = "Booking Failed!!!" };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                }
                            }
                        }
                        else
                        {
                            var responseResult = new { IsSuccess = false, Message = "Onward Booking Failed!!!" };
                            TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                        }
                        #endregion
                    }
                    else
                    {
                        #region OnwWayTicket
                        var Counter = 0;
                        foreach (var pax in lstPax)
                        {
                            if (Counter == 0)
                            {
                                pax.IsLeadPax = true;
                            }
                            else
                            {
                                pax.IsLeadPax = false;
                            }
                            if (lstPax != null && lstPax.Count > 0)
                            {
                                if (model.isDomastic)
                                {
                                    lstPax[Counter].PassportExpiry = "0001-01-01T00:00:00";//setting default value for domestic flight.
                                }
                            }
                            Counter++;
                        }

                        if (model.Onward.FareQuoteVM.Content.Addinfo.Results.IsLcc) //LCC Flight ticket
                        {
                            lstPax.ForEach(a =>
                            {
                                if (a.IsLeadPax)
                                {
                                    if (baggageOnward != null)
                                    {
                                        a.Baggage.Add(baggageOnward);
                                    }
                                }
                            });
                            lstPax.ForEach(a =>
                            {
                                if (a.IsLeadPax)
                                {
                                    if (mealDynamicOnward != null)
                                    {
                                        a.MealDynamic.Add(mealDynamicOnward);
                                    }
                                }
                            });
                            TicketLcc reqObj = new TicketLcc();
                            reqObj.OfferedFare = model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare;
                            reqObj.ResultIndex = model.Onward.FareQuoteVM.Content.Addinfo.Results.ResultIndex;

                            reqObj.Passengers = lstPax;
                            reqObj.TraceId = model.Onward.FareQuoteVM.Content.Addinfo.TraceId;
                            reqObj.PreferredCurrency = null;
                            decimal amtt_add = 0;
                            decimal amtt_total = 0;
                            var client1 = new RestClient(VastbazaarBaseUrl + "api/Air/Margin");
                            var request1 = new RestRequest(Method.POST);
                            request1.AddHeader("authorization", "bearer " + token);
                            IRestResponse response1 = client1.Execute(request1);
                            var respchk = response1.Content;
                            dynamic resp = JsonConvert.DeserializeObject(respchk);
                            if (model.isDomastic == true)
                            {
                                amtt_add = Convert.ToDecimal(resp.Content.ADDINFO.domestic);
                            }
                            else
                            {
                                amtt_add = Convert.ToDecimal(resp.Content.ADDINFO.international);
                            }
                            amtt_total = (Convert.ToDecimal(model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare) * amtt_add) / 100;
                            int totalcount = (model.AdultPax.Count + model.ChildPax.Count + model.InfantPax.Count);

                            System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                            System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                            System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                            var chkkkk = db.proc_FlightBookingPayment(userid, model.TraceId, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.OfferedFare + OptionalServicesCharge - amtt_total, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge,
                              LeadPaxFirstName, LeadPaxLastName, PassengerName, AirlineName, model.isDomastic, JsonConvert.SerializeObject(reqObj), Idno, totalcount, 0, 0, IsSuccess, Message).SingleOrDefault();
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
                                    Details = "Flight Booking ",
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
                                    Details = "Flight Booking ",
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
                                    Details = "Flight Booking ",
                                    RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                    Usertype = "Master"
                                };
                                back.info(model2);
                            }
                            catch { }
                            if (chkkkk.msg == "Success")
                            {
                                if (Convert.ToBoolean(IsSuccess.Value) == true)
                                {


                                    int idvalrch = db.TBO_AirTicketingDetails.Where(aa => aa.RetailerId == userid).OrderByDescending(aa => aa.idno).Select(c => c.idno).FirstOrDefault();
                                    //int idvalrch = db.TBO_AirTicketingDetails.Max(x => x.idno);

                                    var rchinforsss = db.TBO_AirTicketingDetails.Where(x => x.idno == idvalrch).FirstOrDefault();
                                    if (rchinforsss != null)
                                    {

                                        rchinforsss.Devicetoken = null;
                                        rchinforsss.Latitude = null;
                                        rchinforsss.Longitude = null;
                                        rchinforsss.ModelNo = null;
                                        rchinforsss.City = city;
                                        rchinforsss.PostalCode = null;
                                        rchinforsss.InternetTYPE = null;
                                        rchinforsss.IPaddress = externalip;
                                        db.SaveChanges();
                                    }



                                    var client = new RestClient(VastbazaarBaseUrl + "api/Air/Ticket");
                                    var request = new RestRequest(Method.POST);
                                    request.RequestFormat = DataFormat.Json;
                                    request.AddHeader("authorization", "bearer " + token);
                                    request.AddHeader("cache-control", "no-cache");
                                    request.AddHeader("content-type", "application/json");
                                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                                    request.AddBody(reqObj);

                                    IRestResponse response = client.Execute(request);
                                    if (response.StatusCode == HttpStatusCode.BadRequest)
                                    {
                                        db.proc_UpdateFlightBooking(Idno.Value.ToString(), userid, reqObj.OfferedFare + OptionalServicesCharge, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge, response.Content, 1, 0, "", "", "", "", IsSuccess, Message);
                                        var ResponseToView = new { IsSuccess = false, Message = "Server Error!!" };
                                        TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                    }
                                    else
                                    {
                                        dynamic respo = JsonConvert.DeserializeObject(response.Content);
                                        if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                                        {
                                            string PNR, BookingId;

                                            try
                                            {
                                                PNR = respo.Content.ADDINFO.Response.PNR;
                                                BookingId = respo.Content.ADDINFO.Response.BookingId;
                                                TicketStatus = respo.Content.ADDINFO.Response.TicketStatus;
                                            }
                                            catch
                                            {
                                                PNR = "";
                                                BookingId = "";
                                            }
                                            //db.proc_UpdateFlightBooking(Idno.Value.ToString(), reqObj.OfferedFare,responseJs,0,IsSuccess,Message);
                                            db.proc_UpdateFlightBooking(Idno.Value.ToString(), userid, reqObj.OfferedFare + OptionalServicesCharge, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge, response.Content, 0, TicketStatus, "", "", PNR, BookingId, IsSuccess, Message);
                                            var ResponseToView = new { IsSuccess = true, Message = "Success" };
                                            TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                            //return Json(JsonConvert.SerializeObject(ResponseToView), JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            //db.proc_UpdateFlightBooking(Idno.Value.ToString(), reqObj.OfferedFare, responseJs, 1, IsSuccess, Message);
                                            db.proc_UpdateFlightBooking(Idno.Value.ToString(), userid, reqObj.OfferedFare + OptionalServicesCharge, model.Onward.FareQuoteVM.Content.Addinfo.Results.Fare.PublishedFare + OptionalServicesCharge, response.Content, 1, TicketStatus, "", "", "", "", IsSuccess, Message);
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
                                                    Details = "Flight Booking Refund",
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
                                                    Details = "Flight Booking Refund",
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
                                                    Details = "Flight Booking Refund",
                                                    RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                                    Usertype = "Master"
                                                };
                                                back.info(model2);
                                            }
                                            catch { }
                                            var ResponseToView = new { IsSuccess = false, Message = respo.Content.ADDINFO.Error.ErrorMessage ?? "Server Error!!" };
                                            TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                            //return Json(JsonConvert.SerializeObject(ResponseToView), JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    //return respo;
                                }
                                else
                                {
                                    var ResponseToView = new { IsSuccess = false, Message = Convert.ToString(Message.Value) };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ResponseToView)));
                                    //return Json(JsonConvert.SerializeObject(ResponseToView), JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                var responseResult = new { IsSuccess = false, Message = chkkkk.msg };
                                TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (!model.Onward.FareQuoteVM.Content.Addinfo.Results.IsLcc) //Non LCC Flight ticket
                        {
                            List<PassengerNonLcc> lstPaxNonLcc = new List<PassengerNonLcc>();
                            lstPax.ForEach(a =>
                            {
                                PassengerNonLcc item = new PassengerNonLcc(a);
                                //item.AddressLine1 = a.AddressLine1;

                                lstPaxNonLcc.Add(item);
                            });
                            lstPaxNonLcc.ForEach(a =>
                            {
                                if (a.IsLeadPax)
                                {
                                    if (meal != null && meal.Code != null)
                                    {
                                        a.Meal = new Meal { Code = meal.Code, Description = meal.Description };
                                    }
                                }
                            });
                            string responseJson = string.Empty;
                            var bookingResponse = Book(model, lstPaxNonLcc, true, out responseJson);
                            if (bookingResponse != null && bookingResponse == "Success")
                            {
                                TicketNonLcc ticketnonlccmodel = new TicketNonLcc();
                                dynamic bookingResponseJs = JsonConvert.DeserializeObject(responseJson);
                                ticketnonlccmodel.BookingId = bookingResponseJs.Content.ADDINFO.Response.BookingId;
                                ticketnonlccmodel.PNR = bookingResponseJs.Content.ADDINFO.Response.PNR;
                                ticketnonlccmodel.TraceId = bookingResponseJs.Content.ADDINFO.TraceId;
                                ticketnonlccmodel.ResultIndex = model.Onward.ResultIndex;
                                ticketnonlccmodel.OfferedFare = bookingResponseJs.Content.ADDINFO.Response.FlightItinerary.Fare.OfferedFare;
                                ticketnonlccmodel.PublishedFare = bookingResponseJs.Content.ADDINFO.Response.FlightItinerary.Fare.PublishedFare;
                                var ticketResponse = TicketNonLCC(ticketnonlccmodel, OptionalServicesCharge, LeadPaxFirstName, LeadPaxLastName, PassengerName, AirlineName, "", "", model.isDomastic);
                                if (ticketResponse != null && ticketResponse == "Success")
                                {
                                    var responseResult = new { IsSuccess = true, Message = "OnwardBooking Suuccess!!!" };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                    //return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    try
                                    {
                                        RealsePNRModel REALSEPNRMODEL = new RealsePNRModel();
                                        REALSEPNRMODEL.TraceId = bookingResponseJs.Content.ADDINFO.Response.TraceId;
                                        REALSEPNRMODEL.PNR = bookingResponseJs.Content.ADDINFO.Response.PNR;
                                        REALSEPNRMODEL.BookingId = bookingResponseJs.Content.ADDINFO.Response.BookingId;
                                        REALSEPNRMODEL.Source = bookingResponseJs.Content.ADDINFO.Response.FlightItinerary.Source;
                                        string str = ReleasePNRNo(REALSEPNRMODEL);
                                    }
                                    catch { }
                                    var responseResult = new { IsSuccess = false, Message = "OnwardBooking Failed!!!" };
                                    TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                    //return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                var responseResult = new { IsSuccess = false, Message = "OnwardBooking Failed!!!" };
                                TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                                //return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var responseResult = new { IsSuccess = false, Message = "Onward Booking Failed!!!" };
                            TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                            //return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
                        }
                        #endregion
                    }
                }
                return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var responseResult = new { IsSuccess = false, Message = Ex.Message };
                TicketResponses.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(responseResult)));
                return Json(JsonConvert.SerializeObject(TicketResponses), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public ActionResult TicketReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TicketReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR)
        {
            ViewBag.chk = "post";
            return View();
        }

        public class JsonModel
        {
            public string HTMLString { get; set; }
            public bool NoMoredata { get; set; }
        }
        [ChildActionOnly]
        public ActionResult _ticketreport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
                {
                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();
                    ddl_status = "";
                }
                DateTime frm1 = Convert.ToDateTime(txt_frm_date);
                DateTime to1 = Convert.ToDateTime(txt_to_date);

                txt_frm_date = frm1.ToString("dd-MM-yyyy");
                txt_to_date = to1.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime frm_date = dt.Date;
                DateTime to_date = dt1.AddDays(1);
                var userid = User.Identity.GetUserId();
                PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
                ViewBag.checkdata = null;
                var checkdata = db.TBO_AirTicketingDetails.Where(a => a.RetailerId == userid).FirstOrDefault();
                if (checkdata != null)
                {
                    ViewBag.checkdata = "Data";
                }
                //ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
                int pagesize = 20;
                var proc_Response = db.proc_FlightReport(1, pagesize, ddl_status, userid, null, null, null, null, PNR, null, null, frm_date, to_date).ToList();
                //  ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.OfferedFare));
                //  ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
                //  ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.FareAmount));
                return View(proc_Response);
            }
            //int pagesize = 20;
            //var rowdata = db.Sp_Recharge_info_LazyLoad(1, pagesize, "Retailer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), Operator, txtmob, ddl_status).ToList();
            //return View(rowdata);
        }
        public ActionResult PDF_TicketReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
                {
                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();
                    ddl_status = "";
                }
                DateTime frm1 = Convert.ToDateTime(txt_frm_date);
                DateTime to1 = Convert.ToDateTime(txt_to_date);

                txt_frm_date = frm1.ToString("dd-MM-yyyy");
                txt_to_date = to1.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime frm_date = dt.Date;
                DateTime to_date = dt1.AddDays(1);
                var userid = User.Identity.GetUserId();
                PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
                //ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
                int pagesize = 2000;
                var proc_Response = db.proc_FlightReport(1, pagesize, ddl_status, userid, null, null, null, null, PNR, null, null, frm_date, to_date).ToList();
                //  ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.OfferedFare));
                //  ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
                //  ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.FareAmount));
                return new ViewAsPdf(proc_Response);
            }

        }
        public ActionResult Excel_Ticket_Report(string txt_frm_date, string txt_to_date, string ddl_status, string PNR)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                DateTime frm1 = Convert.ToDateTime(txt_frm_date);
                DateTime to1 = Convert.ToDateTime(txt_to_date);

                txt_frm_date = frm1.ToString("dd-MM-yyyy");
                txt_to_date = to1.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime frm_date = dt.Date;
                DateTime to_date = dt1.AddDays(1);
                var userid = User.Identity.GetUserId();
                PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
                int pagesize = 2000;
                var proc_Response = db.proc_FlightReport(1, pagesize, ddl_status, userid, null, null, null, null, PNR, null, null, frm_date, to_date).ToList();
                DataTable dataTbl = new DataTable();
                dataTbl.Columns.Add("Booking Id", typeof(string));
                dataTbl.Columns.Add("Ticket Status", typeof(string));
                dataTbl.Columns.Add("Passanger Name", typeof(string));
                dataTbl.Columns.Add("Flight", typeof(string));
                dataTbl.Columns.Add("PNR ", typeof(string));
                dataTbl.Columns.Add("Booking Date ", typeof(string));
                dataTbl.Columns.Add("Fare Amount", typeof(string));
                dataTbl.Columns.Add("Pre", typeof(string));
                dataTbl.Columns.Add("Post", typeof(string));
                dataTbl.Columns.Add("U Fee", typeof(string));
                dataTbl.Columns.Add("Income", typeof(string));
                if (proc_Response.Count() > 0)
                {
                    foreach (var item in proc_Response)
                    {
                        dataTbl.Rows.Add(item.BookingId, item.TicketStatus, item.LeadPaxFirstName + " " + item.LeadPaxLastName, item.AirlineName, item.PNR, item.TicketDate, item.FareAmount, item.RemPre, item.RemPost, "UFee", item.RemInc);
                    }

                }
                else
                {
                    dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "", "");
                }
                var grid = new GridView();
                grid.DataSource = dataTbl;
                grid.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Excel_Ticket_Report.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                grid.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                return View();
            }
        }
        protected string renderPartialViewtostring(string Viewname, object model)
        {
            if (string.IsNullOrEmpty(Viewname))

                Viewname = ControllerContext.RouteData.GetRequiredString("action");
            ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewresult = ViewEngines.Engines.FindPartialView(ControllerContext, Viewname);
                ViewContext viewcontext = new ViewContext(ControllerContext, viewresult.View, ViewData, TempData, sw);
                viewresult.View.Render(viewcontext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
        [HttpPost]
        public ActionResult InfiniteScroll_ticket(int pageindex, string PNR, string ddl_status, DateTime frm_date, DateTime to_date)
        {
            string userid = User.Identity.GetUserId();
            System.Threading.Thread.Sleep(1000);
            int pagesize = 20;
            ViewBag.checkdata = null;
            var checkdata = db.TBO_AirTicketingDetails.Where(a => a.RetailerId == userid).FirstOrDefault();
            if (checkdata != null)
            {
                ViewBag.checkdata = "Data";
            }
            var tbrow = db.proc_FlightReport(pageindex, pagesize, ddl_status, userid, null, null, null, null, PNR, null, null, frm_date, to_date).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_ticketreport", tbrow);
            return Json(jsonmodel);
        }

        [HttpPost]
        public ActionResult GetFlightStatus(TicketBookinDetailsModel model)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var ticket = db.TBO_AirTicketingDetails.Where(a => a.TraceId == model.TraceId).FirstOrDefault();
                    var token = string.Empty;
                    token = getAuthToken();
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        var viewRespo = new { Status = "Failed", StatusCode = 0, Message = "Server Down!!" };
                        return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                    }
                    var client = new RestClient(VastbazaarBaseUrl + "api/Air/GetBookingDetails");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                    request.AddBody(model);
                    IRestResponse response = client.Execute(request);
                    dynamic respo = JsonConvert.DeserializeObject<dynamic>(response.Content);

                    if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                    {
                        var viewRespo = new { Status = "Success", StatusCode = respo.Content.ADDINFO.FlightItinerary.Status, Message = "" };
                        return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //if (ticket != null)
                        //{
                        //    try
                        //    {
                        //        System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                        //        System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                        //        //System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                        //        db.proc_UpdateFlightBooking(ticket.idno.ToString(), ticket.RetailerId, ticket.OfferedFare, ticket.PublishedFare, response.Content, 1, 0, "", "", "", "", IsSuccess, Message);
                        //    }
                        //    catch
                        //    {

                        //    }
                        //}
                        var viewRespo = new { Status = "Processed", StatusCode = 0, Message = "Processed!!" };
                        return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch
            {
                var viewRespo = new { Status = "Processed", StatusCode = 0, Message = "Processed!!" };
                return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
            }
            //return View(respo);
        }
        [HttpPost]
        public ActionResult ReleasePNR(RealsePNRModel model)
        {
            var userid = User.Identity.GetUserId();
            var token = string.Empty;
            token = getAuthToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Travel", "Home");
            }
            var responseJs = ReleasePNRNo(model);
            dynamic respo = JsonConvert.DeserializeObject(responseJs);
            if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
            {
                var entry = db.TBO_AirBookingForNonLccDetail.SingleOrDefault(a => a.PNR == model.PNR);
                entry.TicketStatus = "Released";
                entry.TicketStatusCode = 7;
                db.SaveChanges();
            }
            //return respo;
            return Json(JsonConvert.SerializeObject(respo), JsonRequestBehavior.AllowGet);
        }
        public string ReleasePNRNo(RealsePNRModel model)
        {
            var token = string.Empty;
            token = getAuthToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }
            var client = new RestClient(VastbazaarBaseUrl + "api/Air/ReleasePNR");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + token);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
            request.AddBody(model);
            IRestResponse response = client.Execute(request);

            dynamic respo = JsonConvert.DeserializeObject(response.Content);

            string responseJs = response.Content;
            //string responseJs = "{\"Version\":\"1.0\",\"StatusCode\":200,\"Content\":{\"ResponseCode\":1,\"ADDINFO\":{\"B2B2BStatus\":null,\"ResponseStatus\":2,\"TraceId\":\"c4a3dd8e-f2ac-4fde-bfc0-6653821286fa\",\"Error\":{\"ErrorCode\":0,\"ErrorMessage\":\"\"}}}}";
            return responseJs;
        }
        [HttpPost]
        public ActionResult getAirSurcharge(decimal OfferedFare, decimal publishedFare, bool isDomestic)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                var a = getAirTicketSurcarge(OfferedFare, publishedFare, isDomestic, userid);
                OfferedFare = ((OfferedFare * a) / 100) + OfferedFare;
                return Json(OfferedFare, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }
        public decimal getAirTicketSurcarge(decimal OfferedFare, decimal publishedFare, bool isDomestic, string userid)
        {
            try
            {
                //var entry = db.Flight_Commission_admin.FirstOrDefault();
                var entry = db.Slab_Flight.FirstOrDefault(a => a.Role == "Retailer" && a.UserId == userid && a.IsDomestic == isDomestic);
                if (entry != null)
                {
                    if ((((OfferedFare * entry.marginPercentage) / 100) + OfferedFare) > publishedFare)
                    {
                        return 0;
                    }
                    return (decimal)entry.marginPercentage;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
        [HttpPost]
        public ActionResult PRICERBD(PriceRBDModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var token = string.Empty;
                    token = getAuthToken();
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        return null;
                    }
                    var client = new RestClient(VastbazaarBaseUrl + "api/Air/PriceRBD");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                    request.AddBody(model);
                    IRestResponse response = client.Execute(request);

                    dynamic respo = JsonConvert.DeserializeObject(response.Content);

                    string responseJs = response.Content;
                    //string responseJs = "{\"Version\":\"1.0\",\"StatusCode\":200,\"Content\":{\"ResponseCode\":1,\"ADDINFO\":{\"B2B2BStatus\":null,\"ResponseStatus\":2,\"TraceId\":\"c4a3dd8e-f2ac-4fde-bfc0-6653821286fa\",\"Error\":{\"ErrorCode\":0,\"ErrorMessage\":\"\"}}}}";
                    if (respo.StatusCode == 200 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                    {
                        var resultIndex = respo.Content.ADDINFO.Results[0][0].ResultIndex;
                        var jsrespo = new { Status = "Success", Message = "Proccessed Successfully", ResultIndex = resultIndex };
                        return Json(JsonConvert.SerializeObject(jsrespo));
                    }
                    else
                    {
                        var jsrespo = new { Status = "Failed", Message = respo.Content.ADDINFO.Error.ErrorMessage, ResultIndex = "" };
                        return Json(JsonConvert.SerializeObject(jsrespo));
                    }

                }
                else
                {
                    var respo = new { Status = "Failed", Message = ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage), ResultIndex = "" };
                    return Json(JsonConvert.SerializeObject(respo));
                }
            }
            catch (Exception ex)
            {
                var respo = new { Status = "Failed", Message = ex.Message, ResultIndex = "" };
                return Json(JsonConvert.SerializeObject(respo));
            }
        }

        [HttpPost]
        public ActionResult CancelTicket(int idno, long bookingid, string origin, string destination, string ticketIds)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    UpdateAuthToken();
                    var jsrespo = new { Status = "Failed", Message = "Server Error", Result = "" };
                    return Json(JsonConvert.SerializeObject(jsrespo));
                }
                var StringNum = ticketIds.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var SendChangeRequest = new TicketChangeOrCancelModel
                {
                    BookingId = bookingid,
                    RequestType = 1,//TODO Hint : -> NotSet = 0, FullCancellation = 1, PartialCancellation = 2, Reissuance = 3 (Not In Use)
                    CancellationType = 3,//TODO  Hint => NotSet = 0,  NoShow = 1, FlightCancelled = 2 (Used when flight cancelled from operator end travel will get full refund benefit),  Others = 3 (Used when Traveler want ot cancel ticket)
                    //do not pass in case of full cancel 
                    Sectors = null,//new List<Sector>() { new Sector { Origin = origin,Destination = destination} } ,
                    TicketId = null,//StringNum.Select(long.Parse).ToList(), //pass if partial cancel request
                    Remarks = "testing",
                };
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var entry = db.FlightCancellations.FirstOrDefault(a => a.Idno == idno);
                    if (entry != null)
                    {
                        var jsrespo = new { Status = "Success", Message = "Cancellation already processed and current Status is " + ((Enums.ChangeRequestStatus)entry.ChangeRequestStatus).ToString(), Result = "" };
                        return Json(JsonConvert.SerializeObject(jsrespo));
                    }
                    var client = new RestClient(VastbazaarBaseUrl + "api/Air/ChangeOrCancelRequest");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                    request.AddBody(SendChangeRequest);
                    IRestResponse response = client.Execute(request);
                    dynamic respo = JsonConvert.DeserializeObject(response.Content);
                    string responseJs = response.Content;
                    //string responseJs = "{\"Version\":\"1.0\",\"StatusCode\":200,\"Content\":{\"ResponseCode\":1,\"ADDINFO\":{\"B2B2BStatus\":null,\"ResponseStatus\":2,\"TraceId\":\"c4a3dd8e-f2ac-4fde-bfc0-6653821286fa\",\"Error\":{\"ErrorCode\":0,\"ErrorMessage\":\"\"}}}}";
                    if (respo.StatusCode == 200 && respo.Content.ADDINFO.ResponseStatus == 1)
                    {
                        int ChangeRequestStatus = Convert.ToInt32(respo.Content.ADDINFO.TicketCRInfo[0].ChangeRequestStatus);
                        string TraceId = Convert.ToString(respo.Content.ADDINFO.TraceId);
                        int ChangeRequestId = Convert.ToInt32(respo.Content.ADDINFO.TicketCRInfo[0].ChangeRequestId);
                        if (entry == null)
                        {
                            entry = new FlightCancellation();
                            entry.Idno = idno;
                            entry.userid = userid;
                            entry.ChangeRequestStatus = ChangeRequestStatus;
                            entry.TraceId = TraceId;//This Trace Id is Callellation Trace Id that is diferent than  Hotel_info TraceId
                            entry.ChangeRequestId = ChangeRequestId;
                            entry.RequestDate = DateTime.Now;
                            entry.RequestType = 1;
                            entry.CancellationType = 2;
                            entry.Remarks = "";
                            entry.CancelRequestJson = JsonConvert.SerializeObject(SendChangeRequest);
                            db.FlightCancellations.Add(entry);
                            db.SaveChanges();
                        }
                        var result = respo.Content.ADDINFO;
                        var jsrespo = new { Status = "Success", Message = "Proccessed Successfully", Result = result };
                        return Json(JsonConvert.SerializeObject(jsrespo));
                    }
                    else
                    {
                        var jsrespo = new { Status = "Failed", Message = respo.Content.ADDINFO.Error.ErrorMessage, Result = "" };
                        return Json(JsonConvert.SerializeObject(jsrespo));
                    }
                }
            }
            catch
            {
                var jsrespo = new { Status = "Failed", Message = "Server Error", Result = "" };
                return Json(JsonConvert.SerializeObject(jsrespo));
            }
        }
        public ActionResult CancellationReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CancellationReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR)
        {
            ViewBag.chk = "post";
            return View();
        }

        [ChildActionOnly]
        public ActionResult _CancellationReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string bookingid, string cancelreqid, string tracid)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
                {
                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();
                    //ddl_status = "";
                }
                //if (ddl_status == "ALL")
                //{
                //    ddl_status = null;
                //}
                DateTime frm1 = Convert.ToDateTime(txt_frm_date);
                DateTime to1 = Convert.ToDateTime(txt_to_date);

                txt_frm_date = frm1.ToString("dd-MM-yyyy");
                txt_to_date = to1.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime frm_date = dt.Date;
                DateTime to_date = dt1.AddDays(1);
                var userid = User.Identity.GetUserId();
                PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
                ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? "ALL" : ddl_status;
                int pagesize = 20;
                var proc_Response = db.proc_FlightCancellationReport(1, pagesize, ddl_status, userid, null, null, bookingid, cancelreqid, tracid, frm_date, to_date).ToList();
                //  ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.OfferedFare));
                //  ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
                //  ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.FareAmount));
                return View(proc_Response);
            }
            //int pagesize = 20;
            //var rowdata = db.Sp_Recharge_info_LazyLoad(1, pagesize, "Retailer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), Operator, txtmob, ddl_status).ToList();
            //return View(rowdata);
        }
        public ActionResult PDF_CancellationReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string bookingid, string cancelreqid, string tracid)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
                {
                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();
                    //ddl_status = "";
                }
                //if (ddl_status == "ALL")
                //{
                //    ddl_status = null;
                //}
                DateTime frm1 = Convert.ToDateTime(txt_frm_date);
                DateTime to1 = Convert.ToDateTime(txt_to_date);

                txt_frm_date = frm1.ToString("dd-MM-yyyy");
                txt_to_date = to1.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime frm_date = dt.Date;
                DateTime to_date = dt1.AddDays(1);
                var userid = User.Identity.GetUserId();
                PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
                ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? "ALL" : ddl_status;
                int pagesize = 20;
                var proc_Response = db.proc_FlightCancellationReport(1, pagesize, ddl_status, userid, null, null, bookingid, cancelreqid, tracid, frm_date, to_date).ToList();
                //  ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.OfferedFare));
                //  ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
                //  ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.FareAmount));
                return new ViewAsPdf(proc_Response);
            }
        }
        public ActionResult Excel_Cancellation_Report(string txt_frm_date, string txt_to_date, string ddl_status, string PNR, string bookingid, string cancelreqid, string tracid)
        {
            DateTime frm1 = Convert.ToDateTime(txt_frm_date);
            DateTime to1 = Convert.ToDateTime(txt_to_date);

            txt_frm_date = frm1.ToString("dd-MM-yyyy");
            txt_to_date = to1.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime frm_date = dt.Date;
            DateTime to_date = dt1.AddDays(1);
            var userid = User.Identity.GetUserId();
            PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
            ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? "ALL" : ddl_status;
            int pagesize = 20;
            var proc_Response = db.proc_FlightCancellationReport(1, pagesize, ddl_status, userid, null, null, bookingid, cancelreqid, tracid, frm_date, to_date).ToList();
            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Live Status", typeof(string));
            dataTbl.Columns.Add("Request Type", typeof(string));
            dataTbl.Columns.Add("Cancel Type", typeof(string));
            dataTbl.Columns.Add("Cancellation Id", typeof(string));
            dataTbl.Columns.Add("Request Date ", typeof(string));
            dataTbl.Columns.Add("Response Date ", typeof(string));
            dataTbl.Columns.Add("Refunded Amount", typeof(string));
            dataTbl.Columns.Add("Charge", typeof(string));


            if (proc_Response.Count() > 0)
            {
                foreach (var item in proc_Response)
                {
                    dataTbl.Rows.Add(item.ChangeRequestStatus, item.RequestType, item.CancellationType, item.ChangeRequestId, item.CancellationRequestDate, item.CancellationResponseDate, item.RefundedAmount, item.CancellationCharge);
                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "");
            }
            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Excel_Cancellation_Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View();
        }
        [HttpPost]
        public ActionResult InfiniteScroll_cancelticket(int pageindex, string PNR, string ddl_status, DateTime frm_date, DateTime to_date, string bookingid, string cancelreqid, string tracid)
        {
            string userid = User.Identity.GetUserId();
            System.Threading.Thread.Sleep(1000);
            int pagesize = 20;
            if (ddl_status == "")
            {
                ddl_status = "ALL";
            }
            var tbrow = db.proc_FlightCancellationReport(pageindex, pagesize, ddl_status, userid, null, null, bookingid, cancelreqid, tracid, frm_date, to_date).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_CancellationReport", tbrow);
            return Json(jsonmodel);
        }
        [HttpPost]
        public ActionResult CancellationStatus(int ChangeReqId, int idno)
        {
            try
            {
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    UpdateAuthToken();
                    var jsrespo = new { Status = "Failed", Message = "Failed to connect with provider,please try later" };
                    return Json(JsonConvert.SerializeObject(jsrespo));
                }
                var SendChangeRequest = new
                {
                    ChangeRequestId = ChangeReqId
                };
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var client = new RestClient(VastbazaarBaseUrl + "api/Air/GetCancellationRequestStatus");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                    request.AddBody(SendChangeRequest);
                    IRestResponse response = client.Execute(request);
                    dynamic respo = JsonConvert.DeserializeObject(response.Content);
                    //string responseJs = "{\"Version\":\"1.0\",\"StatusCode\":200,\"Content\":{\"ResponseCode\":1,\"ADDINFO\":{\"B2B2BStatus\":null,\"ResponseStatus\":2,\"TraceId\":\"c4a3dd8e-f2ac-4fde-bfc0-6653821286fa\",\"Error\":{\"ErrorCode\":0,\"ErrorMessage\":\"\"}}}}";
                    if (respo.StatusCode == 200 && respo.Content.ADDINFO.ResponseStatus == 1)
                    {
                        if (respo.Content.Error == null)
                        {
                            int ChangeRequestStatus = Convert.ToInt32(respo.Content.ADDINFO.ChangeRequestStatus);
                            // string Remark = "";
                            decimal RefundAmount = Convert.ToDecimal(respo.Content.ADDINFO.RefundedAmount);
                            decimal CancellationCharge = Convert.ToDecimal(respo.Content.ADDINFO.CancellationCharge);
                            System.Data.Entity.Core.Objects.ObjectParameter Status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(bool));
                            System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                            // var procRespo = db.proc_FlightCancellationRefund(idno, ChangeReqId, RefundAmount, CancellationCharge, ChangeRequestStatus, response.Content, Status, Message);
                            //var errMsg = Convert.ToString(respo.Content.ADDINFO.Error.ErrorMessage);
                            var jsrespo = new { Status = "Success", Message = "Ticket Cancellation Status : " + ((Vastwebmulti.Areas.RETAILER.Enums.ChangeRequestStatus)ChangeRequestStatus).ToString() };
                            return Json(JsonConvert.SerializeObject(jsrespo), JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var errMsg = Convert.ToString(respo.Content.ADDINFO.Error.ErrorMessage);
                            var jsrespo = new { Status = "Failed", Message = errMsg };
                            return Json(JsonConvert.SerializeObject(jsrespo), JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsrespo = new { Status = "Failed", Message = "Unable to fatch data" };
                        return Json(JsonConvert.SerializeObject(jsrespo), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch
            {
                var jsrespo = new { Status = "Failed", Message = "Server Error", Result = "" };
                return Json(JsonConvert.SerializeObject(jsrespo), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult ViewTicket(int idno, string firsName, string lastName)
        {
            try
            {
                var airoplanedetails = db.TBO_AirTicketingDetails.Where(a => a.idno == idno).FirstOrDefault();
                var RequestObject = new
                {
                    TraceId = airoplanedetails.TraceId,
                    PNR = airoplanedetails.PNR
                };
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    var viewRespo = new { Status = "Failed", StatusCode = 0, Message = "Server Down!!" };
                    return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                }
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/GetBookingDetails");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(RequestObject);
                IRestResponse response = client.Execute(request);
                Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM respo = JsonConvert.DeserializeObject<Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM>(response.Content);
                if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                {
                    var logo = db.tblHeaderLogoes.Where(p => p.Role == "ADMIN").ToList();
                    if (logo.Count > 0)
                    {
                        ViewBag.logoimage = logo.SingleOrDefault().LogoImage;
                    }
                    else
                    {
                        ViewBag.logoimage = "";
                    }
                    var ticket = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault();
                    var admin = db.Admin_details.Single();
                    ViewBag.Admin = admin;
                    var retailer = db.Retailer_Details.Where(a => a.RetailerId == ticket.RetailerId).SingleOrDefault();
                    ViewBag.CreatedBy = retailer.Frm_Name ?? ".............................";
                    ViewBag.CreatorAddresss = retailer.Address ?? "............................";
                    ViewBag.CreatorState = db.State_Desc.FirstOrDefault(a => a.State_id == retailer.State).State_name;
                    ViewBag.CreatorDistrict = db.District_Desc.FirstOrDefault(d => d.Dist_id == retailer.District && d.State_id == retailer.State).Dist_Desc;
                    ViewBag.CreatorPIN = retailer.Pincode;
                    ViewBag.Retailer = retailer.Frm_Name;
                    ViewBag.CreatorMobile = retailer.Mobile;
                    ViewBag.TraceId = airoplanedetails.TraceId;
                    ViewBag.idno = idno;
                    ViewBag.LeadPaxFirstName = firsName;
                    ViewBag.LeadPaxFirstName = lastName;
                    ViewBag.RetailerMarkup = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault().RetailerMarkup;
                    ViewBag.BaggagePerPax = respo.Content.ADDINFO.FlightItinerary.Passenger.FirstOrDefault().SegmentAdditionalInfo.FirstOrDefault().Baggage;
                    return View(respo);
                }
                else
                {
                    TempData["Status"] = "Processed";
                    TempData["Message"] = "Processed";
                    return RedirectToAction("TicketReport", "Air");
                }

            }
            catch
            {
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        public ActionResult PrintTicket(int idno, string firsName, string lastName)
        {
            try
            {
                var airoplanedetails = db.TBO_AirTicketingDetails.Where(a => a.idno == idno).FirstOrDefault();
                var RequestObject = new
                {
                    TraceId = airoplanedetails.TraceId,
                    PNR = airoplanedetails.PNR
                };
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    var viewRespo = new { Status = "Failed", StatusCode = 0, Message = "Server Down!!" };
                    return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                }
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/GetBookingDetails");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(RequestObject);
                IRestResponse response = client.Execute(request);
                Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM respo = JsonConvert.DeserializeObject<Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM>(response.Content);
                if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                {
                    var logo = db.tblHeaderLogoes.Where(p => p.Role == "ADMIN").ToList();
                    if (logo.Count > 0)
                    {
                        ViewBag.logoimage = logo.SingleOrDefault().LogoImage;
                    }
                    else
                    {
                        ViewBag.logoimage = "";
                    }
                    var ticket = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault();
                    var admin = db.Admin_details.Single();
                    ViewBag.Admin = admin;
                    var retailer = db.Retailer_Details.Where(a => a.RetailerId == ticket.RetailerId).SingleOrDefault();
                    ViewBag.CreatedBy = retailer.Frm_Name ?? ".............................";
                    ViewBag.CreatorAddresss = retailer.Address ?? "............................";
                    ViewBag.CreatorState = db.State_Desc.FirstOrDefault(a => a.State_id == retailer.State).State_name;
                    ViewBag.CreatorDistrict = db.District_Desc.FirstOrDefault(d => d.Dist_id == retailer.District && d.State_id == retailer.State).Dist_Desc;
                    ViewBag.CreatorPIN = retailer.Pincode;
                    ViewBag.Retailer = retailer.Frm_Name;
                    ViewBag.CreatorMobile = retailer.Mobile;
                    ViewBag.TraceId = airoplanedetails.TraceId;
                    ViewBag.idno = idno;
                    ViewBag.LeadPaxFirstName = firsName;
                    ViewBag.LeadPaxFirstName = lastName;
                    ViewBag.RetailerMarkup = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault().RetailerMarkup;
                    ViewBag.BaggagePerPax = respo.Content.ADDINFO.FlightItinerary.Passenger.FirstOrDefault().SegmentAdditionalInfo.FirstOrDefault().Baggage;
                    return new ViewAsPdf("ViewTicket", respo);
                }
                else
                {
                    TempData["Status"] = "Processed";
                    TempData["Message"] = "Processed";
                    return RedirectToAction("TicketReport", "Air");
                }

            }
            catch
            {
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        public ActionResult PrintWithoutFare(int idno, string firsName, string lastName)
        {
            try
            {
                var airoplanedetails = db.TBO_AirTicketingDetails.Where(a => a.idno == idno).FirstOrDefault();
                var RequestObject = new
                {
                    TraceId = airoplanedetails.TraceId,
                    PNR = airoplanedetails.PNR
                };
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    var viewRespo = new { Status = "Failed", StatusCode = 0, Message = "Server Down!!" };
                    return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                }
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/GetBookingDetails");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(RequestObject);
                IRestResponse response = client.Execute(request);
                Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM respo = JsonConvert.DeserializeObject<Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM>(response.Content);
                if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                {
                    var logo = db.tblHeaderLogoes.Where(p => p.Role == "ADMIN").ToList();
                    if (logo.Count > 0)
                    {
                        ViewBag.logoimage = logo.SingleOrDefault().LogoImage;
                    }
                    else
                    {
                        ViewBag.logoimage = "";
                    }
                    var ticket = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault();
                    var admin = db.Admin_details.Single();
                    ViewBag.Admin = admin;
                    var retailer = db.Retailer_Details.Where(a => a.RetailerId == ticket.RetailerId).SingleOrDefault();
                    ViewBag.CreatedBy = retailer.Frm_Name ?? ".............................";
                    ViewBag.CreatorAddresss = retailer.Address ?? "............................";
                    ViewBag.CreatorState = db.State_Desc.FirstOrDefault(a => a.State_id == retailer.State).State_name;
                    ViewBag.CreatorDistrict = db.District_Desc.FirstOrDefault(d => d.Dist_id == retailer.District && d.State_id == retailer.State).Dist_Desc;
                    ViewBag.CreatorPIN = retailer.Pincode;
                    ViewBag.CreatorMobile = retailer.Mobile;
                    ViewBag.TraceId = airoplanedetails.TraceId;
                    ViewBag.idno = idno;
                    ViewBag.LeadPaxFirstName = firsName;
                    ViewBag.LeadPaxFirstName = lastName;
                    ViewBag.RetailerMarkup = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault().RetailerMarkup;
                    ViewBag.BaggagePerPax = respo.Content.ADDINFO.FlightItinerary.Passenger.FirstOrDefault().SegmentAdditionalInfo.FirstOrDefault().Baggage;
                    return new ViewAsPdf("PrintWithoutFare", respo);
                }
                else
                {
                    TempData["Status"] = "Processed";
                    TempData["Message"] = "Processed";
                    return RedirectToAction("TicketReport", "Air");
                }

            }
            catch
            {
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("TicketReport", "Air");
            }
        }
        [HttpGet]
        public ActionResult ViewTicketWithoutFare(int idno, string firsName, string lastName)
        {
            try
            {
                var airoplanedetails = db.TBO_AirTicketingDetails.Where(a => a.idno == idno).FirstOrDefault();
                var RequestObject = new
                {
                    TraceId = airoplanedetails.TraceId,
                    PNR = airoplanedetails.PNR
                };
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    var viewRespo = new { Status = "Failed", StatusCode = 0, Message = "Server Down!!" };
                    return Json(JsonConvert.SerializeObject(viewRespo), JsonRequestBehavior.AllowGet);
                }
                var client = new RestClient(VastbazaarBaseUrl + "api/Air/GetBookingDetails");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(RequestObject);
                IRestResponse response = client.Execute(request);
                Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM respo = JsonConvert.DeserializeObject<Vastwebmulti.Areas.RETAILER.ViewModels.GetFlightDetailsVM>(response.Content);
                if (respo.StatusCode == 200 && respo.Content != null && respo.Content.ResponseCode == 0 && respo.Content.ADDINFO.Error.ErrorCode == 0)
                {
                    var logo = db.tblHeaderLogoes.Where(p => p.Role == "ADMIN").ToList();
                    if (logo.Count > 0)
                    {
                        ViewBag.logoimage = logo.SingleOrDefault().LogoImage;
                    }
                    else
                    {
                        ViewBag.logoimage = "";
                    }
                    var ticket = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault();
                    var admin = db.Admin_details.Single();
                    ViewBag.Admin = admin;
                    var retailer = db.Retailer_Details.Where(a => a.RetailerId == ticket.RetailerId).SingleOrDefault();
                    ViewBag.CreatedBy = retailer.Frm_Name ?? ".............................";
                    ViewBag.CreatorAddresss = retailer.Address ?? "............................";
                    ViewBag.CreatorState = db.State_Desc.FirstOrDefault(a => a.State_id == retailer.State).State_name;
                    ViewBag.CreatorDistrict = db.District_Desc.FirstOrDefault(d => d.Dist_id == retailer.District && d.State_id == retailer.State).Dist_Desc;
                    ViewBag.CreatorPIN = retailer.Pincode;
                    ViewBag.CreatorMobile = retailer.Mobile;
                    ViewBag.TraceId = airoplanedetails.TraceId;
                    ViewBag.idno = idno;
                    ViewBag.LeadPaxFirstName = firsName;
                    ViewBag.LeadPaxFirstName = lastName;
                    ViewBag.RetailerMarkup = db.TBO_AirTicketingDetails.Where(a => a.PNR == respo.Content.ADDINFO.FlightItinerary.Pnr).FirstOrDefault().RetailerMarkup;
                    ViewBag.BaggagePerPax = respo.Content.ADDINFO.FlightItinerary.Passenger.FirstOrDefault().SegmentAdditionalInfo.FirstOrDefault().Baggage;
                    return View(respo);
                }
                else
                {
                    TempData["Status"] = "Processed";
                    TempData["Message"] = "Processed";
                    return RedirectToAction("TicketReport", "Air");
                }

            }
            catch
            {
                TempData["Status"] = "Processed";
                TempData["Message"] = "Processed.";
                return RedirectToAction("TicketReport", "Air");
            }
        }

        public ActionResult FlightFinalresult()
        {
            return View();
        }

        #endregion
        public IRestResponse tokencheck()
        {
            var apidetails = db.Money_API_URLS.Where(aa => aa.API_Name == "VASTWEB").SingleOrDefault();
            var token = apidetails == null ? "" : apidetails.Token;
            var apiid = apidetails == null ? "" : apidetails.API_ID;
            var apiidpwd = apidetails == null ? "" : apidetails.Api_pwd;
            var client = new RestClient(VastbazaarBaseUrl + "token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("iptoken", token);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "UserName=" + apiid + "&Password=" + HttpUtility.UrlEncode(apiidpwd) + "&grant_type=password", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }
        public string getAuthToken()
        {
            try
            {
                var tokn = db.vastbazzartokens.SingleOrDefault();
                if (tokn == null)
                {
                    var response = tokencheck();
                    var responsechk = response.Content.ToString();
                    var responsecode = response.StatusCode.ToString();
                    if (responsecode == "OK")
                    {
                        Models.Vastbillpay vb = new Models.Vastbillpay();
                        dynamic json = JsonConvert.DeserializeObject(responsechk);
                        var token = json.access_token.ToString();
                        var expire = json[".expires"].ToString();
                        DateTime exp = Convert.ToDateTime(expire);
                        vastbazzartoken vast = new vastbazzartoken();
                        vast.apitoken = token;
                        vast.exptime = exp;
                        db.vastbazzartokens.Add(vast);
                        db.SaveChanges();
                        return tokn.apitoken;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    DateTime curntdate = DateTime.Now.Date;
                    DateTime expdate = Convert.ToDateTime(tokn.exptime).Date;
                    if (expdate > curntdate)
                    {
                        return tokn.apitoken;
                    }
                    else
                    {
                        var response = tokencheck();
                        var responsechk = response.Content.ToString();
                        var responsecode = response.StatusCode.ToString();
                        if (responsecode == "OK")
                        {

                            dynamic json = JsonConvert.DeserializeObject(responsechk);
                            var token = json.access_token.ToString();
                            var expire = json[".expires"].ToString();
                            DateTime exp = Convert.ToDateTime(expire);

                            tokn.apitoken = token;
                            tokn.exptime = exp;
                            db.SaveChanges();
                            return token;
                        }
                        else
                        {
                            return null;
                        }
                    }

                }
            }
            catch
            {
                return null;
            }
        }
        public void UpdateAuthToken()
        {
            var response = tokencheck();
            var responsechk = response.Content.ToString();
            var responsecode = response.StatusCode.ToString();
            if (responsecode == "OK")
            {
                var tokn = db.vastbazzartokens.SingleOrDefault();
                dynamic json = JsonConvert.DeserializeObject(responsechk);
                var token = json.access_token.ToString();
                var expire = json[".expires"].ToString();
                DateTime exp = Convert.ToDateTime(expire);

                tokn.apitoken = token;
                tokn.exptime = exp;
                db.SaveChanges();

            }

        }
    }
}
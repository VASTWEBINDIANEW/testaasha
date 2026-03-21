using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
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
    public class BusController : Controller
    {
        //string VastbazaarBaseUrl = "http://localhost:62147/";
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        VastwebmultiEntities db = new VastwebmultiEntities();
        public BusController()
        {

        }
        public BusController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        public ActionResult Search(string txtSourceBus, string txtDestinationBus, DateTime txt_frm_dateBus)
        {
            try
            {
                ViewBag.txtSourceBus = txtSourceBus;
                ViewBag.txtDestinationBus = txtDestinationBus;
                ViewBag.txt_frm_dateBus = txt_frm_dateBus;
                ViewBag.chk = "post";
                var userid = User.Identity.GetUserId();
                var entry = db.Convence_Fees.FirstOrDefault(a => a.RetailerId == userid && a.Role == "Bus");
                if (entry != null)
                {
                    ViewBag.RetailerMarkup = entry.Amount;
                }
                else
                {
                    ViewBag.RetailerMarkup = 0;
                }
                var slabbus = db.Slab_Bus.FirstOrDefault(aa => aa.UserId == userid).marginPercentage;
                ViewBag.charge = slabbus;
                var sts = db.Retailer_Details.Where(a => a.RetailerId == userid && a.Bussts == "Y" && a.Bussts != null).Any();
                if (sts == true)
                {
                    if (string.IsNullOrWhiteSpace(txtSourceBus))
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Source is required.";
                        return RedirectToAction("Travel", "Home");
                    }
                    else
                    {
                        txtSourceBus = txtSourceBus.Split(new string[] { "--" }, StringSplitOptions.RemoveEmptyEntries).Last();
                    }
                    if (string.IsNullOrWhiteSpace(txtDestinationBus))
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Destination is required.";
                        return RedirectToAction("Travel", "Home");
                    }
                    else
                    {
                        txtDestinationBus = txtDestinationBus.Split(new string[] { "--" }, StringSplitOptions.RemoveEmptyEntries).Last();
                    }
                    if (txt_frm_dateBus.Date < DateTime.Now.Date)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Invalid departure date.";
                        return RedirectToAction("Travel", "Home");
                    }
                    var reqObject = new
                    {
                        DateOfJourney = txt_frm_dateBus.ToString("yyyy-MM-dd"),
                        DestinationId = txtDestinationBus,
                        OriginId = txtSourceBus,
                        PreferredCurrency = "INR",
                    };
                    var token = string.Empty;
                    token = getAuthToken();
                    //token = "v8Vr_d0PbVU9mnzxjludAfL12RYq9moHIiJeWSxVL5fzydzLguHYRCfB-uws7pH79vB5pmqEYQqM3agIh6kx5r4Oby4Dvv5eWWAzbpQtuMLGhrdDl3FZ9ySXd2rlYGz_mn0svSJyFp8-LvHmM_qAIJRZ3tTxr0vstIdC14YLGdFKMiiXeWsIunHt5b6rZ3jqnXTDTFzXpOZFUH_arKpdFKUpR_VXg9Z9S10JCDt72jQp2_jQERaLFQ-SJbidA7VlSGxXRrtnBx-j1d1XpKg3JE_zeop02Q3iQEMmhDYrPkJBfTeEUHoFU7zLALcg2LL_mayd1IPnnCx4--rwD_ssiRyzmoqFKSaUqcAbHBbuG6opAqyBqI8f4dsp8PKNECll-PpO4fUIiOpDUbHNT0sWk1Yf5k2quxbc2MI2YnSFFscydX0B8KCLqSgPdGGjmaMXBcSwB0spXw2iikwLuOWkOmKwE1m0e2g3yr72g2O4srQZN8-6Icf6GAhU85G4LUXqimcnuhnEP0g6qaAyP7zgbg";
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        UpdateAuthToken();
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Failed to connect with provider.please try again.";
                        return RedirectToAction("Travel", "Home");
                    }

                    var client1 = new RestClient(VastbazaarBaseUrl + "api/Air/Margin");
                    var request1 = new RestRequest(Method.POST);
                    request1.AddHeader("authorization", "bearer " + token); IRestResponse response = client1.Execute(request1);
                    var respchk = response.Content;
                    dynamic resp = JsonConvert.DeserializeObject(respchk);
                    ViewBag.Margin = Convert.ToDecimal(resp.Content.ADDINFO.Margin);

                    var client = new RestClient(VastbazaarBaseUrl + "api/Bus/Search");
                    var request = new RestRequest(Method.POST);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("accept-encoding", "gzip");
                    request.AddBody(reqObject);
                    Task<IRestResponse> task = Task.Run(() =>
                    {
                        return client.Execute(request);
                    });
                    bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                    if (isCompletedSuccessfully)
                    {
                        //IRestResponse response = client.Execute(request);
                        if (task.Result.StatusCode == HttpStatusCode.BadRequest || task.Result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            UpdateAuthToken();
                            TempData["Status"] = "Failed";
                            TempData["Message"] = "Something went wrong,please later!";
                            return RedirectToAction("Travel", "Home");
                        }
                        var respo = JsonConvert.DeserializeObject<BusSearchResultVM>(task.Result.Content);

                        if (string.IsNullOrWhiteSpace(task.Result.Content))
                        {
                            TempData["Status"] = "Failed";
                            TempData["Message"] = "Server down please try later";
                            return RedirectToAction("Travel", "Home");
                        }
                        //layout.source = txtSourceBus;
                        // layout.destination = txtDestinationBus;
                        respo.doj = txt_frm_dateBus.ToString("yyyy-MM-dd");
                        TempData["BusSearchResult"] = respo;
                        if (respo.Content.Addinfo.Error.ErrorCode != 0)
                        {
                            TempData["Status"] = "Failed";
                            TempData["Message"] = respo.Content.Addinfo.Error.ErrorMessage;
                            return RedirectToAction("Travel", "Home");
                        }

                        return View(respo);
                    }
                    else
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Unable to get bus search result,try again.";
                        return RedirectToAction("Travel", "Home");
                    }
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Your Bus Status Inactive Please Contact Admin";
                    return RedirectToAction("Travel", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = ex.Message;
                return RedirectToAction("Travel", "Home");
            }
        }
        public PartialViewResult _BusFilter(BusSearchResultVM model)
        {
            var FirmName = db.Admin_details.FirstOrDefault().Companyname;
            ViewBag.FirmName = FirmName;
            return PartialView(model);
        }
        [HttpGet]
        public ActionResult GetBusLayout(string ResultIndex, string TraceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ResultIndex))
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Invalid choice.";
                    return RedirectToAction("Travel", "Home");
                }
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    UpdateAuthToken();
                    return PartialView(new SeatsLayoutVM());
                    //TempData["Status"] = "Failed";
                    //TempData["Message"] = "Failed to connect with provider.please try again.";
                    //return RedirectToAction("Travel", "Home");
                }
                var reqObject = new
                {
                    ResultIndex = ResultIndex,
                    TraceId = TraceId
                };
                var client = new RestClient(VastbazaarBaseUrl + "api/Bus/BusLayout");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("accept-encoding", "gzip");
                request.AddBody(reqObject);
                Task<IRestResponse> task = Task.Run(() =>
                {
                    return client.Execute(request);
                });
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                if (isCompletedSuccessfully)
                {
                    if (task.Result.StatusCode == HttpStatusCode.BadRequest || task.Result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Something went wrong,please later!";
                        return RedirectToAction("Travel", "Home");
                    }
                    var userid = User.Identity.GetUserId();
                    var entry = db.Convence_Fees.FirstOrDefault(a => a.RetailerId == userid && a.Role == "Bus");
                    if (entry != null)
                    {
                        ViewBag.RetailerMarkup = entry.Amount;
                    }
                    else
                    {
                        ViewBag.RetailerMarkup = 0;
                    }
                    var layout = JsonConvert.DeserializeObject<SeatsLayoutVM>(task.Result.Content);
                    layout.ResultIndex = ResultIndex;
                    if (layout.Content.Addinfo.Error.ErrorCode != 0)
                    {
                        return PartialView(new SeatsLayoutVM());
                    }
                    var vm = (BusSearchResultVM)TempData.Peek("BusSearchResult");
                    if (vm == null)
                    {
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                    layout.doj = Convert.ToDateTime(vm.doj);
                    layout.source = vm.Content.Addinfo.Origin;
                    layout.destination = vm.Content.Addinfo.Destination;
                    #region getBusBordingDetails
                    try
                    {
                        var getBoradingModel = new { ResultIndex = ResultIndex, TraceId = TraceId };
                        var client1 = new RestClient(VastbazaarBaseUrl + "api/Bus/BoardingPointsInfo");
                        var request1 = new RestRequest(Method.POST);
                        request1.RequestFormat = DataFormat.Json;
                        request1.AddHeader("authorization", "bearer " + token);
                        request1.AddHeader("cache-control", "no-cache");
                        request1.AddHeader("content-type", "application/json");
                        request1.AddHeader("accept-encoding", "gzip");
                        request1.AddBody(getBoradingModel);
                        Task<IRestResponse> task1 = Task.Run(() =>
                        {
                            return client1.Execute(request);
                        });
                        isCompletedSuccessfully = task1.Wait(TimeSpan.FromMilliseconds(120000));
                        if (isCompletedSuccessfully)
                        {
                            if (task1.Result.StatusCode != HttpStatusCode.BadRequest && task1.Result.StatusCode != HttpStatusCode.Unauthorized)
                            {
                                if (task1.Result.Content != null)
                                {
                                    var boradingInfo = JsonConvert.DeserializeObject<BoradingPointDetails>(task1.Result.Content);
                                    if (boradingInfo != null && boradingInfo.Content.Addinfo != null && boradingInfo.Content.Addinfo.BoardingPointsDetails.Count() > 1)
                                    {
                                        var xx = boradingInfo.Content.Addinfo.BoardingPointsDetails.Select(a => new PickAndDropPointsDetail
                                        {
                                            CityPointIndex = a.CityPointIndex,
                                            CityPointLocation = a.CityPointLocation,
                                            CityPointName = a.CityPointName,
                                            CityPointTime = a.CityPointTime
                                        }).ToList();
                                        layout.boardingPoints.AddRange(xx);
                                        layout.boardingPoints = layout.boardingPoints.Distinct().ToList();
                                    }
                                }
                            }

                        }
                    }
                    catch
                    {

                    }
                    #endregion
                    TempData["BusLayoutResult"] = layout;
                    if (vm != null && vm.Content != null && vm.Content.Addinfo != null && vm.Content.Addinfo.BusResults.Count > 0)
                    {
                        if (vm.Content.Addinfo.BusResults.Where(a => a.ResultIndex == ResultIndex) != null)
                        {
                            List<PickAndDropPointsDetail> ss = new List<PickAndDropPointsDetail>();
                            var bps = vm.Content.Addinfo.BusResults.FirstOrDefault(a => a.ResultIndex == ResultIndex).BoardingPointsDetails;
                            if (bps != null)
                            {
                                foreach (var item in bps)
                                {
                                    PickAndDropPointsDetail entr = new PickAndDropPointsDetail();
                                    entr.CityPointIndex = item.CityPointIndex;
                                    entr.CityPointLocation = item.CityPointLocation;
                                    entr.CityPointName = item.CityPointName;
                                    entr.CityPointTime = item.CityPointTime;
                                    ss.Add(entr);
                                }
                            }
                            layout.boardingPoints = ss;
                        }
                        if (vm.Content.Addinfo.BusResults.Where(a => a.ResultIndex == ResultIndex) != null)
                        {
                            List<PickAndDropPointsDetail> ss = new List<PickAndDropPointsDetail>();
                            var dps = vm.Content.Addinfo.BusResults.FirstOrDefault(a => a.ResultIndex == ResultIndex).DroppingPointsDetails;
                            if (dps != null)
                            {
                                foreach (var item in dps)
                                {
                                    PickAndDropPointsDetail entr = new PickAndDropPointsDetail();
                                    entr.CityPointIndex = item.CityPointIndex;
                                    entr.CityPointLocation = item.CityPointLocation;
                                    entr.CityPointName = item.CityPointName;
                                    entr.CityPointTime = item.CityPointTime;
                                    ss.Add(entr);
                                }
                            }
                            layout.droppingPoints = ss;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(task.Result.Content))
                    {
                        return PartialView(new SeatsLayoutVM());
                    }
                    return PartialView(layout);
                }
                else
                {
                    return PartialView(new SeatsLayoutVM());
                }
            }
            catch
            {
                return PartialView(new SeatsLayoutVM());
            }
        }
        public ActionResult PaxInfo()
        {
            try
            {
                if (Request.UrlReferrer != null)
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
                else
                {
                    return RedirectToAction("Travel", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Travel", "Home");
            }
        }
        [HttpPost]
        public ActionResult PaxInfo(string txtSourceBus, string txtDestinationBus, string txt_frm_dateBus, string ResultIndex, string BoardingPoints, string DropingPoints, string selectedSeats, string selectedSeatsName)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                var entry = db.Convence_Fees.FirstOrDefault(a => a.RetailerId == userid && a.Role == "Bus");
                if (entry != null)
                {
                    ViewBag.RetailerMarkup = entry.Amount;
                }
                else
                {
                    ViewBag.RetailerMarkup = 0;
                }
                var vm = (BusSearchResultVM)TempData.Peek("BusSearchResult");
                var busLayout = (SeatsLayoutVM)TempData.Peek("BusLayoutResult");
                if (vm == null || busLayout == null)
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }

                DateTime doj = DateTime.Parse(txt_frm_dateBus);

                BusPaxInfoVM ViewModel = new BusPaxInfoVM();
                ViewModel.selectedSeats = selectedSeats;
                ViewModel.selectedSeatsName = selectedSeatsName;
                ViewModel.busOperatorName = vm.Content.Addinfo.BusResults.SingleOrDefault(a => a.ResultIndex == ResultIndex).TravelName;
                ViewModel.IdProofRequired = vm.Content.Addinfo.BusResults.SingleOrDefault(a => a.ResultIndex == ResultIndex).IdProofRequired;
                BlockBusTicketModel ticket = new BlockBusTicketModel();
                if (string.IsNullOrWhiteSpace(BoardingPoints) || !BoardingPoints.Contains("~"))
                {
                    ticket.boardingPoint = new PickAndDropPointsDetail { CityPointIndex = 0, CityPointLocation = "", CityPointTime = new DateTime() };
                }
                else
                {
                    var selectedBoradingPointNode = BoardingPoints.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
                    ticket.boardingPoint = new PickAndDropPointsDetail { CityPointIndex = Convert.ToInt64(selectedBoradingPointNode[0]), CityPointLocation = selectedBoradingPointNode[1], CityPointTime = Convert.ToDateTime(selectedBoradingPointNode[2]) };
                    ticket.BoardingPointId = Convert.ToInt64(selectedBoradingPointNode[0]);
                }
                ticket.destinationCity = txtDestinationBus;
                ticket.doj = doj.ToString("yyyy-MM-dd");
                if (string.IsNullOrWhiteSpace(DropingPoints))
                {
                    ticket.droppingPoint = new PickAndDropPointsDetail { CityPointIndex = 0, CityPointLocation = "", CityPointTime = new DateTime() };
                }
                else
                {
                    var selectedDropingPointNode = DropingPoints.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
                    ticket.droppingPoint = new PickAndDropPointsDetail { CityPointIndex = Convert.ToInt64(selectedDropingPointNode[0]), CityPointLocation = selectedDropingPointNode[1], CityPointTime = Convert.ToDateTime(selectedDropingPointNode[2]) };
                    ticket.BoardingPointId = Convert.ToInt64(selectedDropingPointNode[0]);
                }
                ticket.ResultIndex = ResultIndex;
                ticket.sourceCity = txtSourceBus;
                ticket.TraceId = busLayout.Content.Addinfo.TraceId;
                ticket.Passenger = new List<BusPassenger>();
                ViewModel.bookInfo = ticket;
                string[] seats = selectedSeats.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < seats.Count(); i++)
                {
                    foreach (var seatsLayouts in busLayout.Content.Addinfo.SeatLayoutDetails.SeatLayout.SeatDetails)
                    {
                        var selectedSeatNode = seatsLayouts.SingleOrDefault(a => a.SeatIndex == Convert.ToInt32(seats[i]));
                        if (selectedSeatNode != null)
                        {
                            BusPassenger pax = new BusPassenger();
                            pax.Seat = selectedSeatNode;
                            //pax. = selectedSeatNode.Sleeper;
                            //pax.ladiesSeat = selectedSeatNode.LadiesSeat;
                            //pax.ac = selectedSeatNode.Ac;
                            //pax.email = "";
                            //pax.fare = selectedSeatNode.Fare;
                            //pax.serviceTaxAmount = selectedSeatNode.ServiceTaxAmount;
                            //pax.operatorServiceChargeAbsolute = selectedSeatNode.OperatorServiceChargeAbsolute;
                            //pax.totalFareWithTaxes = selectedSeatNode.TotalFareWithTaxes;
                            ticket.Passenger.Add(pax);
                        }
                    }
                }
                return View(ViewModel);
            }
            catch (Exception ex)
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = ex.Message;
                return RedirectToAction("Travel", "Home");
            }
        }
        [HttpPost]
        public ActionResult SeatBooking(BusPaxInfoVM model)
        {
            try
            {
                var userid = User.Identity.GetUserId();

                var retailer = db.Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
                #region Hit_Client
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    UpdateAuthToken();
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Failed to connect with provider.please try again.";
                    return RedirectToAction("Travel", "Home");
                }

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
                if (retailermanagelocation.Any())
                {
                    var findexectlocation = db.Manage_rem_Location_by_Admin.Where(sss => sss.userid == userid && sss.nameofcity.Trim().ToUpper() == city.Trim().ToUpper()).FirstOrDefault();
                    if (findexectlocation == null)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Bus Booking  not allowed at this location.";
                        return RedirectToAction("Travel", "Home");



                    }

                }




                var client = new RestClient(VastbazaarBaseUrl + "api/Bus/Block");
                model.bookInfo.Passenger[0].LeadPassenger = true;

                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("accept-encoding", "gzip");
                //request.AddParameter("application/json", "{\r\n\"EndUserIp\": \"192.168.10.10\",\r\n\"TokenId\": \"ac2751e9-4cc3-406f-b678-c947e4f57a00\",\r\n\"AdultCount\": \"1\",\r\n\"ChildCount\": \"0\",\r\n\"InfantCount\": \"0\",\r\n\"DirectFlight\": \"false\",\r\n\"OneStopFlight\": \"false\",\r\n\"JourneyType\": \"1\",\r\n\"PreferredAirlines\": null,\r\n\"Segments\": [\r\n{\r\n\"Origin\": \"DEL\",\r\n\"Destination\": \"BOM\",\r\n        \"FlightCabinClass\": \"1\",\r\n\"PreferredDepartureTime\": \"2018-09-06T00: 00: 00\",\r\n\"PreferredArrivalTime\": \"2018-09-06T00: 00: 00\"\r\n}\r\n        ],\r\n\"Sources\": [\r\n\"6E\"\r\n]\r\n}", ParameterType.RequestBody);
                request.AddBody(model.bookInfo);
                Task<IRestResponse> task = Task.Run(() =>
                {
                    return client.Execute(request);
                });
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                if (isCompletedSuccessfully)
                {
                    if (task.Result.StatusCode == HttpStatusCode.BadRequest || task.Result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Something went wrong,please later!";
                        return RedirectToAction("Travel", "Home");
                    }
                    dynamic respo = JsonConvert.DeserializeObject(task.Result.Content);
                    if (string.IsNullOrWhiteSpace(task.Result.Content))
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Server down please try later";
                        return RedirectToAction("Travel", "Home");
                    }
                    if (respo.Content.ADDINFO.Error.ErrorCode != 0)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = respo.Content.ADDINFO.Error.ErrorMessage;
                        return RedirectToAction("Travel", "Home");
                    }
                    else
                    {
                        var AvilalbleBuses = (BusSearchResultVM)TempData.Peek("BusSearchResult");
                        var busLayout = (SeatsLayoutVM)TempData.Peek("BusLayoutResult");
                        string TraceId = busLayout.Content.Addinfo.TraceId;
                        if (AvilalbleBuses != null)
                        {
                            #region hit_DB
                            // int totalseat = model.bookInfo.Passenger.Sum(a => a.Seat.SeatIndex);
                            decimal totalFare = model.bookInfo.Passenger.Sum(a => a.Seat.Price.OfferedPriceRoundedOff);
                            decimal totalFarepub = model.bookInfo.Passenger.Sum(a => a.Seat.Price.PublishedPriceRoundedOff);
                            if (totalFare <= 0)
                            {
                                TempData["Status"] = "Failed";
                                TempData["Message"] = "Invalid fair #TODO";
                                return RedirectToAction("Travel", "Home");
                            }
                            var client11 = new RestClient(VastbazaarBaseUrl + "api/Air/Margin");
                            var request11 = new RestRequest(Method.POST);
                            request11.AddHeader("authorization", "bearer " + token);
                            IRestResponse response1 = client11.Execute(request11);
                            var respchk = response1.Content;
                            dynamic resp = JsonConvert.DeserializeObject(respchk);
                            decimal vastcharge = Convert.ToDecimal(resp.Content.ADDINFO.Margin);

                            var leadPax = model.bookInfo.Passenger.FirstOrDefault();
                            var PassengerName = string.Join(",", model.bookInfo.Passenger.Select(a => a.Title + " " + a.FirstName + " " + a.LastName));
                            DateTime doj = DateTime.Parse(model.bookInfo.doj, System.Globalization.CultureInfo.InvariantCulture);
                            System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                            System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                            System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                            db.proc_BusBookingPayment(userid, (totalFare - vastcharge), totalFarepub, leadPax.FirstName, leadPax.LastName, PassengerName, model.bookInfo.sourceCity,
                            model.bookInfo.destinationCity, doj, model.busOperatorName, model.bookInfo.boardingPoint.CityPointLocation,
                            model.bookInfo.droppingPoint.CityPointLocation, model.bookInfo.boardingPoint.CityPointTime.ToString(), model.bookInfo.droppingPoint.CityPointTime.ToString()
                            , TraceId, model.selectedSeats, JsonConvert.SerializeObject(model.bookInfo), 0, Idno, IsSuccess, Message);
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
                                    Details = "Bus Booking ",
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
                                    Details = "Bus Booking ",
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
                                    Details = "Bus Booking ",
                                    RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                    Usertype = "Master"
                                };
                                back.info(model2);
                            }
                            catch { }
                            if (Convert.ToBoolean(IsSuccess.Value) == true)
                            {

                                int idvalrch = db.BusTicketingDetails.Where(aa => aa.RetailerId == userid).OrderByDescending(aa => aa.idno).Select(c => c.idno).FirstOrDefault();

                                var rchinforsss = db.BusTicketingDetails.Where(x => x.idno == idvalrch).FirstOrDefault();
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






                                var client1 = new RestClient(VastbazaarBaseUrl + "api/Bus/Book");
                                var request1 = new RestRequest(Method.POST);

                                request1.RequestFormat = DataFormat.Json;
                                request1.AddHeader("authorization", "bearer " + token);
                                request1.AddHeader("cache-control", "no-cache");
                                request1.AddHeader("content-type", "application/json");
                                request1.AddHeader("accept-encoding", "gzip");
                                client1.ReadWriteTimeout = 180000;
                                request1.AddBody(model.bookInfo);
                                IRestResponse response = client1.Execute(request1);
                                if (response.StatusCode == HttpStatusCode.BadRequest)
                                {
                                    db.proc_UpdateBusBooking(Idno.Value.ToString(), userid, (totalFare - vastcharge), response.Content, 1,
                                    "Failed", "", "", "", "", IsSuccess, Message);
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
                                            Details = "Bus Booking Refund ",
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
                                            Details = "Bus Booking Refund ",
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
                                            Details = "Bus Booking Refund ",
                                            RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                            Usertype = "Master"
                                        };
                                        back.info(model2);
                                    }
                                    catch { }
                                    TempData["Status"] = "Failed";
                                    TempData["Message"] = "Internal server error.";
                                    return RedirectToAction("Travel", "Home");
                                }
                                else
                                {
                                    dynamic SeatBookingRespo = JsonConvert.DeserializeObject(response.Content);
                                    //dynamic layout = JsonConvert.DeserializeObject(responseJs);
                                    if (SeatBookingRespo.Content.ADDINFO.Error.ErrorCode == 0 && SeatBookingRespo.Content.ADDINFO.BusBookingStatus == "Confirmed")
                                    {
                                        string PNR = string.Empty, TicketNo = string.Empty, BusId = string.Empty;
                                        string BusbookingStatus = string.Empty;
                                        try
                                        {
                                            BusbookingStatus = SeatBookingRespo.Content.ADDINFO.BusBookingStatus;
                                            PNR = SeatBookingRespo.Content.ADDINFO.TravelOperatorPNR;
                                            TicketNo = SeatBookingRespo.Content.ADDINFO.TicketNo;
                                            BusId = SeatBookingRespo.Content.ADDINFO.BusId;
                                        }
                                        catch
                                        {
                                            PNR = !string.IsNullOrWhiteSpace(PNR) ? PNR : "";
                                            TicketNo = !string.IsNullOrWhiteSpace(TicketNo) ? TicketNo : "";
                                            BusId = "";
                                        }
                                        db.proc_UpdateBusBooking(Idno.Value.ToString(), userid, totalFare, response.Content, 0, BusbookingStatus, "", PNR, TicketNo, BusId, IsSuccess, Message);
                                        //var ResponseToView = new { IsSuccess = true, Message = "Success" };
                                        //return Json(JsonConvert.SerializeObject(ResponseToView), JsonRequestBehavior.AllowGet);
                                        TempData["Status"] = "Success";
                                        TempData["Message"] = "Seat booked successfully.";
                                        return RedirectToAction("Travel", "Home");
                                    }
                                    else
                                    {
                                        db.proc_UpdateBusBooking(Idno.Value.ToString(), userid, totalFare, response.Content, 1, "Failed", "", "", "", "", IsSuccess, Message);
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
                                                Details = "Bus Booking Refund ",
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
                                                Details = "Bus Booking Refund ",
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
                                                Details = "Bus Booking Refund ",
                                                RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                                Usertype = "Master"
                                            };
                                            back.info(model2);
                                        }
                                        catch { }
                                        //var ResponseToView = new { IsSuccess = false, Message = SeatBookingRespo.apiStatus.message ?? "Server Error!!" };
                                        //return Json(JsonConvert.SerializeObject(ResponseToView), JsonRequestBehavior.AllowGet);
                                        TempData["Status"] = "Failed";
                                        TempData["Message"] = Convert.ToString(SeatBookingRespo.apiStatus.message);
                                        return RedirectToAction("Travel", "Home");
                                    }
                                }
                                //return layout;
                            }
                            else
                            {
                                //var ResponseToView = new { IsSuccess = false, Message = "Server Error!!" };
                                //return Json(JsonConvert.SerializeObject(ResponseToView), JsonRequestBehavior.AllowGet);
                                TempData["Status"] = "Failed";
                                TempData["Message"] = Convert.ToString(Message.Value);
                                return RedirectToAction("Travel", "Home");
                            }
                            #endregion
                        }
                        else
                        {
                            TempData["Status"] = "Failed";
                            TempData["Message"] = "Internal Server error.";
                            return RedirectToAction("Travel", "Home");
                        }
                    }
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Failed by provider,try again.";
                    return RedirectToAction("Travel", "Home");
                }
                #endregion
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Something went wrong,please later!";
                return RedirectToAction("Travel", "Home");
            }
        }
        public ActionResult BusBookingReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BusBookingReport(string txt_frm_date, string txt_to_date, string ddl_status)
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
        public ActionResult _ticketbusreport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (txt_frm_date == null && txt_to_date == null)
                {
                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();

                }
                if (ddl_status == "ALL")
                {
                    ddl_status = null;
                }
                DateTime frm1 = Convert.ToDateTime(txt_frm_date);
                DateTime to1 = Convert.ToDateTime(txt_to_date);

                txt_frm_date = frm1.ToString("yyyy-MM-dd");
                txt_to_date = to1.ToString("yyyy-MM-dd");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime dt1 = DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime frm_date = dt.Date;
                DateTime to_date = dt1.AddDays(1);
                var userid = User.Identity.GetUserId();
                PNR = string.IsNullOrWhiteSpace(PNR) ? null : PNR;
                ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
                ViewBag.checkdata = null;
                var checkdata = db.BusTicketingDetails.Where(a => a.RetailerId == userid).FirstOrDefault();
                if (checkdata != null)
                {
                    ViewBag.checkdata = "Data";
                }
                int pagesize = 20;
                var proc_Response = db.proc_BusReport(1, pagesize, ddl_status, userid, null, null, null, null, null, null, null, frm_date, to_date).ToList();
                //  ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.OfferedFare));
                //  ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
                //  ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.FareAmount));
                return View(proc_Response);
            }
            //int pagesize = 20;
            //var rowdata = db.Sp_Recharge_info_LazyLoad(1, pagesize, "Retailer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), Operator, txtmob, ddl_status).ToList();
            //return View(rowdata);
        }
        public ActionResult PDF_TicketBusReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (txt_frm_date == null && txt_to_date == null)
                {
                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();

                }
                if (ddl_status == "ALL")
                {
                    ddl_status = null;
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
                ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
                int pagesize = 2000;
                var proc_Response = db.proc_BusReport(1, pagesize, ddl_status, userid, null, null, null, null, null, null, null, frm_date, to_date).ToList();
                //  ViewData["Totalofferfare"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Success")).Sum(s => Convert.ToInt32(s.OfferedFare));
                //  ViewData["totalf"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("FAILED")).Sum(s => Convert.ToInt32(s.FareAmount));
                //  ViewData["totalp"] = proc_Response.Where(s => s.TicketStatus.ToUpper().Contains("Proccessed")).Sum(s => Convert.ToInt32(s.FareAmount));
                return new ViewAsPdf(proc_Response);
            }
        }
        public ActionResult Excel_Ticketbus_Report(string txt_frm_date, string txt_to_date, string ddl_status, string PNR)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (txt_frm_date == null && txt_to_date == null)
                {
                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();

                }
                if (ddl_status == "ALL")
                {
                    ddl_status = null;
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
                ddl_status = string.IsNullOrWhiteSpace(ddl_status) ? null : ddl_status;
                int pagesize = 2000;
                var proc_Response = db.proc_BusReport(1, pagesize, ddl_status, userid, null, null, null, null, null, null, null, frm_date, to_date).ToList();
                DataTable dataTbl = new DataTable();
                dataTbl.Columns.Add("PNR", typeof(string));
                dataTbl.Columns.Add("Passanger Name", typeof(string));
                dataTbl.Columns.Add("Booking No", typeof(string));
                dataTbl.Columns.Add("Source", typeof(string));
                dataTbl.Columns.Add("Destination ", typeof(string));
                dataTbl.Columns.Add("DOJ", typeof(string));
                dataTbl.Columns.Add("Fare", typeof(string));
                dataTbl.Columns.Add("Pre", typeof(string));
                dataTbl.Columns.Add("Post", typeof(string));
                dataTbl.Columns.Add("Income", typeof(string));
                dataTbl.Columns.Add("Date", typeof(string));
                if (proc_Response.Any())
                {
                    foreach (var item in proc_Response)
                    {
                        dataTbl.Rows.Add(item.TicketStatus == null ? item.PNR : item.TicketStatus, item.PassengerName, item.TicketNo, item.sourceStationName, item.destinationStationName, item.dateOfJourney, item.FareAmount, item.RemPre, item.RemPost, item.RemInc, item.TicketDate);
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
                Response.AddHeader("content-disposition", "attachment; filename=Excel_Ticketbus_Report.xls");
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
        public ActionResult InfiniteScroll_bus(int pageindex, string ddl_status, DateTime frm_date, DateTime to_date)
        {
            string userid = User.Identity.GetUserId();
            System.Threading.Thread.Sleep(1000);
            ViewBag.checkdata = null;
            var checkdata = db.BusTicketingDetails.Where(a => a.RetailerId == userid).FirstOrDefault();
            if (checkdata != null)
            {
                ViewBag.checkdata = "Data";
            }
            int pagesize = 20;
            if (ddl_status == "ALL")
            {
                ddl_status = null;
            }
            var tbrow = db.proc_BusReport(pageindex, pagesize, ddl_status, userid, null, null, null, null, null, null, null, frm_date, to_date).ToList();
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_ticketbusreport", tbrow);
            return Json(jsonmodel);
        }

        [HttpPost]
        public ActionResult getBookingDetails(string TraceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TraceId))
                {
                    var ajaxRespo = new { Status = "Failed", Message = "Invalid request Id." };
                    return Json(JsonConvert.SerializeObject(ajaxRespo));
                }
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    UpdateAuthToken();
                    var ajaxRespo = new { Status = "Failed", Message = "Failed to connect with provider.please try again." };
                    return Json(JsonConvert.SerializeObject(ajaxRespo));
                }
                var model = new
                {
                    TraceId = TraceId
                };
                var client = new RestClient(VastbazaarBaseUrl + "api/Bus/GetBookingDetails");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("accept-encoding", "gzip");
                request.AddBody(model);
                Task<IRestResponse> task = Task.Run(() =>
                {
                    return client.Execute(request);
                });
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                if (isCompletedSuccessfully)
                {
                    if (task.Result.StatusCode == HttpStatusCode.BadRequest || task.Result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        UpdateAuthToken();
                        var ajaxRespo = new { Status = "Failed", Message = "Unauthorized Request." };
                        return Json(JsonConvert.SerializeObject(ajaxRespo));
                    }
                    dynamic respo = JsonConvert.DeserializeObject(task.Result.Content);
                    if (string.IsNullOrWhiteSpace(task.Result.Content))
                    {
                        var ajaxRespo = new { Status = "Failed", Message = "Server is down,please try later." };
                        return Json(JsonConvert.SerializeObject(ajaxRespo));
                    }
                    if (respo.Content.ADDINFO.Error.ErrorCode != 0)
                    {
                        var ajaxRespo = new { Status = "Failed", Message = respo.Content.ADDINFO.Error.ErrorMessage };
                        return Json(JsonConvert.SerializeObject(ajaxRespo));
                    }
                    else
                    {
                        var ajaxRespo = new { Status = "Success", Message = respo.Content.ADDINFO };
                        return Json(JsonConvert.SerializeObject(ajaxRespo));
                    }
                }
                else
                {
                    var ajaxRespo = new { Status = "Failed", Message = "Server is down,please try later." };
                    return Json(JsonConvert.SerializeObject(ajaxRespo));
                }
            }
            catch
            {
                var ajaxRespo = new { Status = "Failed", Message = "Server is down,please try later." };
                return Json(JsonConvert.SerializeObject(ajaxRespo));
            }
        }
        [HttpPost]
        public ActionResult seatCancellationConfirm(string TraceId, string BusId, string Ticketno)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                if (string.IsNullOrWhiteSpace(TraceId) || string.IsNullOrWhiteSpace(BusId))
                {
                    var ajaxRespo = new { Status = "Failed", Message = "Invalid or empty booking id." };
                    return Json(JsonConvert.SerializeObject(ajaxRespo));
                }
                var ticket = db.BusTicketingDetails.SingleOrDefault(a => a.RetailerId == userid && a.TraceId == TraceId && a.TicketStatus == "Confirmed");
                if (ticket == null)
                {
                    var ajaxRespo = new { Status = "Failed", Message = "Invalid ticket for cancellation." };
                    return Json(JsonConvert.SerializeObject(ajaxRespo));
                }
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    UpdateAuthToken();
                    var ajaxRespo = new { Status = "Failed", Message = "Failed to connect with provider.please try again." };
                    return Json(JsonConvert.SerializeObject(ajaxRespo));
                }
                var client = new RestClient(VastbazaarBaseUrl + "api/Bus/ChangeRequest");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("accept-encoding", "gzip");
                var requestObj = new
                {
                    BookingTraceId = TraceId,
                    BusId = BusId,
                    TicketNo = Ticketno,
                    RequestType = 11
                };
                request.AddBody(requestObj);
                Task<IRestResponse> task = Task.Run(() =>
                {
                    return client.Execute(request);
                });
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                if (isCompletedSuccessfully)
                {
                    if (task.Result.StatusCode == HttpStatusCode.BadRequest || task.Result.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        UpdateAuthToken();
                        var ajaxRespo = new { Status = "Failed", Message = "Unauthorized Request." };
                        return Json(JsonConvert.SerializeObject(ajaxRespo));
                    }
                    var respo = JsonConvert.DeserializeObject<BusCancellationResponse>(task.Result.Content);
                    if (string.IsNullOrWhiteSpace(task.Result.Content))
                    {
                        var ajaxRespo = new { Status = "Failed", Message = "Server is down,please try later." };
                        return Json(JsonConvert.SerializeObject(ajaxRespo));
                    }
                    if (respo.Content.Addinfo.Error.ErrorCode != 0)
                    {
                        var ajaxRespo = new { Status = "Failed", Message = respo.Content.Addinfo.Error.ErrorMessage };
                        return Json(JsonConvert.SerializeObject(ajaxRespo));
                    }
                    else
                    {
                        System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                        System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                        System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                        decimal fareAmount = respo.Content.Addinfo.BusCrInfo.Sum(a => a.TotalPrice);
                        decimal refundAmount = respo.Content.Addinfo.BusCrInfo.Sum(a => a.RefundedAmount);
                        decimal cancellationCharges = respo.Content.Addinfo.BusCrInfo.Sum(a => a.CancellationCharge);
                        string CancellationTraceId = respo.Content.Addinfo.TraceId;
                        if (respo.Content.Addinfo.BusCrInfo != null && respo.Content.Addinfo.BusCrInfo.First().ChangeRequestStatus == BusCancellationStatusEnum.SUCCESS) // 3 means success
                        {
                            db.proc_UpdateBusBooking(ticket.idno.ToString(), userid, fareAmount, "", 2, "", "", "", "", "",
                               IsSuccess, Message);

                            var reqJson = JsonConvert.SerializeObject(requestObj);
                            var resJson = task.Result.Content;
                            if (Convert.ToBoolean(IsSuccess.Value) == true)
                            {
                                db.proc_SaveCancellationHistory(ticket.idno.ToString(), CancellationTraceId, fareAmount, cancellationCharges, reqJson
                                       , resJson, IsSuccess, Message);
                            }
                        }
                        var ajaxRespo = new { Status = "Success", Message = respo };
                        return Json(JsonConvert.SerializeObject(ajaxRespo));
                    }
                }
                else
                {
                    var ajaxRespo = new { Status = "Failed", Message = "Server is down,please try later." };
                    return Json(JsonConvert.SerializeObject(ajaxRespo));
                }
            }
            catch
            {
                var ajaxRespo = new { Status = "Failed", Message = "Internal server error." };
                return Json(JsonConvert.SerializeObject(ajaxRespo));
            }
        }
        public ActionResult cancellationReport()
        {
            try
            {
                var userid = User.Identity.GetUserId();
                string txt_frm_date = DateTime.Now.ToString();
                string txt_to_date = DateTime.Now.ToString();
                string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
                string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
                DateTime d = DateTime.Now.Date;
                var entries = db.BusCancellationDetails.Where(a => a.RetailerId == userid && a.TicketDate > d).OrderByDescending(a => a.idno).ToList();
                return View(entries);
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Something went wrong,please later!";
                return RedirectToAction("Travel", "Home");
            }
        }
        [HttpPost]
        public ActionResult cancellationReport(string txt_frm_date, string txt_to_date)
        {
            try
            {
                ViewBag.chk = "post";
                var userid = User.Identity.GetUserId();

                DateTime frm = Convert.ToDateTime(txt_frm_date);
                DateTime to = Convert.ToDateTime(txt_to_date);
                txt_frm_date = frm.ToString("dd-MM-yyyy");
                txt_to_date = to.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                DateTime frm_date = Convert.ToDateTime(dt).Date;
                DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
                var entries = db.BusCancellationDetails.Where(a => a.RetailerId == userid && a.TicketDate >= frm_date && a.TicketDate <= to_date).OrderByDescending(a => a.idno).ToList();
                return View(entries);
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Something went wrong,please later!";
                return RedirectToAction("Travel", "Home");
            }
        }
        public ActionResult PDFCancellationReport(string txt_frm_date, string txt_to_date)
        {
            try
            {
                ViewBag.chk = "post";
                var userid = User.Identity.GetUserId();

                DateTime frm = Convert.ToDateTime(txt_frm_date);
                DateTime to = Convert.ToDateTime(txt_to_date);
                txt_frm_date = frm.ToString("dd-MM-yyyy");
                txt_to_date = to.ToString("dd-MM-yyyy");
                string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
                DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
                DateTime frm_date = Convert.ToDateTime(dt).Date;
                DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
                var entries = db.BusCancellationDetails.Where(a => a.RetailerId == userid && a.TicketDate >= frm_date && a.TicketDate <= to_date).OrderByDescending(a => a.idno).ToList();
                return new ViewAsPdf(entries);
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "Something went wrong,please later!";
                return RedirectToAction("Travel", "Home");
            }
        }
        public ActionResult ExcelancellationReport(string txt_frm_date, string txt_to_date)
        {

            var userid = User.Identity.GetUserId();

            DateTime frm = Convert.ToDateTime(txt_frm_date);
            DateTime to = Convert.ToDateTime(txt_to_date);
            txt_frm_date = frm.ToString("dd-MM-yyyy");
            txt_to_date = to.ToString("dd-MM-yyyy");
            string[] formats = new[] { "MM/dd/yyyy", "dd-MMM-yyyy",
                            "yyyy-MM-dd", "dd-MM-yyyy", "dd MMM yyyy" };
            DateTime dt = !string.IsNullOrWhiteSpace(txt_frm_date) ? DateTime.ParseExact(txt_frm_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime dt1 = !string.IsNullOrWhiteSpace(txt_to_date) ? DateTime.ParseExact(txt_to_date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : DateTime.Now;
            DateTime frm_date = Convert.ToDateTime(dt).Date;
            DateTime to_date = Convert.ToDateTime(dt1).Date.AddDays(1);
            var entries = db.BusCancellationDetails.Where(a => a.RetailerId == userid && a.TicketDate >= frm_date && a.TicketDate <= to_date).OrderByDescending(a => a.idno).ToList();
            DataTable dataTbl = new DataTable();

            dataTbl.Columns.Add("Passanger Name", typeof(string));
            dataTbl.Columns.Add("PNR", typeof(string));
            dataTbl.Columns.Add("Source", typeof(string));
            dataTbl.Columns.Add("Destination", typeof(string));
            dataTbl.Columns.Add("DOJ ", typeof(string));
            dataTbl.Columns.Add("Fare", typeof(string));
            dataTbl.Columns.Add("Charge", typeof(string));
            dataTbl.Columns.Add("Pre", typeof(string));
            dataTbl.Columns.Add("Post", typeof(string));
            dataTbl.Columns.Add("Date", typeof(string));
            dataTbl.Columns.Add("Cancellation Date", typeof(string));


            if (entries.Any())
            {
                foreach (var item in entries)
                {
                    dataTbl.Rows.Add(item.LeadPaxFirstName, item.PNR, item.sourceStationName, item.destinationStationName, item.dateOfJourney, item.FareAmount, item.CancallationCharge, item.RemPre, item.RemPost, item.TicketDate, item.CancellationDate);
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
            Response.AddHeader("content-disposition", "attachment; filename=ExcelancellationReport.xls");
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
        [OutputCache(Duration = 2000, VaryByParam = "term")]
        public JsonResult GetBusStopName(string term)
        {
            try
            {
                string filepath = Server.MapPath("~/Doc/TBO_BUS_CITY_LIST.json");
                using (StreamReader r = new StreamReader(filepath))
                {
                    string json = r.ReadToEnd();
                    var items = JsonConvert.DeserializeObject<List<BusCityModel>>(json);
                    List<string> planets = (from p in items
                                            where p.CityName.StartsWith(term, true, null) || p.CityName.StartsWith(term, true, null)
                                            select p.CityName + "--" + p.CityId).ToList<string>();
                    //select p.AirportName).ToList<string>() ;
                    return Json(planets, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult test()
        {
            return View();
        }
        public IRestResponse tokencheck()
        {
            var apidetails = db.Money_API_URLS.Where(aa => aa.API_Name == "VASTWEB").SingleOrDefault();
            var token = apidetails == null ? "" : apidetails.Token;
            var apiid = apidetails == null ? "" : apidetails.API_ID;
            var apiidpwd = apidetails == null ? "" : apidetails.Api_pwd;
            var client = new RestClient(VastbazaarBaseUrl + "token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("iptoken", token.Trim());
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "UserName=" + HttpUtility.UrlEncode(apiid) + "&Password=" + HttpUtility.UrlEncode(apiidpwd) + "&grant_type=password", ParameterType.RequestBody);
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


        public ActionResult Bus_Print_Pdf(string TraceId)
        {
            try
            {
                Buspdfmodel pdf = new Buspdfmodel();

                if (string.IsNullOrWhiteSpace(TraceId))
                {
                    TempData["status"] = "Failed";
                    TempData["Message"] = "Invalid request Id.";
                    return RedirectToAction("BusBookingReport", "Bus");
                }
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    UpdateAuthToken();
                    TempData["status"] = "Failed";
                    TempData["Message"] = "Failed to connect with provider.please try again.";
                    return RedirectToAction("BusBookingReport", "Bus");
                }
                var model = new
                {
                    TraceId = TraceId
                };
                var client = new RestClient(VastbazaarBaseUrl + "api/Bus/GetBookingDetails");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("accept-encoding", "gzip");
                request.AddBody(model);

                Task<IRestResponse> task = Task.Run(() =>
                {
                    return client.Execute(request);
                });
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(120000));
                if (isCompletedSuccessfully)
                {
                    if (task.Result.StatusCode == HttpStatusCode.BadRequest || task.Result.StatusCode == HttpStatusCode.Unauthorized)
                    {

                        UpdateAuthToken();

                        TempData["status"] = "Failed";
                        TempData["Message"] = "Unauthorized Request.";
                        return RedirectToAction("BusBookingReport", "Bus");

                    }
                    dynamic respo = JsonConvert.DeserializeObject(task.Result.Content);

                    if (string.IsNullOrWhiteSpace(task.Result.Content))
                    {
                        TempData["status"] = "Failed";
                        TempData["Message"] = "Server is down,please try later.";
                        return RedirectToAction("BusBookingReport", "Bus");

                    }
                    if (respo.Content.ADDINFO.Error.ErrorCode != 0)
                    {
                        TempData["status"] = "Failed";
                        TempData["Message"] = respo.Content.ADDINFO.Error.ErrorMessage.ToString();
                        return RedirectToAction("BusBookingReport", "Bus");
                    }
                    else
                    {
                        var admindetils = db.Admin_details.FirstOrDefault();
                        pdf.AdminCompanyName = admindetils.Companyname;
                        //pdf.adminaddress = admindetils.Address;
                        //pdf.adminMobile = admindetils.mobile;
                        //pdf.adminPin = admindetils.pincode.ToString();
                        //pdf.adminemail = admindetils.email;
                        pdf.BookingDetails = JsonConvert.DeserializeObject<BusDetailsResponseModel>(task.Result.Content);
                        //retailer info
                        var userid = User.Identity.GetUserId();
                        var rem = db.Retailer_Details.SingleOrDefault(a => a.RetailerId == userid);
                        var markup = db.Convence_Fees.Where(a => a.RetailerId == rem.RetailerId && a.Role == "Bus").FirstOrDefault();
                        pdf.retailerAddress = rem.Address;
                        pdf.retailerEmail = rem.Email;
                        pdf.retailerMobile = rem.Mobile;
                        pdf.retailerName = rem.RetailerName;
                        pdf.Markup = Convert.ToDecimal(markup.Amount);
                        return new ViewAsPdf("Bus_Print_Pdf", pdf);
                    }
                }
                else
                {
                    TempData["status"] = "Failed";
                    TempData["Message"] = "Server is down,please try later.";
                    return RedirectToAction("BusBookingReport", "Bus");

                }
            }
            catch (Exception ex)
            {
                TempData["status"] = "Failed";
                TempData["Message"] = ex.Message.ToString();
                return RedirectToAction("BusBookingReport", "Bus");

            }
        }
    }
}
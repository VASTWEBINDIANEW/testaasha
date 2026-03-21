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
    public class HotelController : Controller
    {
        //string VastbazaarBaseUrl = "http://localhost:62147/";
        string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        VastwebmultiEntities db = new VastwebmultiEntities();
        public HotelController()
        {

        }
        public HotelController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        //[OutputCache(Duration = 2000, VaryByParam = "term")]
        public JsonResult GetHotelName(string term)
        {
            try
            {
                string filepath = Server.MapPath("~/Doc/AllHotelCityCodes.json");
                using (StreamReader r = new StreamReader(filepath))
                {
                    string json = r.ReadToEnd();
                    var items = JsonConvert.DeserializeObject<List<HotelsModel>>(json);
                    List<string> planets = items.Where(aa => aa.Destination.StartsWith(term, true, null) || aa.Country.StartsWith(term, true, null)).OrderByDescending(aa => aa.Countrycode == "IN").ThenBy(aa => aa.Countrycode).Select(p => p.Destination + "," + p.Country + "(" + p.Countrycode + ")--" + p.CityId).Take(10).ToList<string>();
                    return Json(planets, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
        }
        //[OutputCache(Duration = 2000, VaryByParam = "term")]
        public JsonResult GetHotelCountryCode(string term)
        {
            try
            {
                string filepath = Server.MapPath("~/Doc/AllHotelCityCodes.json");
                using (StreamReader r = new StreamReader(filepath))
                {
                    //string json = r.ReadToEnd();
                    //var items = JsonConvert.DeserializeObject<List<HotelsModel>>(json);
                    //List<string> planets = (from p in items
                    //                        where p.Country.StartsWith(term, true, null) || p.Countrycode.StartsWith(term, true, null)
                    //                        select p.Country + "(" + p.Countrycode + ")").ToList<string>();
                    ////select p.AirportName).ToList<string>() ;
                    //return Json(planets, JsonRequestBehavior.AllowGet);

                    string json = r.ReadToEnd();
                    var items = JsonConvert.DeserializeObject<List<HotelsModel>>(json);
                    List<string> planets = items.Where(aa => aa.Country.StartsWith(term, true, null) || aa.Countrycode.StartsWith(term, true, null)).OrderByDescending(aa => aa.Countrycode == "IN").ThenBy(aa => aa.Countrycode).Select(p => p.Country + "(" + p.Countrycode + ")").Distinct().Take(10).ToList<string>();
                    return Json(planets, JsonRequestBehavior.AllowGet);

                }
            }
            catch
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult Search()
        //{
        //    return RedirectToAction("Travel", "Home");
        //}

        //[MenuAccessFilter] //used in paid and nonpaid services
        [HttpGet]
        public ActionResult Search(string CityId, DateTime CheckInDate, string guestNationality, int[] txtAdultCountHotel,
            int[] txtChildCountHotel, int[] txtChildAge, int txtNights, int NoOfRooms, int starRating, string roomsDetails)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                ViewBag.noofdays = txtNights;
                var sts = db.Retailer_Details.Where(a => a.RetailerId == userid && a.Hotelsts == "Y" && a.Hotelsts != null).Any();
                if (sts == true)
                {

                    #region validation
                    if (string.IsNullOrWhiteSpace(roomsDetails))
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Guest and room mapping is not proper.";
                        return RedirectToAction("Travel", "Home");
                    }
                    if (string.IsNullOrWhiteSpace(CityId) || string.IsNullOrWhiteSpace(guestNationality))
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Location is required.";
                        return RedirectToAction("Travel", "Home");
                    }
                    #endregion
                    var entry = db.Convence_Fees.FirstOrDefault(a => a.RetailerId == userid && a.Role == "Hotel");
                    if (entry != null)
                    {
                        ViewBag.RetailerMarkup = entry.Amount * NoOfRooms;
                    }
                    else
                    {
                        ViewBag.RetailerMarkup = 0;
                    }

                    var hotelper = db.Slab_Hotel.Where(aa => aa.UserId == userid && aa.IsDomestic == true).SingleOrDefault().marginPercentage;
                    ViewBag.charge = hotelper;
                    DateTime CheckOutDate = CheckInDate.AddDays(txtNights);
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
                    var client1 = new RestClient(VastbazaarBaseUrl + "api/Hotel/Margin");
                    var request1 = new RestRequest(Method.POST);
                    request1.AddHeader("authorization", "bearer " + token); IRestResponse response = client1.Execute(request1);
                    var respchk = response.Content;
                    dynamic resp = JsonConvert.DeserializeObject(respchk);
                    ViewBag.vastdomestic = Convert.ToDecimal(resp.Content.ADDINFO.domestic);
                    ViewBag.vastinternational = Convert.ToDecimal(resp.Content.ADDINFO.international);

                    ViewBag.city = CityId;
                    var locationParts = CityId.Split(new string[] { "--" }, StringSplitOptions.None);
                    var subStringForCountry = CityId.Split('(', ')');
                    guestNationality = guestNationality.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                    //ViewBag.chk = "post";
                    List<RoomGuestMapping> guestsObj = new List<RoomGuestMapping>();
                    //for (int i = 0; i < txtAdultCountHotel.Length; i++)
                    //{
                    //    guestsObj.Add(new RoomGuestMapping
                    //    {
                    //        NoOfAdults = txtAdultCountHotel[i],
                    //        NoOfChild = txtChildCountHotel[i],
                    //        ChildAge = txtChildAge[i] == 0 ? null : new[] { txtChildAge[i] }
                    //    });
                    //}
                    var roomGuests = JsonConvert.DeserializeObject<HotelGuests>(roomsDetails);
                    guestsObj = roomGuests.RoomNodes;
                    guestsObj.ForEach(a =>
                    {
                        if (a.NoOfChild == 0)
                        {
                            a.ChildAge = null;
                        }
                    });
                    TempData["guestsInfo"] = guestsObj;
                    var reqObject = new
                    {
                        CheckInDate = CheckInDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        NoOfNights = txtNights.ToString(),
                        CountryCode = subStringForCountry[1],
                        CityId = locationParts[1],
                        //ResultCount = null,
                        PreferredCurrency = "INR",
                        GuestNationality = guestNationality,//"IN",
                        NoOfRooms = NoOfRooms.ToString(),
                        RoomGuests = guestsObj,
                        PreferredHotel = "",
                        MaxRating = 5,
                        MinRating = 0,
                        //ReviewScore = null,
                        IsNearBySearchAllowed = false,
                        EndUserIp = "",
                        TokenId = ""
                    };
                    //var res =  TBO_Utils.GetResponse(JsonConvert.SerializeObject(reqObject), FlightURLS.BaseAddress+ FlightURLS.SearchFlight);
                    var url = VastbazaarBaseUrl + "api/Hotel/Search";
                    var client = new RestClient(url);
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("authorization", "bearer " + token);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("accept-encoding", "gzip");
                    request.AddParameter("application/json", JsonConvert.SerializeObject(reqObject), ParameterType.RequestBody);
                    //request.AddBody(reqObject);
                    Task<IRestResponse> task = Task.Run(() =>
                    {
                        return client.Execute(request);
                    });
                    bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(40000));
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
                        var respo = JsonConvert.DeserializeObject<HotelSearchResultVm>(task.Result.Content);

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
                        TempData["AdultCount"] = txtAdultCountHotel;
                        TempData["ChildCount"] = txtChildCountHotel;
                        TempData["CheckInDate"] = CheckInDate;
                        TempData["CheckOutDate"] = CheckOutDate;
                        TempData["TotalNights"] = txtNights;
                        TempData["TotalRooms"] = NoOfRooms;
                        TempData["GuestNationality"] = guestNationality;
                        TempData["IsDomestic"] = guestNationality.Equals(subStringForCountry[1]);
                        var sourceCountry = string.Empty;
                        return View(respo);
                    }
                    else
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "It seems that provider server is down,try again.";
                        return RedirectToAction("Travel", "Home");
                    }
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Your Bus Hotel Inactive Please Contact Admin";
                    return RedirectToAction("Travel", "Home");
                }
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }
        public PartialViewResult _HotelFilter(HotelSearchResultVm model)
        {
            return PartialView(model);
        }
        [HttpGet]
        public ActionResult HotelInfo(string RIndex, string HCode, string TId, int Srate)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                HotelDetailedVM model = new HotelDetailedVM();
                var reqObject = new
                {
                    ResultIndex = RIndex,
                    HotelCode = HCode,
                    TraceId = TId
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
                #region HotelInfo
                var url = VastbazaarBaseUrl + "api/Hotel/HotelInfo";
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("accept-encoding", "gzip");
                request.AddParameter("application/json", JsonConvert.SerializeObject(reqObject), ParameterType.RequestBody);
                //request.AddBody(reqObject);
                Task<IRestResponse> task = Task.Run(() =>
                {
                    return client.Execute(request);
                });
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(40000));
                if (isCompletedSuccessfully)
                {
                    if (task.Result.StatusCode == HttpStatusCode.BadRequest)
                    {
                        //UpdateAuthToken();
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Something went wrong,please later!";
                        return RedirectToAction("Travel", "Home");
                    }
                    var respo = JsonConvert.DeserializeObject<HotelInfoResultModel>(task.Result.Content);
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
                    model.hotelInfo = respo;
                    //return View(respo);
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Unable to get hotel search reasult,try again.";
                    return RedirectToAction("Travel", "Home");
                }
                #endregion



                #region HotelRooms
                var url1 = VastbazaarBaseUrl + "api/Hotel/GetHotelRoom";
                var client1 = new RestClient(url1);
                var request1 = new RestRequest(Method.POST);
                request1.RequestFormat = DataFormat.Json;
                request1.AddHeader("authorization", "bearer " + token);
                request1.AddHeader("cache-control", "no-cache");
                request1.AddHeader("content-type", "application/json");
                request1.AddHeader("accept-encoding", "gzip");
                request1.AddParameter("application/json", JsonConvert.SerializeObject(reqObject), ParameterType.RequestBody);
                //request.AddBody(reqObject);
                Task<IRestResponse> task1 = Task.Run(() =>
                {
                    return client1.Execute(request1);
                });
                bool isCompletedSuccessfully1 = task1.Wait(TimeSpan.FromMilliseconds(40000));
                if (isCompletedSuccessfully1)
                {
                    if (task1.Result.StatusCode == HttpStatusCode.BadRequest)
                    {
                        //UpdateAuthToken();
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Something went wrong,please later!";
                        return RedirectToAction("Travel", "Home");
                    }
                    if (string.IsNullOrWhiteSpace(task1.Result.Content))
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Server down please try later";
                        return RedirectToAction("Travel", "Home");
                    }
                    var respo1 = JsonConvert.DeserializeObject<HotelRoomResultModel>(task1.Result.Content);
                    if (respo1.Content == null || respo1.Content.Addinfo == null || respo1.Content.Addinfo.Error.ErrorCode != 0)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = respo1.Content.Addinfo.Error.ErrorMessage;
                        return RedirectToAction("Travel", "Home");
                    }
                    if (respo1.Content.Addinfo.RoomCombinations.InfoSource == "OpenCombination")
                    {
                        #region convertOpenCombinationIntoFixCombination
                        RoomCombinations alteredRoomCombination = new RoomCombinations();
                        alteredRoomCombination.InfoSource = "FixedCombination";
                        alteredRoomCombination.IsPolicyPerStay = respo1.Content.Addinfo.RoomCombinations.IsPolicyPerStay;
                        List<RoomCombination> lstNewCombination = new List<RoomCombination>();
                        //var json = " {\"RoomCombination\": [ {\"RoomIndex\": [ 1,  2] }, { \"RoomIndex\": [3, 4, 5 ] }, { \"RoomIndex\": [6, 7, 8 ] }]}";
                        //dynamic jsonObject = JsonConvert.DeserializeObject(respo1);
                        int count = respo1.Content.Addinfo.RoomCombinations.RoomCombination.Count;


                        for (int j = 0; j < respo1.Content.Addinfo.RoomCombinations.RoomCombination[0].RoomIndex.Length; j++)
                        {
                            int[] finalroom = new int[count];
                            for (int k = 0; k < respo1.Content.Addinfo.RoomCombinations.RoomCombination.Count; k++)
                            {
                                finalroom[k] = respo1.Content.Addinfo.RoomCombinations.RoomCombination[k].RoomIndex[j];
                            }
                            RoomCombination item = new RoomCombination();
                            item.RoomIndex = finalroom;
                            lstNewCombination.Add(item);
                        }
                        alteredRoomCombination.RoomCombination = lstNewCombination;
                        respo1.Content.Addinfo.RoomCombinations = alteredRoomCombination;
                        #endregion
                    }
                    model.roomInfo = respo1;
                    model.CheckInDate = Convert.ToDateTime(TempData.Peek("CheckInDate"));
                    model.CheckOutDate = Convert.ToDateTime(TempData.Peek("CheckOutDate"));
                    model.TotalNights = Convert.ToInt32(TempData.Peek("TotalNights"));
                    model.TotalRooms = Convert.ToInt32(TempData.Peek("TotalRooms"));
                    model.StarRating = Srate;
                    model.RIndex = RIndex;
                    model.HCode = HCode;
                    model.GuestNationality = Convert.ToString(TempData.Peek("GuestNationality"));
                    TempData["HotelDetailedVM"] = model;
                    var entry = db.Convence_Fees.FirstOrDefault(a => a.RetailerId == userid && a.Role == "Hotel");
                    if (entry != null)
                    {
                        ViewBag.RetailerMarkup = entry.Amount;
                    }
                    else
                    {
                        ViewBag.RetailerMarkup = 0;
                    }

                    var hotelper = db.Slab_Hotel.Where(aa => aa.UserId == userid && aa.IsDomestic == true).SingleOrDefault().marginPercentage;
                    ViewBag.charge = hotelper;
                    return View(model);
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Unable to get hotel search reasult,try again.";
                    return RedirectToAction("Travel", "Home");
                }
                #endregion
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }
        [HttpGet]
        public ActionResult BookHotel(string RIndex, string HCode, string RTypeCode)
        {
            try
            {
                //BookingType : Hold/Book
                var userid = User.Identity.GetUserId();
                var entry = db.Convence_Fees.FirstOrDefault(a => a.RetailerId == userid && a.Role == "Hotel");

                var HotelDetailedVM = (HotelDetailedVM)TempData.Peek("HotelDetailedVM");
                if (HotelDetailedVM == null)
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Session expired,try again.";
                    return RedirectToAction("Travel", "Home");
                }
                var hotel = HotelDetailedVM.hotelInfo.Content.Addinfo;

                bool isVoucherBooking = false;
                List<BlockHotelRoomsDetail> lstRoom = new List<BlockHotelRoomsDetail>();
                string[] roomIndexes = RTypeCode.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                var totalroom = roomIndexes.Length;
                if (entry != null)
                {
                    ViewBag.RetailerMarkup = entry.Amount * totalroom;
                }
                else
                {
                    ViewBag.RetailerMarkup = 0;
                }
                for (int i = 0; i < roomIndexes.Length; i++)
                {
                    long ri = Convert.ToInt64(roomIndexes[i]);
                    var room = HotelDetailedVM.roomInfo.Content.Addinfo.HotelRoomsDetails.FirstOrDefault(a => a.RoomIndex == ri);

                    isVoucherBooking = room.LastCancellationDate.Date > DateTime.Now.Date ? false : true;
                    BlockHotelRoomsDetail roomItem = new BlockHotelRoomsDetail();
                    roomItem.BedTypeCode = "";
                    roomItem.Price = new BlockPrice
                    {
                        AgentCommission = room.Price.AgentCommission,
                        AgentMarkUp = room.Price.AgentMarkUp,
                        ChildCharge = room.Price.ChildCharge,
                        CurrencyCode = room.Price.CurrencyCode,
                        Discount = room.Price.Discount,
                        ExtraGuestCharge = room.Price.ExtraGuestCharge,
                        OfferedPrice = room.Price.OfferedPrice,
                        OfferedPriceRoundedOff = room.Price.OfferedPriceRoundedOff,
                        OtherCharges = room.Price.OtherCharges,
                        PublishedPrice = room.Price.PublishedPrice,
                        PublishedPriceRoundedOff = room.Price.PublishedPriceRoundedOff,
                        RoomPrice = room.Price.RoomPrice,
                        ServiceTax = room.Price.ServiceTax,
                        Tax = room.Price.Tax,
                        TDS = room.Price.Tds
                    };
                    roomItem.RatePlanCode = room.RatePlanCode;
                    roomItem.RoomIndex = room.RoomIndex;
                    roomItem.RoomTypeCode = room.RoomTypeCode;
                    roomItem.RoomTypeName = room.RoomTypeName;
                    roomItem.SmokingPreference = room.SmokingPreference;
                    roomItem.Supplements = room.HotelSupplements;
                    lstRoom.Add(roomItem);
                }
                HotelBlockRoomRequestModel roomblockModel = new HotelBlockRoomRequestModel();
                roomblockModel.ResultIndex = Convert.ToInt32(RIndex);
                //roomblockModel.ClientReferenceNo = "0"; //Currently not in use at TBO End.
                roomblockModel.GuestNationality = HotelDetailedVM.GuestNationality;// "IN";
                roomblockModel.HotelCode = HCode;
                roomblockModel.HotelName = hotel.HotelDetails.HotelName;
                roomblockModel.NoOfRooms = HotelDetailedVM.TotalRooms;
                roomblockModel.IsVoucherBooking = isVoucherBooking; //If cancallationdate is today date then room hold is not allowed.
                roomblockModel.TraceId = HotelDetailedVM.roomInfo.Content.Addinfo.TraceId;
                roomblockModel.HotelRoomsDetails = lstRoom;
                #region HotelBlockRoom
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

                var url = VastbazaarBaseUrl + "api/Hotel/BlockRoom";
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", "bearer " + token);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("accept-encoding", "gzip");
                request.AddParameter("application/json", JsonConvert.SerializeObject(roomblockModel), ParameterType.RequestBody);
                Task<IRestResponse> task = Task.Run(() =>
                {
                    return client.Execute(request);
                });
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(40000));
                if (isCompletedSuccessfully)
                {
                    if (task.Result.StatusCode == HttpStatusCode.BadRequest)
                    {
                        UpdateAuthToken();
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Something went wrong,please later!";
                        return RedirectToAction("Travel", "Home");
                    }
                    if (string.IsNullOrWhiteSpace(task.Result.Content))
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Unable to get booking status,please check booking status before proceeding";
                        return RedirectToAction("Travel", "Home");
                    }
                    var respo = JsonConvert.DeserializeObject<HotelBlockRoomResponseModel>(task.Result.Content);
                    if (respo.Content == null || respo.Content.Addinfo == null || respo.Content.Addinfo.Error.ErrorCode != 0)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = respo.Content.Addinfo.Error.ErrorMessage;
                        return RedirectToAction("Travel", "Home");
                    }
                    //Online booking is not avilalbe
                    if (respo.Content.Addinfo.AvailabilityType == "Available")
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Online booking is not working for this hotel, please contact admin for offline booking";
                        return RedirectToAction("Travel", "Home");
                    }
                    respo.RTypeCode = RTypeCode;
                    respo.IsDomestic = Convert.ToBoolean(TempData.Peek("IsDomestic"));
                    TempData["HotelBlockRoomResponseModel"] = respo;

                    return View(respo);
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Unable to get hotel search reasult,try again.";
                    return RedirectToAction("Travel", "Home");
                }
                #endregion
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }
        [HttpPost]
        public ActionResult BookHotel(HotelBlockRoomResponseModel model, string BokingType)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var userid = User.Identity.GetUserId();
                    //var BlockRoomResponseModel = (HotelBlockRoomResponseModel)TempData.Peek("HotelBlockRoomResponseModel");
                    var BlockRoomResponseModel = (HotelBlockRoomResponseModel)TempData["HotelBlockRoomResponseModel"];
                    if (BlockRoomResponseModel == null)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Session Expired.try again";
                        return RedirectToAction("Travel", "Home");
                    }
                    model.Content.Addinfo = BlockRoomResponseModel.Content.Addinfo;
                    var HotelDetailedVM = (HotelDetailedVM)TempData.Peek("HotelDetailedVM");
                    if (HotelDetailedVM == null)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Session expired,try again.";
                        return RedirectToAction("Travel", "Home");
                    }
                    //string TXNID = DateTime.Now.ToString("dd-MM-yyy hh-mm-ss").Replace("-", "").Replace(" ", "");
                    string TXNID = BlockRoomResponseModel.Content.Addinfo.TraceId;//DateTime.Now.ToString("hh-mm-ss").Replace("-", "").Replace(" ", "");
                    #region InitializeRequest
                    var hotel = HotelDetailedVM.hotelInfo.Content.Addinfo;
                    bool isVoucherBooking = false;
                    DateTime lastCancallationdat = DateTime.Now.AddDays(1).AddMinutes(-1);

                    List<BookHotelRoomsDetail> lstRoom = new List<BookHotelRoomsDetail>();
                    string[] roomIndexes = model.RTypeCode.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < roomIndexes.Length; i++)
                    {
                        long ri = Convert.ToInt64(roomIndexes[i]);
                        var room = HotelDetailedVM.roomInfo.Content.Addinfo.HotelRoomsDetails.FirstOrDefault(a => a.RoomIndex == ri);
                        lastCancallationdat = room.LastCancellationDate;
                        isVoucherBooking = room.LastCancellationDate.Date > DateTime.Now.Date ? false : true;
                        BookHotelRoomsDetail roomItem = new BookHotelRoomsDetail();
                        roomItem.HotelPassenger = model.BookHotelRoomDetails[i].HotelPassenger;
                        roomItem.BedTypeCode = "";
                        roomItem.Price = new BlockPrice
                        {
                            AgentCommission = room.Price.AgentCommission,
                            AgentMarkUp = room.Price.AgentMarkUp,
                            ChildCharge = room.Price.ChildCharge,
                            CurrencyCode = room.Price.CurrencyCode,
                            Discount = room.Price.Discount,
                            ExtraGuestCharge = room.Price.ExtraGuestCharge,
                            OfferedPrice = room.Price.OfferedPrice,
                            OfferedPriceRoundedOff = room.Price.OfferedPriceRoundedOff,
                            OtherCharges = room.Price.OtherCharges,
                            PublishedPrice = room.Price.PublishedPrice,
                            PublishedPriceRoundedOff = room.Price.PublishedPriceRoundedOff,
                            RoomPrice = room.Price.RoomPrice,
                            ServiceTax = room.Price.ServiceTax,
                            Tax = room.Price.Tax,
                            TDS = room.Price.Tds
                        };
                        roomItem.RatePlanCode = room.RatePlanCode;
                        roomItem.RoomIndex = room.RoomIndex;
                        roomItem.RoomTypeCode = room.RoomTypeCode;
                        roomItem.SmokingPreference = room.SmokingPreference;
                        roomItem.Supplements = room.HotelSupplements;
                        lstRoom.Add(roomItem);
                    }
                    if (BokingType == "PAY NOW")
                    {
                        isVoucherBooking = true;
                    }
                    else
                    {
                        isVoucherBooking = false;
                    }
                    HotelBookRoomRequestModel roomblockModel = new HotelBookRoomRequestModel();
                    roomblockModel.ResultIndex = Convert.ToInt32(HotelDetailedVM.RIndex);
                    //roomblockModel.ClientReferenceNo = "0"; //currently not in use at TBO end.
                    roomblockModel.GuestNationality = HotelDetailedVM.GuestNationality;//"IN";
                    roomblockModel.HotelCode = HotelDetailedVM.HCode;
                    roomblockModel.HotelName = hotel.HotelDetails.HotelName;
                    roomblockModel.NoOfRooms = HotelDetailedVM.TotalRooms;
                    roomblockModel.IsVoucherBooking = isVoucherBooking; //If cancallationdate is today date then room hold is not allowed.
                    roomblockModel.TraceId = HotelDetailedVM.roomInfo.Content.Addinfo.TraceId;
                    roomblockModel.HotelRoomsDetails = lstRoom;
                    roomblockModel.checkindate = HotelDetailedVM.CheckInDate;
                    roomblockModel.checkoutdate = HotelDetailedVM.CheckOutDate;
                    #endregion
                    if (isVoucherBooking) //Direct Booking
                    {
                        #region savePriceAndPaxDetails
                        List<Vastwebmulti.Models.HotelPassenger> paxInfo = roomblockModel.HotelRoomsDetails.SelectMany(a => a.HotelPassenger).Select(a => new Vastwebmulti.Models.HotelPassenger
                        {
                            Age = a.Age.ToString(),
                            AgentRefno = TXNID, //Must be Numaric characters only.
                            Email = a.Email,
                            FirstName = a.FirstName,
                            LastName = a.LastName,
                            LeadPassenger = a.LeadPassenger,
                            MiddleName = a.MiddleName,
                            retailerid = userid,
                            Title = a.Title,
                            PaxType = (int)a.PaxType,
                            Phoneno = a.phoneno,
                            PassportNo = a.PassportNo,
                            PassportIssueDate = a.PassportIssueDate != null ? Convert.ToDateTime(a.PassportIssueDate) : Convert.ToDateTime("1900-01-01 00:00:00.000"),
                            PassportExpDate = a.PassportExpDate != null ? Convert.ToDateTime(a.PassportExpDate) : Convert.ToDateTime("1900-01-01 00:00:00.000")
                        }).ToList();
                        db.HotelPassengers.AddRange(paxInfo);
                        List<Vastwebmulti.Models.HotelPrice> priceInfo = roomblockModel.HotelRoomsDetails.Select(a => new Vastwebmulti.Models.HotelPrice
                        {
                            AgentCommission = a.Price.AgentCommission,
                            AgentMarkUp = a.Price.AgentMarkUp,
                            AgentRefno = TXNID,//Numaric charcters only
                            ChildCharge = a.Price.ChildCharge,
                            CurrencyCode = a.Price.CurrencyCode,
                            Discount = a.Price.Discount,
                            ExtraGuestCharge = a.Price.ExtraGuestCharge,
                            OfferedPrice = a.Price.OfferedPrice,
                            OfferedPriceRoundedOff = a.Price.OfferedPriceRoundedOff,
                            OtherCharges = a.Price.OtherCharges,
                            PublishedPrice = a.Price.PublishedPrice,
                            PublishedPriceRoundedOff = a.Price.PublishedPriceRoundedOff,
                            retailerid = userid,
                            RoomPrice = a.Price.RoomPrice,
                            ServiceTax = a.Price.ServiceTax,
                            Tax = a.Price.Tax,
                            TDS = a.Price.TDS,
                        }).ToList();
                        db.HotelPrices.AddRange(priceInfo);
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
                        var client1 = new RestClient(VastbazaarBaseUrl + "api/Hotel/Margin");
                        var request1 = new RestRequest(Method.POST);
                        request1.AddHeader("authorization", "bearer " + token); IRestResponse response = client1.Execute(request1);
                        var respchk = response.Content;
                        dynamic resp = JsonConvert.DeserializeObject(respchk);
                        decimal vastmargin = Convert.ToDecimal(resp.Content.ADDINFO.domestic);

                        System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                        System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));
                        System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                        decimal totalOferFare = roomblockModel.HotelRoomsDetails.Sum(a => a.Price.OfferedPriceRoundedOff);
                        decimal vastmm = (totalOferFare * vastmargin) / 100;
                        totalOferFare = totalOferFare - vastmm;
                        decimal totalPublishedFare = roomblockModel.HotelRoomsDetails.Sum(a => a.Price.PublishedPriceRoundedOff);
                        var dbrespo = db.proc_HotelBookingPayment(userid, "", roomblockModel.GuestNationality, roomblockModel.NoOfRooms, "", roomblockModel.IsVoucherBooking, "", "", "", roomblockModel.HotelCode, roomblockModel.HotelName, roomblockModel.ResultIndex.ToString(),
                         roomblockModel.TraceId, "", "", "", "", "", "", "", JsonConvert.SerializeObject(roomblockModel), totalOferFare, totalPublishedFare
                         , roomblockModel.checkindate, roomblockModel.checkoutdate, model.IsDomestic, Idno, IsSuccess, Message);
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
                                Details = "Hotel Booking ",
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
                                Details = "Hotel Booking ",
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
                                Details = "Hotel Booking ",
                                RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                Usertype = "Master"
                            };
                            back.info(model2);
                        }
                        catch { }
                        #endregion
                        if (Convert.ToBoolean(IsSuccess.Value) == true)
                        {
                            db.SaveChanges();
                            #region HotelBlockRoom
                            token = string.Empty;
                            token = getAuthToken();
                            //token = "v8Vr_d0PbVU9mnzxjludAfL12RYq9moHIiJeWSxVL5fzydzLguHYRCfB-uws7pH79vB5pmqEYQqM3agIh6kx5r4Oby4Dvv5eWWAzbpQtuMLGhrdDl3FZ9ySXd2rlYGz_mn0svSJyFp8-LvHmM_qAIJRZ3tTxr0vstIdC14YLGdFKMiiXeWsIunHt5b6rZ3jqnXTDTFzXpOZFUH_arKpdFKUpR_VXg9Z9S10JCDt72jQp2_jQERaLFQ-SJbidA7VlSGxXRrtnBx-j1d1XpKg3JE_zeop02Q3iQEMmhDYrPkJBfTeEUHoFU7zLALcg2LL_mayd1IPnnCx4--rwD_ssiRyzmoqFKSaUqcAbHBbuG6opAqyBqI8f4dsp8PKNECll-PpO4fUIiOpDUbHNT0sWk1Yf5k2quxbc2MI2YnSFFscydX0B8KCLqSgPdGGjmaMXBcSwB0spXw2iikwLuOWkOmKwE1m0e2g3yr72g2O4srQZN8-6Icf6GAhU85G4LUXqimcnuhnEP0g6qaAyP7zgbg";
                            if (string.IsNullOrWhiteSpace(token))
                            {
                                UpdateAuthToken();
                                TempData["Status"] = "Failed";
                                TempData["Message"] = "Failed to connect with provider.please try again.";
                                return RedirectToAction("Travel", "Home");
                            }
                            var url = VastbazaarBaseUrl + "api/Hotel/BookHotel";
                            var client = new RestClient(url);
                            var request = new RestRequest(Method.POST);
                            request.RequestFormat = DataFormat.Json;
                            request.AddHeader("authorization", "bearer " + token);
                            request.AddHeader("cache-control", "no-cache");
                            request.AddHeader("content-type", "application/json");
                            request.AddHeader("accept-encoding", "gzip");
                            request.AddParameter("application/json", JsonConvert.SerializeObject(roomblockModel), ParameterType.RequestBody);
                            client.ReadWriteTimeout = 180000;
                            Task<IRestResponse> task = Task.Run(() =>
                            {
                                return client.Execute(request);
                            });
                            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(180000));
                            if (isCompletedSuccessfully)
                            {
                                if (task.Result.StatusCode == HttpStatusCode.BadRequest)
                                {
                                    UpdateAuthToken();
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
                                if (respo.Content == null || respo.Content.ADDINFO == null || respo.Content.ADDINFO.Error.ErrorCode != 0)
                                {
                                    db.proc_UpdateHotelBooking(Idno.Value.ToString(), userid, task.Result.Content, 1, "Failed", "", "", null, isVoucherBooking, "", null, "", IsSuccess, Message);
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
                                            Details = "Hotel Booking Refund " ,
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
                                            Details = "Hotel Booking Refund ",
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
                                            Details = "Hotel Booking Refund ",
                                            RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                            Usertype = "Master"
                                        };
                                        back.info(model2);
                                    }
                                    catch { }
                                    TempData["Status"] = "Failed";
                                    TempData["Message"] = respo.Content.ADDINFO.Error.ErrorMessage;
                                    return RedirectToAction("Travel", "Home");
                                }
                                string BookingSts = Convert.ToString(respo.Content.ADDINFO.HotelBookingStatus);
                                string BookingId = Convert.ToString(respo.Content.ADDINFO.BookingId);
                                string BookingRefNo = Convert.ToString(respo.Content.ADDINFO.BookingRefNo);
                                string TraceId = Convert.ToString(respo.Content.ADDINFO.TraceId);
                                string InvoiceNumber = Convert.ToString(respo.Content.ADDINFO.InvoiceNumber);
                                bool VoucherStatus = Convert.ToBoolean(respo.Content.ADDINFO.VoucherStatus);
                                bool IsPriceChanged = Convert.ToBoolean(respo.Content.ADDINFO.IsPriceChanged);
                                bool IsCancellationPolicyChanged = Convert.ToBoolean(respo.Content.ADDINFO.IsCancellationPolicyChanged);
                                if (BookingSts == "Confirmed")
                                {
                                    db.proc_UpdateHotelBooking(Idno.Value.ToString(), userid, task.Result.Content, 0, BookingSts, InvoiceNumber,
                                        "", IsPriceChanged, isVoucherBooking, BookingRefNo, IsCancellationPolicyChanged, BookingId, IsSuccess, Message);
                                    TempData["Status"] = "Success";
                                    TempData["Message"] = "Hotel booked successfully.";
                                    return RedirectToAction("Travel", "Home");
                                }
                                else
                                {
                                    db.proc_UpdateHotelBooking(Idno.Value.ToString(), userid, task.Result.Content, 1, BookingSts, InvoiceNumber, "", IsPriceChanged,
                                    isVoucherBooking, BookingRefNo, IsCancellationPolicyChanged, BookingId, IsSuccess, Message);
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
                                            Details = "Hotel Booking Refund ",
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
                                            Details = "Hotel Booking Refund ",
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
                                            Details = "Hotel Booking Refund ",
                                            RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                            Usertype = "Master"
                                        };
                                        back.info(model2);
                                    }
                                    catch { }
                                    TempData["Status"] = "Failed";
                                    TempData["Message"] = "Hotel booking is " + BookingSts;
                                    return RedirectToAction("Travel", "Home");
                                }
                            }
                            else
                            {
                                TempData["Status"] = "Failed";
                                TempData["Message"] = "Unable to get hotel search result,try again.";
                                return RedirectToAction("Travel", "Home");
                            }
                            #endregion
                        }
                        else
                        {
                            TempData["Status"] = "Failed";
                            TempData["Message"] = Convert.ToString(Message.Value);
                            return RedirectToAction("Travel", "Home");
                        }
                    }
                    else //Hold and booking  //TODO
                    {
                        #region savePriceAndPaxDetails
                        List<Vastwebmulti.Models.HotelPassenger> paxInfo = roomblockModel.HotelRoomsDetails.SelectMany(a => a.HotelPassenger).Select(a => new Vastwebmulti.Models.HotelPassenger
                        {
                            Age = a.Age.ToString(),
                            AgentRefno = TXNID, //Must be Numaric characters only.
                            Email = a.Email,
                            FirstName = a.FirstName,
                            LastName = a.LastName,
                            LeadPassenger = a.LeadPassenger,
                            MiddleName = a.MiddleName,
                            retailerid = userid,
                            Title = a.Title,
                            PaxType = (int)a.PaxType,
                            Phoneno = a.phoneno,
                            PassportNo = a.PassportNo,
                            PassportIssueDate = a.PassportIssueDate != null ? Convert.ToDateTime(a.PassportIssueDate) : Convert.ToDateTime("1900-01-01 00:00:00.000"),
                            PassportExpDate = a.PassportExpDate != null ? Convert.ToDateTime(a.PassportExpDate) : Convert.ToDateTime("1900-01-01 00:00:00.000")
                        }).ToList();
                        db.HotelPassengers.AddRange(paxInfo);
                        List<Vastwebmulti.Models.HotelPrice> priceInfo = roomblockModel.HotelRoomsDetails.Select(a => new Vastwebmulti.Models.HotelPrice
                        {
                            AgentCommission = a.Price.AgentCommission,
                            AgentMarkUp = a.Price.AgentMarkUp,
                            AgentRefno = TXNID,//Numaric charcters only
                            ChildCharge = a.Price.ChildCharge,
                            CurrencyCode = a.Price.CurrencyCode,
                            Discount = a.Price.Discount,
                            ExtraGuestCharge = a.Price.ExtraGuestCharge,
                            OfferedPrice = a.Price.OfferedPrice,
                            OfferedPriceRoundedOff = a.Price.OfferedPriceRoundedOff,
                            OtherCharges = a.Price.OtherCharges,
                            PublishedPrice = a.Price.PublishedPrice,
                            PublishedPriceRoundedOff = a.Price.PublishedPriceRoundedOff,
                            retailerid = userid,
                            RoomPrice = a.Price.RoomPrice,
                            ServiceTax = a.Price.ServiceTax,
                            Tax = a.Price.Tax,
                            TDS = a.Price.TDS,
                        }).ToList();
                        db.HotelPrices.AddRange(priceInfo);
                        System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                        System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                        System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                        decimal totalOferFare = roomblockModel.HotelRoomsDetails.Sum(a => a.Price.OfferedPriceRoundedOff);
                        decimal totalPublishedFare = roomblockModel.HotelRoomsDetails.Sum(a => a.Price.PublishedPriceRoundedOff);
                        var dbrespo = db.proc_HoldHotelBooking(userid, TXNID, roomblockModel.GuestNationality, roomblockModel.NoOfRooms, TXNID, roomblockModel.IsVoucherBooking, lastCancallationdat, "", "", "", roomblockModel.HotelCode, roomblockModel.HotelName, roomblockModel.ResultIndex.ToString(),
                         roomblockModel.TraceId, "", "", "", "", "", "", "", JsonConvert.SerializeObject(roomblockModel), totalOferFare, totalPublishedFare,
                         true, Idno, IsSuccess, Message);
                        #endregion
                        if (Convert.ToBoolean(IsSuccess.Value) == true)
                        {
                            db.SaveChanges();
                            #region HotelBlockRoom
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
                            var url = VastbazaarBaseUrl + "api/Hotel/BookHotel";
                            var client = new RestClient(url);
                            var request = new RestRequest(Method.POST);
                            request.RequestFormat = DataFormat.Json;
                            request.AddHeader("authorization", "bearer " + token);
                            request.AddHeader("cache-control", "no-cache");
                            request.AddHeader("content-type", "application/json");
                            request.AddHeader("accept-encoding", "gzip");
                            request.AddParameter("application/json", JsonConvert.SerializeObject(roomblockModel), ParameterType.RequestBody);
                            client.ReadWriteTimeout = 180000;
                            Task<IRestResponse> task = Task.Run(() =>
                            {
                                return client.Execute(request);
                            });
                            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(180000));
                            if (isCompletedSuccessfully)
                            {
                                if (task.Result.StatusCode == HttpStatusCode.BadRequest)
                                {
                                    UpdateAuthToken();
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
                                if (respo.Content == null || respo.Content.ADDINFO == null || respo.Content.ADDINFO.Error.ErrorCode != 0)
                                {
                                    db.proc_UpdateHotelBooking(Idno.Value.ToString(), userid, task.Result.Content, 1, "Failed", "", "", null,
                                        isVoucherBooking, "", null, "", IsSuccess, Message);
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
                                            Details = "Hotel Booking Refund " ,
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
                                            Details = "Hotel Booking Refund ",
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
                                            Details = "Hotel Booking Refund ",
                                            RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                            Usertype = "Master"
                                        };
                                        back.info(model2);
                                    }
                                    catch { }
                                    TempData["Status"] = "Failed";
                                    TempData["Message"] = respo.Content.ADDINFO.Error.ErrorMessage;
                                    return RedirectToAction("Travel", "Home");
                                }
                                string BookingSts = Convert.ToString(respo.Content.ADDINFO.HotelBookingStatus);
                                string BookingId = Convert.ToString(respo.Content.ADDINFO.BookingId);
                                string BookingRefNo = Convert.ToString(respo.Content.ADDINFO.BookingRefNo);
                                string TraceId = Convert.ToString(respo.Content.ADDINFO.TraceId);
                                string InvoiceNumber = Convert.ToString(respo.Content.ADDINFO.InvoiceNumber);
                                bool VoucherStatus = Convert.ToBoolean(respo.Content.ADDINFO.VoucherStatus);
                                bool IsPriceChanged = Convert.ToBoolean(respo.Content.ADDINFO.IsPriceChanged);
                                bool IsCancellationPolicyChanged = Convert.ToBoolean(respo.Content.ADDINFO.IsCancellationPolicyChanged);
                                if (BookingSts == "Confirmed")
                                {
                                    db.proc_UpdateHotelBooking(Idno.Value.ToString(), userid, task.Result.Content, 0, BookingSts, InvoiceNumber, "", IsPriceChanged,
                                    isVoucherBooking, BookingRefNo, IsCancellationPolicyChanged, BookingId, IsSuccess, Message);
                                    TempData["Status"] = "Success";
                                    TempData["Message"] = "Hotel booked successfully.";
                                    return RedirectToAction("Travel", "Home");
                                }
                                else
                                {
                                    db.proc_UpdateHotelBooking(Idno.Value.ToString(), userid, task.Result.Content, 1, BookingSts, InvoiceNumber, "", IsPriceChanged,
                                    isVoucherBooking, BookingRefNo, IsCancellationPolicyChanged, BookingId, IsSuccess, Message);
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
                                            Details = "Hotel Booking Refund ",
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
                                            Details = "Hotel Booking Refund ",
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
                                            Details = "Hotel Booking Refund ",
                                            RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                            Usertype = "Master"
                                        };
                                        back.info(model2);
                                    }
                                    catch { }
                                    TempData["Status"] = "Failed";
                                    TempData["Message"] = "Hotel booking is " + BookingSts;
                                    return RedirectToAction("Travel", "Home");
                                }
                            }
                            else
                            {
                                TempData["Status"] = "Failed";
                                TempData["Message"] = "Unable to get hotel search result,try again.";
                                return RedirectToAction("Travel", "Home");
                            }
                            #endregion
                        }
                        else
                        {
                            TempData["Status"] = "Failed";
                            TempData["Message"] = Convert.ToString(Message.Value);
                            return RedirectToAction("Travel", "Home");
                        }
                    }
                }
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while processing booking request,please check booking status if any payment is deducted.";
                return RedirectToAction("Travel", "Home");
            }
        }

        public ActionResult HotelReport()
        {
            var userid = User.Identity.GetUserId();
            DateTime from = DateTime.Now.Date;
            DateTime to = from.AddDays(1);
            var ch = db.proc_HotelReport_new(from, to, "", "", "Retailer", userid).ToList();
            return View(ch);
        }
        [HttpPost]
        public ActionResult HotelReport(string ddl_status, string ddl_status_ticket, DateTime txt_frm_date, DateTime txt_to_date)
        {
            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            DateTime to = txt_to_date.AddDays(1);
            var ch = db.proc_HotelReport_new(txt_frm_date, to, ddl_status, ddl_status_ticket, "Retailer", userid).ToList();
            return View(ch);
        }
        public ActionResult PDF_HotelReport(string ddl_status, string ddl_status_ticket, DateTime txt_frm_date, DateTime txt_to_date)
        {
            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            DateTime to = txt_to_date.AddDays(1);
            var ch = db.proc_HotelReport_new(txt_frm_date, to, ddl_status, ddl_status_ticket, "Retailer", userid).ToList();
            return new ViewAsPdf(ch);
        }
        public ActionResult ExcelHotelReport(string ddl_status, string ddl_status_ticket, DateTime txt_frm_date, DateTime txt_to_date)
        {
            ViewBag.chk = "post";
            var userid = User.Identity.GetUserId();
            DateTime to = txt_to_date.AddDays(1);
            var ch = db.proc_HotelReport_new(txt_frm_date, to, ddl_status, ddl_status_ticket, "Retailer", userid).ToList();
            DataTable dataTbl = new DataTable();
            dataTbl.Columns.Add("Hotel Name", typeof(string));
            dataTbl.Columns.Add("Rooms", typeof(string));
            dataTbl.Columns.Add("Publish Fare", typeof(string));
            dataTbl.Columns.Add("Pre", typeof(string));
            dataTbl.Columns.Add("Offer Fare", typeof(string));
            dataTbl.Columns.Add("Debit Amount ", typeof(string));
            dataTbl.Columns.Add("Tds", typeof(string));
            dataTbl.Columns.Add("Post", typeof(string));
            dataTbl.Columns.Add("Booking Id ", typeof(string));
            dataTbl.Columns.Add("Booking Status", typeof(string));
            dataTbl.Columns.Add("Ticket Status", typeof(string));
            dataTbl.Columns.Add("Date", typeof(string));
            dataTbl.Columns.Add("Check In Date", typeof(string));
            dataTbl.Columns.Add("Check Out Date", typeof(string));


            if (ch.Any())
            {
                foreach (var item in ch)
                {
                    dataTbl.Rows.Add(item.HotelName, item.NoOfRooms, item.totalpublishfare, item.rempre, item.totalOfferedFare, item.reminc, item.remtds, item.rempost, item.BookingId, item.HotelBookingStatus, item.TicketStatus, item.ticketdate, item.checkindate, item.checkoutdate);
                }
            }
            else
            {
                dataTbl.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }
            var grid = new GridView();
            grid.DataSource = dataTbl;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ExcelHotelReport.xls");
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
        public class JsonModel
        {
            public string HTMLString { get; set; }
            public bool NoMoredata { get; set; }
        }
        [ChildActionOnly]
        public ActionResult _HotelReport(string txt_frm_date, string txt_to_date, string ddl_status, string PNR)
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                if (txt_frm_date == null && txt_to_date == null && ddl_status == null)
                {
                    txt_frm_date = DateTime.Now.ToString();
                    txt_to_date = DateTime.Now.ToString();
                    ddl_status = "";
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
                int pagesize = 20;
                var proc_Response = db.proc_HotelReport(1, 20, ddl_status, userid, null, null, null, null, null, null, null, frm_date, to_date).ToList();
                return View(proc_Response);
            }
            //int pagesize = 20;
            //var rowdata = db.Sp_Recharge_info_LazyLoad(1, pagesize, "Retailer", userid, Convert.ToDateTime(frm_date), Convert.ToDateTime(to_date), Operator, txtmob, ddl_status).ToList();
            //return View(rowdata);
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
            if (ddl_status == "ALL")
            {
                ddl_status = null;
            }
            var tbrow = db.proc_HotelReport(pageindex, pagesize, ddl_status, userid, null, null, null, null, null, null, null, frm_date, to_date).ToList();

            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoredata = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewtostring("_hotelreport", tbrow);
            return Json(jsonmodel);
        }


        [HttpPost]
        public ActionResult GuestDetails(string TXNID)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var guestList = db.HotelPassengers.Where(a => a.AgentRefno == TXNID).ToList();
                    return PartialView(guestList);
                }
            }
            catch
            {
                return PartialView();
            }
        }
        [HttpPost]
        public ActionResult HotelPriceDetails(string TXNID)
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var guestList = db.HotelPrices.Where(a => a.AgentRefno == TXNID).ToList();
                    return PartialView(guestList);
                }
            }
            catch
            {
                return PartialView();
            }
        }
        [HttpPost]
        public ActionResult HotelBookingStatus(string TXNID)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                if (!string.IsNullOrWhiteSpace(TXNID))
                {
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
                    var respo = GetBookingDetails(token, "", "", TXNID);
                    if (respo == null)
                    {
                        var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    else if (respo.StatusCode == HttpStatusCode.OK)
                    {
                        dynamic respoObject = JsonConvert.DeserializeObject(respo.Content);
                        if (respoObject.Content != null || respoObject.Content.ADDINFO != null)
                        {
                            if (respoObject.Content.ADDINFO.Error.ErrorCode == 0)
                            {
                                using (VastwebmultiEntities db = new VastwebmultiEntities())
                                {

                                    string BookingSts = Convert.ToString(respoObject.Content.ADDINFO.HotelBookingStatus);
                                    string BookingId = Convert.ToString(respoObject.Content.ADDINFO.BookingId);
                                    string BookingRefNo = Convert.ToString(respoObject.Content.ADDINFO.BookingRefNo);
                                    string TraceId = Convert.ToString(respoObject.Content.ADDINFO.TraceId);
                                    string InvoiceNumber = Convert.ToString(respoObject.Content.ADDINFO.InvoiceNo);
                                    bool VoucherStatus = Convert.ToBoolean(respoObject.Content.ADDINFO.VoucherStatus);
                                    bool IsPriceChanged = Convert.ToBoolean(respoObject.Content.ADDINFO.IsPriceChanged);
                                    bool IsCancellationPolicyChanged = Convert.ToBoolean(respoObject.Content.ADDINFO.IsCancellationPolicyChanged);
                                    var Entry = db.Hotel_info.FirstOrDefault(a => a.TraceId == TXNID);
                                    System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                                    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                                    if (BookingSts == "Confirmed" && Entry.HotelBookingStatus == "Proccessed")
                                    {
                                        db.proc_UpdateHotelBooking(Entry.idno.ToString(), userid, respo.Content, 0, BookingSts, InvoiceNumber, "", IsPriceChanged,
                                    VoucherStatus, BookingRefNo, IsCancellationPolicyChanged, BookingId, IsSuccess, Message);
                                        var AjaxRespo1 = new { Status = "Success", Message = "Booking success." };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }
                                    else if (Entry.HotelBookingStatus == "Proccessed")
                                    {
                                        db.proc_UpdateHotelBooking(Entry.idno.ToString(), userid, respo.Content, 1, BookingSts, InvoiceNumber, "", IsPriceChanged,
                                   VoucherStatus, BookingRefNo, IsCancellationPolicyChanged, BookingId, IsSuccess, Message);
                                   
                                        var AjaxRespo1 = new { Status = "Failed", Message = "Booking status : " + BookingSts };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }
                                    else
                                    {
                                        var AjaxRespo1 = new { Status = "Success", Message = "Booking Status : " + BookingSts };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }
                                }
                            }
                            else
                            {
                                var AjaxRespo11 = new { Status = "Failed", Message = Convert.ToString(respoObject.Content.ADDINFO.Error.ErrorMessage) };
                                return Json(JsonConvert.SerializeObject(AjaxRespo11));
                            }
                        }
                        var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    else
                    {
                        var AjaxRespo = new { Status = "Failed", Message = "StatusCode : " + respo.StatusCode.ToString() };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                }
                else
                {
                    var AjaxRespo = new { Status = "Failed", Message = "Invalid Request" };
                    return Json(JsonConvert.SerializeObject(AjaxRespo));
                }
            }
            catch (Exception ex)
            {
                var AjaxRespo = new { Status = "Failed", Message = ex.Message };
                return Json(JsonConvert.SerializeObject(AjaxRespo));
            }
        }
        [HttpGet]
        public ActionResult HotelFullDetails(string TXNID)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                if (!string.IsNullOrWhiteSpace(TXNID))
                {
                    var token = string.Empty;
                    token = getAuthToken();
                    //token = "v8Vr_d0PbVU9mnzxjludAfL12RYq9moHIiJeWSxVL5fzydzLguHYRCfB-uws7pH79vB5pmqEYQqM3agIh6kx5r4Oby4Dvv5eWWAzbpQtuMLGhrdDl3FZ9ySXd2rlYGz_mn0svSJyFp8-LvHmM_qAIJRZ3tTxr0vstIdC14YLGdFKMiiXeWsIunHt5b6rZ3jqnXTDTFzXpOZFUH_arKpdFKUpR_VXg9Z9S10JCDt72jQp2_jQERaLFQ-SJbidA7VlSGxXRrtnBx-j1d1XpKg3JE_zeop02Q3iQEMmhDYrPkJBfTeEUHoFU7zLALcg2LL_mayd1IPnnCx4--rwD_ssiRyzmoqFKSaUqcAbHBbuG6opAqyBqI8f4dsp8PKNECll-PpO4fUIiOpDUbHNT0sWk1Yf5k2quxbc2MI2YnSFFscydX0B8KCLqSgPdGGjmaMXBcSwB0spXw2iikwLuOWkOmKwE1m0e2g3yr72g2O4srQZN8-6Icf6GAhU85G4LUXqimcnuhnEP0g6qaAyP7zgbg";
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        UpdateAuthToken();
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Failed to connect with provider.please try again.";
                        return RedirectToAction("HotelReport", "Hotel");
                    }
                    var respo = GetBookingDetails(token, "", "", TXNID);
                    if (respo == null)
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Unable to fatch data.";
                        return RedirectToAction("HotelReport", "Hotel");
                    }
                    else if (respo.StatusCode == HttpStatusCode.OK)
                    {
                        var respoObject = JsonConvert.DeserializeObject<HotelBookingDetailsResponseModel>(respo.Content);
                        if (respoObject.Content != null || respoObject.Content.Addinfo != null)
                        {
                            if (respoObject.Content.Addinfo.Error.ErrorCode == 0)
                            {
                                return View(respoObject);
                            }
                            else
                            {
                                TempData["Status"] = "Failed";
                                TempData["Message"] = Convert.ToString(respoObject.Content.Addinfo.Error.ErrorMessage);
                                return RedirectToAction("HotelReport", "Hotel");
                            }
                        }
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "Unable to fatch data.";
                        return RedirectToAction("HotelReport", "Hotel");
                    }
                    else
                    {
                        TempData["Status"] = "Failed";
                        TempData["Message"] = "StatusCode : " + respo.StatusCode.ToString();
                        return RedirectToAction("HotelReport", "Hotel");
                    }
                }
                else
                {
                    TempData["Status"] = "Failed";
                    TempData["Message"] = "Invalid request.";
                    return RedirectToAction("HotelReport", "Hotel");
                }
            }
            catch (Exception ex)
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = ex.Message;
                return RedirectToAction("HotelReport", "Hotel");
            }
        }
        [HttpPost]
        public ActionResult GenerateVoucher(int Id, string BId)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                if (Id > 0 && !string.IsNullOrWhiteSpace(BId))
                {
                    var token = string.Empty;
                    token = getAuthToken();
                    //token = "v8Vr_d0PbVU9mnzxjludAfL12RYq9moHIiJeWSxVL5fzydzLguHYRCfB-uws7pH79vB5pmqEYQqM3agIh6kx5r4Oby4Dvv5eWWAzbpQtuMLGhrdDl3FZ9ySXd2rlYGz_mn0svSJyFp8-LvHmM_qAIJRZ3tTxr0vstIdC14YLGdFKMiiXeWsIunHt5b6rZ3jqnXTDTFzXpOZFUH_arKpdFKUpR_VXg9Z9S10JCDt72jQp2_jQERaLFQ-SJbidA7VlSGxXRrtnBx-j1d1XpKg3JE_zeop02Q3iQEMmhDYrPkJBfTeEUHoFU7zLALcg2LL_mayd1IPnnCx4--rwD_ssiRyzmoqFKSaUqcAbHBbuG6opAqyBqI8f4dsp8PKNECll-PpO4fUIiOpDUbHNT0sWk1Yf5k2quxbc2MI2YnSFFscydX0B8KCLqSgPdGGjmaMXBcSwB0spXw2iikwLuOWkOmKwE1m0e2g3yr72g2O4srQZN8-6Icf6GAhU85G4LUXqimcnuhnEP0g6qaAyP7zgbg";
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        UpdateAuthToken();
                        var AjaxRespo = new { Status = "Failed", Message = "Failed to connect with provider.please try again." };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    var voucherModeljson = JsonConvert.SerializeObject(new { BookingId = BId });
                    System.Data.Entity.Core.Objects.ObjectParameter IsSuccess = new System.Data.Entity.Core.Objects.ObjectParameter("IsSuccess", typeof(bool));
                    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(Boolean));
                    System.Data.Entity.Core.Objects.ObjectParameter Idno = new System.Data.Entity.Core.Objects.ObjectParameter("IdNo", typeof(int));
                    var dbrespo = db.proc_HotelGenerateVoucherForHoldBooking(userid, BId, voucherModeljson, Id, IsSuccess, Message);

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
                            Details = "Hotel Booking  ",
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
                            Details = "Hotel Booking  ",
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
                            Details = "Hotel Booking  ",
                            RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                            Usertype = "Master"
                        };
                        back.info(model2);
                    }
                    catch { }
                    if (Convert.ToBoolean(IsSuccess.Value) == true)
                    {

                        var respo = GenerateVoucherForHoldBooking(token, voucherModeljson);
                        if (respo == null)
                        {
                            var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                            return Json(JsonConvert.SerializeObject(AjaxRespo));
                        }
                        else if (respo.StatusCode == HttpStatusCode.OK)
                        {
                            dynamic respoObject = JsonConvert.DeserializeObject(respo.Content);
                            if (respoObject.Content != null || respoObject.Content.ADDINFO != null)
                            {
                                if (respoObject.Content.ADDINFO.Error.ErrorCode == 0)
                                {
                                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                                    {
                                        string BookingSts = Convert.ToString(respoObject.Content.ADDINFO.HotelBookingStatus);
                                        string BookingId = Convert.ToString(respoObject.Content.ADDINFO.BookingId);
                                        string BookingRefNo = Convert.ToString(respoObject.Content.ADDINFO.BookingRefNo);
                                        string TraceId = Convert.ToString(respoObject.Content.ADDINFO.TraceId);
                                        string InvoiceNumber = Convert.ToString(respoObject.Content.ADDINFO.InvoiceNumber);
                                        bool VoucherStatus = Convert.ToBoolean(respoObject.Content.ADDINFO.VoucherStatus);
                                        bool IsPriceChanged = Convert.ToBoolean(respoObject.Content.ADDINFO.IsPriceChanged);
                                        bool IsCancellationPolicyChanged = Convert.ToBoolean(respoObject.Content.ADDINFO.IsCancellationPolicyChanged);
                                        var Entry = db.Hotel_info.FirstOrDefault(a => a.BookingId == BId);

                                        if (Entry.IsVoucherBooking == false && Entry.HotelBookingStatus == "Proccessed")
                                        {
                                            db.proc_UpdateHotel_Hold_Booking(Entry.idno.ToString(), userid, respo.Content, 0, BookingSts, InvoiceNumber, "", IsPriceChanged,
                                        true, BookingRefNo, IsCancellationPolicyChanged, BookingId, IsSuccess, Message);
                                            var AjaxRespo1 = new { Status = "Success", Message = "Booking success." };
                                            return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                        }
                                        else if (Entry.HotelBookingStatus == "Proccessed")
                                        {
                                            db.proc_UpdateHotel_Hold_Booking(Entry.idno.ToString(), userid, respo.Content, 1, BookingSts, InvoiceNumber, "", IsPriceChanged,
                                        true, BookingRefNo, IsCancellationPolicyChanged, BookingId, IsSuccess, Message);
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
                                                    Details = "Hotel Booking Refund ",
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
                                                    Details = "Hotel Booking Refund ",
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
                                                    Details = "Hotel Booking Refund ",
                                                    RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                                    Usertype = "Master"
                                                };
                                                back.info(model2);
                                            }
                                            catch { }
                                            var AjaxRespo1 = new { Status = "Failed", Message = "Booking status : " + BookingSts };
                                            return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                        }
                                        else
                                        {
                                            var AjaxRespo1 = new { Status = "Success", Message = "Booking Status : " + BookingSts };
                                            return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                        }
                                    }
                                }
                                else
                                {
                                    var AjaxRespo11 = new { Status = "Failed", Message = Convert.ToString(respoObject.Content.ADDINFO.Error.ErrorMessage) };
                                    return Json(JsonConvert.SerializeObject(AjaxRespo11));
                                }
                            }
                            var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                            return Json(JsonConvert.SerializeObject(AjaxRespo));
                        }
                        else
                        {
                            var AjaxRespo = new { Status = "Failed", Message = "StatusCode : " + respo.StatusCode.ToString() };
                            return Json(JsonConvert.SerializeObject(AjaxRespo));
                        }
                    }
                    else
                    {
                        var AjaxRespo = new { Status = "Failed", Message = Convert.ToString(Message.Value) };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                }
                else
                {
                    var AjaxRespo = new { Status = "Failed", Message = "Invalid Request" };
                    return Json(JsonConvert.SerializeObject(AjaxRespo));
                }
            }
            catch (Exception ex)
            {
                var AjaxRespo = new { Status = "Failed", Message = ex.Message };
                return Json(JsonConvert.SerializeObject(AjaxRespo));
            }
        }
        [HttpPost]
        public ActionResult BookingCancallation(int id, string BookingId)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                if (id > 0)
                {
                    var token = string.Empty;
                    token = getAuthToken();
                    //token = "v8Vr_d0PbVU9mnzxjludAfL12RYq9moHIiJeWSxVL5fzydzLguHYRCfB-uws7pH79vB5pmqEYQqM3agIh6kx5r4Oby4Dvv5eWWAzbpQtuMLGhrdDl3FZ9ySXd2rlYGz_mn0svSJyFp8-LvHmM_qAIJRZ3tTxr0vstIdC14YLGdFKMiiXeWsIunHt5b6rZ3jqnXTDTFzXpOZFUH_arKpdFKUpR_VXg9Z9S10JCDt72jQp2_jQERaLFQ-SJbidA7VlSGxXRrtnBx-j1d1XpKg3JE_zeop02Q3iQEMmhDYrPkJBfTeEUHoFU7zLALcg2LL_mayd1IPnnCx4--rwD_ssiRyzmoqFKSaUqcAbHBbuG6opAqyBqI8f4dsp8PKNECll-PpO4fUIiOpDUbHNT0sWk1Yf5k2quxbc2MI2YnSFFscydX0B8KCLqSgPdGGjmaMXBcSwB0spXw2iikwLuOWkOmKwE1m0e2g3yr72g2O4srQZN8-6Icf6GAhU85G4LUXqimcnuhnEP0g6qaAyP7zgbg";
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        UpdateAuthToken();
                        var AjaxRespo = new { Status = "Failed", Message = "Failed to connect with provider.please try again." };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    using (VastwebmultiEntities db = new VastwebmultiEntities())
                    {
                        var entry = db.HotelCancellations.FirstOrDefault(a => a.Idno == id);
                        if (entry == null)
                        {
                            var CancalltionJsonString = JsonConvert.SerializeObject(new { BookingId = BookingId, BookingMode = 5, RequestType = 4 });
                            var respo = InitializeCancellation(token, CancalltionJsonString);
                            if (respo == null)
                            {
                                var AjaxRespo = new { Status = "Failed", Message = "Failed to get hotel cancellation response" };
                                return Json(JsonConvert.SerializeObject(AjaxRespo));
                            }
                            else if (respo.StatusCode == HttpStatusCode.OK)
                            {
                                dynamic respoObject = JsonConvert.DeserializeObject(respo.Content);
                                if (respoObject.Content != null || respoObject.Content.ADDINFO != null)
                                {
                                    if (respoObject.Content.ADDINFO.Error.ErrorCode == 0)
                                    {
                                        string ChangeRequestStatus = Convert.ToString(respoObject.Content.ADDINFO.ChangeRequestStatus);
                                        string TraceId = Convert.ToString(respoObject.Content.ADDINFO.TraceId);
                                        int ChangeRequestId = Convert.ToInt32(respoObject.Content.ADDINFO.ChangeRequestId);
                                        entry = new HotelCancellation();
                                        entry.Idno = id;
                                        entry.ChangeRequestStatus = ChangeRequestStatus;
                                        entry.TraceId = TraceId;//This Trace Id Callellation Trace Id that is diferent than  Hotel_info TraceId
                                        entry.ChangeRequestId = ChangeRequestId;
                                        entry.RequestDate = DateTime.Now;
                                        entry.RequestType = 4;
                                        entry.Remarks = "";
                                        entry.CancelRequestJson = CancalltionJsonString;
                                        db.HotelCancellations.Add(entry);
                                        db.SaveChanges();
                                        var AjaxRespo1 = new { Status = "Success", Message = "A cancellation request sent,You can track request using STATUS button." };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }
                                    else
                                    {
                                        var AjaxRespo11 = new { Status = "Failed", Message = Convert.ToString(respoObject.Content.ADDINFO.Error.ErrorMessage) };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo11));
                                    }
                                }
                                var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                                return Json(JsonConvert.SerializeObject(AjaxRespo));
                            }
                            else
                            {
                                var AjaxRespo = new { Status = "Failed", Message = "StatusCode : " + respo.StatusCode.ToString() };
                                return Json(JsonConvert.SerializeObject(AjaxRespo));
                            }
                        }
                        else
                        {
                            var AjaxRespo = new { Status = "Failed", Message = "A cancellation request is already sent. please hit status button. " };
                            return Json(JsonConvert.SerializeObject(AjaxRespo));
                        }
                    }
                }
                else
                {
                    var AjaxRespo = new { Status = "Failed", Message = "Invalid Request" };
                    return Json(JsonConvert.SerializeObject(AjaxRespo));
                }
            }
            catch (Exception ex)
            {
                var AjaxRespo = new { Status = "Failed", Message = ex.Message };
                return Json(JsonConvert.SerializeObject(AjaxRespo));
            }
        }
        [HttpGet]
        public ActionResult CancellationQueue()
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var userid = User.Identity.GetUserId();
                    string txt_frm_date = DateTime.Now.ToString();
                    string txt_to_date = DateTime.Now.ToString();
                    //string frm_date = Convert.ToDateTime(txt_frm_date).Date.ToString("yyyy-MM-dd");
                    //string to_date = Convert.ToDateTime(txt_to_date).AddDays(1).ToString("yyyy-MM-dd");
                    var frm_date = Convert.ToDateTime(txt_frm_date).Date;
                    var to_date = Convert.ToDateTime(txt_to_date).AddDays(1);
                    var reportResult = db.proc_HotelCancellationReport(5000000, null, userid, null, null, null, null, null, null, null, frm_date, to_date).ToList();
                    return View(reportResult);
                }
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }
        [HttpPost]
        public ActionResult CancellationQueue(string ddl_status, string ddl_top, string BookingId, string txt_frm_date, string txt_to_date)
        {
            try
            {
                ViewBag.chk = "post";
                var userid = User.Identity.GetUserId();
                var frm_date = Convert.ToDateTime(txt_frm_date).Date;
                var to_date = Convert.ToDateTime(txt_to_date).AddDays(1);
                if (ddl_status == "ALL")
                {
                    ddl_status = null;
                }
                if (string.IsNullOrWhiteSpace(ddl_top))
                {
                    ddl_top = "50";
                }
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var reportResult = db.proc_HotelCancellationReport(5000000, ddl_status, userid, null, null, null, null, null, null, null, frm_date, to_date).ToList();
                    return View(reportResult);
                }
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }
        public ActionResult PDFCancellationQueue(string ddl_status, string ddl_top, string BookingId, string txt_frm_date, string txt_to_date)
        {
            try
            {
                ViewBag.chk = "post";
                var userid = User.Identity.GetUserId();
                var frm_date = Convert.ToDateTime(txt_frm_date).Date;
                var to_date = Convert.ToDateTime(txt_to_date).AddDays(1);
                if (ddl_status == "ALL")
                {
                    ddl_status = null;
                }
                if (string.IsNullOrWhiteSpace(ddl_top))
                {
                    ddl_top = "50";
                }
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var reportResult = db.proc_HotelCancellationReport(5000000, ddl_status, userid, null, null, null, null, null, null, null, frm_date, to_date).ToList();
                    return new ViewAsPdf(reportResult);
                }
            }
            catch
            {
                TempData["Status"] = "Failed";
                TempData["Message"] = "An error occured while proccessing request.";
                return RedirectToAction("Travel", "Home");
            }
        }
        public ActionResult ExcelCancellationQueue(string ddl_status, string ddl_top, string BookingId, string txt_frm_date, string txt_to_date)
        {
            var userid = User.Identity.GetUserId();
            var frm_date = Convert.ToDateTime(txt_frm_date).Date;
            var to_date = Convert.ToDateTime(txt_to_date).AddDays(1);
            if (ddl_status == "ALL")
            {
                ddl_status = null;
            }
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                var reportResult = db.proc_HotelCancellationReport(5000000, ddl_status, userid, null, null, null, null, null, null, null, frm_date, to_date).ToList();
                DataTable dataTbl = new DataTable();
                dataTbl.Columns.Add("Live Status", typeof(string));
                dataTbl.Columns.Add("Cancellation Id", typeof(string));
                dataTbl.Columns.Add("Booking Id", typeof(string));
                dataTbl.Columns.Add("Cancallation Charge", typeof(string));
                dataTbl.Columns.Add("Refund Amount ", typeof(string));
                dataTbl.Columns.Add("Request Date ", typeof(string));
                dataTbl.Columns.Add("Response Date", typeof(string));
                if (reportResult.Any())
                {
                    foreach (var item in reportResult)
                    {
                        dataTbl.Rows.Add(item.ChangeRequestStatus, item.ChangeRequestId, item.BookingId, item.CancellationCharge, item.RefundedAmount, item.CancellationRequestDate, item.CancellationResponseDate);
                    }
                }
                else
                {
                    dataTbl.Rows.Add("", "", "", "", "", "", "");
                }
                var grid = new GridView();
                grid.DataSource = dataTbl;
                grid.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=ExcelCancellationQueue.xls");
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
        [HttpPost]
        public ActionResult CancellationStatus(string ChangeReqId, int idno)
        {
            try
            {
                var userid = User.Identity.GetUserId();
                if (!string.IsNullOrWhiteSpace(ChangeReqId) && idno > 0)
                {
                    var token = string.Empty;
                    token = getAuthToken();
                    //token = "v8Vr_d0PbVU9mnzxjludAfL12RYq9moHIiJeWSxVL5fzydzLguHYRCfB-uws7pH79vB5pmqEYQqM3agIh6kx5r4Oby4Dvv5eWWAzbpQtuMLGhrdDl3FZ9ySXd2rlYGz_mn0svSJyFp8-LvHmM_qAIJRZ3tTxr0vstIdC14YLGdFKMiiXeWsIunHt5b6rZ3jqnXTDTFzXpOZFUH_arKpdFKUpR_VXg9Z9S10JCDt72jQp2_jQERaLFQ-SJbidA7VlSGxXRrtnBx-j1d1XpKg3JE_zeop02Q3iQEMmhDYrPkJBfTeEUHoFU7zLALcg2LL_mayd1IPnnCx4--rwD_ssiRyzmoqFKSaUqcAbHBbuG6opAqyBqI8f4dsp8PKNECll-PpO4fUIiOpDUbHNT0sWk1Yf5k2quxbc2MI2YnSFFscydX0B8KCLqSgPdGGjmaMXBcSwB0spXw2iikwLuOWkOmKwE1m0e2g3yr72g2O4srQZN8-6Icf6GAhU85G4LUXqimcnuhnEP0g6qaAyP7zgbg";
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        UpdateAuthToken();
                        var AjaxRespo = new { Status = "Failed", Message = "Failed to connect with provider.please try again." };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    var changeReqModeljson = JsonConvert.SerializeObject(new { BookingMode = 5, ChangeRequestId = ChangeReqId });
                    var respo = GetCancellationRequestStatus(token, changeReqModeljson);
                    if (respo == null)
                    {
                        var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    else if (respo.StatusCode == HttpStatusCode.OK)
                    {
                        dynamic respoObject = JsonConvert.DeserializeObject(respo.Content);
                        if (respoObject.Content != null || respoObject.Content.ADDINFO != null)
                        {
                            if (respoObject.Content.ADDINFO.Error.ErrorCode == 0)
                            {
                                decimal RefundedAmount = Convert.ToDecimal(respoObject.Content.ADDINFO.RefundedAmount);
                                decimal CancellationCharge = Convert.ToDecimal(respoObject.Content.ADDINFO.CancellationCharge);
                                int ChangeRequestStatus = Convert.ToInt32(respoObject.Content.ADDINFO.ChangeRequestStatus);
                                using (VastwebmultiEntities db = new VastwebmultiEntities())
                                {
                                    System.Data.Entity.Core.Objects.ObjectParameter Message = new System.Data.Entity.Core.Objects.ObjectParameter("Message", typeof(string));
                                    System.Data.Entity.Core.Objects.ObjectParameter Status = new System.Data.Entity.Core.Objects.ObjectParameter("Status", typeof(string));
                                    var dbRespo = db.proc_HotelCancellationRefund(idno, ChangeReqId, RefundedAmount, CancellationCharge, ChangeRequestStatus, respo.Content, Status, Message);
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
                                            Details = "Hotel Booking Refund ",
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
                                            Details = "Hotel Booking Refund ",
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
                                            Details = "Hotel Booking Refund ",
                                            RemainBalance = Convert.ToDecimal(Masterdetails.Remainamount),
                                            Usertype = "Master"
                                        };
                                        back.info(model2);
                                    }
                                    catch { }
                                    if (ChangeRequestStatus == 2 || ChangeRequestStatus == 1)
                                    {
                                        var AjaxRespo1 = new { Status = "Pending", Message = "Cancellation request is in proccess." };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }
                                    else if (ChangeRequestStatus == 3)
                                    {
                                        var AjaxRespo1 = new { Status = "Success", Message = "Cancellation request accepted." };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }
                                    else
                                    {
                                        var AjaxRespo1 = new { Status = "Failed", Message = "Cancellation request rejected." };
                                        return Json(JsonConvert.SerializeObject(AjaxRespo1));
                                    }

                                }
                            }
                            else
                            {
                                var AjaxRespo1 = new { Status = "Failed", Message = Convert.ToString(respoObject.Content.ADDINFO.Error.ErrorMessage) };
                                return Json(JsonConvert.SerializeObject(AjaxRespo1));
                            }
                        }
                        var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                    else
                    {
                        var AjaxRespo = new { Status = "Failed", Message = "Unable to fatch Data" };
                        return Json(JsonConvert.SerializeObject(AjaxRespo));
                    }
                }
                else
                {
                    var AjaxRespo = new { Status = "Failed", Message = "Invalid Request" };
                    return Json(JsonConvert.SerializeObject(AjaxRespo));
                }
            }
            catch (Exception ex)
            {
                var AjaxRespo = new { Status = "Failed", Message = ex.Message };
                return Json(JsonConvert.SerializeObject(AjaxRespo));
            }
        }
        public IRestResponse GenerateVoucherForHoldBooking(string vastwebToken, string RequestString)
        {
            #region HotelGenerateVoucher
            var url = VastbazaarBaseUrl + "api/Hotel/GenerateVoucher";
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + vastwebToken);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept-encoding", "gzip");
            request.AddParameter("application/json", RequestString, ParameterType.RequestBody);
            client.ReadWriteTimeout = 180000;
            var respo = client.Execute(request);
            return respo;
            #endregion
        }
        public IRestResponse GetBookingDetails(string vastwebToken, string BookingId, string ConfirmationNo, string TraceId)
        {
            #region HotelGetBookingDetails
            var GetBookinDetailsModel = new { BookingId = BookingId, ConfirmationNo = ConfirmationNo, TraceId = TraceId };

            var url = VastbazaarBaseUrl + "api/Hotel/GetBookingDetails";
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + vastwebToken);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept-encoding", "gzip");
            request.AddParameter("application/json", JsonConvert.SerializeObject(GetBookinDetailsModel), ParameterType.RequestBody);
            client.ReadWriteTimeout = 180000;
            var respo = client.Execute(request);
            return respo;
            #endregion
        }
        public IRestResponse InitializeCancellation(string vastwebToken, string changeRequestString)
        {
            #region HotelGetBookingDetails

            var url = VastbazaarBaseUrl + "api/Hotel/ChangeRequest";
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + vastwebToken);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept-encoding", "gzip");
            request.AddParameter("application/json", changeRequestString, ParameterType.RequestBody);
            client.ReadWriteTimeout = 180000;
            var respo = client.Execute(request);
            return respo;
            #endregion
        }
        public IRestResponse GetCancellationRequestStatus(string vastwebToken, string changeRequestString)
        {
            #region HotelGetBookingDetails

            var url = VastbazaarBaseUrl + "api/Hotel/ChangeRequestStatus";
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "bearer " + vastwebToken);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept-encoding", "gzip");
            request.AddParameter("application/json", changeRequestString, ParameterType.RequestBody);
            client.ReadWriteTimeout = 180000;
            var respo = client.Execute(request);
            return respo;
            #endregion
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
    }
}
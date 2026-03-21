using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Vastwebmulti.Models;

namespace Vastwebmulti.Controllers
{

    public class RetailerAPIController : ApiController
    {

        [HttpPost]
        [Route("Website/Admin/DeleteAdminAuthoriseLIPADDRESS")]

        public IHttpActionResult DeleteAllIP()
        {
            JObject ressss = new JObject();
            VastwebmultiEntities db = new VastwebmultiEntities();

            var result = db.ipadreesvalidates.ToList();
            foreach (var item in result)
            {

                db.ipadreesvalidates.Remove(item);
            }
            var data = db.SaveChanges() > 0;
            if (data)
            {
                ressss.Add("status", "Success");
                return Content(HttpStatusCode.OK, ressss);
            }
            else
            {
                ressss.Add("status", "Failed");
                return Content(HttpStatusCode.BadRequest, ressss);
            }

        }

        [HttpPost]
        [Route("api/user/UploadUserImages")]
        public HttpResponseMessage UploadUserImages(Imagephoto model)
        {
            VastwebmultiEntities db = new VastwebmultiEntities();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var message1 = "";
            try
            {
                string filePath = "";
                string base64String = model.ProfileImagess.ToString();
                byte[] imageBytes = Convert.FromBase64String(base64String);
                var newFileName = "";
                newFileName = Guid.NewGuid().ToString() + ".jpg";
                filePath = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileName);
                var imagepath = "/Retailer_image/" + newFileName;
                if (model.currentrole == "Retailer")
                {
                    var ad = db.Retailer_Details.Single(a => a.RetailerId == model.txtretailerid);

                    using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                    {

                        Image image = Image.FromStream(ms, true);

                        image.Save(filePath);




                    }

                    ad.Photo = string.IsNullOrWhiteSpace(imagepath) ? ad.Photo : imagepath;
                    db.SaveChanges();
                    // postedFile.SaveAs(filePath);
                    message1 = string.Format("Image Updated Successfully.");
                }
                else if (model.currentrole == "Dealer")
                {
                    var ad = db.Dealer_Details.Single(a => a.DealerId == model.txtretailerid);

                    using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                    {

                        Image image = Image.FromStream(ms, true);

                        image.Save(filePath);




                    }
                    ad.Photo = string.IsNullOrWhiteSpace(imagepath) ? ad.Photo : imagepath;
                    db.SaveChanges();

                    message1 = string.Format("Image Updated Successfully.");
                }
                else if (model.currentrole == "Master")
                {
                    var ad = db.Superstokist_details.Single(a => a.SSId == model.txtretailerid);
                    using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                    {

                        Image image = Image.FromStream(ms, true);

                        image.Save(filePath);




                    }
                    ad.Photo = string.IsNullOrWhiteSpace(imagepath) ? ad.Photo : imagepath;
                    db.SaveChanges();

                    message1 = string.Format("Image Updated Successfully.");
                }
                else if (model.currentrole == "Whitelabelretailer")
                {
                    var ad = db.Whitelabel_Retailer_Details.Single(a => a.RetailerId == model.txtretailerid);

                    using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                    {
                        Image image = Image.FromStream(ms, true);
                        image.Save(filePath);
                    }

                    ad.photo = string.IsNullOrWhiteSpace(imagepath) ? ad.photo : imagepath;
                    db.SaveChanges();

                    message1 = string.Format("Image Updated Successfully.");
                }
                else if (model.currentrole == "Whitelabeldealer")
                {
                    var ad = db.whitelabel_Dealer_Details.Single(a => a.DealerId == model.txtretailerid);

                    using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                    {
                        Image image = Image.FromStream(ms, true);
                        image.Save(filePath);
                    }

                    ad.Photo = string.IsNullOrWhiteSpace(imagepath) ? ad.Photo : imagepath;
                    db.SaveChanges();

                    message1 = string.Format("Image Updated Successfully.");
                }
                else if (model.currentrole == "Whitelabelmaster")
                {
                    var ad = db.Whitelabel_Superstokist_details.Single(a => a.SSId == model.txtretailerid);

                    using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                    {
                        Image image = Image.FromStream(ms, true);
                        image.Save(filePath);
                    }

                    ad.Photo = string.IsNullOrWhiteSpace(imagepath) ? ad.Photo : imagepath;
                    db.SaveChanges();

                    message1 = string.Format("Image Updated Successfully.");
                }
                return Request.CreateResponse(HttpStatusCode.OK, message1);
            }
            catch (Exception ex)
            {
                dict.Add("error", ex.Message);
                dict.Add("Stack", ex.StackTrace);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, dict);
            }
        }

        [HttpPost]
        [Route("api/user/Uploadcancelledcheque")]
        public HttpResponseMessage Uploadcancelledcheque(Imagephoto model)
        {
            VastwebmultiEntities db = new VastwebmultiEntities();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var message1 = "";
            try
            {

                string filePath = "";
                string base64String = model.cancelledcheque.ToString();
                byte[] imageBytes = Convert.FromBase64String(base64String);
                var newFileName = "";
                newFileName = Guid.NewGuid().ToString() + ".jpg";
                filePath = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileName);
                var imagepath = "/Retailer_image/" + newFileName;
                if (model.currentrole == "Retailer")
                {
                    var ad = db.UpdateREMAccounts.Where(a => a.Idno == model.cancellchecque_idno).FirstOrDefault();

                    using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                    {

                        Image image = Image.FromStream(ms, true);

                        image.Save(filePath);
                    }

                    ad.CancelCheckFile = string.IsNullOrWhiteSpace(imagepath) ? ad.CancelCheckFile : imagepath;
                    db.SaveChanges();
                    // postedFile.SaveAs(filePath);
                    message1 = string.Format("Image Updated Successfully.");
                    // dict.Add("status", "Success");
                    // dict.Add("status", "Image Updated Successfully.");
                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1);
                }
                return Request.CreateErrorResponse(HttpStatusCode.Created, message1);

            }
            catch (Exception ex)
            {
                var res = string.Format("Something Went Wrong");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }


        [HttpPost]
        [Route("api/user/MoneyTransfer_Image_Upload")]
        public HttpResponseMessage IMPSTXN_Image_Upload(Imagephoto model)
        {

            VastwebmultiEntities db = new VastwebmultiEntities();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var message1 = "";
            try
            {

                string filePath = "";
                string base64String = model.Imps_photo_images.ToString();
                byte[] imageBytes = Convert.FromBase64String(base64String);
                var newFileName = "";
                newFileName = Guid.NewGuid().ToString() + ".jpg";
                filePath = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileName);
                var imagepath = "/Retailer_image/" + newFileName;
                if (model.currentrole == "Retailer")
                {
                    var ad = db.IMPS_transtion_detsils.Where(a => a.trans_id == model.txn_ids && a.rch_from == model.txtretailerid).FirstOrDefault();
                    if (ad != null)
                    {
                        using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                        {

                            Image image = Image.FromStream(ms, true);

                            image.Save(filePath);
                        }

                        ad.PhotoCaptured = imagepath;
                        db.SaveChanges();
                        // postedFile.SaveAs(filePath);
                        message1 = string.Format("Image Updated Successfully.");
                        // dict.Add("status", "Success");
                        // dict.Add("status", "Image Updated Successfully.");
                        return Request.CreateErrorResponse(HttpStatusCode.OK, message1);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.OK, "Not Found Data");
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.OK, message1);

            }
            catch (Exception ex)
            {
                var res = string.Format("");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }



        [HttpPost]
        [Route("api/user/UploadDocumentsImages")]
        public HttpResponseMessage UploadAadharcarddoc(Imagephoto model)
        {
            var message1 = "";
            try
            {
                if ((!string.IsNullOrEmpty(model.AadharcardFront) && !string.IsNullOrEmpty(model.AadharcardBack)))
                {
                    VastwebmultiEntities db = new VastwebmultiEntities();
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    string filePath = "";
                    string filePathback = "";
                    string base64frontString = model.AadharcardFront.ToString();
                    byte[] imageBytesfrontimg = Convert.FromBase64String(base64frontString);
                    string base64BackString = model.AadharcardBack.ToString();
                    byte[] imageBytesBackimg = Convert.FromBase64String(base64BackString);
                    var newFileName = "";
                    var newFileNameback = "";

                    newFileName = Guid.NewGuid().ToString() + ".jpg";
                    newFileNameback = Guid.NewGuid().ToString() + ".jpg";
                    filePath = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileName);
                    filePathback = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileNameback);
                    var imagepath = "/Retailer_image/" + newFileName;
                    var imagepathback = "/Retailer_image/" + newFileNameback;
                    if (model.currentrole == "Retailer")
                    {
                        var ad = db.Retailer_Details.Single(a => a.RetailerId == model.txtretailerid);

                        using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                        {

                            Image image = Image.FromStream(ms, true);

                            image.Save(filePath);




                        }
                        using (var ms1 = new MemoryStream(imageBytesBackimg, 0, imageBytesBackimg.Length))
                        {

                            Image image = Image.FromStream(ms1, true);

                            image.Save(filePathback);




                        }
                        ad.aadharcardPath = string.IsNullOrWhiteSpace(imagepath) ? ad.aadharcardPath : imagepath;

                        ad.BackSideAadharcardphoto = string.IsNullOrWhiteSpace(imagepathback) ? ad.BackSideAadharcardphoto : imagepathback;
                        db.SaveChanges();



                        message1 = string.Format("Image Updated Successfully.");
                    }
                    if (model.currentrole == "Dealer")
                    {
                        var ad = db.Dealer_Details.Single(a => a.DealerId == model.txtretailerid);
                        using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                        {

                            Image image = Image.FromStream(ms, true);

                            image.Save(filePath);




                        }
                        using (var ms1 = new MemoryStream(imageBytesBackimg, 0, imageBytesBackimg.Length))
                        {

                            Image image = Image.FromStream(ms1, true);

                            image.Save(filePathback);




                        }
                        ad.aadharcardPath = string.IsNullOrWhiteSpace(imagepath) ? ad.aadharcardPath : imagepath;

                        ad.BackSideAadharcardphoto = string.IsNullOrWhiteSpace(imagepathback) ? ad.BackSideAadharcardphoto : imagepathback;
                        db.SaveChanges();
                        message1 = string.Format("Image Updated Successfully.");
                    }
                    if (model.currentrole == "Master")
                    {
                        var ad = db.Superstokist_details.Single(a => a.SSId == model.txtretailerid);
                        using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                        {

                            Image image = Image.FromStream(ms, true);

                            image.Save(filePath);




                        }
                        using (var ms1 = new MemoryStream(imageBytesBackimg, 0, imageBytesBackimg.Length))
                        {

                            Image image = Image.FromStream(ms1, true);

                            image.Save(filePathback);




                        }
                        ad.aadharcardPath = string.IsNullOrWhiteSpace(imagepath) ? ad.aadharcardPath : imagepath;

                        ad.BackSideAadharcardphoto = string.IsNullOrWhiteSpace(imagepathback) ? ad.BackSideAadharcardphoto : imagepathback;
                        db.SaveChanges();
                        message1 = string.Format("Image Updated Successfully.");
                    }
                    if (model.currentrole == "Whitelabel")
                    {
                        var wh = db.WhiteLabel_userList.Where(w => w.WhiteLabelID == model.txtretailerid).SingleOrDefault();
                        var pathaadhar = HttpContext.Current.Server.MapPath("~/" + wh.aadharcardPath);
                        if (!System.IO.File.Exists(pathaadhar))
                        {
                            var backpath = HttpContext.Current.Server.MapPath("~/" + wh.BackSideAadharcardphoto);
                            using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                            {
                                Image image = Image.FromStream(ms, true);
                                image.Save(pathaadhar);
                            }
                            using (var ms1 = new MemoryStream(imageBytesBackimg, 0, imageBytesBackimg.Length))
                            {
                                Image image = Image.FromStream(ms1, true);
                                image.Save(backpath);
                            }
                        }
                        message1 = string.Format("Image Updated Successfully.");
                    }
                    if (model.currentrole == "Whitelabelretailer")
                    {
                        var wh = db.Whitelabel_Retailer_Details.Where(w => w.RetailerId == model.txtretailerid).Single();
                        //var pathaadhar = HttpContext.Current.Server.MapPath("~/" + model.AadharcardFront);
                        var pathaadhar = filePath;
                        if (!System.IO.File.Exists(pathaadhar))
                        {
                            //var backpath = HttpContext.Current.Server.MapPath("~/" + model.AadharcardBack);
                            var backpath = filePathback;
                            using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                            {
                                Image image = Image.FromStream(ms, true);
                                image.Save(pathaadhar);
                            }
                            using (var ms1 = new MemoryStream(imageBytesBackimg, 0, imageBytesBackimg.Length))
                            {
                                Image image = Image.FromStream(ms1, true);
                                image.Save(backpath);
                            }

                            wh.aadharcardPath = imagepath;
                            wh.BackSideAadharcardphoto = imagepathback;
                            db.SaveChanges();
                        }
                        message1 = string.Format("Image Updated Successfully.");
                    }
                    if (model.currentrole == "Whitelabeldealer")
                    {
                        var wh = db.whitelabel_Dealer_Details.Where(w => w.DealerId == model.txtretailerid).Single();
                        //var pathaadhar = HttpContext.Current.Server.MapPath("~/" + model.AadharcardFront);
                        var pathaadhar = filePath;
                        if (!System.IO.File.Exists(pathaadhar))
                        {
                            //var backpath = HttpContext.Current.Server.MapPath("~/" + model.AadharcardBack);
                            var backpath = filePathback;
                            using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                            {
                                Image image = Image.FromStream(ms, true);
                                image.Save(pathaadhar);
                            }
                            using (var ms1 = new MemoryStream(imageBytesBackimg, 0, imageBytesBackimg.Length))
                            {
                                Image image = Image.FromStream(ms1, true);
                                image.Save(backpath);
                            }

                            wh.aadharcardPath = imagepath;
                            wh.BackSideAadharcardphoto = imagepathback;
                            db.SaveChanges();
                        }
                        message1 = string.Format("Image Updated Successfully.");
                    }
                    if (model.currentrole == "Whitelabelmaster")
                    {
                        var wh = db.Whitelabel_Superstokist_details.Where(w => w.SSId == model.txtretailerid).Single();
                        var pathaadhar = filePath;
                        if (!System.IO.File.Exists(pathaadhar))
                        {
                            var backpath = filePathback;
                            using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                            {
                                Image image = Image.FromStream(ms, true);
                                image.Save(pathaadhar);
                            }
                            using (var ms1 = new MemoryStream(imageBytesBackimg, 0, imageBytesBackimg.Length))
                            {
                                Image image = Image.FromStream(ms1, true);
                                image.Save(backpath);
                            }

                            wh.aadharcardPath = imagepath;
                            wh.BackSideAadharcardphoto = imagepathback;
                            db.SaveChanges();
                        }
                        message1 = string.Format("Image Updated Successfully.");
                    }
                }
                else if (!string.IsNullOrEmpty(model.PancardFront))
                {
                    VastwebmultiEntities db = new VastwebmultiEntities();
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    string filePath = "";

                    string base64frontString = model.PancardFront.ToString();
                    byte[] imageBytesfrontimg = Convert.FromBase64String(base64frontString);


                    var newFileName = "";

                    newFileName = Guid.NewGuid().ToString() + ".jpg";

                    filePath = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileName);

                    var imagepath = "/Retailer_image/" + newFileName;

                    if (model.currentrole == "Retailer")
                    {
                        var ad = db.Retailer_Details.Single(a => a.RetailerId == model.txtretailerid);

                        using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                        {

                            Image image = Image.FromStream(ms, true);

                            image.Save(filePath);
                        }

                        ad.pancardPath = string.IsNullOrWhiteSpace(imagepath) ? ad.pancardPath : imagepath;

                        db.SaveChanges();

                        message1 = string.Format("Image Updated Successfully.");
                    }
                    if (model.currentrole == "Dealer")
                    {
                        var ad = db.Dealer_Details.Single(a => a.DealerId == model.txtretailerid);
                        using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                        {

                            Image image = Image.FromStream(ms, true);

                            image.Save(filePath);




                        }

                        ad.pancardPath = string.IsNullOrWhiteSpace(imagepath) ? ad.pancardPath : imagepath;
                        db.SaveChanges();
                        message1 = string.Format("Image Updated Successfully.");
                    }
                    if (model.currentrole == "Master")
                    {
                        var ad = db.Superstokist_details.Single(a => a.SSId == model.txtretailerid);
                        using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                        {

                            Image image = Image.FromStream(ms, true);

                            image.Save(filePath);




                        }

                        ad.pancardPath = string.IsNullOrWhiteSpace(imagepath) ? ad.pancardPath : imagepath;

                        db.SaveChanges();
                        message1 = string.Format("Image Updated Successfully.");
                    }
                    if (model.currentrole == "Whitelabel")
                    {
                        var wh = db.WhiteLabel_userList.Where(w => w.WhiteLabelID == model.txtretailerid).SingleOrDefault();
                        var pathpancard = HttpContext.Current.Server.MapPath("~/" + wh.pancardPath);
                        if (!System.IO.File.Exists(pathpancard))
                        {
                            using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                            {
                                Image image = Image.FromStream(ms, true);

                                image.Save(pathpancard);
                            }

                        }
                        message1 = string.Format("Image Updated Successfully.");

                    }
                    if (model.currentrole == "Whitelabelretailer")
                    {
                        var wh = db.Whitelabel_Retailer_Details.Where(w => w.RetailerId == model.txtretailerid).Single();
                        var pathpancard = filePath;
                        if (!System.IO.File.Exists(pathpancard))
                        {
                            using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                            {
                                Image image = Image.FromStream(ms, true);

                                image.Save(pathpancard);
                            }
                            wh.pancardPath = imagepath;
                            db.SaveChanges();

                        }
                        message1 = string.Format("Image Updated Successfully.");

                    }
                    if (model.currentrole == "Whitelabeldealer")
                    {
                        var wh = db.whitelabel_Dealer_Details.Where(w => w.DealerId == model.txtretailerid).Single();
                        var pathpancard = filePath;
                        if (!System.IO.File.Exists(pathpancard))
                        {
                            using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                            {
                                Image image = Image.FromStream(ms, true);

                                image.Save(pathpancard);
                            }
                            wh.pancardPath = imagepath;
                            db.SaveChanges();

                        }
                        message1 = string.Format("Image Updated Successfully.");

                    }
                    if (model.currentrole == "Whitelabelmaster")
                    {
                        var wh = db.Whitelabel_Superstokist_details.Where(w => w.SSId == model.txtretailerid).Single();
                        var pathpancard = filePath;
                        if (!System.IO.File.Exists(pathpancard))
                        {
                            using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                            {
                                Image image = Image.FromStream(ms, true);

                                image.Save(pathpancard);
                            }
                            wh.pancardPath = imagepath;
                            db.SaveChanges();

                        }
                        message1 = string.Format("Image Updated Successfully.");

                    }
                }

                else if (!string.IsNullOrEmpty(model.ShopeWithSelfie))
                {
                    VastwebmultiEntities db = new VastwebmultiEntities();
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    string filePath = "";

                    string base64frontString = model.ShopeWithSelfie.ToString();
                    byte[] imageBytesfrontimg = Convert.FromBase64String(base64frontString);


                    var newFileName = "";

                    newFileName = Guid.NewGuid().ToString() + ".jpg";

                    filePath = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileName);

                    var imagepath = "/Retailer_image/" + newFileName;

                    if (model.currentrole == "Retailer")
                    {
                        var ad = db.Retailer_Details.Single(a => a.RetailerId == model.txtretailerid);

                        using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                        {

                            Image image = Image.FromStream(ms, true);

                            image.Save(filePath);




                        }

                        ad.ShopwithSalfie = string.IsNullOrWhiteSpace(imagepath) ? ad.ShopwithSalfie : imagepath;

                        db.SaveChanges();



                        message1 = string.Format("Image Updated Successfully.");
                    }

                }
                else if (!string.IsNullOrEmpty(model.Serviceaggreementpath))
                {
                    VastwebmultiEntities db = new VastwebmultiEntities();
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    string filePath = "";

                    string base64frontString = model.Serviceaggreementpath.ToString();
                    byte[] imageBytesfrontimg = Convert.FromBase64String(base64frontString);


                    var newFileName = "";

                    newFileName = Guid.NewGuid().ToString() + ".jpg";

                    filePath = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileName);

                    var imagepath = "/Retailer_image/" + newFileName;

                    if (model.currentrole == "Retailer")
                    {
                        var ad = db.Retailer_Details.Single(a => a.RetailerId == model.txtretailerid);

                        using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                        {

                            Image image = Image.FromStream(ms, true);

                            image.Save(filePath);


                        }

                        ad.serviceagreementpath = string.IsNullOrWhiteSpace(imagepath) ? ad.serviceagreementpath : imagepath;

                        db.SaveChanges();



                        message1 = string.Format("Image Updated Successfully.");
                    }

                }
                else if (!string.IsNullOrEmpty(model.Registrationcertificatepath))
                {
                    VastwebmultiEntities db = new VastwebmultiEntities();
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    string filePath = "";

                    string base64frontString = model.Registrationcertificatepath.ToString();
                    byte[] imageBytesfrontimg = Convert.FromBase64String(base64frontString);


                    var newFileName = "";

                    newFileName = Guid.NewGuid().ToString() + ".jpg";

                    filePath = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileName);

                    var imagepath = "/Retailer_image/" + newFileName;

                    if (model.currentrole == "Retailer")
                    {
                        var ad = db.Retailer_Details.Single(a => a.RetailerId == model.txtretailerid);

                        using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                        {

                            Image image = Image.FromStream(ms, true);

                            image.Save(filePath);


                        }

                        ad.Registractioncertificatepath = string.IsNullOrWhiteSpace(imagepath) ? ad.Registractioncertificatepath : imagepath;

                        db.SaveChanges();



                        message1 = string.Format("Image Updated Successfully.");

                    }

                }
                else
                {
                    message1 = string.Format("Uploading Failed.");
                }
            }
            catch (Exception ex)
            {
                message1 = string.Format("Error While Uploadin Please Try again Later");
                return Request.CreateErrorResponse(HttpStatusCode.Created, message1);
            }
            return Request.CreateResponse(HttpStatusCode.Created, message1);
        }

        [HttpPost]
        [Route("api/user/UploadRetailerDocumentsByDealer")]
        public HttpResponseMessage UploadRetailerDocumentsByDealer(Imagephoto model)
        {
            var message1 = "";
            try
            {
                if ((!string.IsNullOrEmpty(model.AadharcardFront) && !string.IsNullOrEmpty(model.AadharcardBack)))
                {
                    VastwebmultiEntities db = new VastwebmultiEntities();
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    string filePath = "";
                    string filePathback = "";
                    string base64frontString = model.AadharcardFront.ToString();
                    byte[] imageBytesfrontimg = Convert.FromBase64String(base64frontString);
                    string base64BackString = model.AadharcardBack.ToString();
                    byte[] imageBytesBackimg = Convert.FromBase64String(base64BackString);
                    var newFileName = "";
                    var newFileNameback = "";

                    newFileName = Guid.NewGuid().ToString() + ".jpg";
                    newFileNameback = Guid.NewGuid().ToString() + ".jpg";
                    filePath = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileName);
                    filePathback = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileNameback);
                    var imagepath = "/Retailer_image/" + newFileName;
                    var imagepathback = "/Retailer_image/" + newFileNameback;

                    var ad = db.Retailer_Details.Single(a => a.RetailerId == model.txtretailerid);

                    using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                    {
                        Image image = Image.FromStream(ms, true);
                        image.Save(filePath);
                    }
                    using (var ms1 = new MemoryStream(imageBytesBackimg, 0, imageBytesBackimg.Length))
                    {
                        Image image = Image.FromStream(ms1, true);
                        image.Save(filePathback);
                    }
                    ad.aadharcardPath = string.IsNullOrWhiteSpace(imagepath) ? ad.aadharcardPath : imagepath;
                    ad.BackSideAadharcardphoto = string.IsNullOrWhiteSpace(imagepathback) ? ad.BackSideAadharcardphoto : imagepathback;
                    db.SaveChanges();

                    message1 = string.Format("Image Updated Successfully.");

                }
                else if (!string.IsNullOrEmpty(model.PancardFront))
                {
                    VastwebmultiEntities db = new VastwebmultiEntities();
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    string filePath = "";

                    string base64frontString = model.PancardFront.ToString();
                    byte[] imageBytesfrontimg = Convert.FromBase64String(base64frontString);


                    var newFileName = "";

                    newFileName = Guid.NewGuid().ToString() + ".jpg";

                    filePath = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileName);

                    var imagepath = "/Retailer_image/" + newFileName;

                    var ad = db.Retailer_Details.Single(a => a.RetailerId == model.txtretailerid);

                    using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                    {

                        Image image = Image.FromStream(ms, true);

                        image.Save(filePath);
                    }

                    ad.pancardPath = string.IsNullOrWhiteSpace(imagepath) ? ad.pancardPath : imagepath;

                    db.SaveChanges();

                    message1 = string.Format("Image Updated Successfully.");
                }
                else if (!string.IsNullOrEmpty(model.ShopeWithSelfie))
                {
                    VastwebmultiEntities db = new VastwebmultiEntities();
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    string filePath = "";

                    string base64frontString = model.ShopeWithSelfie.ToString();
                    byte[] imageBytesfrontimg = Convert.FromBase64String(base64frontString);


                    var newFileName = "";

                    newFileName = Guid.NewGuid().ToString() + ".jpg";

                    filePath = HttpContext.Current.Server.MapPath("~/Retailer_image/" + newFileName);

                    var imagepath = "/Retailer_image/" + newFileName;

                    var ad = db.Retailer_Details.Single(a => a.RetailerId == model.txtretailerid);

                    using (var ms = new MemoryStream(imageBytesfrontimg, 0, imageBytesfrontimg.Length))
                    {

                        Image image = Image.FromStream(ms, true);

                        image.Save(filePath);
                    }

                    ad.ShopwithSalfie = string.IsNullOrWhiteSpace(imagepath) ? ad.ShopwithSalfie : imagepath;
                    db.SaveChanges();
                    message1 = string.Format("Image Updated Successfully.");
                }
            }
            catch (Exception ex)
            {
                message1 = string.Format("Error While Uploadin Please Try again Later");
                return Request.CreateErrorResponse(HttpStatusCode.Created, message1);
            }
            return Request.CreateResponse(HttpStatusCode.Created, message1);

        }
        [HttpGet]
        [Route("api/user/CompanyPolicyData")]
        public IHttpActionResult Helpterm()
        {
            JObject jo = new JObject();
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                List<Vastwebmulti.JsonData.Hlp_TC_Privacy> people = new List<Vastwebmulti.JsonData.Hlp_TC_Privacy>();
                JSONReadWrite readWrite = new JSONReadWrite();
                people = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Vastwebmulti.JsonData.Hlp_TC_Privacy>>(readWrite.Read("PrivacyAdmin.json", "~/Areas/ADMIN/Models/JsonData/"));
                var Privacy = people.ToList();

                people = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Vastwebmulti.JsonData.Hlp_TC_Privacy>>(readWrite.Read("OutHelpForAdmin.json", "~/Areas/ADMIN/Models/JsonData/"));
                var Help = people.ToList();

                people = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Vastwebmulti.JsonData.Hlp_TC_Privacy>>(readWrite.Read("TnCAdmin.json", "~/Areas/ADMIN/Models/JsonData/"));
                var TC = people.ToList();

                var HelpElement = Help.SingleOrDefault().Name;
                var TcElement = TC.SingleOrDefault().Name;
                var PrivacyElement = Privacy.SingleOrDefault().Name;
                string host = HttpContext.Current.Request.Url.Host;

                string companyname = "";
                string url = "";
                string SupportMail = "";

                var admininfo = db.Admin_details.SingleOrDefault();
                var whitelabeladmininfo = db.WhiteLabel_userList.Where(a => a.websitename.Contains(host)).FirstOrDefault();
                if (admininfo != null)
                {
                    url = admininfo.WebsiteUrl;
                    SupportMail = admininfo.email;
                    if (admininfo.Companyname != null)
                    {
                        companyname = admininfo.Companyname;
                    }

                    if (admininfo.email1 != null)
                    {
                        SupportMail = admininfo.email1;
                    }
                }

                else if (whitelabeladmininfo != null)
                {
                    url = whitelabeladmininfo.websitename;
                    SupportMail = whitelabeladmininfo.EmailId;
                    if (whitelabeladmininfo.FrmName != null)
                    {
                        companyname = whitelabeladmininfo.FrmName;
                    }

                    if (whitelabeladmininfo.Support_Email != null)
                    {
                        SupportMail = whitelabeladmininfo.Support_Email;
                    }
                }

                jo.Add("HelpElement", HelpElement);
                jo.Add("TcElement", TcElement);
                jo.Add("PrivacyElement", PrivacyElement);
                jo.Add("companyname", companyname);
                jo.Add("SupportMail", SupportMail);
                jo.Add("url", url);
                return Json(jo);

            }
        }

        [HttpGet]
        [Route("api/user/WhiteCompanyPolicyData")]
        public IHttpActionResult White_Helpterm()
        {
            JObject jo = new JObject();
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                List<Vastwebmulti.JsonData.Hlp_TC_Privacy> people = new List<Vastwebmulti.JsonData.Hlp_TC_Privacy>();
                JSONReadWrite readWrite = new JSONReadWrite();
                people = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Vastwebmulti.JsonData.Hlp_TC_Privacy>>(readWrite.Read("PrivacyAdmin.json", "~/Areas/ADMIN/Models/JsonData/"));
                var Privacy = people.ToList();

                people = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Vastwebmulti.JsonData.Hlp_TC_Privacy>>(readWrite.Read("OutHelpForAdmin.json", "~/Areas/ADMIN/Models/JsonData/"));
                var Help = people.ToList();

                people = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Vastwebmulti.JsonData.Hlp_TC_Privacy>>(readWrite.Read("TnCAdmin.json", "~/Areas/ADMIN/Models/JsonData/"));
                var TC = people.ToList();

                var HelpElement = Help.SingleOrDefault().Name;
                var TcElement = TC.SingleOrDefault().Name;
                var PrivacyElement = Privacy.SingleOrDefault().Name;

                var admininfo = db.WhiteLabel_userList.SingleOrDefault();
                string companyname = "";
                string url = admininfo.websitename;
                string SupportMail = admininfo.EmailId;
                if (admininfo.FrmName != null)
                {
                    companyname = admininfo.FrmName;
                }

                if (admininfo.Support_Email != null)
                {
                    SupportMail = admininfo.Support_Email;
                }
                jo.Add("HelpElement", HelpElement);
                jo.Add("TcElement", TcElement);
                jo.Add("PrivacyElement", PrivacyElement);
                jo.Add("companyname", companyname);
                jo.Add("SupportMail", SupportMail);
                jo.Add("url", url);
                return Json(jo);

            }
        }

        [HttpPost]
        [Route("api/user/UploadKYCVIDEO")]
        public async Task<IHttpActionResult> UploadUserKYCVideo(Kycvideoesclass model)
        {
            VastwebmultiEntities db = new VastwebmultiEntities();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                VastwebmultiEntities dbs = new VastwebmultiEntities();
                // string result = await Request.Content.ReadAsStringAsync();
                // dynamic data = JsonConvert.DeserializeObject(result);
                string kycvideo = model.kycvideo;// data.kycvideo;
                string userids = model.userids;//data.userids;
                string role = model.role; //data.role;

                string base64frontString = kycvideo.ToString();
                byte[] imageBytesfrontimg = Convert.FromBase64String(base64frontString);
                // String file = Convert.ToBase64String(file);
                var newFileName = Guid.NewGuid().ToString() + ".mp4";
                var imagePath = @"\Retailer_Video\" + newFileName;
                var filePath = HttpContext.Current.Server.MapPath("~/" + imagePath);
                // FileInfo fil = new FileInfo("D://test.mp4");
                //  file.SaveAs(filePath);
                File.WriteAllBytes(filePath, imageBytesfrontimg);
                if (role == "Retailer")
                {
                    var retailersdetails = db.Retailer_Details.Where(x => x.RetailerId == userids).SingleOrDefault();
                    if (retailersdetails != null)
                    {
                        retailersdetails.videokycpath = imagePath;
                        retailersdetails.videokycstatus = "N";
                        db.SaveChanges();
                    }
                }
                JObject response = new JObject();
                response.Add("status", "Success");
                response.Add("msg", imagePath);
                return Json(response);


            }
            catch (Exception ex)
            {
                var res = string.Format("Something Went Wrong");
                dict.Add("error", res);
                return Json(dict);

                //   return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }

        [HttpPost]
        [Route("api/user/Videolink")]
        public async Task<IHttpActionResult> Videolink(AepsVideoUpload model)
        {
            VastwebmultiEntities db = new VastwebmultiEntities();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                VastwebmultiEntities dbs = new VastwebmultiEntities();
                // string result = await Request.Content.ReadAsStringAsync();
                // dynamic data = JsonConvert.DeserializeObject(result);
                string videolink = model.Videolink;// data.kycvideo;
                string agentid = model.Agentid;//data.userids;

                string base64frontString = videolink.ToString();
                byte[] imageBytesfrontimg = Convert.FromBase64String(base64frontString);
                // String file = Convert.ToBase64String(file);
                var newFileName = Guid.NewGuid().ToString() + ".mp4";
                var imagePath = @"\AepsVideos\" + newFileName;
                var filePath = HttpContext.Current.Server.MapPath("~/" + imagePath);
                // FileInfo fil = new FileInfo("D://test.mp4");
                //  file.SaveAs(filePath);
                File.WriteAllBytes(filePath, imageBytesfrontimg);

                string currentUrl = HttpContext.Current.Request.Url.ToString().ToLower();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;
                string path = HttpContext.Current.Request.Url.AbsolutePath;
                WriteLog("URL " + url);
                url = url.Replace(path, "");
                var fullurl = url + "/" + imagePath;
                WriteLog("Full URL " + fullurl);
                WriteLog("agentid " + agentid);
                var token = string.Empty;
                token = getAuthToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    JObject response1 = new JObject();
                    response1.Add("status", false);
                    response1.Add("msg", "Failed at provider server.");
                    return Json(response1);
                }
                else
                {
                    WriteLog("Token " + agentid);
                    var Aepsinfo = db.AEPS_TXN_Details.Where(aa => aa.MerchantTxnId == agentid).SingleOrDefault();
                    if (string.IsNullOrEmpty(Aepsinfo.videolink) == true)
                    {
                        Aepsinfo.videolink = imagePath;
                        Aepsinfo.videostatus = "Done";
                        db.SaveChanges();
                    }
                    var client = new RestClient("http://api.vastbazaar.com/api/AEPS/AepsVideolink?agentid=" + agentid + "&videolink=" + fullurl + "");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", "Bearer " + token);
                    IRestResponse response1 = client.Execute(request);
                    var respp = response1.Content;
                    JObject response11 = new JObject();
                    response11.Add("status", true);
                    response11.Add("msg", "Done");
                    return Json(response11);

                }

            }
            catch (Exception ex)
            {
                JObject response11 = new JObject();
                response11.Add("status", false);
                response11.Add("msg", "Failed at provider server");
                return Json(response11);

                //   return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }

        [HttpPost]
        [Route("api/user/check")]
        public async Task<IHttpActionResult> check()
        {
            string url = HttpContext.Current.Request.Url.AbsoluteUri;
            string path = HttpContext.Current.Request.Url.AbsolutePath;
            url = url.Replace(path, "");
            JObject response1 = new JObject();
            response1.Add("status", false);
            response1.Add("msg", url);
            return Json(response1);
        }

        public string getAuthToken()
        {
            try
            {
                using (VastwebmultiEntities db = new VastwebmultiEntities())
                {
                    var tokn = db.vastbazzartokens.SingleOrDefault();
                    if (tokn == null)
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
            }
            catch
            {
                return null;
            }
        }
        public IRestResponse tokencheck()
        {
            using (VastwebmultiEntities db = new VastwebmultiEntities())
            {
                string VastbazaarBaseUrl = "http://api.vastbazaar.com/";
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
                return response;
            }
        }

        public static void WriteLog(string strMessage)
        {
            try
            {
                StreamWriter log;
                FileStream fileStream = null;
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo;

                string logFilePath = "C:\\Logs\\";
                logFilePath = logFilePath + "TESTUPLOAD- " + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
                logFileInfo = new FileInfo(logFilePath);
                logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                if (!logFileInfo.Exists)
                {
                    fileStream = logFileInfo.Create();
                }
                else
                {
                    fileStream = new FileStream(logFilePath, FileMode.Append);
                }
                log = new StreamWriter(fileStream);
                log.WriteLine(strMessage);
                log.Close();

            }
            catch (Exception ex)
            {

            }
        }

    }
    public class Imagephoto
    {
        public int cancellchecque_idno { get; set; }
        public string ProfileImagess { get; set; }
        public string AadharcardFront { get; set; }
        public string AadharcardBack { get; set; }
        public string PancardFront { get; set; }
        public string ShopeWithSelfie { get; set; }
        public string cancelledcheque { get; set; }
        public string DOCTYPE { get; set; }
        public string Serviceaggreementpath { get; set; }
        public string Registrationcertificatepath { get; set; }

        public string txtretailerid { get; set; }
        public string txn_ids { get; set; }
        public string Imps_photo_images { get; set; }
        public string currentrole { get; set; }


    }
    public class Kycvideoesclass
    {
        public string kycvideo { get; set; }
        public string userids { get; set; }
        public string role { get; set; }
    }
    public class AepsVideoUpload
    {
        public string Videolink { get; set; }
        public string Agentid { get; set; }
    }
    public class Responsests
    {
        public string status { get; set; }
        public string message { get; set; }
    }

}
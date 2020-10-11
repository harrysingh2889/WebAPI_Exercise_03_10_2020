using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebAPI_Exercise_03_10_2020.Controllers
{
    [RoutePrefix("api/ImageUpload")]
    public class ImageUploadController : ApiController
    {
        [Route("PostUserImage")]
        [AllowAnonymous]
        public object PostUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

                            dict.Add("error", message);
                            //return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                            return new { error = dict };
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 1 mb.");

                            dict.Add("error", message);
                            //return Request.CreateResponse(HttpStatusCode.BadRequest, dict);

                            return new { error = dict };
                        }
                        else
                        {
                            string UniqueGenaratedFileName = GenerateFileName("Image");
                            var filePath = HttpContext.Current.Server.MapPath("~/UserImages/" + UniqueGenaratedFileName + extension);
                            postedFile.SaveAs(filePath);

                        }
                    }

                    var message1 = string.Format("Image Updated Successfully.");
                    //return Request.CreateErrorResponse(HttpStatusCode.Created, message1);
                    return new { result = message1 };
                }
                var res = string.Format("Please Upload a image.");
                dict.Add("error", res);
                //return Request.CreateResponse(HttpStatusCode.NotFound, dict);
                return new { error = dict };
            }
            catch (Exception ex)
            {
                return new { error = ex.Message.ToString() };
            }
        }

        public string GenerateFileName(string context)
        {
            return context + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N");
        }
    }
}

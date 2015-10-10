using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using video_question.Models;

namespace video_question.Controllers
{
    public class FilesController : ApiController
    {
        [HttpPost] // This is from System.Web.Http, and not from System.Web.Mvc
        public async Task<HttpResponseMessage> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = GetMultipartProvider();
            var result = await Request.Content.ReadAsMultipartAsync(provider);

            // On upload, files are given a generic name like "BodyPart_26d6abe1-3ae1-416a-9429-b35f15e6e5d5"
            // so this is how you can get the original file name
            var originalFileName = GetDeserializedFileName(result.FileData.First());

            // uploadedFileInfo object will give you some additional stuff like file length,
            // creation time, directory name, a few filesystem methods etc..
            var uploadedFileInfo = new FileInfo(result.FileData.First().LocalFileName);

            // Remove this line as well as GetFormData method if you're not
            // sending any form data with your upload request
            var fileUploadObj = GetFormData<UploadDataModel>(result);

            // Through the request response you can return an object to the Angular controller
            // You will be able to access this in the .success callback through its data attribute
            // If you want to send something to the .error callback, use the HttpStatusCode.BadRequest instead
            var returnData = "ReturnTest";
            return this.Request.CreateResponse(HttpStatusCode.OK, new { returnData });
        }

        [HttpGet]
        public HttpResponseMessage Download(string fileName)
        {
            // serve the file to the client
            //I have used the x-filename header to send the filename. This is a custom header for convenience.
            //You should set the content-type mime header for your response too, so the browser knows the data format.
            HttpResponseMessage result =null;
            var downloadFolder = "~/App_Data/Tmp/FileUploads";
            var localFilePath = Path.Combine( downloadFolder , fileName);
            var info = System.IO.File.GetAttributes(HttpContext.Current.Request.MapPath(localFilePath));
            result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.Add("x-filename", fileName);
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = fileName;

            return result;
        }


        //public HttpResponseMessage Get(string fileName)
        //{
        //    var resp = Request.CreateResponse();
        //    resp.Content=new PushStreamContent( (stream,content,context)=>{
        //        byte[] bytes=new byte[5000];
        //       FileStream f= System.IO.File.OpenRead(fileName);
        //       int length=(int)f.Length;
        //        while(length > 0)
        //        {
        //        var bytesRead=f.Read(bytes,0,Math.Min(length,bytes.Length));
        //            stream.WriteAsync(bytes,0,bytesRead);
        //            length-=bytesRead;
                
        //        }
                    
        //    },new MediaTypeHeaderValue("video/mp4"));
        //    return resp;
        
        
        
        //}

        public HttpResponseMessage Get(string filename, string ext)
        {

            var response = Request.CreateResponse();
            var video = new VideoStreamModel(filename, ext);

           response.Content = new PushStreamContent(video.WriteToStream, new MediaTypeHeaderValue("video/" + ext));

            return response;
        }


        // You could extract these two private methods to a separate utility class since
        // they do not really belong to a controller class but that is up to you
        private MultipartFormDataStreamProvider GetMultipartProvider()
        {
            // IMPORTANT: replace "(tilde)" with the real tilde character
            // (our editor doesn't allow it, so I just wrote "(tilde)" instead)
            var uploadFolder = "~/App_Data/Tmp/FileUploads"; // you could put this to web.config
            var root = HttpContext.Current.Server.MapPath(uploadFolder);
            Directory.CreateDirectory(root);
            return new MultipartFormDataStreamProvider(root);
        }

        // Extracts Request FormatData as a strongly typed model
        private object GetFormData<T>(MultipartFormDataStreamProvider result)
        {
            if (result.FormData.HasKeys())
            {
                var unescapedFormData = Uri.UnescapeDataString(result.FormData
                    .GetValues(0).FirstOrDefault() ?? String.Empty);
                if (!String.IsNullOrEmpty(unescapedFormData))
                    return JsonConvert.DeserializeObject<T>(unescapedFormData);
            }

            return null;
        }

        private string GetDeserializedFileName(MultipartFileData fileData)
        {
            var fileName = GetFileName(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }

        private string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }
    }
}

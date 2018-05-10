using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using PbsApi.Models;
using PbsApi.Auth;
using System.Collections;
using System.Web;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http.Headers;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace PbsApi.Controllers
{
    public class FilesController : ApiController
    {
        private pbsEntities db = new pbsEntities();
        log4net.ILog log = log4net.LogManager.GetLogger("FilesController");

        private class FileInfoList
        {
            public int code { get; set; }
            public String msg { get; set; }

            public List<sys_upload_file> resultList { get; set; }
        } 
        /*
        [HttpPost]
        [AuthFilterOutside]
        [Route("api/Files/mutilUpload")]
        public async Task<Dictionary<string, string>> Post(int id = 0)
        {

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string root = HttpContext.Current.Server.MapPath("~/App_Data/images");//指定要将文件存入的服务器物理位置  
            var provider = new MultipartFormDataStreamProvider(root);
            try
            {
                // Read the form data.  
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.  
                foreach (MultipartFileData file in provider.FileData)
                {//接收文件  
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);//获取上传文件实际的文件名  
                    Trace.WriteLine("Server file path: " + file.LocalFileName);//获取上传文件在服务上默认的文件名  
                }//TODO:这样做直接就将文件存到了指定目录下，暂时不知道如何实现只接收文件数据流但并不保存至服务器的目录下，由开发自行指定如何存储，比如通过服务存到图片服务器  
                foreach (var key in provider.FormData.AllKeys)
                {//接收FormData  
                    dic.Add(key, provider.FormData[key]);
                }
            }
            catch
            {
                throw;
            }
            return dic;
        }
         * */
        protected readonly static List<string> VALID_FILE_TYPES = new List<string> { "jpg", "bmp", "gif", "jpeg", "png","tif" };

        protected static bool ValidateFileType(string fileName)
        {
            string fileType = String.Empty;
            int lastDotIndex = fileName.LastIndexOf(".");
            if (lastDotIndex >= 0)
            {
                fileType = fileName.Substring(lastDotIndex + 1).ToLower();
            }

            if (VALID_FILE_TYPES.Contains(fileType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        [AuthFilterOutside]
        [Route("api/Files/singleUpload")]
        public IHttpActionResult upload()
        {
            log.Info("begin to resceive files");
            HttpFileCollection files = HttpContext.Current.Request.Files;  
             
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (string key in files.AllKeys)  
            {
                log.Debug("key = " + key);
                HttpPostedFile file = files[key];
                if (string.IsNullOrEmpty(file.FileName) == false)
                {
                    string fileName = "";
                    string filePath = "";
                    if (key.Equals("file"))
                    {
                        fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName);                        
                    }
                    else
                    {
                        fileName = key;
                    }

                    filePath = HttpContext.Current.Server.MapPath("~/upload/") + fileName;
                    log.Debug("filePath = " + filePath);
                    file.SaveAs(filePath);
                    log.Debug("fileName = " + fileName);
                    dic.Add(key, fileName);
                }
            }

            return Ok(dic);  

        }
        [Route("api/GetWordFile")]
        public HttpResponseMessage getWordFile(string fileName)
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath("~/pbs_data/");
                FileStream file = new FileStream(path + fileName, FileMode.Open);
               
                //img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(file);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = fileName;
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/ms-word");
                result.Content.Headers.ContentLength = new FileInfo(path+fileName).Length;                
                return result;
            }
            catch(Exception e)
            {
                log.Fatal(e.Message);
                throw e;
                //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.NotFound);
                //return result;
            }
        }

        [Route("api/GetFile")]
        public HttpResponseMessage getFile(string fileName,string uuid)
        {
            try
            {
                log.Info("begin to resceive files");
                Image img = Image.FromFile(HttpContext.Current.Server.MapPath("~/pbs_data/" + uuid + "/") + fileName);

                MemoryStream ms = new MemoryStream();
                int toWidth = 600, toHeight = 800;
                if(img.Height < toHeight)
                {
                    toHeight = img.Height;
                }
                if(img.Width < toWidth)
                {
                    toWidth = img.Width;
                }
                Bitmap bm = new Bitmap(toWidth, toHeight);  
                Graphics g = Graphics.FromImage(bm);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.White);
                g.DrawImage(img, 0, 0, toWidth, toHeight);

                long[] quality = new long[1];
                quality[0] = 80;
                EncoderParameters encoderParams = new EncoderParameters();
                EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                encoderParams.Param[0] = encoderParam;
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();//获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象。  
                ImageCodecInfo jpegICI = null;
                for (int i = 0; i < arrayICI.Length; i++)  
               {  
                    if (arrayICI[i].FormatDescription.Equals("JPEG"))  
                    {  
                        jpegICI = arrayICI[i];//设置JPEG编码  
                        break;  
                    }  
                }  
                if (jpegICI != null)  
                {  
                    bm.Save(ms, jpegICI, encoderParams);  
                } 

                //img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(ms.ToArray());
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                return result;
            }catch
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.NotFound);
                return result;
            }
        }
        // GET: api/Files
        [Route("api/Files/GetFileList")]
        [AuthFilterOutside]
        [ResponseType(typeof(FileInfoList))]
        public IHttpActionResult  Getsys_upload_file(string uuid)
        {
            FileInfoList fiList = new FileInfoList();
            try
            {                
                if (uuid == "")
                {
                    fiList.code = 103;
                    fiList.msg = "参数验证失败";
                    return Ok(fiList);
                }
                fiList.resultList = db.sys_upload_file.Where(m => m.rec_uuid.Equals(uuid)).ToList();

                fiList.code = 100;
                return Ok(fiList);
            }
            catch (Exception e)
            {
                fiList.code = 104;
                fiList.msg = "查询文件列表失败: " + e.Message;
                return Ok(fiList);
            }
        }

        // GET: api/Files/5
        [Route("api/Files/GetFileInfo")]
        [AuthFilterOutside]
        [ResponseType(typeof(sys_upload_file))]
        public async Task<IHttpActionResult> Getsys_upload_file(int id)
        {
            sys_upload_file sys_upload_file = await db.sys_upload_file.FindAsync(id);
            if (sys_upload_file == null)
            {
                return NotFound();
            }

            return Ok(sys_upload_file);
        }

        // PUT: api/Files/5
        [Route("api/Files/UpdateFileInfo")]
        [AuthFilterOutside]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putsys_upload_file(int id, sys_upload_file sys_upload_file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sys_upload_file.file_id)
            {
                return BadRequest();
            }

            db.Entry(sys_upload_file).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!sys_upload_fileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Files
        [Route("api/Files/AddFileInfo")]
        [AuthFilterOutside]
        [ResponseType(typeof(sys_upload_file))]
        public async Task<IHttpActionResult> Postsys_upload_file(sys_upload_file sys_upload_file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.sys_upload_file.Add(sys_upload_file);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = sys_upload_file.file_id }, sys_upload_file);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool sys_upload_fileExists(int id)
        {
            return db.sys_upload_file.Count(e => e.file_id == id) > 0;
        }
    }
}
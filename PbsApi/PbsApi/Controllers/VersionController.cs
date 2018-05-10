using PbsApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace PbsApi.Controllers
{
    public class VersionController : ApiController
    {
        private pbsEntities db = new pbsEntities();
        private class VersionInfo
        {
            public int code { get; set; }
            public String msg { get; set; }

            public sys_version result { get; set; }
        }

        [Route("api/Version/GetNewestVersionCode")]
        public IHttpActionResult GetNewestVersionCode()
        {
            VersionInfo resp = new VersionInfo();

            var sv = db.sys_version.OrderByDescending(m => m.version_code).First();
            resp.code = 200;
            resp.result = sv;
            
            return Ok(resp);
        }

        [Route("api/Version/GetNewestApk")]
        public HttpResponseMessage GetNewestApk()
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath("~/pbs_data/");
                var sv = db.sys_version.OrderByDescending(m => m.version_code).First();
                string fileName = sv.apk_name;
                FileStream file = new FileStream(path + fileName, FileMode.Open);
                if (!file.CanRead)
                {
                    HttpResponseMessage rsl = new HttpResponseMessage(HttpStatusCode.NotFound);
                    return rsl;
                }
                sv.download_count = sv.download_count + 1;
                db.sys_version.Attach(sv);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(sv);
                stateEntity.SetModifiedProperty("download_count");
                db.SaveChanges();
                //img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(file);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = fileName;
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                result.Content.Headers.ContentLength = new FileInfo(path + fileName).Length;
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

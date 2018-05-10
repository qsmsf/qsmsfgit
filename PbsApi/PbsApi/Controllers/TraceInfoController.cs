using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using PbsApi.Auth;
using PbsApi.Models;

namespace PbsApi.Controllers
{
    public class TraceInfoController : ApiController
    {
        private pbsEntities db = new pbsEntities();
        log4net.ILog log = log4net.LogManager.GetLogger("TraceInfoController");

        private class TraceInfoList
        {
            public int code { get; set; }
            public String msg { get; set; }

            public List<view_trace_info> resultList { get; set; }
        }
        private class TraceInfoDetail
        {
            public int code { get; set; }
            public String msg { get; set; }
            public view_trace_info result { get; set; }
        }

        // GET: api/TraceInfo
        [Route("api/TraceInfo/GetTraceInfoList")]
        [AuthFilterOutside]
        [ResponseType(typeof(TraceInfoList))]
        public IHttpActionResult GetTraceInfoList(string uuid)
        {
            TraceInfoList til = new TraceInfoList();
            try
            {
                if (uuid == "")
                {
                    til.code = 103;
                    til.msg = "参数验证失败";
                    return Ok(til);
                }

                var traceList = db.view_trace_info.Where(p => p.uuid.Equals(uuid)).ToList();
                til.code = 100;
                til.resultList = traceList;
                return Ok(til);
            }
            catch (Exception e)
            {
                til.code = 104;
                til.msg = "查询痕迹列表失败: " + e.Message;
                return Ok(til);
            }
            
        }

        // GET: api/TraceInfo/5
        [Route("api/TraceInfo/GetTraceInfoList")]
        [AuthFilterOutside]
        [ResponseType(typeof(TraceInfoDetail))]
        public IHttpActionResult GetTraceInfo(int id)
        {
            trace_info trace_info = db.trace_info.Find(id);
            if (trace_info == null)
            {
                return NotFound();
            }

            return Ok(trace_info);
        }

        // PUT: api/TraceInfo/5
        [Route("api/TraceInfo/UpdateTraceInfo")]
        [AuthFilterOutside]
        [ResponseType(typeof(TraceInfoDetail))]
        public IHttpActionResult UpdateTraceInfo()
        {
            TraceInfoDetail tid = new TraceInfoDetail();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                String jsonBean = context.Request.Form["jsonBean"];
                if (jsonBean == null)
                {
                    tid.code = 101;
                    tid.msg = "参数验证失败!";
                    return Ok(tid);
                    //return BadRequest(ModelState);
                }
                view_trace_info ti = new view_trace_info();
                ti = JsonConvert.DeserializeObject<view_trace_info>(jsonBean);

                int id = ti.sid;
                var traceInfo = db.trace_info.Find(id);
                if(traceInfo == null)
                {
                    tid.code = 102;
                    tid.msg = "找不到痕迹记录!";
                    return Ok(tid);
                }

                traceInfo.name = ti.name;
                traceInfo.barcode = ti.barcode;
                traceInfo.disp = ti.disp;
                traceInfo.image1 = ti.image1;
                traceInfo.tqbw = ti.tqbw;
                traceInfo.trace_type = ti.traceType;

                db.trace_info.Attach(traceInfo);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(traceInfo);
                stateEntity.SetModifiedProperty("name");
                stateEntity.SetModifiedProperty("barcode");
                stateEntity.SetModifiedProperty("disp");
                stateEntity.SetModifiedProperty("image1");
                stateEntity.SetModifiedProperty("tqbw");
                stateEntity.SetModifiedProperty("trace_type");

                db.SaveChanges();

                tid.code = 100;
                
                return Ok(tid);
            }
            catch (Exception e)
            {
                tid.code = 104;
                tid.msg = "更新痕迹失败: " + e.Message;
                return Ok(tid);
            }
            
        }

        // POST: api/TraceInfo
        [Route("api/TraceInfo/AddTraceInfo")]
        [AuthFilterOutside]
        [ResponseType(typeof(TraceInfoDetail))]
        public IHttpActionResult AddTraceInfo()
        {
            TraceInfoDetail tid = new TraceInfoDetail();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                String jsonBean = context.Request.Form["jsonBean"];
                if (jsonBean == null)
                {
                    tid.code = 101;
                    tid.msg = "参数验证失败!";
                    return Ok(tid);
                    //return BadRequest(ModelState);
                }
                view_trace_info ti = new view_trace_info();
                ti = JsonConvert.DeserializeObject<view_trace_info>(jsonBean);

                trace_info traceInfo = new trace_info();
                
                traceInfo.name = ti.name;
                traceInfo.barcode = ti.barcode;
                traceInfo.disp = ti.disp;
                traceInfo.image1 = ti.image1;
                traceInfo.tqbw = ti.tqbw;
                traceInfo.trace_type = ti.traceType;
                traceInfo.create_id = ti.creatorId;
                traceInfo.create_time = DateTime.Now;

                db.trace_info.Add(traceInfo);

                db.SaveChanges();

                tid.code = 100;

                return Ok(tid);
            }
            catch (Exception e)
            {
                tid.code = 104;
                tid.msg = "新增痕迹失败: " + e.Message;
                return Ok(tid);
            }
        }

        // DELETE: api/TraceInfo/5
        [ResponseType(typeof(trace_info))]
        public IHttpActionResult Deletetrace_info(int id)
        {
            trace_info trace_info = db.trace_info.Find(id);
            

            return Ok(trace_info);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool trace_infoExists(int id)
        {
            return db.trace_info.Count(e => e.sid == id) > 0;
        }
    }
}
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
    public class RecordInfoController : ApiController
    {
        private pbsEntities db = new pbsEntities();
        log4net.ILog log = log4net.LogManager.GetLogger("RecordInfoController");

        private class RecordInfoList
        {
            public int code { get; set; }
            public String msg { get; set; }

            public List<view_recordinfo> resultList { get; set; }
        }
        private class RecordInfoDetail
        {
            public int code { get; set; }
            public String msg { get; set; }
            public String recNo { get; set; }
            public view_recordinfo result { get; set; }
        }

        // GET: api/RecordInfo
        [Route("api/RecordInfo/GetRecordList")]
        [AuthFilterOutside]
        [ResponseType(typeof(RecordInfoList))]
        public IHttpActionResult GetRecordInfoList(int userId, int state, String sdate, String edate)
        {
            try
            {
                RecordInfoList ril = new RecordInfoList();
                var recordList = db.view_recordinfo.Where(p => p.creatorId == userId && p.state == 1);
                DateTime sd, ed;
                if (sdate != null && edate == null)
                {
                    sd = Convert.ToDateTime(sdate);
                    ed = sd.AddDays(7);
                }
                else if (sdate == null && edate != null)
                {
                    ed = Convert.ToDateTime(edate);
                    sd = ed.AddDays(-7);
                }
                else if (sdate == null && edate == null)
                {
                    sd = DateTime.Today.AddDays(-7);
                    ed = DateTime.Today;
                }
                else
                {
                    sd = Convert.ToDateTime(sdate);
                    ed = Convert.ToDateTime(edate);
                }
                ed = ed.AddDays(1);
                recordList = recordList.Where(p => p.createTime >= sd && p.createTime <= ed);
                ril.resultList = recordList.ToList();
                ril.code = 100;
                return Ok(ril);
            }catch(Exception e)
            {
                log.Error("GetRecordInfo error, message = " + e.Message);
                return BadRequest();
            }
        }

        // GET: api/RecordInfo/5
        [ResponseType(typeof(RecordInfoDetail))]
        [Route("api/RecordInfo/GetRecordInfo")]
        [AuthFilterOutside]
        public IHttpActionResult GetRecordInfo(int id)
        {
            RecordInfoDetail rsl = new RecordInfoDetail();
            try
            {
                view_recordinfo record = db.view_recordinfo.Find(id);
                if (record == null)
                {
                    rsl.code = 101;
                    rsl.msg = "记录不存在!";
                    return Ok(rsl);
                }
                rsl.code = 100;
                rsl.recNo = record.recordNo;
                rsl.result = record;
                return Ok(rsl);
            }
            catch (Exception e)
            {
                rsl.code = 103;
                rsl.msg = e.Message;
                return Ok(rsl);
            }

        }

        // PUT: api/RecordInfo/5
        [ResponseType(typeof(void))]
        [AuthFilterOutside]
        [Route("api/RecordInfo/UpdateRecordInfo")]
        public IHttpActionResult UpdateRecordInfo()
        {
            RecordInfoDetail rsl = new RecordInfoDetail();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                String jsonBean = context.Request.Form["jsonBean"];
                if (jsonBean == null)
                {
                    rsl.code = 101;
                    rsl.msg = "参数验证失败!";
                    return Ok(rsl);
                    //return BadRequest(ModelState);
                }

                view_recordinfo record = new view_recordinfo();
                record = JsonConvert.DeserializeObject<view_recordinfo>(jsonBean);

                int id = record.sid;
                var rec = db.record_info.Find(id);
                if(rec == null)
                {
                    rsl.code = 102;
                    rsl.msg = "记录不存在!";
                    return Ok(rsl);
                }

                rec.content = record.title;
                rec.loc_disp = record.locDisp;
                rec.loc_name = record.locName;
                rec.record_loc = record.recordLoc;
                rec.record_type = record.recordType;
                rec.type = record.type;

                db.record_info.Attach(rec);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(rec);
                stateEntity.SetModifiedProperty("content");
                stateEntity.SetModifiedProperty("loc_disp");
                stateEntity.SetModifiedProperty("loc_name");
                stateEntity.SetModifiedProperty("record_loc");
                stateEntity.SetModifiedProperty("record_type");
                stateEntity.SetModifiedProperty("type");
                db.SaveChanges();
            }
            catch (Exception e)
            {
                rsl.code = 103;
                rsl.msg = e.Message;
                return Ok(rsl);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/RecordInfo
        [ResponseType(typeof(RecordInfoDetail))]
        [AuthFilterOutside]
        [Route("api/RecordInfo/AddRecordInfo")]
        public IHttpActionResult AddRecordInfo()
        {
            RecordInfoDetail rsl = new RecordInfoDetail();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                String jsonBean = context.Request.Form["jsonBean"];
                if (jsonBean == null)
                {
                    rsl.code = 101;
                    rsl.msg = "参数验证失败!";
                    return Ok(rsl);
                    //return BadRequest(ModelState);
                }

                view_recordinfo record = new view_recordinfo();
                record = JsonConvert.DeserializeObject<view_recordinfo>(jsonBean);

                record_info rec = new record_info();
                rec.content = record.title;
                rec.loc_disp = record.locDisp;
                rec.loc_name = record.locName;
                rec.record_loc = record.recordLoc;
                rec.record_type = record.recordType;
                rec.type = record.type;
                rec.uuid = record.uuid;
                rec.creater_id = record.creatorId;
                rec.create_time = DateTime.Now;
                db.record_info.Add(rec);
                db.SaveChanges();

                string recNo = DateTime.Now.ToString("yyyyMMdd");
                for (int i = 0; i < 5 - rec.sid.ToString().Length; i++)
                {
                    recNo += "0";
                }
                recNo += rec.sid.ToString();

                rec.record_no = recNo;

                db.record_info.Attach(rec);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(rec);
                stateEntity.SetModifiedProperty("record_no");
                db.SaveChanges();

                rsl.code = 100;
                rsl.recNo = recNo.Substring(recNo.Length - 5);
                return Ok(rsl);
            }
            catch (Exception e)
            {
                rsl.code = 103;
                rsl.msg = e.Message;
                return Ok(rsl);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RecordInfoExists(int id)
        {
            return db.record_info.Count(e => e.sid == id) > 0;
        }
    }
}
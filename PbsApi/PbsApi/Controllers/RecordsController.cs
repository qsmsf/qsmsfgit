using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PbsApi.Models;
using PbsApi.Auth;
using PbsApi.Code;
using Newtonsoft.Json;
using System.Web;
using System.IO;

namespace PbsApi.Controllers
{
    public class RecordsController : ApiController
    {
        private pbsEntities db = new pbsEntities();
        log4net.ILog log = log4net.LogManager.GetLogger("RecordsController");
        private class RecordInfoList
        {
            public int code { get; set; }
            public String msg { get; set; }

            public List<pbs_record> resultList { get; set; }
        }
        private class RecordDetailInfo
        {
            public int code { get; set; }
            public String msg { get; set; }
            public String recNo { get; set; }
            public pbs_record result { get; set; }
        } 

        private class RecordAddRequest
        {
            public pbs_record entity { get; set; }
            public List<sys_upload_file> fileList { get; set; }

        }
        // GET: api/Records
        [Route("api/Records/GetRecordList")]
        [AuthFilterOutside]
        [ResponseType(typeof(RecordInfoList))]
        public IHttpActionResult Getpbs_record(int userId,int bgState,String sdate,String edate)
        {
            RecordInfoList ril = new RecordInfoList();
            try
            {
                var tmp = db.pbs_record.Where(m => m.creater_id == userId);
                if (bgState != 0)
                {
                    tmp = tmp.Where(m => m.record_state == bgState);
                }
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
                tmp = tmp.Where(m => m.ky_date >= sd && m.ky_date <= ed).OrderByDescending(m => m.create_time);
                ril.code = 100;
                ril.resultList = tmp.ToList();
                return Ok(ril);
            }catch(Exception e)
            {
                ril.code = 102;
                ril.msg = e.Message;
                return Ok(ril);
            }
        }

        // GET: api/Records/5
        [Route("api/Records/GetRecordDetail")]
        [AuthFilterOutside]
        [ResponseType(typeof(pbs_record))]
        public IHttpActionResult Getpbs_record(int id)
        {
            RecordDetailInfo rdi = new RecordDetailInfo();
            try
            {
                pbs_record pbs_record = db.pbs_record.Find(id);
                if (pbs_record == null)
                {
                    rdi.code = 101;
                    rdi.msg = "记录不存在!";
                    return Ok(rdi);
                }
                rdi.code = 100;
                rdi.result = pbs_record;
                return Ok(rdi);
            }
            catch (Exception e)
            {
                rdi.code = 103;
                rdi.msg = e.Message;
                return Ok(rdi);
            }
        }

        [Route("api/Records/UpdateRecordDetail")]
        [AuthFilterOutside]
        [ResponseType(typeof(RecordDetailInfo))]
        public IHttpActionResult Putpbs_record()
        {
            RecordDetailInfo rdi = new RecordDetailInfo();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                String jsonBean = context.Request.Form["jsonBean"];
                if (jsonBean == null)
                {
                    rdi.code = 101;
                    rdi.msg = "参数验证失败!";
                    return Ok(rdi);
                    //return BadRequest(ModelState);
                }

                RecordAddRequest rar = new RecordAddRequest();
                pbs_record record = new pbs_record();
                rar = JsonConvert.DeserializeObject<RecordAddRequest>(jsonBean);
                rar.entity.record_state = 1001;
                db.pbs_record.Attach(rar.entity);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(rar.entity);
                stateEntity.SetModifiedProperty("record_title");
                stateEntity.SetModifiedProperty("record_ky_no");
                stateEntity.SetModifiedProperty("record_jj_no");
                stateEntity.SetModifiedProperty("record_aj_no");
                stateEntity.SetModifiedProperty("bg_unit");
                stateEntity.SetModifiedProperty("ky_unit");
                stateEntity.SetModifiedProperty("ky_date");
                stateEntity.SetModifiedProperty("af_time");
                stateEntity.SetModifiedProperty("bj_time");
                stateEntity.SetModifiedProperty("kyks_time");
                stateEntity.SetModifiedProperty("kyjs_time");
                stateEntity.SetModifiedProperty("xz");
                stateEntity.SetModifiedProperty("fs_loc");
                stateEntity.SetModifiedProperty("xc_loc");
                stateEntity.SetModifiedProperty("xc_locpt");
                stateEntity.SetModifiedProperty("weather_info");
                stateEntity.SetModifiedProperty("trend_info");
                stateEntity.SetModifiedProperty("temper_info");
                stateEntity.SetModifiedProperty("humidity_info");
                stateEntity.SetModifiedProperty("light_info");
                stateEntity.SetModifiedProperty("bh_flag");
                stateEntity.SetModifiedProperty("bhr");
                stateEntity.SetModifiedProperty("bhr_name");
                stateEntity.SetModifiedProperty("bhr_unit");
                stateEntity.SetModifiedProperty("bhr_unit_name");
                stateEntity.SetModifiedProperty("bhr_pos");
                stateEntity.SetModifiedProperty("bh_function");
                stateEntity.SetModifiedProperty("xc_info");
                stateEntity.SetModifiedProperty("bd_reason");
                stateEntity.SetModifiedProperty("jzr");
                stateEntity.SetModifiedProperty("jzr_sex");
                stateEntity.SetModifiedProperty("jzr_birth");
                stateEntity.SetModifiedProperty("jzr_address");
                stateEntity.SetModifiedProperty("zhr");
                stateEntity.SetModifiedProperty("zhr_name");
                stateEntity.SetModifiedProperty("zhr_unit");
                stateEntity.SetModifiedProperty("zhr_unit_name");
                stateEntity.SetModifiedProperty("zhr_pos");
                stateEntity.SetModifiedProperty("blr");
                stateEntity.SetModifiedProperty("blr_name");
                stateEntity.SetModifiedProperty("ztr");
                stateEntity.SetModifiedProperty("ztr_name");
                stateEntity.SetModifiedProperty("zxr");
                stateEntity.SetModifiedProperty("zxr_name");
                stateEntity.SetModifiedProperty("lxr");
                stateEntity.SetModifiedProperty("lxr_name");
                stateEntity.SetModifiedProperty("lyr");
                stateEntity.SetModifiedProperty("lyr_name");
                stateEntity.SetModifiedProperty("xc_disp");
                stateEntity.SetModifiedProperty("record_reason");
                stateEntity.SetModifiedProperty("east");
                stateEntity.SetModifiedProperty("west");
                stateEntity.SetModifiedProperty("south");
                stateEntity.SetModifiedProperty("north");
                //db.pbs_record.Add(rar.entity);
                string path = HttpContext.Current.Server.MapPath("~/");
                string desPath = path+"pbs_data\\"+rar.entity.uuid;
                if(!Directory.Exists(desPath))
                {
                    Directory.CreateDirectory(desPath);
                }
                for (int i = 0; i < rar.fileList.Count; i++)
                {
                    sys_upload_file tmp = new sys_upload_file();
                    tmp =  rar.fileList[i];
                    tmp.rec_uuid = rar.entity.uuid;
                    db.sys_upload_file.Add(tmp);
                    string path1 = path + "upload\\" + tmp.file_url;
                    string path2 = desPath + "\\"+tmp.file_url;
                    File.Move(path1, path2);
                }
                string qmPicPath = path + "upload\\" + rar.entity.uuid + ".png";

                if (File.Exists(qmPicPath))
                {
                    sys_upload_file tmp = db.sys_upload_file.Where(p => p.file_name.Equals(rar.entity.uuid + ".png")).FirstOrDefault();
                    if (tmp == null)
                    {
                        tmp = new sys_upload_file();
                        tmp.rec_uuid = rar.entity.uuid;
                        tmp.file_type = 2005;
                        tmp.file_name = rar.entity.uuid + ".png";
                        tmp.file_uploader = rar.entity.creater_id;
                        tmp.file_url = rar.entity.uuid + ".png";
                        tmp.file_upload_time = DateTime.Now;
                        tmp.file_hint = "见证人签名";

                        db.sys_upload_file.Add(tmp);
                    }
                    else
                    {
                        tmp.file_uploader = rar.entity.creater_id;
                        tmp.file_upload_time = DateTime.Now;
                        db.sys_upload_file.Attach(tmp);
                        var stateEntity2 = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(tmp);
                        stateEntity2.SetModifiedProperty("file_uploader");
                        stateEntity2.SetModifiedProperty("file_upload_time");
                    }
                    string path2 = desPath + "\\" + tmp.file_url;
                    if (File.Exists(path2))
                    {
                        File.Delete(path2);
                    }
                    File.Move(qmPicPath, path2);
                }
                string fwPicPath = path + "upload\\" + rar.entity.uuid + "_xct.jpeg";
                if (File.Exists(fwPicPath))
                {
                    sys_upload_file tmp = db.sys_upload_file.Where(p => p.file_name.Equals(rar.entity.uuid + "_xct.jpeg")).FirstOrDefault();
                    if(tmp == null)
                    {
                        tmp = new sys_upload_file();
                        tmp.rec_uuid = rar.entity.uuid;
                        tmp.file_type = 2004;
                        tmp.file_name = rar.entity.uuid + "_xct.jpeg";
                        tmp.file_uploader = rar.entity.creater_id;
                        tmp.file_url = rar.entity.uuid + "_xct.jpeg";
                        tmp.file_upload_time = DateTime.Now;
                        tmp.file_hint = "方位示意图";

                        db.sys_upload_file.Add(tmp);
                    }
                    else
                    {
                        tmp.file_uploader = rar.entity.creater_id;
                        tmp.file_upload_time = DateTime.Now;
                        db.sys_upload_file.Attach(tmp);
                        var stateEntity2 = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(tmp);
                        stateEntity2.SetModifiedProperty("file_uploader");
                        stateEntity2.SetModifiedProperty("file_upload_time");
                    }
                    
                    string path2 = desPath + "\\" + tmp.file_url;
                    if (File.Exists(path2))
                    {
                        File.Delete(path2);
                    }
                    File.Move(fwPicPath, path2);
                }

                string pmPicPath = path + "upload\\" + rar.entity.uuid + "_pmt.jpeg";
                if (File.Exists(pmPicPath))
                {
                    sys_upload_file tmp = db.sys_upload_file.Where(p => p.file_name.Equals(rar.entity.uuid + "_pmt.jpeg")).FirstOrDefault();
                    if (tmp == null)
                    {
                        tmp = new sys_upload_file();
                        tmp.rec_uuid = rar.entity.uuid;
                        tmp.file_type = 2004;
                        tmp.file_name = rar.entity.uuid + "_pmt.jpeg";
                        tmp.file_uploader = rar.entity.creater_id;
                        tmp.file_url = rar.entity.uuid + "_pmt.jpeg";
                        tmp.file_upload_time = DateTime.Now;
                        tmp.file_hint = "平面示意图";

                        db.sys_upload_file.Add(tmp);
                    }
                    else
                    {
                        tmp.file_uploader = rar.entity.creater_id;
                        tmp.file_upload_time = DateTime.Now;
                        db.sys_upload_file.Attach(tmp);
                        var stateEntity2 = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(tmp);
                        stateEntity2.SetModifiedProperty("file_uploader");
                        stateEntity2.SetModifiedProperty("file_upload_time");
                    }
                    string path2 = desPath + "\\" + tmp.file_url;
                    if (File.Exists(path2))
                    {
                        File.Delete(path2);
                    }
                    File.Move(pmPicPath, path2);
                }

                db.SaveChanges();
                rdi.recNo = rar.entity.record_no;
                rdi.recNo = rdi.recNo.Substring(rdi.recNo.Length - 6);
                rdi.code = 100;
                return Ok(rdi);
            }
            catch (Exception e)
            {
                rdi.code = 104;
                rdi.msg = "更新记录失败: " + e.Message;
                log.Error(e.Message + " -- " + e.InnerException.Message);
                return Ok(rdi);
            }
        }

        [Route("api/Records/UpdateRecordDetail2")]//android使用
        [AuthFilterOutside]
        [ResponseType(typeof(RecordDetailInfo))]
        public IHttpActionResult Putpbs_record2()
        {
            RecordDetailInfo rdi = new RecordDetailInfo();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                StreamReader reader = new StreamReader(context.Request.InputStream);
                String jsonBean = reader.ReadToEnd();
                if (jsonBean == null)
                {
                    rdi.code = 101;
                    rdi.msg = "参数验证失败!";
                    return Ok(rdi);
                    //return BadRequest(ModelState);
                }
                jsonBean = jsonBean.Replace("\\", "");
                jsonBean = jsonBean.Substring(1, jsonBean.Length - 2);

                log.Debug("jsonbean2 = " + jsonBean);

                RecordAddRequest rar = new RecordAddRequest();
                pbs_record record = new pbs_record();
                rar = JsonConvert.DeserializeObject<RecordAddRequest>(jsonBean);
                rar.entity.record_state = 1001;
                db.pbs_record.Attach(rar.entity);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(rar.entity);
                stateEntity.SetModifiedProperty("record_title");
                stateEntity.SetModifiedProperty("record_ky_no");
                stateEntity.SetModifiedProperty("record_jj_no");
                stateEntity.SetModifiedProperty("record_aj_no");
                stateEntity.SetModifiedProperty("bg_unit");
                stateEntity.SetModifiedProperty("ky_unit");
                stateEntity.SetModifiedProperty("ky_date");
                stateEntity.SetModifiedProperty("af_time");
                stateEntity.SetModifiedProperty("bj_time");
                stateEntity.SetModifiedProperty("kyks_time");
                stateEntity.SetModifiedProperty("kyjs_time");
                stateEntity.SetModifiedProperty("xz");
                stateEntity.SetModifiedProperty("fs_loc");
                stateEntity.SetModifiedProperty("xc_loc");
                stateEntity.SetModifiedProperty("xc_locpt");
                stateEntity.SetModifiedProperty("weather_info");
                stateEntity.SetModifiedProperty("trend_info");
                stateEntity.SetModifiedProperty("temper_info");
                stateEntity.SetModifiedProperty("humidity_info");
                stateEntity.SetModifiedProperty("light_info");
                stateEntity.SetModifiedProperty("bh_flag");
                stateEntity.SetModifiedProperty("bhr");
                stateEntity.SetModifiedProperty("bhr_name");
                stateEntity.SetModifiedProperty("bhr_unit");
                stateEntity.SetModifiedProperty("bhr_unit_name");
                stateEntity.SetModifiedProperty("bhr_pos");
                stateEntity.SetModifiedProperty("bh_function");
                stateEntity.SetModifiedProperty("xc_info");
                stateEntity.SetModifiedProperty("bd_reason");
                stateEntity.SetModifiedProperty("jzr");
                stateEntity.SetModifiedProperty("jzr_sex");
                stateEntity.SetModifiedProperty("jzr_birth");
                stateEntity.SetModifiedProperty("jzr_address");
                stateEntity.SetModifiedProperty("zhr");
                stateEntity.SetModifiedProperty("zhr_name");
                stateEntity.SetModifiedProperty("zhr_unit");
                stateEntity.SetModifiedProperty("zhr_unit_name");
                stateEntity.SetModifiedProperty("zhr_pos");
                stateEntity.SetModifiedProperty("blr");
                stateEntity.SetModifiedProperty("blr_name");
                stateEntity.SetModifiedProperty("ztr");
                stateEntity.SetModifiedProperty("ztr_name");
                stateEntity.SetModifiedProperty("zxr");
                stateEntity.SetModifiedProperty("zxr_name");
                stateEntity.SetModifiedProperty("lxr");
                stateEntity.SetModifiedProperty("lxr_name");
                stateEntity.SetModifiedProperty("lyr");
                stateEntity.SetModifiedProperty("lyr_name");
                stateEntity.SetModifiedProperty("xc_disp");
                stateEntity.SetModifiedProperty("record_reason");
                stateEntity.SetModifiedProperty("east");
                stateEntity.SetModifiedProperty("west");
                stateEntity.SetModifiedProperty("south");
                stateEntity.SetModifiedProperty("north");
                //db.pbs_record.Add(rar.entity);
                string path = HttpContext.Current.Server.MapPath("~/");
                string desPath = path + "pbs_data\\" + rar.entity.uuid;
                if (!Directory.Exists(desPath))
                {
                    Directory.CreateDirectory(desPath);
                }
                for (int i = 0; i < rar.fileList.Count; i++)
                {
                    sys_upload_file tmp = new sys_upload_file();
                    tmp = rar.fileList[i];
                    tmp.rec_uuid = rar.entity.uuid;
                    db.sys_upload_file.Add(tmp);
                    string path1 = path + "upload\\" + tmp.file_url;
                    string path2 = desPath + "\\" + tmp.file_url;
                    File.Move(path1, path2);
                }

                db.SaveChanges();
                rdi.recNo = rar.entity.record_no;
                rdi.recNo = rdi.recNo.Substring(rdi.recNo.Length - 5);
                rdi.code = 100;
                return Ok(rdi);
            }
            catch (Exception e)
            {
                rdi.code = 104;
                rdi.msg = "更新记录失败: " + e.Message;
                return Ok(rdi);
            }
        }

        [Route("api/Records/AddRecordDetail")]
        [AuthFilterOutside]
        [ResponseType(typeof(RecordDetailInfo))]
        public IHttpActionResult Postpbs_record()
        {
            RecordDetailInfo rdi = new RecordDetailInfo();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                String jsonBean = context.Request.Form["jsonBean"];
                if (jsonBean == null)
                {
                    rdi.code = 101;
                    rdi.msg = "参数验证失败!";
                    return Ok(rdi);
                    //return BadRequest(ModelState);
                }
                log.Debug("jsonbean1 = " + jsonBean);
                RecordAddRequest rar = new RecordAddRequest();
                pbs_record record = new pbs_record();
                rar = JsonConvert.DeserializeObject<RecordAddRequest>(jsonBean);
                rar.entity.create_time = DateTime.Now;
                rar.entity.record_state = 1001;
                if(rar.entity.kyjs_time == null)
                {
                    rar.entity.kyjs_time = DateTime.Now;
                }
                db.pbs_record.Add(rar.entity);
                string path = HttpContext.Current.Server.MapPath("~/");
                string desPath = path + "pbs_data\\" + rar.entity.uuid;
                if (!Directory.Exists(desPath))
                {
                    Directory.CreateDirectory(desPath);
                }
                for (int i = 0; i < rar.fileList.Count; i++)
                {
                    log.Debug("begin to move file. name = " + rar.fileList[i].file_name);
                    sys_upload_file tmp = new sys_upload_file();
                    tmp = rar.fileList[i];
                    tmp.rec_uuid = rar.entity.uuid;
                    db.sys_upload_file.Add(tmp);
                    string path1 = path + "upload\\" + tmp.file_url;
                    string path2 = desPath + "\\" + tmp.file_url;
                    if (File.Exists(path1))
                    {
                        File.Move(path1, path2);
                    }
                    else
                    {
                        log.Error("file is exist. name = " + path1);
                    }
                }

                string qmPicPath = path + "upload\\" + rar.entity.uuid + ".png";

                if (File.Exists(qmPicPath))
                {
                    sys_upload_file tmp = new sys_upload_file();
                    tmp.rec_uuid = rar.entity.uuid;
                    tmp.file_type = 2005;
                    tmp.file_name = rar.entity.uuid + ".png";
                    tmp.file_uploader = rar.entity.creater_id;
                    tmp.file_url = rar.entity.uuid + ".png";
                    tmp.file_upload_time = DateTime.Now;
                    tmp.file_hint = "见证人签名";

                    db.sys_upload_file.Add(tmp);
                    string path2 = desPath + "\\" + tmp.file_url;
                    File.Move(qmPicPath, path2);
                }

                string fwPicPath = path + "upload\\" + rar.entity.uuid + "_xct.jpeg";
                if (File.Exists(fwPicPath))
                {
                    sys_upload_file tmp = new sys_upload_file();
                    tmp.rec_uuid = rar.entity.uuid;
                    tmp.file_type = 2004;
                    tmp.file_name = rar.entity.uuid + "_xct.jpeg";
                    tmp.file_uploader = rar.entity.creater_id;
                    tmp.file_url = rar.entity.uuid + "_xct.jpeg";
                    tmp.file_upload_time = DateTime.Now;
                    tmp.file_hint = "方位示意图";

                    db.sys_upload_file.Add(tmp);
                    string path2 = desPath + "\\" + tmp.file_url;
                    File.Move(fwPicPath, path2);
                }

                string pmPicPath = path + "upload\\" + rar.entity.uuid + "_pmt.jpeg";
                if (File.Exists(pmPicPath))
                {
                    sys_upload_file tmp = new sys_upload_file();
                    tmp.rec_uuid = rar.entity.uuid;
                    tmp.file_type = 2004;
                    tmp.file_name = rar.entity.uuid + "_pmt.jpeg";
                    tmp.file_uploader = rar.entity.creater_id;
                    tmp.file_url = rar.entity.uuid + "_pmt.jpeg";
                    tmp.file_upload_time = DateTime.Now;
                    tmp.file_hint = "平面示意图";

                    db.sys_upload_file.Add(tmp);
                    string path2 = desPath + "\\" + tmp.file_url;
                    File.Move(pmPicPath, path2);
                }

                db.SaveChanges();
                string recNo = "440300"+DateTime.Now.Year.ToString();
                string unitNo = rar.entity.ky_unit.ToString().PadLeft(3,'0');
                recNo += unitNo;
                recNo += "9";
                DateTime toYear = Convert.ToDateTime(DateTime.Now.Year.ToString()+"-01-01");
                int num = db.pbs_record.Where(p => p.ky_unit == rar.entity.ky_unit && p.ky_date >= toYear).Count()+1;
                
                recNo += num.ToString().PadLeft(5,'0');
                rar.entity.record_no = recNo;
                db.pbs_record.Attach(rar.entity);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(rar.entity);
                stateEntity.SetModifiedProperty("record_no");
                db.SaveChanges();

                rdi.recNo = recNo.Substring(recNo.Length - 5);
                rdi.code = 100;
                return Ok(rdi);
            }
            catch (Exception e)
            {
                rdi.code = 105;
                rdi.msg = "新增记录失败: " + e.Message;
                log.Error(e.Message + " -- " + e.InnerException.Message);
                return Ok(rdi);
            }
        }

        [Route("api/Records/AddRecordDetail2")]//android用
        [AuthFilterOutside]
        [ResponseType(typeof(RecordDetailInfo))]
        public IHttpActionResult Postpbs_record2()
        {
            RecordDetailInfo rdi = new RecordDetailInfo();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];                
                StreamReader reader = new StreamReader(context.Request.InputStream);
                String jsonBean = reader.ReadToEnd();
                if (jsonBean == null)
                {
                    rdi.code = 101;
                    rdi.msg = "参数验证失败!";
                    return Ok(rdi);
                    //return BadRequest(ModelState);
                }
                jsonBean = jsonBean.Replace("\\", "");
                jsonBean = jsonBean.Substring(1, jsonBean.Length - 2);

                log.Debug("jsonbean2 = " + jsonBean);

                RecordAddRequest rar = new RecordAddRequest();
                pbs_record record = new pbs_record();
                rar = JsonConvert.DeserializeObject<RecordAddRequest>(jsonBean);
                rar.entity.create_time = DateTime.Now;
                rar.entity.record_state = 1001;
                if (rar.entity.kyjs_time == null)
                {
                    rar.entity.kyjs_time = DateTime.Now;
                }
                db.pbs_record.Add(rar.entity);
                string path = HttpContext.Current.Server.MapPath("~/");
                string desPath = path + "pbs_data\\" + rar.entity.uuid;
                if (!Directory.Exists(desPath))
                {
                    Directory.CreateDirectory(desPath);
                }
                for (int i = 0; i < rar.fileList.Count; i++)
                {
                    sys_upload_file tmp = new sys_upload_file();
                    tmp = rar.fileList[i];
                    tmp.rec_uuid = rar.entity.uuid;
                    db.sys_upload_file.Add(tmp);
                    string path1 = path + "upload\\" + tmp.file_url;
                    string path2 = desPath + "\\" + tmp.file_url;
                    File.Move(path1, path2);
                }

                db.SaveChanges();
                string recNo = "440305 " + DateTime.Now.Year.ToString() + " ";
                for (int i = 0; i < 5 - rar.entity.record_id.ToString().Length; i++)
                {
                    recNo += "0";
                }
                recNo += rar.entity.record_id.ToString();
                rar.entity.record_no = recNo;
                db.pbs_record.Attach(rar.entity);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(rar.entity);
                stateEntity.SetModifiedProperty("record_no");
                db.SaveChanges();

                rdi.recNo = recNo.Substring(recNo.Length - 5);
                rdi.code = 100;
                return Ok(rdi);
            }
            catch (Exception e)
            {
                rdi.code = 105;
                rdi.msg = "新增记录失败: " + e.Message;
                return Ok(rdi);
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

        private bool pbs_recordExists(int id)
        {
            return db.pbs_record.Count(e => e.record_id == id) > 0;
        }
    }
}
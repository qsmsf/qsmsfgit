using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FineUIMvc.QuickStart.Models;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Infrastructure;
using FineUIMvc.QuickStart.Code;
using System.IO;
using System.Text;
using Microsoft.Office.Interop;
using System.Web.Security;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;

namespace FineUIMvc.QuickStart.Controllers
{
    [Authorize]
    public class RecordsController : BaseController
    {
        private pbsEntities db = new pbsEntities();
        public class QueryCond
        {
            public int qKyUnit {get;set;}
            public int qCreator { get; set; }
            public int qKyState {get;set;}
            public DateTime qKyDate1 {get;set;}
            public DateTime qKyDate2 {get;set;}
            public string qTitle { get;set; }
            public string qTitle2 { get; set; }
            public int userId { get; set; }
            public int unitId { get; set; }
            public int dataType { get; set; }
            public QueryCond()
            {
                qKyState = 0;
                qKyUnit = 0;
                qCreator = 0;
                qKyDate1 = DateTime.Now.AddMonths(-1);
                qKyDate2 = DateTime.Now;
                qTitle = "";
                qTitle2 = "";
                userId = 0;
                unitId = 0;
                dataType = 0;
            }
        }
        private QueryCond gbQC = new QueryCond();
        // GET: /Records/
        public ActionResult Index()
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int userId = Global.loginInfo.user_id;
            int unitId = Global.loginInfo.unit_id;
            loadData(userId, unitId, Global.ALL_DATA, true);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic = getDataPriv(userId.ToString());
            int dataType = 0;
            if (dic.ContainsKey("all"))
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.ALL_DATA, true);
                ViewBag.UserList = GetUserList(userId, unitId, Global.ALL_DATA, true);
                dataType = Global.ALL_DATA;
            }
            else if (dic.ContainsKey("family"))
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.FAMILY_DATA, true);
                ViewBag.UserList = GetUserList(userId, unitId, Global.FAMILY_DATA, true);
            }
            else if (dic.ContainsKey("unit"))
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.UNIT_DATA, true);
                ViewBag.UserList = GetUserList(userId, unitId, Global.UNIT_DATA, true);
                dataType = Global.UNIT_DATA;
            }
            else
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.SELF_DATA, true);
                ViewBag.UserList = GetUserList(userId, unitId, Global.SELF_DATA, true);
                dataType = Global.SELF_DATA;
            }

            dic = getButtonPriv(userId.ToString(),"record");
            ViewBag.showDeleteBtn = false;
            ViewBag.showAddBtn = false;
            ViewBag.showEditBtn = false;
            ViewBag.showDetailBtn = false;
            ViewBag.showInvestigateBtn = false;
            ViewBag.showAntiInvestBtn = false;
            if (dic.ContainsKey("delete"))
            {
                ViewBag.showDeleteBtn = true;
            }
            if (dic.ContainsKey("add"))
            {
                ViewBag.showAddBtn = true;
            }
            if (dic.ContainsKey("edit"))
            {
                ViewBag.showEditBtn = true;
            }
            if (dic.ContainsKey("detail"))
            {
                ViewBag.showDetailBtn = true;
            }
            if (dic.ContainsKey("investigate"))
            {
                ViewBag.showInvestigateBtn = true;
            }
            if (dic.ContainsKey("antiInvest"))
            {
                ViewBag.showAntiInvestBtn = true;
            }
            var recordCount = 0;
            gbQC.unitId = unitId;
            gbQC.userId = userId;
            gbQC.dataType = dataType;
            var dataSource = QueryRecord(gbQC, 0, 10,ref recordCount);        
            ViewBag.Grid1RecordCount = recordCount;
            ViewBag.Grid1DataSource = dataSource;
            ViewBag.userId = userId.ToString();
            ViewBag.unitId = unitId.ToString();
            ViewBag.dataType = dataType.ToString();            
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Grid1_PageIndexChanged(JArray Grid1_fields, int Grid1_pageIndex)
        {
            var grid1 = UIHelper.Grid("Grid1");
            int count = 0;
            var dataSource = QueryRecord(gbQC, Grid1_pageIndex, 10,ref count);
            ViewBag.Grid1RecordCount = count;
            grid1.RecordCount(count);
            grid1.DataSource(dataSource, Grid1_fields);

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Grid1_Query(int userId, int unitId, int dataType, JArray Grid1_fields, string qKyUnit, string qCreator, string qKyState, string qKyDate1, string qKyDate2, string qTitle, string qTitle2)
        {
            gbQC.qKyUnit =Int32.Parse(qKyUnit);
            gbQC.qCreator = Int32.Parse(qCreator);
            gbQC.qKyState = Int32.Parse(qKyState);
            gbQC.qKyDate1 = Md5Helper.GMT2Local(qKyDate1);
            gbQC.qKyDate2 = Md5Helper.GMT2Local(qKyDate2);
            gbQC.qTitle = qTitle;
            gbQC.qTitle2 = qTitle2;
            gbQC.userId = userId;
            gbQC.unitId = unitId;
            gbQC.dataType = dataType;

            var grid1 = UIHelper.Grid("Grid1");
            int count = 0;
            var dataSource = QueryRecord(gbQC, 0, 10, ref count);
            ViewBag.Grid1RecordCount = count;
            grid1.RecordCount(count);
            grid1.DataSource(dataSource, Grid1_fields);
            return UIHelper.Result();
        }

        private List<pbs_record> QueryRecord(QueryCond qc,int pageIndex, int pageSize,ref int count)
        {
            var records = (from p in db.pbs_record
                           where p.ky_date >= qc.qKyDate1 && p.ky_date <= qc.qKyDate2
                           where p.record_no.Contains(qc.qTitle)
                           where p.record_title.Contains(qc.qTitle2)
                           select p);
            if (qc.dataType == Global.SELF_DATA)
            {
                records = records.Where(p => p.creater_id == qc.userId);
            }else if (qc.qCreator != 0)
            {
                records = records.Where(p => p.creater_id == qc.qCreator);
            }
            if (qc.dataType == Global.UNIT_DATA)
            {
                records = records.Where(p => p.ky_unit == qc.unitId);
            }else if (qc.qKyUnit != 0)
            {
                records = records.Where(p => p.ky_unit == qc.qKyUnit);
            }
            if (qc.qKyState != 0)
            {
                records = records.Where(p => p.record_state == qc.qKyState);
            }
            if (qc.userId != 1)//除超级管理员外已删除记录不可见
            {
                records = records.Where(p => p.record_state != 1000);
            }
            count = records.Count();
            records = (from p in records orderby p.create_time descending select p).Skip((pageIndex) * pageSize).Take(pageSize);
            return records.ToList();
        }

        // GET: /Records/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pbs_record pbs_record = db.pbs_record.Find(id);
            if (pbs_record == null)
            {
                return HttpNotFound();
            }
            bool showOutputBtn = false;
            if(pbs_record.record_state == 1004)
            {
                showOutputBtn = true;
            }
            ViewBag.showOutputBtn = showOutputBtn;
            bool showBhPanel = true;
            if (pbs_record.bh_flag == 0)
            {
                showBhPanel = false;
            }
            ViewBag.showBhPanel = showBhPanel;
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int unitId = Global.loginInfo.unit_id;
            int userId = Global.loginInfo.user_id;
            ViewBag.UserList = GetUserList(userId,unitId,Global.ALL_DATA,true);
            ViewBag.UnitList = GetUnitList(unitId, Global.ALL_DATA, true);
            
            return View(pbs_record);
        }

        /// <summary>
        /// 更新表格数据
        /// </summary>
        /// <param name="Grid1_fields"></param>
        private void UpdateGrid(JArray Grid1_fields)
        {
            var grid1 = UIHelper.Grid("Grid1");
            var recordCount = 0;
            var dataSource = QueryRecord(gbQC, 0, 10,ref recordCount);
            ViewBag.Grid1RecordCount = recordCount;
            grid1.RecordCount(recordCount);
            grid1.DataSource(dataSource, Grid1_fields);
            //UIHelper.Grid("Grid1").DataSource(db.pbs_record.ToList(), Grid1_fields);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Window1_Close(JArray Grid1_fields)
        {
            UpdateGrid(Grid1_fields);

            return UIHelper.Result();
        }

        // GET: /Records/Create
        public ActionResult Create()
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int unitId = Global.loginInfo.unit_id;
            int userId = Global.loginInfo.user_id;
            ViewBag.uuid = Guid.NewGuid().ToString();
            ViewBag.UnitList = GetUnitList(unitId, Global.ALL_DATA, true);
            ViewBag.UnitNameList = GetUnitList(unitId, Global.ALL_DATA, true, true);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic = getDataPriv(userId.ToString());
            if (dic.ContainsKey("all"))
            {
                ViewBag.UserList = GetUserList(userId, unitId, Global.ALL_DATA, true);
                ViewBag.UserNameList = GetUserList(userId, unitId, Global.ALL_DATA, true, true);
            }
            else
            {
                ViewBag.UserList = GetUserList(userId,unitId,Global.UNIT_DATA,true);
                ViewBag.UserNameList = GetUserList(userId, unitId, Global.UNIT_DATA, true, true);
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult btnCreate_Click([Bind(Include = "uuid,bg_unit,ky_unit,ky_date,bj_time,bjr,bjr_sex,bjr_idcard,bjr_phoneNo,kyks_time,kyjs_time,dest_unit,xz,fs_loc,xc_loc,xc_locpt,weather_info,trend_info,temper_info,humidity_info,light_info,bh_flag,bhr,bhr_pos,bh_function,xc_info,bd_reason,jzr,jzr_sex,jzr_birth,jzr_address,zhr_name,zhr_unit_name,zhr_pos,bhr_name,blr_name,ztr_name,zxr_name,lxr_name,lyr_name,xc_disp,record_reason")] pbs_record record)
        {
            if (ModelState.IsValid)
            {
                //record.uuid = Guid.NewGuid().ToString();
                record.create_time = DateTime.Now;
                record.creater_id = Global.loginInfo.user_id;
                record.record_state = 1001;
                record.record_title = record.fs_loc + record.xc_loc + "-" + record.xz;
                db.pbs_record.Add(record);
                db.SaveChanges();
                string recNo = "K440305 " + DateTime.Now.Year.ToString() + " ";
                for (int i = 0; i < 5 - record.record_id.ToString().Length; i++)
                {
                    recNo += "0";
                }
                recNo += record.record_id.ToString();
                record.record_no = recNo;
                db.pbs_record.Attach(record);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(record);
                stateEntity.SetModifiedProperty("record_no");
                db.SaveChanges();
                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult btnCreateSubmit_Click([Bind(Include = "uuid,bg_unit,ky_unit,ky_date,bj_time,bjr,bjr_sex,bjr_idcard,bjr_phoneNo,kyks_time,kyjs_time,dest_unit,xz,fs_loc,xc_loc,xc_locpt,weather_info,trend_info,temper_info,humidity_info,light_info,bh_flag,bhr_name,bhr_unit_name,bhr_pos,bh_function,xc_info,bd_reason,jzr,jzr_sex,jzr_birth,jzr_address,zhr_name,zhr_unit_name,zhr_pos,bhr_name,blr_name,ztr_name,zxr_name,lxr_name,lyr_name,xc_disp,record")] pbs_record record)
        {
            if (ModelState.IsValid)
            {
                //record.uuid = Guid.NewGuid().ToString();
                record.create_time = DateTime.Now;
                record.creater_id = Global.loginInfo.user_id;
                record.record_state = 1002;
                record.record_title = record.fs_loc + record.xc_loc + "-" + record.xz;
                db.pbs_record.Add(record);
                db.SaveChanges();
                string recNo = "440305 " + DateTime.Now.Year.ToString() + " ";
                for (int i = 0; i < 5 - record.record_id.ToString().Length; i++)
                {
                    recNo += "0";
                }
                recNo += record.record_id.ToString();
                record.record_no = recNo;
                db.pbs_record.Attach(record);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(record);
                stateEntity.SetModifiedProperty("record_no");
                db.SaveChanges();
                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }
        // GET: /Records/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pbs_record pbs_record = db.pbs_record.Find(id);
            if (pbs_record == null)
            {
                return HttpNotFound();
            }
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int unitId = Global.loginInfo.unit_id;
            int userId = Global.loginInfo.user_id;
            ViewBag.UnitList = GetUnitList(unitId, Global.ALL_DATA, true);
            ViewBag.UnitNameList = GetUnitList(unitId, Global.ALL_DATA, true, true);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic = getDataPriv(userId.ToString());
            if (dic.ContainsKey("all"))
            {
                ViewBag.UserList = GetUserList(userId, unitId, Global.ALL_DATA, true);
                ViewBag.UserNameList = GetUserList(userId, unitId, Global.ALL_DATA, true, true);
            }
            else
            {
                ViewBag.UserList = GetUserList(userId, unitId, Global.UNIT_DATA, true);
                ViewBag.UserNameList = GetUserList(userId, unitId, Global.UNIT_DATA, true, true);
            }
            bool showBhPanel = true;
            if (pbs_record.bh_flag == 0)
            {
                showBhPanel = false;
            }
            ViewBag.showBhPanel = showBhPanel;
            return View(pbs_record);
        }

        // GET: /Records/Investigate/5
        public ActionResult Investigate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pbs_record pbs_record = db.pbs_record.Find(id);
            if (pbs_record == null)
            {
                return HttpNotFound();
            }
            return View(pbs_record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnInvestigate_Click([Bind(Include = "record_id,record_state,check_hint")] pbs_record record)
        {

            if (ModelState.IsValid)
            {
                db.pbs_record.Attach(record);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(record);
                stateEntity.SetModifiedProperty("record_state");
                stateEntity.SetModifiedProperty("check_hint");

                db.SaveChanges();

                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnAntiInvest_Click(int userId, int unitId, int dataType, int rowId, JArray Grid1_fields)
        {
            if (ModelState.IsValid)
            {
                var record = db.pbs_record.Find(rowId);
                record.record_state = 1003;
                record.check_hint = "";
                db.pbs_record.Attach(record);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(record);
                stateEntity.SetModifiedProperty("record_state");
                stateEntity.SetModifiedProperty("check_hint");

                db.SaveChanges();

                gbQC.unitId = unitId;
                gbQC.userId = userId;
                gbQC.dataType = dataType;
                UpdateGrid(Grid1_fields);
            }

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult btnOutputWord_Click(int record_id)
        {
            // 关闭本窗体（触发窗体的关闭事件）
            //PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            try
            {
                var record = db.view_record2word.Find(record_id);
                string dotName = record.bg_dot_name;
                if (dotName.Equals(""))
                {
                    dotName = "xcbl.dot";
                }
                string path = Server.MapPath("~/pbs_data/");
                NameValueCollection val1 = (NameValueCollection)ConfigurationManager.GetSection("myUrl");
                if(System.IO.File.Exists(path+"\\"+record.record_no+".doc"))
                {
                    return Redirect(val1["apiUrl"] + "GetWordFile?fileName=" + record.record_no + ".doc");
                }
                CCWordApp word = new CCWordApp();
                word.OpenFromDot(Server.MapPath("~/App_Data/") + dotName);

                word.GotoBookMark("kyyear");                
                word.InsertText(record.ky_date.Value.Year.ToString());

                word.GotoBookMark("ky_no");
                word.InsertText(record.record_no.Substring(record.record_no.Count()-5));

                word.GotoBookMark("ky_unit");
                word.SetFont("Underlined");
                word.InsertText(record.unit_full_name);
                word.SetFont("Underlined");

                word.GotoBookMark("kyyear2");
                word.InsertText(record.ky_date.Value.Year.ToString());

                word.GotoBookMark("ky_no2");
                word.InsertText(record.record_no.Substring(record.record_no.Count() - 5));

                word.GotoBookMark("ky_unit2");
                word.InsertText(record.ky_unit_name);

                word.GotoBookMark("bg_unit");
                word.InsertText(record.bg_unit_name);


                word.GotoBookMark("ky_date");
                word.InsertText(record.ky_date.Value.ToString("yyyy"));
                word.GotoBookMark("ky_date_month");
                word.InsertText(record.ky_date.Value.ToString("MM"));
                word.GotoBookMark("ky_date_dd");
                word.InsertText(record.ky_date.Value.ToString("dd"));
                word.GotoBookMark("ky_date_hh");
                word.InsertText(record.ky_date.Value.ToString("hh"));
                word.GotoBookMark("ky_date_mm");
                word.InsertText(record.ky_date.Value.ToString("mm"));

                word.GotoBookMark("record_reason");
                string reason = record.record_reason;
                if(reason == null)
                {
                    reason = "";
                }
                for(int k = reason.Length; k < 320; k++)
                {
                    reason += " ";
                }
                word.InsertText(reason);

                word.GotoBookMark("kykssj");
                word.InsertText(record.kyks_time.Value.ToString("yyyy"));
                word.GotoBookMark("kykssj_month");
                word.InsertText(record.kyks_time.Value.ToString("MM"));
                word.GotoBookMark("kykssj_dd");
                word.InsertText(record.kyks_time.Value.ToString("dd"));
                word.GotoBookMark("kykssj_hh");
                word.InsertText(record.kyks_time.Value.ToString("hh"));
                word.GotoBookMark("kykssj_mm");
                word.InsertText(record.kyks_time.Value.ToString("mm"));

                word.GotoBookMark("kyjssj");
                word.InsertText(record.kyjs_time.Value.ToString("yyyy"));
                word.GotoBookMark("kyjssj_month");
                word.InsertText(record.kyjs_time.Value.ToString("MM"));
                word.GotoBookMark("kyjssj_dd");
                word.InsertText(record.kyjs_time.Value.ToString("dd"));
                word.GotoBookMark("kyjssj_hh");
                word.InsertText(record.kyjs_time.Value.ToString("hh"));
                word.GotoBookMark("kyjssj_mm");
                word.InsertText(record.kyjs_time.Value.ToString("mm"));
                

                word.GotoBookMark("xc_loc");
                string loc = "";
                if (record.xc_loc == null)
                {
                    loc = record.fs_loc;
                }
                else
                {
                    loc = record.fs_loc + record.xc_loc;
                }
                for (int k = loc.Length; k < 160; k++)
                {
                    loc += " ";
                }
                word.InsertText(loc);

                word.GotoBookMark("bh_flag");
                if(record.bh_flag == null)
                {
                    word.InsertText("无");
                }
                else
                {
                    if (record.bh_flag.Value == 1)
                    {
                        word.InsertText("有");
                    }
                    else
                    {
                        word.InsertText("无");
                    }
                }
                              

                word.GotoBookMark("bhr");
                word.InsertText(record.bhr_name);

                word.GotoBookMark("bhr_unit");
                word.InsertText(record.bhr_unit_name);

                word.GotoBookMark("bhr_pos");
                word.InsertText(record.bhr_pos);

                //保护方式
                if (record.bh_function != null && record.bh_function.Length>0 && record.bh_function.Substring(0, 1).Equals("专"))
                {
                    word.GotoBookMark("bh_function_flag1");
                    word.replaceText("□", "☑");
                }
                else if (record.bh_function != null && record.bh_function.Length > 0 && record.bh_function.Substring(0, 1).Equals("设"))
                {
                    word.GotoBookMark("bh_function_flag2");
                    word.replaceText("□", "☑");
                }
                else if (record.bh_function != null && record.bh_function.Length > 0 && record.bh_function.Substring(0, 1).Equals("封"))
                {
                    word.GotoBookMark("bh_function_flag3");
                    word.replaceText("□", "☑");
                    word.GotoBookMark("bhr_function");
                    word.InsertText(record.bh_function);
                }
                //现场情况
                if (record.xc_info != null && record.xc_info.Length > 1 && record.xc_info.Substring(0, 2).Equals("原始"))
                {
                    word.GotoBookMark("xc_info_flag1");
                    word.replaceText("□", "☑");
                }
                else if (record.xc_info != null && record.xc_info.Length > 1 && record.xc_info.Substring(0, 2).Equals("变动"))
                {
                    word.GotoBookMark("xc_info_flag2");
                    word.replaceText("□", "☑");
                }
                //变动原因
                if (record.bd_reason != null && record.bd_reason.Length > 1 && record.bd_reason.Substring(0, 2).Equals("事主"))
                {
                    word.GotoBookMark("bd_reason_flag1");
                    word.replaceText("□", "☑");
                }
                else if (record.bd_reason != null && record.bd_reason.Length > 1 && record.bd_reason.Substring(0, 2).Equals("报案"))
                {
                    word.GotoBookMark("bd_reason_flag2");
                    word.replaceText("□", "☑");
                }
                else if (record.bd_reason != null && !record.bd_reason.Equals(""))
                {
                    word.GotoBookMark("bd_reason_flag3");
                    word.replaceText("□", "☑");
                    word.GotoBookMark("bd_reason");
                    word.InsertText(record.bd_reason);
                }

                //天气
                switch (record.weather_info)
                {
                    case "阴":
                        word.GotoBookMark("tq_flag1");
                        word.replaceText("□", "☑");
                        break;
                    case "晴":
                        word.GotoBookMark("tq_flag2");
                        word.replaceText("□", "☑");
                        break;
                    case "雨":
                        word.GotoBookMark("tq_flag3");
                        word.replaceText("□", "☑");
                        break;
                    case "雪":
                        word.GotoBookMark("tq_flag4");
                        word.replaceText("□", "☑");
                        break;
                    case "雾":
                        word.GotoBookMark("tq_flag5");
                        word.replaceText("□", "☑");
                        break;
                }
                word.GotoBookMark("wd");
                word.InsertText(record.temper_info);

                word.GotoBookMark("sd");
                word.InsertText(record.humidity_info);

                word.GotoBookMark("fx");
                word.InsertText(record.trend_info);

                //光照
                if (record.light_info != null && record.light_info.Contains("自然光"))
                {
                    word.GotoBookMark("light_flag1");
                    word.replaceText("□", "☑");
                }
                if (record.light_info != null && record.light_info.Contains("灯光"))
                {
                    word.GotoBookMark("light_flag2");
                    word.replaceText("□", "☑");
                }
                if (record.light_info != null && record.light_info.Contains("特种光"))
                {
                    word.GotoBookMark("light_flag3");
                    word.replaceText("□", "☑");
                }

                word.GotoBookMark("zhr");
                word.InsertText(record.zhr_name);

                word.GotoBookMark("zhr_unit");
                word.InsertText(record.zhr_unit_name);

                word.GotoBookMark("zhr_pos");
                word.InsertText(record.zhr_pos);

                word.GotoBookMark("xc_disp");
                /*
                string xc_disp = record.xc_disp;
                for (int k = xc_disp.Length; k < 900; k++)
                {
                    xc_disp += " ";
                }*/
                word.InsertText(record.xc_disp);

                word.GotoBookMark("kyyear3");
                word.InsertText(record.ky_date.Value.Year.ToString());

                word.GotoBookMark("ky_no3");
                word.InsertText(record.record_no.Substring(record.record_no.Count() - 5));

                word.GotoBookMark("blr");
                string blr = record.blr_name;
                if (blr == null)
                {
                    blr = "";
                }
                for (int k = blr.Length; k < 70; k++)
                {
                    blr += " ";
                }
                word.InsertText(blr);

                word.GotoBookMark("ztr");
                string ztr = record.ztr_name;
                if (ztr == null)
                {
                    ztr = "";
                }
                for (int k = ztr.Length; k < 70; k++)
                {
                    ztr += " ";
                }
                word.InsertText(ztr);

                word.GotoBookMark("zxr");
                string zxr = record.zxr_name;
                if (zxr == null)
                {
                    zxr = "";
                }
                for (int k = zxr.Length; k < 70; k++)
                {
                    zxr += " ";
                }
                word.InsertText(zxr);

                word.GotoBookMark("lxr");
                string lxr = record.lxr_name;
                if (lxr == null)
                {
                    lxr = "";
                }
                for (int k = lxr.Length; k < 70; k++)
                {
                    lxr += " ";
                }
                word.InsertText(lxr);

                word.GotoBookMark("lyr");
                string lyr = record.lyr_name;
                if (lyr == null)
                {
                    lyr = "";
                }
                for (int k = lyr.Length; k < 70; k++)
                {
                    lyr += " ";
                }
                word.InsertText(lyr);

                word.GotoBookMark("bg_date1_year");
                word.InsertText(UtilsTool.NumtoUpper(DateTime.Now.Year%10));
                word.GotoBookMark("bg_date1_month");
                word.InsertText(UtilsTool.MonthtoUpper(DateTime.Now.Month));
                word.GotoBookMark("bg_date1_day");
                word.InsertText(UtilsTool.DaytoUpper(DateTime.Now.Day));

                word.GotoBookMark("bg_date2_year");
                word.InsertText(UtilsTool.NumtoUpper(DateTime.Now.Year % 10));
                word.GotoBookMark("bg_date2_month");
                word.InsertText(UtilsTool.MonthtoUpper(DateTime.Now.Month));
                word.GotoBookMark("bg_date2_day");
                word.InsertText(UtilsTool.DaytoUpper(DateTime.Now.Day));

                word.GotoBookMark("photos");
                
                int i = 1;
                var pics = db.sys_upload_file.Where(m => m.rec_uuid.Equals(record.uuid));
                var ztpics = pics.Where(m => m.file_type == 2004);
                int ztnum = ztpics.Count();
                word.GotoBookMark("zt_num");
                word.InsertText(""+ ztnum);

                foreach (sys_upload_file pic in ztpics)
                {
                    if(ValidateFileType(pic.file_url))
                    {
                        word.GotoBookMark("photo" + (i++).ToString());
                        word.InsertLineBreak(14);
                        word.InsertPicture(Server.MapPath("~/App_Data/") + "blank.png", 10, 10);//矫正后续图片位置
                        string bgFilePath = Server.MapPath("~/App_Data/") + "xct_bg.jpg";
                        Bitmap bmp = new Bitmap(bgFilePath);
                        Graphics g = Graphics.FromImage(bmp);
                        Font titlefont = new Font("宋体", 65);
                        Font contentfont = new Font("宋体", 40);
                        SolidBrush sbrush = new SolidBrush(Color.Black);
                        string title = record.record_title;

                        g.DrawString(title, titlefont, sbrush, new PointF(500, 200));
                        g.DrawString(record.xc_loc, contentfont, sbrush, new PointF(2600, 1800));
                        g.DrawString(record.bj_time.ToString(), contentfont, sbrush, new PointF(2600, 1900));
                        g.DrawString(record.bg_unit_name, contentfont, sbrush, new PointF(2600, 2030));
                        g.DrawString(record.ztr_name, contentfont, sbrush, new PointF(2600, 2150));
                        g.DrawString(DateTime.Now.ToString(), contentfont, sbrush, new PointF(2600, 2270));

                        System.Drawing.Image mapImg = System.Drawing.Image.FromFile(path + record.uuid + "\\" + pic.file_url);

                        g.DrawImage(mapImg, 100, 400, 2100, 1600);
                        //bmp.Save(path + record.uuid + "\\tmp_" + pic.file_url);

                        System.Drawing.Image img = UtilsTool.RotateImg(bmp, -90);  //旋转90度
                        RectSize target = UtilsTool.getCTSize(img);
                        img.Save(path + record.uuid + "\\tmp_" + pic.file_url);
                        word.InsertPicture(path + record.uuid + "\\tmp_" + pic.file_url, target.width, target.height);
                        word.InsertLineBreak();
                        //word.SetAlignment("Center");
                        //word.InsertText(pic.file_hint);
                        //word.InsertPagebreak();
                    }
                }
                                
                var fwpics = pics.Where(m => m.file_type == 2001 || m.file_type == 2002 || m.file_type == 2003).OrderBy(m => m.file_type);
                int photonum = fwpics.Count();
                word.GotoBookMark("photo_num");
                word.InsertText("" + photonum);

                foreach (sys_upload_file pic in fwpics)
                {
                    if (ValidateFileType(pic.file_url))
                    {
                        word.GotoBookMark("photo" + (i++).ToString());
                        word.InsertLineBreak(6);
                        word.InsertPicture(Server.MapPath("~/App_Data/") + "blank.png", 10, 10);//矫正后续图片位置
                        System.Drawing.Image img = System.Drawing.Image.FromFile(path + record.uuid + "\\" + pic.file_url);
                        RectSize target = UtilsTool.getConfSize(img);
                        word.InsertPicture(path + record.uuid + "\\" + pic.file_url, target.width, target.height);
                        word.InsertLineBreak();
                        word.SetAlignment("Center");
                        word.InsertText(pic.file_hint);
                    }
                }

                string newFile = path + record.record_no + ".doc";
                word.SaveAs(newFile);
                word.Close();
                word.Quit();
                
                return Redirect(val1["apiUrl"] + "GetWordFile?fileName=" + record.record_no + ".doc");
               
            }
            catch (Exception exc)
            {
                throw (exc);
            }

            //return UIHelper.Result();
        }

        private readonly static List<string> vALID_FILE_TYPES = new List<string> { "jpg", "bmp", "gif", "jpeg", "png", "tif" };

        protected static List<string> GetVALID_FILE_TYPES()
        {
            return vALID_FILE_TYPES;
        }

        protected static new bool ValidateFileType(string fileName)
        {
            string fileType = String.Empty;
            int lastDotIndex = fileName.LastIndexOf(".");
            if (lastDotIndex >= 0)
            {
                fileType = fileName.Substring(lastDotIndex + 1).ToLower();
            }

            if (GetVALID_FILE_TYPES().Contains(fileType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult btnEdit_Click([Bind(Include = "record_id,bg_unit,ky_unit,ky_date,bj_time,bjr,bjr_sex," +
            "bjr_idcard,bjr_phoneNo,kyks_time,kyjs_time,dest_unit,xz,fs_loc,xc_loc,xc_locpt,weather_info,trend_info,temper_info," +
            "humidity_info,light_info,bh_flag,bhr_name,bhr_unit_name,bhr_pos,bh_function,xc_info,bd_reason,jzr,jzr_sex,jzr_birth,jzr_address,zhr_name,zhr_unit_name,zhr_pos,bhr_name,blr_name,ztr_name,zxr_name,lxr_name,lyr_name,xc_disp,record_reason")] pbs_record record)
        {

            if (ModelState.IsValid)
            {
                record.record_title = record.fs_loc + record.xc_loc + "-" + record.xz;
                
                //UIHelper.HtmlEditor("HtmlEditor1");
                db.pbs_record.Attach(record);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(record);
                stateEntity.SetModifiedProperty("record_title");
                stateEntity.SetModifiedProperty("bg_unit");
                stateEntity.SetModifiedProperty("ky_unit");
                stateEntity.SetModifiedProperty("ky_date");
                stateEntity.SetModifiedProperty("bj_time");
                stateEntity.SetModifiedProperty("bjr");
                stateEntity.SetModifiedProperty("bjr_sex");
                stateEntity.SetModifiedProperty("bjr_idcard");
                stateEntity.SetModifiedProperty("bjr_phoneNo");
                stateEntity.SetModifiedProperty("kyks_time");
                stateEntity.SetModifiedProperty("kyjs_time");
                stateEntity.SetModifiedProperty("dest_unit");
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
                stateEntity.SetModifiedProperty("bhr_name");
                stateEntity.SetModifiedProperty("bhr_unit_name");
                stateEntity.SetModifiedProperty("bhr_pos");
                stateEntity.SetModifiedProperty("bh_function");
                stateEntity.SetModifiedProperty("xc_info");
                stateEntity.SetModifiedProperty("bd_reason");
                stateEntity.SetModifiedProperty("jzr");
                stateEntity.SetModifiedProperty("jzr_sex");
                stateEntity.SetModifiedProperty("jzr_birth");
                stateEntity.SetModifiedProperty("jzr_address");
                stateEntity.SetModifiedProperty("zhr_name");
                stateEntity.SetModifiedProperty("zhr_unit_name");
                stateEntity.SetModifiedProperty("zhr_pos");
                stateEntity.SetModifiedProperty("blr_name");
                stateEntity.SetModifiedProperty("ztr_name");
                stateEntity.SetModifiedProperty("zxr_name");
                stateEntity.SetModifiedProperty("lxr_name");
                stateEntity.SetModifiedProperty("lyr_name");
                stateEntity.SetModifiedProperty("xc_disp");
                stateEntity.SetModifiedProperty("record_reason");
                db.SaveChanges();

                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult btnEditSubmit_Click([Bind(Include = "record_id,bg_unit,ky_unit,ky_date,bj_time,bjr,bjr_sex,bjr_idcard,bjr_phoneNo,kyks_time,kyjs_time,dest_unit,xz,fs_loc,xc_loc,xc_locpt,weather_info,trend_info,temper_info,humidity_info,light_info,bh_flag,bhr_name,bhr_unit_name,bhr_pos,bh_function,xc_info,bd_reason,jzr,jzr_sex,jzr_birth,jzr_address,zhr_name,zhr_unit_name,zhr_pos,bhr_name,blr_name,ztr_name,zxr_name,lxr_name,lyr_name,xc_disp,record_reason")] pbs_record record)
        {

            if (ModelState.IsValid)
            {
                record.record_state = 1002;
                record.record_title = record.fs_loc + record.xc_loc + "-" + record.xz;
                db.pbs_record.Attach(record);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(record);
                stateEntity.SetModifiedProperty("record_title");
                stateEntity.SetModifiedProperty("bg_unit");
                stateEntity.SetModifiedProperty("ky_unit");
                stateEntity.SetModifiedProperty("ky_date");
                stateEntity.SetModifiedProperty("bj_time");
                stateEntity.SetModifiedProperty("bjr");
                stateEntity.SetModifiedProperty("bjr_sex");
                stateEntity.SetModifiedProperty("bjr_idcard");
                stateEntity.SetModifiedProperty("bjr_phoneNo");
                stateEntity.SetModifiedProperty("kyks_time");
                stateEntity.SetModifiedProperty("kyjs_time");
                stateEntity.SetModifiedProperty("dest_unit");
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
                stateEntity.SetModifiedProperty("bhr_name");
                stateEntity.SetModifiedProperty("bhr_unit_name");
                stateEntity.SetModifiedProperty("bhr_pos");
                stateEntity.SetModifiedProperty("bh_function");
                stateEntity.SetModifiedProperty("xc_info");
                stateEntity.SetModifiedProperty("bd_reason");
                stateEntity.SetModifiedProperty("jzr");
                stateEntity.SetModifiedProperty("jzr_sex");
                stateEntity.SetModifiedProperty("jzr_birth");
                stateEntity.SetModifiedProperty("jzr_address");
                stateEntity.SetModifiedProperty("zhr_name");
                stateEntity.SetModifiedProperty("zhr_unit_name");
                stateEntity.SetModifiedProperty("zhr_pos");
                stateEntity.SetModifiedProperty("blr_name");
                stateEntity.SetModifiedProperty("ztr_name");
                stateEntity.SetModifiedProperty("zxr_name");
                stateEntity.SetModifiedProperty("lxr_name");
                stateEntity.SetModifiedProperty("lyr_name");
                stateEntity.SetModifiedProperty("record_state");
                stateEntity.SetModifiedProperty("xc_disp");
                stateEntity.SetModifiedProperty("record_reason");
                db.SaveChanges();

                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Grid1_Delete(int userId, int unitId, int dataType,JArray selectedRows, JArray Grid1_fields)
        {
            foreach (string rowId in selectedRows)
            {
                pbs_record record = db.pbs_record.Find(Convert.ToInt32(rowId));
                record.record_state = 1000;
                db.pbs_record.Attach(record);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(record);
                stateEntity.SetModifiedProperty("record_state");
                db.SaveChanges();
                //db.pbs_record.Remove(record);
            }

            db.SaveChanges();
            gbQC.unitId = unitId;
            gbQC.userId = userId;
            gbQC.dataType = dataType;
            UpdateGrid(Grid1_fields);

            return UIHelper.Result();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

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
using System.Collections;
using System.IO;
using System.Web.Security;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Configuration;

namespace FineUIMvc.QuickStart.Controllers
{
    public class UploadFilesController : BaseController
    {
        private pbsEntities db = new pbsEntities();        
        // GET: UploadFiles
        public ActionResult Index(string type,int modifyFlag,string uuid)
        {
            if (uuid==null || uuid.Equals("") || type == null || type .Equals(""))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.uuid = uuid;
            ViewBag.type = type;
            ViewBag.EnableModify = modifyFlag == 0 ? false : true;
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int unitId = Global.loginInfo.unit_id;
            loadData(Global.loginInfo.user_id, unitId, Global.ALL_DATA, true);

            NameValueCollection val1 = (NameValueCollection)ConfigurationManager.GetSection("myUrl");
            ViewBag.apiUrl = val1["apiUrl"] + "GetFile?fileName=[name]&uuid="+uuid;
            if(type.Equals("zt"))
            {
                return View(db.sys_upload_file.Where(m => m.rec_uuid.Equals(uuid) && m.file_type == 2004).ToList());
            }
            else
            {
                return View(db.sys_upload_file.Where(m => m.rec_uuid.Equals(uuid) && m.file_type != 2004).ToList());
            }
            //var fils = db.sys_upload_file as IQueryable<sys_upload_file>;
            //return View(fils.Where(m => m.rec_uuid.Equals(uuid)).ToList());
        }

        // GET: UploadFiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sys_upload_file sys_upload_file = db.sys_upload_file.Find(id);
            if (sys_upload_file == null)
            {
                return HttpNotFound();
            }
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int unitId = Global.loginInfo.unit_id;
            int userId = Global.loginInfo.user_id;
            ViewBag.UserList = GetUserList(userId,unitId,Global.ALL_DATA,true);
            return View(sys_upload_file);
        }

        /// <summary>
        /// 更新表格数据
        /// </summary>
        /// <param name="Grid1_fields"></param>
        private void UpdateGrid(JArray Grid1_fields, string uuid,string type)
        {
            var fils = db.sys_upload_file as IQueryable<sys_upload_file>;
            List<sys_upload_file> dbsour = new List<sys_upload_file>();
            dbsour = fils.Where(m => m.rec_uuid.Equals(uuid)).ToList();
            if (type.Equals("zt"))
            {
                dbsour = dbsour.Where(m => m.file_type == 2004).ToList();
            }
            else
            {
                dbsour = dbsour.Where(m => m.file_type != 2004).ToList();
            }
            UIHelper.Grid("Grid1").DataSource(dbsour, Grid1_fields);
        }
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WindowFile_Close(JArray Grid1_fields)
        {
            UpdateGrid(Grid1_fields);
            
            return UIHelper.Result();
        }
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WindowFile_Close2(JArray Grid1_fields,string uuid,string type)
        {
            if (uuid == null || uuid.Equals(""))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UpdateGrid(Grid1_fields,uuid,type);

            return UIHelper.Result();
        }

        // GET: UploadFiles/Create
        public ActionResult Create(string uuid)
        {
            if(uuid==null || uuid.Equals( ""))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.uuid = uuid;
            return View();
        }

        // GET: UploadFiles/Create
        public ActionResult CreateZT(string uuid)
        {
            if (uuid == null || uuid.Equals(""))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.uuid = uuid;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnCreate_Click([Bind(Include = "file_url,file_name,file_hint,file_type,rec_uuid")] sys_upload_file upFiles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var filePhoto = UIHelper.FileUpload("filePhoto");

                    if (upFiles.file_url == null)
                    {
                        filePhoto.MarkInvalid("请先上传图片！");
                        ShowNotify("请先上传图片！");
                    }
                    else
                    {
                        string path1 = Server.MapPath("~/pbs_data/tmp/") + upFiles.file_url;
                        string path2 = Server.MapPath("~/pbs_data/" + upFiles.rec_uuid + "/");
                        if (!Directory.Exists(path2))
                        {
                            Directory.CreateDirectory(path2);
                        }
                        System.IO.File.Copy(path1, path2 + upFiles.file_url);

                        upFiles.file_url = upFiles.file_url;
                        upFiles.file_upload_time = DateTime.Now;
                        upFiles.file_uploader = Global.loginInfo.user_id;

                        filePhoto.Reset();
                        db.sys_upload_file.Add(upFiles);
                        db.SaveChanges();

                        // 关闭本窗体（触发窗体的关闭事件）
                        PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
                    }

                }

                return UIHelper.Result();
            }catch(Exception e)
            {
                throw e;
            }
        }
        /*
        [HttpPost]
        [Route("UploadFiles/upload")]
        public ActionResult upload(string uuid)
        {
            if (uuid == null || uuid.Equals(""))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HttpPostedFileBase file = Request.Files["file"];
            if (file != null)
            {
                if(!ValidateFileType(file.FileName))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.UnsupportedMediaType);
                }
                string fileName = Path.GetFileName(file.FileName);
                fileName = fileName.Replace(":", "_").Replace(" ", "_").Replace("\\", "_").Replace("/", "_");
                fileName = DateTime.Now.Ticks.ToString() + "_" + fileName;
                string filePath = Path.Combine(HttpContext.Server.MapPath("~/App_Data/"), fileName);
                file.SaveAs(filePath);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
         * */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult filePhoto_FileSelected(FormCollection values)
        {
            var filePhoto = UIHelper.FileUpload("filePhoto");

            if (filePhoto.HasFile)
            {
                string fileName = filePhoto.ShortFileName;

                if (!ValidateFileType(fileName))
                {
                    ShowNotify("无效的文件类型！");
                }
                else
                {
                    fileName = DateTime.Now.Ticks.ToString() +Path.GetExtension(fileName);

                    filePhoto.SaveAs(Server.MapPath("~/pbs_data/tmp/" + fileName));
                    NameValueCollection val1 = (NameValueCollection)ConfigurationManager.GetSection("myUrl");
                    UIHelper.Image("photoPrev").ImageUrl(val1["apiUrl"] + "GetFile?fileName=" + fileName + "&uuid=tmp");
                    UIHelper.HiddenField("file_url").Text(fileName);
                    // 清空文件上传组件（上传后要记着清空，否则点击提交表单时会再次上传！！）
                    filePhoto.Reset();
                }
            }

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Grid1_RowClick(string file_url,string uuid)
        {
            if (file_url == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NameValueCollection val1 = (NameValueCollection)ConfigurationManager.GetSection("myUrl");
            UIHelper.Image("ImagePreView").ImageUrl(val1["apiUrl"] + "GetFile?fileName="+file_url+"&uuid="+uuid);
            //UIHelper.Window("WindowPreView").Show();

            return UIHelper.Result();
        }

        // GET: UploadFiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sys_upload_file sys_upload_file = db.sys_upload_file.Find(id);
            if (sys_upload_file == null)
            {
                return HttpNotFound();
            }
            /*
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int unitId = Global.loginInfo.unit_id;
            ViewBag.UserList = GetUserList(unitId);
             * */
            return View(sys_upload_file);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnEdit_Click([Bind(Include = "file_id,file_name,file_hint,file_type,rec_uuid")] sys_upload_file upFiles)
        {

            if (ModelState.IsValid)
            {
                upFiles.file_uploader = Global.loginInfo.user_id;
                db.sys_upload_file.Attach(upFiles);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(upFiles);
                stateEntity.SetModifiedProperty("file_name");
                stateEntity.SetModifiedProperty("file_hint");
                stateEntity.SetModifiedProperty("file_type");
                
                db.SaveChanges();

                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }
            //return RedirectToAction("Index",new Parameter("uuid",upFiles.rec_uuid));
            return UIHelper.Result();
        }

        
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Grid1_Delete(JArray selectedRows, JArray Grid1_fields,string uuid,string type)
        {
            foreach (string rowId in selectedRows)
            {
                sys_upload_file file = db.sys_upload_file.Find(Convert.ToInt32(rowId));
                //string path = Server.MapPath("~/pbs_data/" + file.rec_uuid + "/") + file.file_url;
                //System.IO.File.Delete(path);
                db.sys_upload_file.Remove(file);
            }
            db.SaveChanges();

            UpdateGrid(Grid1_fields,uuid,type);

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

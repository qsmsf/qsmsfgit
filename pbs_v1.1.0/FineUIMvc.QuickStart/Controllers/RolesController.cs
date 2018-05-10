using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FineUIMvc.QuickStart.Models;
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Infrastructure;

namespace FineUIMvc.QuickStart.Controllers
{
    public class RolesController : BaseController
    {
        private pbsEntities db = new pbsEntities();

        // GET: Roles
        public ActionResult Index()
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int unitId = Global.loginInfo.unit_id;
            string userId = Global.loginInfo.user_id.ToString();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic = getButtonPriv(userId, "role");
            ViewBag.showDeleteBtn = false;
            ViewBag.showAddBtn = false;
            ViewBag.showEditBtn = false;
            if (dic.ContainsKey("deleteRole")&& userId.Equals("1")) //仅admin用户可删除
            {
                ViewBag.showDeleteBtn = true;
            }
            if (dic.ContainsKey("addRole"))
            {
                ViewBag.showAddBtn = true;
            }
            if (dic.ContainsKey("editRole"))
            {
                ViewBag.showEditBtn = true;
            }
            return View(db.sys_role.ToList());
        }

        // GET: Roles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sys_role sys_role = db.sys_role.Find(id);
            if (sys_role == null)
            {
                return HttpNotFound();
            }
            return View(sys_role);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        private void UpdateGrid(JArray Grid1_fields)
        {
            UIHelper.Grid("Grid1").DataSource(db.sys_role.ToList(), Grid1_fields);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Window1_Close(JArray Grid1_fields)
        {
            UpdateGrid(Grid1_fields);

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnCreate_Click([Bind(Include = "role_name,role_disp")] sys_role sys_role)
        {
            if (ModelState.IsValid)
            {
                sys_role.create_time = DateTime.Now;
                sys_role.creator_id = Global.loginInfo.user_id;
                db.sys_role.Add(sys_role);
                
                db.SaveChanges();

                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }

       
        // GET: Roles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sys_role sys_role = db.sys_role.Find(id);
            if (sys_role == null)
            {
                return HttpNotFound();
            }
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            string userId = Global.loginInfo.user_id.ToString();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic = getButtonPriv(userId, "priv");
            ViewBag.showPrivBtn = false;
            if (dic.ContainsKey("editPriv"))
            {
                ViewBag.showPrivBtn = true;
            }
            return View(sys_role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnEdit_Click([Bind(Include = "role_id,role_name,role_disp")] sys_role role)
        {
            if (ModelState.IsValid)
            {
                db.sys_role.Attach(role);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(role);
                stateEntity.SetModifiedProperty("role_name");
                stateEntity.SetModifiedProperty("role_disp");
                db.SaveChanges();

                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sys_role sys_role = db.sys_role.Find(id);
            if (sys_role == null)
            {
                return HttpNotFound();
            }
            return View(sys_role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Grid1_Delete(JArray selectedRows, JArray Grid1_fields)
        {
            foreach (string rowId in selectedRows)
            {
                sys_role role = db.sys_role.Find(Convert.ToInt32(rowId));
                db.sys_role.Remove(role);
            }
            db.SaveChanges();

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

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
using System.Web.Security;
using Newtonsoft.Json;

namespace FineUIMvc.QuickStart.Controllers
{
    public class UnitsController : BaseController
    {
        private pbsEntities db = new pbsEntities();

        // GET: /Units/
        public ActionResult Index()
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int unitId = Global.loginInfo.unit_id;
            string userId = Global.loginInfo.user_id.ToString();
            loadData(Global.loginInfo.user_id,unitId,Global.ALL_DATA,true);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic = getButtonPriv(userId, "unit");
            ViewBag.showDeleteBtn = false;
            ViewBag.showAddBtn = false;
            ViewBag.showEditBtn = false;
            if (dic.ContainsKey("deleteUnit") && userId.Equals("1")) //仅admin用户可删除
            {
                ViewBag.showDeleteBtn = true;
            }
            if (dic.ContainsKey("addUnit"))
            {
                ViewBag.showAddBtn = true;
            }
            if (dic.ContainsKey("editUnit"))
            {
                ViewBag.showEditBtn = true;
            }
            return View(db.sys_unit.ToList());
        }
        
        // GET: /Units/Create
        public ActionResult Create()
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int unitId = Global.loginInfo.unit_id;
            ViewBag.UnitList = GetUnitList(unitId,Global.ALL_DATA,true);
            return View();
        }

        /// <summary>
        /// 更新表格数据
        /// </summary>
        /// <param name="Grid1_fields"></param>
        private void UpdateGrid(JArray Grid1_fields)
        {
            UIHelper.Grid("Grid1").DataSource(db.sys_unit.ToList(), Grid1_fields);
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
        public ActionResult btnCreate_Click([Bind(Include = "unit_name,is_top,father_unit_id")] sys_unit unit)
        {
            if (ModelState.IsValid)
            {                
                db.sys_unit.Add(unit);
                db.SaveChanges();

                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnEdit_Click([Bind(Include = "unit_id,unit_name,is_top,father_unit_id")] sys_unit unit)
        {

            if (ModelState.IsValid)
            {
                db.sys_unit.Attach(unit);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(unit);
                stateEntity.SetModifiedProperty("unit_name");
                stateEntity.SetModifiedProperty("is_top");
                stateEntity.SetModifiedProperty("father_unit_id");
                db.SaveChanges();

                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }

        // GET: /Units/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sys_unit sys_unit = db.sys_unit.Find(id);
            if (sys_unit == null)
            {
                return HttpNotFound();
            }
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int unitId = Global.loginInfo.unit_id;
            ViewBag.UnitList = GetUnitList(unitId, Global.ALL_DATA, true);
            return View(sys_unit);
        }


        // GET: /Units/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sys_unit sys_unit = db.sys_unit.Find(id);
            if (sys_unit == null)
            {
                return HttpNotFound();
            }
            return View(sys_unit);
        }

        // POST: /Units/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sys_unit sys_unit = db.sys_unit.Find(id);
            db.sys_unit.Remove(sys_unit);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Grid1_Delete(JArray selectedRows, JArray Grid1_fields)
        {
            foreach (string rowId in selectedRows)
            {
                sys_unit unit = db.sys_unit.Find(Convert.ToInt32(rowId));
                db.sys_unit.Remove(unit);
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

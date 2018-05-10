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
using System.Web.Security;
using Newtonsoft.Json;

namespace FineUIMvc.QuickStart.Controllers
{
    public class UsersController : BaseController
    {
        private pbsEntities db = new pbsEntities();

        // GET: /Users/
        public ActionResult Index()
        {
            try
            {
                var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
                int unitId = Global.loginInfo.unit_id;
                int userId = Global.loginInfo.user_id;
                ViewBag.userId = userId.ToString();
                ViewBag.unitId = unitId.ToString();
                loadData(Global.loginInfo.user_id, unitId, Global.ALL_DATA, true);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic = getButtonPriv(userId.ToString(), "user");
                ViewBag.showDeleteBtn = false;
                ViewBag.showAddBtn = false;
                ViewBag.showEditBtn = false;
                if (dic.ContainsKey("deleteUser"))
                {
                    ViewBag.showDeleteBtn = true;
                }
                if (dic.ContainsKey("addUser"))
                {
                    ViewBag.showAddBtn = true;
                }
                if (dic.ContainsKey("editUser"))
                {
                    ViewBag.showEditBtn = true;
                }
                dic = getDataPriv(userId.ToString());
                
                int dataType = 0;
                if (dic.ContainsKey("all"))
                {
                    dataType = Global.ALL_DATA;
                }
                else if (dic.ContainsKey("family"))
                {
                    dataType = Global.FAMILY_DATA;
                }
                else if (dic.ContainsKey("unit"))
                {
                    dataType = Global.UNIT_DATA;
                }
                else
                {
                    dataType = Global.SELF_DATA;
                }
                ViewBag.dataType = dataType.ToString();
                return View(getUserInfoList(dataType,userId,unitId));
            }catch (Exception e)
            {
                throw(e);
            }
        }

        List<sys_user> getUserInfoList(int dataType, int userId, int unitId)
        {
            List<sys_user> userList = new List<sys_user>();
            switch (dataType)
            {
                case Global.SELF_DATA:
                    userList.Add(db.sys_user.Find(userId));
                    break;
                case Global.UNIT_DATA:
                    userList = db.sys_user.Where(m => m.unit_id == unitId).ToList();
                    break;
                case Global.FAMILY_DATA:
                    var unitList = db.showChildUnits(unitId).ToList();
                    List<int> unitIdList = new List<int>();
                    foreach(sys_unit unit in unitList)
                    {
                        unitIdList.Add(unit.unit_id);
                    }

                    userList = db.sys_user.Where(m => unitIdList.Contains(m.unit_id.Value)).ToList();
                    break;
                case Global.ALL_DATA:
                    userList = db.sys_user.ToList();
                    break;
            }
            return userList;
        }
        // GET: /Users/Create
        public ActionResult Create()
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int unitId = Global.loginInfo.unit_id;
            int userId = Global.loginInfo.user_id;
            Dictionary<string, string> dic = new Dictionary<string, string>(); 
            dic = getDataPriv(userId.ToString());
            if (dic.ContainsKey("all"))
            {
                ViewBag.UnitList = GetUnitList(unitId,Global.ALL_DATA,false);
            }
            else if (dic.ContainsKey("family"))
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.FAMILY_DATA, false);
            }
            else if (dic.ContainsKey("unit"))
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.UNIT_DATA, false);
            }
            else
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.SELF_DATA, false);
            }
            
            return View();
        }

        /// <summary>
        /// 更新表格数据
        /// </summary>
        /// <param name="Grid1_fields"></param>
        private void UpdateGrid(JArray Grid1_fields,int dataType ,int userId, int unitId)
        {
            UIHelper.Grid("Grid1").DataSource(getUserInfoList(dataType, userId, unitId), Grid1_fields);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Window1_Close(JArray Grid1_fields,int dataType,int userId,int unitId)
        {
            UpdateGrid(Grid1_fields,dataType,userId,unitId);

            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnCreate_Click([Bind(Include = "user_name,user_fullname,user_pwd,user_state,unit_id,user_zw")] sys_user user)
        {
            if (ModelState.IsValid)
            {
                user.create_time = DateTime.Now;
                user.user_pwd = Md5Helper.Encrypt(user.user_pwd);
                db.sys_user.Add(user);
                db.SaveChanges();
                sys_userrole userRole = new sys_userrole();
                userRole.user_id = user.user_id;
                userRole.role_id = 2; //默认为普通用户
                userRole.create_time = DateTime.Now;
                db.sys_userrole.Add(userRole);
                db.SaveChanges();
                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }

        // GET: /Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sys_user sys_user = db.sys_user.Find(id);
            if (sys_user == null)
            {
                return HttpNotFound();
            }
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int unitId = Global.loginInfo.unit_id;
            int userId = Global.loginInfo.user_id;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic = getDataPriv(userId.ToString());
            if (dic.ContainsKey("all"))
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.ALL_DATA, false);
            }
            else if (dic.ContainsKey("family"))
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.FAMILY_DATA, false);
            }
            else if (dic.ContainsKey("unit"))
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.UNIT_DATA, false);
            }
            else
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.SELF_DATA, false);
            }
            dic = getButtonPriv(userId.ToString(), "priv");
            ViewBag.showRoleBtn = false;
            ViewBag.showPrivBtn = false;
            if (dic.ContainsKey("editPriv"))
            {
                ViewBag.showPrivBtn = true;
            }
            if (dic.ContainsKey("editUserRole"))
            {
                ViewBag.showRoleBtn = true;
            }
            return View(sys_user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnEdit_Click([Bind(Include = "user_id,user_name,user_fullname,user_state,unit_id,user_zw")] sys_user user)
        {
            
            if (ModelState.IsValid)
            {
                db.sys_user.Attach(user);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(user);
                stateEntity.SetModifiedProperty("user_name");
                stateEntity.SetModifiedProperty("user_fullname");
                stateEntity.SetModifiedProperty("user_state");
                stateEntity.SetModifiedProperty("unit_id");
                stateEntity.SetModifiedProperty("user_zw");

                db.SaveChanges();

                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }
        // GET: /Users/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Grid1_Delete(JArray selectedRows, JArray Grid1_fields,int dataType,int userId,int unitId)
        {
            foreach (string rowId in selectedRows)
            {
                sys_user user = db.sys_user.Find(Convert.ToInt32(rowId));
                user.user_state = 0;
                db.sys_user.Attach(user);
                var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(user);
                stateEntity.SetModifiedProperty("user_state");
                db.SaveChanges();
            }            

            UpdateGrid(Grid1_fields,dataType,userId,unitId);

            return UIHelper.Result();
        }

        public ActionResult ChangePwd(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.userId = id.Value.ToString();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnChangePwd_Click(int userId, string oPwd,string newPwd)
        {
            if (ModelState.IsValid)
            {
                var user = db.sys_user.Find(userId);
                Alert alert = new Alert();
                alert.Title = "修改密码";
                alert.Target = Target.Top; 
                if (user == null)
                {
                    alert.MessageBoxIcon = MessageBoxIcon.Error;
                    alert.Message = "用户不存在";
                    alert.Show();
                } else if (!user.user_pwd.Equals(Md5Helper.Encrypt(oPwd))){
                    alert.MessageBoxIcon = MessageBoxIcon.Error;
                    alert.Message = "当前密码输入错误！";
                    alert.Show();
                } else {
                    user.user_pwd = Md5Helper.Encrypt(newPwd);
                    db.sys_user.Attach(user);
                    var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(user);
                    stateEntity.SetModifiedProperty("user_pwd");

                    db.SaveChanges();
                    alert.Message = "密码修改成功";
                    alert.MessageBoxIcon = MessageBoxIcon.Success;
                    alert.Show();
                    // 关闭本窗体（触发窗体的关闭事件）
                    PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
                }
            }

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

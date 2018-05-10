using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FineUIMvc.QuickStart.Models;
using Newtonsoft.Json.Linq;

namespace FineUIMvc.QuickStart.Controllers
{
    public class UserRoleController : Controller
    {
        private pbsEntities db = new pbsEntities();
        protected class RoleInfo
        {
            public int role_id { get; set; }
            public string role_name { get; set; }
            public string role_disp { get; set; }
        }
        // GET: sys_userrole
        public async Task<ActionResult> Index(int? id)
        {
            ViewBag.userId = id.ToString();
            var ds1 = await db.view_userhasrole.Where(m => m.user_id == id).ToListAsync();
            ViewBag.ds1 = ds1;

            string sql = "select t1.role_id ,t1.role_name,t1.role_disp from sys_role t1 ";
            sql += " left join sys_userrole t2 on t1.role_id = t2.role_id and t2.user_id = "+id.ToString();
            sql += " where t2.sid is null ";
            var ds2 = await db.Database.SqlQuery<RoleInfo>(sql).ToListAsync();
            ViewBag.ds2 = ds2;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRole(JArray selectedRows,string userId,JArray Grid1_fields,JArray Grid2_fields)
        {
            foreach (string rowId in selectedRows)
            {
                sys_userrole tmp = db.sys_userrole.Find(Convert.ToInt32(rowId));
                db.sys_userrole.Remove(tmp);
            }
            db.SaveChanges();
            UpdateGrid(Grid1_fields, Grid2_fields, userId);
            //PageContext.RegisterStartupScript(ActiveWindow.HideRefresh());
            return UIHelper.Result();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(JArray selectedRows, string userId, JArray Grid1_fields, JArray Grid2_fields)
        {
            foreach (string rowId in selectedRows)
            {
                sys_userrole tmp = new sys_userrole();
                tmp.user_id = Convert.ToInt32(userId);
                tmp.role_id = Convert.ToInt32(rowId);
                tmp.create_time = DateTime.Now;
                db.sys_userrole.Add(tmp);
            }
            db.SaveChanges();
            UpdateGrid(Grid1_fields, Grid2_fields, userId);
            return UIHelper.Result();
        }

        private void UpdateGrid(JArray Grid1_fields, JArray Grid2_fields,string userId)
        {
            int id = Convert.ToInt32(userId);
            var ds1 = db.view_userhasrole.Where(m => m.user_id ==id ).ToList();

            string sql = "select t1.role_id ,t1.role_name,t1.role_disp from sys_role t1 ";
            sql += " left join sys_userrole t2 on t1.role_id = t2.role_id and t2.user_id = " + userId;
            sql += " where t2.sid is null ";
            var ds2 = db.Database.SqlQuery<RoleInfo>(sql).ToList();
            UIHelper.Grid("Grid1").DataSource(ds1, Grid1_fields);
            UIHelper.Grid("Grid2").DataSource(ds2, Grid2_fields);
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

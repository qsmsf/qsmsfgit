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
    public class PrivilegesController : Controller
    {
        private pbsEntities db = new pbsEntities();
        private pbsEntities dbForCond = new pbsEntities();
        public class PrivInfo
        {
            public int privilege_id { get; set; }
            public int type_id { get; set; }
            public string priv_name { get; set; }
            public string priv_disp { get; set; }
            public bool privilege_operation { get; set; }
        }
        // GET: Privileges
        public ActionResult Index(int tgtId,string type)
        {
            ViewBag.tgtId = tgtId.ToString();
            ViewBag.type = type;
            this.initPrivData(tgtId, type);
            
            ViewBag.ds1 = getMenuPrivs(tgtId,type);
            ViewBag.ds2 = getButtonPrivs(tgtId,type);
            ViewBag.ds3 = getDataPrivs(tgtId,type);
            return View();
        }
        private List<PrivInfo> getMenuPrivs(int tgtId, string type)
        {
            string sql = "select t2.privilege_id,t1.menu_id as type_id,t1.menu_name as priv_name,t1.menu_disp as priv_disp,t2.privilege_operation ";
            sql += " from sys_menu t1 left join privilege t2 on t2.privilege_master='" + type + "' ";
            sql += " and t2.privilege_value = " + tgtId.ToString();
            sql += " and t2.privilege_access='menu' and t1.menu_id = t2.privilege_access_value order by type_id";
            return db.Database.SqlQuery<PrivInfo>(sql).ToList();
        }
        private List<PrivInfo> getButtonPrivs(int tgtId, string type)
        {
            string sql = "select t2.privilege_id,t1.btn_id as type_id,t1.btn_name as priv_name,t1.btn_disp as priv_disp  ,t2.privilege_operation";
            sql += " from sys_button t1 left join privilege t2 on t2.privilege_master='" + type + "' ";
            sql += " and t2.privilege_value = " + tgtId.ToString();
            sql += " and t2.privilege_access='button' and t1.btn_id = t2.privilege_access_value order by type_id";
            return db.Database.SqlQuery<PrivInfo>(sql).ToList();
        }

        private List<PrivInfo> getDataPrivs(int tgtId, string type)
        {
            string sql = "select t2.privilege_id,t1.sid as type_id,t1.data_name as priv_name,t1.data_disp as priv_disp  ,t2.privilege_operation";
            sql += " from sys_data t1 left join privilege t2 on t2.privilege_master='" + type + "' ";
            sql += " and t2.privilege_value = " + tgtId.ToString();
            sql += " and t2.privilege_access='data' and t1.sid = t2.privilege_access_value order by type_id";
            return db.Database.SqlQuery<PrivInfo>(sql).ToList();
        }
        private void initPrivData(int tgtId,string type)
        {
            try
            {
                var menuList = db.sys_menu;
                foreach (sys_menu menu in menuList)
                {
                    if (dbForCond.privilege.Count(m => m.privilege_master.Equals(type) && m.privilege_value == tgtId 
                        && m.privilege_access.Equals("menu") && m.privilege_access_value == menu.menu_id) == 0){
                            privilege priv = new privilege();
                            priv.privilege_master = type;
                            priv.privilege_value = tgtId;
                            priv.privilege_access = "menu";
                            priv.privilege_access_value = menu.menu_id;
                            priv.privilege_operation = false;
                            db.privilege.Add(priv);
                    }
                }
                db.SaveChanges();
                var buttonList = db.sys_button;
                foreach (sys_button button in buttonList)
                {
                    if (dbForCond.privilege.Count(m => m.privilege_master.Equals(type) && m.privilege_value == tgtId
                        && m.privilege_access.Equals("button") && m.privilege_access_value == button.btn_id) == 0)
                    {
                        privilege priv = new privilege();
                        priv.privilege_master = type;
                        priv.privilege_value = tgtId;
                        priv.privilege_access = "button";
                        priv.privilege_access_value = button.btn_id;
                        priv.privilege_operation = false;
                        db.privilege.Add(priv);
                    }
                }
                db.SaveChanges();
                var dataList = db.sys_data;
                foreach (sys_data data in dataList)
                {
                    if (dbForCond.privilege.Count(m => m.privilege_master.Equals(type) && m.privilege_value == tgtId
                        && m.privilege_access.Equals("data") && m.privilege_access_value == data.sid) == 0)
                    {
                        privilege priv = new privilege();
                        priv.privilege_master = type;
                        priv.privilege_value = tgtId;
                        priv.privilege_access = "data";
                        priv.privilege_access_value = data.sid;
                        priv.privilege_operation = false;
                        db.privilege.Add(priv);
                    }
                }
                db.SaveChanges();
            }catch(Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyPriv(int tgtId, string type, JArray Grid1Data, JArray Grid2Data, JArray Grid3Data, JArray Grid_fields)
        {
            try
            {
                foreach(JObject tmp in Grid2Data)
                {
                    Grid1Data.Add(tmp);
                }
                foreach(JObject tmp in Grid3Data)
                {
                    Grid1Data.Add(tmp);
                }
                foreach (JObject modifiedRow in Grid1Data)
                {
                    JObject values = modifiedRow.Value<JObject>("values");
                    string status = values.Value<string>("privilege_operation.status");
                    int rowId = Convert.ToInt32(modifiedRow.Value<string>("id"));

                    if (status == "modified")
                    {
                        var priv = db.privilege.Find(rowId);

                        priv.privilege_operation = !priv.privilege_operation;
                        db.Entry(priv).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
                //UIHelper.Grid("Grid1").DataSource(getMenuPrivs(tgtId,type), Grid_fields);
                //UIHelper.Grid("Grid2").DataSource(getButtonPrivs(tgtId, type), Grid_fields);
                //UIHelper.Grid("Grid3").DataSource(getDataPrivs(tgtId, type), Grid_fields);
                return UIHelper.Result();
            }catch(Exception e)
            {
                throw e;
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
    }
}

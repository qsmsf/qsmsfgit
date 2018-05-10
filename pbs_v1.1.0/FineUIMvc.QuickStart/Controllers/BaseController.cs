using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using FineUIMvc.QuickStart.Models;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.IO;
using FineUIMvc.QuickStart.Code;
using System.Web.Security;
using Newtonsoft.Json;

namespace FineUIMvc.QuickStart.Controllers
{
    
    public class BaseController : Controller
    {
        private pbsEntities dbbase = new pbsEntities();
        log4net.ILog log = log4net.LogManager.GetLogger("BaseController");
        private class MenuInfo
        {
            public string menu_name { get; set; }
            public string menu_url { get; set; }
            public string menu_disp { get; set; }
        }
        protected class BtnInfo
        {
            public string btn_name { get; set; }
            public string btn_disp { get; set; }
        }
        protected class DataInfo
        {
            public string data_name { get; set; }
            public string data_disp { get; set; }
        }
        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        public virtual void ShowNotify(string message)
        {
            ShowNotify(message, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon)
        {
            ShowNotify(message, messageIcon, Target.Top);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        /// <param name="target"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon, Target target)
        {
            Notify n = new Notify();
            n.Target = target;
            n.Message = message;
            n.MessageBoxIcon = messageIcon;
            n.PositionX = Position.Center;
            n.PositionY = Position.Top;
            n.DisplayMilliseconds = 3000;
            n.ShowHeader = false;

            n.Show();
        }
        #region static readonly


        #endregion

        #region OnActionExecuting

        /// <summary>
        /// 动作方法调用之前执行
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }


        #endregion
        #region 上传文件类型判断

        protected readonly static List<string> VALID_FILE_TYPES = new List<string> { "jpg", "bmp", "gif", "jpeg", "png","tif" };

        protected static bool ValidateFileType(string fileName)
        {
            string fileType = String.Empty;
            int lastDotIndex = fileName.LastIndexOf(".");
            if (lastDotIndex >= 0)
            {
                fileType = fileName.Substring(lastDotIndex + 1).ToLower();
            }

            if (VALID_FILE_TYPES.Contains(fileType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        /// <summary>
        /// 获取网址的完整路径
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public string GetAbsoluteUrl(string virtualPath)
        {
            // http://benjii.me/2015/05/get-the-absolute-uri-from-asp-net-mvc-content-or-action/
            var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri)
            {
                Path = Url.Content(virtualPath),
                Query = null,
            };

            return urlBuilder.ToString();
        }


        /// <summary>
        /// 加载XML文件
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        protected XmlDocument LoadXml(string xmlPath)
        {
            // 加载XML配置文件
            xmlPath = Server.MapPath(xmlPath);
            string xmlContent = String.Empty;
            using (StreamReader sr = new StreamReader(xmlPath))
            {
                xmlContent = sr.ReadToEnd();
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);

            return doc;
        }


        public int getLoginInfo(string userName, string userPwd, ref Global.LoginInfo info)
        {
            userPwd = Md5Helper.Encrypt(userPwd);
            var loginInfo = dbbase.login_info.Where(m => m.user_name.Equals(userName) && m.user_pwd.Equals(userPwd)) ;
            if (loginInfo.Count() > 0)
            {
                info.user_id = loginInfo.First().user_id;
                info.user_fullname = loginInfo.First().user_fullname;
                info.user_pwd = loginInfo.First().user_pwd;
                info.unit_id = (int)loginInfo.First().unit_id;
                info.unit_name = loginInfo.First().unit_name;
                info.user_zw = loginInfo.First().user_zw;
            }
            return loginInfo.Count();
        }
        public ListItem[] GetUnitList(int myUnitId ,int dataType,bool includeZero,bool valueENameFlag = false)
        {
            List<sys_unit>Units = new List<sys_unit>();
            switch (dataType)
            {
                case Global.SELF_DATA:
                    Units.Add(dbbase.sys_unit.Find(myUnitId));
                    break;
                case Global.UNIT_DATA:
                    Units.Add(dbbase.sys_unit.Find(myUnitId));
                    break;
                case Global.FAMILY_DATA:
                    //var tmp = dbbase.sp_showChildUnits(myUnitId).ToList();
                    Units = dbbase.showChildUnits(myUnitId).ToList();
                    //Units = dbbase.sys_unit.Where(m => m.father_unit_id == myUnitId || m.unit_id == myUnitId).ToList();
                    break;
                case Global.ALL_DATA:
                    Units = dbbase.sys_unit.ToList();
                    break;
            }
            
            var items = new List<ListItem>();
            if (includeZero)
            {
                if (valueENameFlag)
                {
                    items.Add(new ListItem
                    {
                        Text = "无",
                        Value = "无"
                    });
                }
                else
                {
                    items.Add(new ListItem
                    {
                        Text = "无",
                        Value = "0"
                    });
                }
            }
            foreach (sys_unit unit in Units)
            {
                if (valueENameFlag)
                {
                    items.Add(new ListItem
                    {
                        Text = unit.unit_name,
                        Value = unit.unit_name
                    });
                }
                else
                {
                    items.Add(new ListItem
                    {
                        Text = unit.unit_name,
                        Value = unit.unit_id.ToString()
                    });
                }
            }
            return items.ToArray();
        }
        public ListItem[] GetUserList(int myUserId, int myUnitId,int dataType,bool includeZero,bool valueENameFlag = false)
        {
            List<login_info> Users = new List<login_info>();
            switch (dataType)
            {
                case Global.SELF_DATA:
                    Users.Add(dbbase.login_info.Find(myUserId));
                    break;
                case Global.UNIT_DATA:
                    Users = dbbase.login_info.Where(m => m.unit_id == myUnitId).ToList();
                    break;
                case Global.FAMILY_DATA:
                    Users = dbbase.login_info.Where(m => m.father_unit_id == myUnitId || m.unit_id == myUnitId).ToList();
                    break;
                case Global.ALL_DATA:
                    Users = dbbase.login_info.ToList();
                    break;
            }
            var items = new List<ListItem>();
            if (includeZero)
            {
                if (valueENameFlag)
                {
                    items.Add(new ListItem
                    {
                        Text = "无",
                        Value = "无"
                    });
                }
                else
                {
                    items.Add(new ListItem
                    {
                        Text = "无",
                        Value = "0"
                    });
                }
            }
            foreach (login_info user in Users)
            {
                if (valueENameFlag)
                {
                    items.Add(new ListItem
                    {
                        Text = user.user_fullname,
                        Value = user.user_fullname
                    });
                }
                else
                {
                    items.Add(new ListItem
                    {
                        Text = user.user_fullname,
                        Value = user.user_id.ToString()
                    });
                }
            }
            return items.ToArray();
        }
        public string genericUnitString(int myUnitId,int dataType,bool includeZero)
        {
            JArray units = new JArray();

            var items = GetUnitList(myUnitId,dataType,includeZero);
            foreach(ListItem item in items)
            {
                units.Add(new JArray(item.Value, item.Text));
            }
            return String.Format("window._Units={0};", units.ToString(Newtonsoft.Json.Formatting.None));
        }

        public string genericUserString(int myUserId,int myUnitId, int dataType, bool includeZero)
        {
            JArray users = new JArray();

            var items = GetUserList(myUserId,myUnitId,dataType,includeZero);
            foreach (ListItem item in items)
            {
                users.Add(new JArray(item.Value, item.Text));
            }
            return String.Format("window._Users={0};", users.ToString(Newtonsoft.Json.Formatting.None));
        }

        public void loadData(int myUserId, int myUnitId, int dataType, bool includeZero)
        {
            ViewBag.StartupScript = genericUnitString(myUnitId, dataType, includeZero) + genericUserString(myUserId, myUnitId, dataType, includeZero);
        }

        public void loadMenus()
        {
            string sql = "";
            try
            {
                List<TreeNode> nodes = new List<TreeNode>();
                var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
                string userId = Global.loginInfo.user_id.ToString();
                sql = "select DISTINCT(t3.menu_name),t3.menu_url,t3.menu_disp from privilege t1 ";
                sql += " left join sys_menu t3 on t1.privilege_access_value = t3.menu_id ";
                sql += " where(( t1.privilege_master = 'role'  and t1.privilege_value in ";
                sql += " (select t2.role_id from sys_userrole t2 where t2.user_id = " + userId + ") ) ";
                sql += " or ( t1.privilege_master = 'user' and t1.privilege_value = " + userId + ") )and t1.privilege_access = 'menu' ";
                sql += " and t1.privilege_operation = 1 order by t3.menu_name; ";
                var menuList = dbbase.Database.SqlQuery<MenuInfo>(sql);

                foreach (MenuInfo mi in menuList)
                {
                    TreeNode node = new TreeNode();
                    node.Text = mi.menu_disp;
                    node.NavigateUrl = mi.menu_url;
                    nodes.Add(node);
                }

                ViewBag.menuNodes = nodes.ToArray();
            }catch (Exception e)
            {
                log.Fatal("sql = " + sql);
                throw e;
            }
        }

        public Dictionary<string,string> getButtonPriv(string userId,string buttonClass)
        {   
            string sql = "";
            try
            {
                sql = "select DISTINCT(t3.btn_name),t3.btn_disp from privilege t1 ";
                sql += " left join sys_button t3 on t1.privilege_access_value = t3.btn_id ";
                sql += " where ( ( t1.privilege_master = 'role'  and t1.privilege_value in ";
                sql += " (select t2.role_id from sys_userrole t2 where t2.user_id = " + userId + ") ) ";
                sql += " or ( t1.privilege_master = 'user' and t1.privilege_value = " + userId + ") ) and t1.privilege_access = 'button' ";
                sql += " and t1.privilege_operation = 1 and t3.btn_class = '" + buttonClass + "'; ";
                var btnList = dbbase.Database.SqlQuery<BtnInfo>(sql);
                //btnList.ToDictionary(k => k.btn_name, k => k.btn_disp);
                return btnList.ToDictionary(k => k.btn_name, k => k.btn_disp);
            }
            catch (Exception e)
            {
                log.Fatal("sql = " + sql);
                throw e;
            }
        }
        public Dictionary<string, string> getDataPriv(string userId)
        {
            string sql = "select DISTINCT(t3.data_name),t3.data_disp from privilege t1 ";
            sql += " left join sys_data t3 on t1.privilege_access_value = t3.sid ";
            sql += " where ( ( t1.privilege_master = 'role'  and t1.privilege_value in ";
            sql += " (select t2.role_id from sys_userrole t2 where t2.user_id = " + userId + ") ) ";
            sql += " or ( t1.privilege_master = 'user' and t1.privilege_value = " + userId + ") ) and t1.privilege_access = 'data' ";
            sql += " and t1.privilege_operation = 1; ";
            var dataList = dbbase.Database.SqlQuery<DataInfo>(sql);
            //dataList.ToDictionary(k => k.data_name, k => k.data_disp);
            return dataList.ToDictionary(k => k.data_name, k => k.data_disp);
        }
    }
}
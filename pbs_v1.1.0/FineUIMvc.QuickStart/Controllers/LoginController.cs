using FineUIMvc.QuickStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FineUIMvc.QuickStart.Controllers
{
    public class LoginController : BaseController
    {
        
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnLogin_Click(string tbxUserName, string tbxPassword)
        {
            int cnt = getLoginInfo(tbxUserName, tbxPassword, ref Global.loginInfo);
            if (cnt > 0)
            {
                Authentication.SetCookie(Global.loginInfo.user_fullname, Global.loginInfo.user_pwd, Global.loginInfo);
                //FormsAuthentication.RedirectFromLoginPage(Global.loginInfo.user_fullname, false);
                return Redirect("~/Home/Index");
                //return RedirectToAction
            }
            else
            {
                ShowNotify("用户名或密码错误！", MessageBoxIcon.Error);
            }

            return UIHelper.Result();
        }
    }
}
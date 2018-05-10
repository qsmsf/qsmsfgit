using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FineUIMvc.QuickStart.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {           
            loadMenus();
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            ViewBag.userId = Global.loginInfo.user_id.ToString();
            return View();
        }

        public ActionResult Hello()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnHello_Click()
        {
            return UIHelper.Result();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult onSignOut_Click()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }



        // GET: Themes
        public ActionResult Themes()
        {
            return View();
        }
    }
}
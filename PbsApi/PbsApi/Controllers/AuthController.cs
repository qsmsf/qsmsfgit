using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using PbsApi.Models;

namespace PbsApi.Controllers
{
    public class AuthController : ApiController
    {
        private pbsEntities db = new pbsEntities();
        log4net.ILog log = log4net.LogManager.GetLogger("AuthController");
        private class LoginToken
        {
            public login_info member { get; set; }
            public int code { get; set; }
            public string message { get; set; }
            public string token{get;set;}
        }

        private class WxLoginToken
        {
            public wx_user member { get; set; }
            public int code { get; set; }
            public string message { get; set; }
            public string token { get; set; }
        }

        [Route("Auth/Test")]
        public string Test()
        {
           
            return "hah";
        }

        // GET: api/Auth/5
        [Route("Auth/Login")]
        [ResponseType(typeof(LoginToken))]
        public async Task<IHttpActionResult> Getlogin_info(string username, string password)
        {
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                String clientIP = context.Request.UserHostAddress;

                login_info info = await db.login_info.SingleOrDefaultAsync(m => m.user_name.Equals(username) && m.user_pwd.Equals(password));
                if (info != null)
                {
                    int userId = info.user_id;
                    string Token = Guid.NewGuid().ToString();
                    var dtNow = DateTime.Now;

                    ticketauth tka = await db.ticketauth.SingleOrDefaultAsync(m => m.user_id == userId && m.state == 1 && m.user_type == 1);
                    if (tka != null)
                    {
                        tka.state = 0;
                        db.ticketauth.Attach(tka);
                        var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(tka);
                        stateEntity.SetModifiedProperty("state");
                        //db.ticketauth.Remove(tka);
                    }
                    ticketauth tmp = new ticketauth();
                    tmp.user_id = userId;
                    tmp.token = Token;
                    tmp.create_time = dtNow;
                    tmp.expire_time = dtNow.AddYears(1);
                    tmp.state = 1;
                    tmp.user_type = 1;
                    tmp.client_ip = clientIP;
                    db.ticketauth.Add(tmp);
                    await db.SaveChangesAsync();

                    LoginToken res = new LoginToken();
                    res.code = 100;
                    res.member = info;
                    res.token = Token;
                    return Ok(res);
                }
                else
                {
                    LoginToken res = new LoginToken();
                    res.code = 101;
                    res.message = "错误的用户名或密码!";
                    return Ok(res);
                }
            }catch(Exception e)
            {
                LoginToken res = new LoginToken();
                log.Error(e.Message);
                res.code = 102;
                res.message = "未知错误:"+e.Message+"--"+e.InnerException.Message;
                return Ok(res);
            }
        }

        [Route("Auth/WxLogin")]
        [ResponseType(typeof(LoginToken))]
        public async Task<IHttpActionResult> GetWxLoginInfo(string openId)
        {
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                String clientIP = context.Request.UserHostAddress;
                string username = context.Request.Headers.Get("username");
                string userfullname = context.Request.Headers.Get("userfullname");
                string avatarUrl = context.Request.Headers.Get("avatarUrl");
                int userId = 0;
                wx_user info = await db.wx_user.SingleOrDefaultAsync(m => m.wx_id.Equals(openId));            
                if (info != null)
                {
                    userId = info.sid;
                }
                else
                {
                    info = new wx_user();
                    info.avatar_url = avatarUrl;
                    info.create_time = DateTime.Now;
                    info.wx_id = openId;
                    info.real_name = userfullname;
                    info.wx_name = username;
                    info.state = 0;

                    db.wx_user.Add(info);
                    await db.SaveChangesAsync();

                    userId = info.sid;
                }

            
                string Token = Guid.NewGuid().ToString();
                var dtNow = DateTime.Now;

                ticketauth tka = await db.ticketauth.SingleOrDefaultAsync(m => m.user_id == userId && m.state == 1 && m.user_type == 2);
                if (tka != null)
                {
                    tka.state = 0;
                    db.ticketauth.Attach(tka);
                    var stateEntity = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(tka);
                    stateEntity.SetModifiedProperty("state");
                }
                ticketauth tmp = new ticketauth();
                tmp.user_id = userId;
                tmp.token = Token;
                tmp.create_time = dtNow;
                tmp.expire_time = dtNow.AddYears(1);
                tmp.state = 1;
                tmp.user_type = 2;
                tmp.client_ip = clientIP;
                db.ticketauth.Add(tmp);
                await db.SaveChangesAsync();

                WxLoginToken res = new WxLoginToken();
                res.code = 100;
                res.member = info;
                res.token = Token;
                return Ok(res);
            }
            catch (Exception e)
            {
                WxLoginToken res = new WxLoginToken();
                log.Error(e.Message);
                res.code = 102;
                res.message = "未知错误:" + e.Message + "--" + e.InnerException.Message;
                return Ok(res);
            }
        }

        [Route("Auth/Logout")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Logoutlogin_info(string token)
        {
            ticketauth tka = await db.ticketauth.SingleAsync(m => m.token.Equals(token));
            if (tka != null)
            {
                db.ticketauth.Remove(tka);
                await db.SaveChangesAsync();
            }         

            return Ok();
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
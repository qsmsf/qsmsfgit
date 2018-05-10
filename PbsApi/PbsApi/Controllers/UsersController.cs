using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using PbsApi.Models;
using PbsApi.Auth;
using System.Collections;

namespace PbsApi.Controllers
{
    public class UsersController : ApiController
    {
        private pbsEntities db = new pbsEntities();

        private class UserInfoList
        {
            public int code { get; set; }
            public String msg { get; set; }

            public List<sys_user> resultList { get; set; }
        } 

        // GET: api/Users
        [Route("api/Users/GetUserList")]
        [AuthFilterOutside]
        [ResponseType(typeof(UserInfoList))]
        public IHttpActionResult Getsys_user(int myUnitId)
        {
            //var tmp = db.sys_user.Select(m => new UserItem { m.user_name, m.user_id });
            UserInfoList userList = new UserInfoList();
            userList.resultList = db.sys_user.Where(m => m.unit_id == myUnitId).ToList();
            userList.code = 100;
            return Ok(userList);
        }

        // PUT: api/Users/5
        [Route("api/Users/UpdateUserInfo")]
        [AuthFilterOutside]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putsys_user(int id, sys_user sys_user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sys_user.user_id)
            {
                return BadRequest();
            }

            db.Entry(sys_user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!sys_userExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [Route("api/Users/AddUserInfo")]
        [AuthFilterOutside]
        [ResponseType(typeof(sys_user))]
        public async Task<IHttpActionResult> Postsys_user(sys_user sys_user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.sys_user.Add(sys_user);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = sys_user.user_id }, sys_user);
        }
                
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool sys_userExists(int id)
        {
            return db.sys_user.Count(e => e.user_id == id) > 0;
        }
    }
}
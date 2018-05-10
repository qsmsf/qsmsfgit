using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using FineUIMvc.QuickStart.Models;

namespace FineUIMvc.QuickStart.Controllers
{
    public class ApiLoginController : ApiController
    {
        private pbsEntities db = new pbsEntities();

        // GET: api/ApiLogin
        public IQueryable<login_info> Getlogin_info()
        {
            return db.login_info;
        }

        // GET: api/ApiLogin/5
        [ResponseType(typeof(login_info))]
        public IHttpActionResult Getlogin_info(int id)
        {/*
            login_info login_info = db.login_info.Find(id);
            if (login_info == null)
            {
                return NotFound();
            }

            return Ok(login_info);
          */
            return Ok("888");
        }

        // PUT: api/ApiLogin/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putlogin_info(int id, login_info login_info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != login_info.user_id)
            {
                return BadRequest();
            }

            db.Entry(login_info).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!login_infoExists(id))
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

        // POST: api/ApiLogin
        [ResponseType(typeof(login_info))]
        public IHttpActionResult Postlogin_info(login_info login_info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.login_info.Add(login_info);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (login_infoExists(login_info.user_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = login_info.user_id }, login_info);
        }

        // DELETE: api/ApiLogin/5
        [ResponseType(typeof(login_info))]
        public IHttpActionResult Deletelogin_info(int id)
        {
            login_info login_info = db.login_info.Find(id);
            if (login_info == null)
            {
                return NotFound();
            }

            db.login_info.Remove(login_info);
            db.SaveChanges();

            return Ok(login_info);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool login_infoExists(int id)
        {
            return db.login_info.Count(e => e.user_id == id) > 0;
        }
    }
}
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

namespace PbsApi.Controllers
{
    public class UnitsController : ApiController
    {
        private pbsEntities db = new pbsEntities();

        private class UnitInfoList{
            public int code{get;set;}
            public String msg{get;set;}

            public List<sys_unit> resultList { get; set; }
        } 
        // GET: api/Units
        [Route("api/Unit/GetUnitList")]
        [AuthFilterOutside]
        [ResponseType(typeof(UnitInfoList))]
        public IHttpActionResult GetUnitList(int myUnitId)
        {
            UnitInfoList unitList = new UnitInfoList();
            unitList.resultList = db.sys_unit.Where(m => m.father_unit_id == myUnitId || m.unit_id == myUnitId).ToList();
            unitList.code = 100;
            
            return Ok(unitList);
        }

        // GET: api/Units/5
        [Route("api/Unit/GetUnitInfo")]
        [AuthFilterOutside]
        [ResponseType(typeof(sys_unit))]
        public async Task<IHttpActionResult> Getsys_unit(int id)
        {
            sys_unit sys_unit = await db.sys_unit.FindAsync(id);
            if (sys_unit == null)
            {
                return NotFound();
            }

            return Ok(sys_unit);
        }

        // PUT: api/Units/5
        [Route("api/Unit/UpdateUnitInfo")]
        [AuthFilterOutside]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putsys_unit(int id, sys_unit sys_unit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sys_unit.unit_id)
            {
                return BadRequest();
            }

            db.Entry(sys_unit).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!sys_unitExists(id))
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

        // POST: api/Units
        [Route("api/Unit/AddUnitInfo")]
        [AuthFilterOutside]
        [ResponseType(typeof(sys_unit))]
        public async Task<IHttpActionResult> Postsys_unit(sys_unit sys_unit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.sys_unit.Add(sys_unit);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = sys_unit.unit_id }, sys_unit);
        }        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool sys_unitExists(int id)
        {
            return db.sys_unit.Count(e => e.unit_id == id) > 0;
        }
    }
}
using FineUIMvc.QuickStart.Code;
using FineUIMvc.QuickStart.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FineUIMvc.QuickStart.Controllers
{
    [Authorize]
    public class ReportsController : BaseController
    {
        private pbsEntities db = new pbsEntities();
        public class ChartSeriesModel
        {
            public string name { get; set; }
            public string type { get; set; }
            public List<int> data { get; set; }
        }
        public class DataZoomModel
        {
            public string type { get; set; }
            public int end { get; set; }
            public int start { get; set; }
        }
        public class MakerPoint
        {
            public double Lng { get; set; }
            public double Lat { get; set; }
        }
        // GET: Reports
        public ActionResult Index()
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            Global.loginInfo = JsonConvert.DeserializeObject<Global.LoginInfo>(ticket.UserData);
            int userId = Global.loginInfo.user_id;
            int unitId = Global.loginInfo.unit_id;
            loadData(userId, unitId, Global.ALL_DATA, true);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic = getDataPriv(userId.ToString());
            int dataType = 0;
            if (dic.ContainsKey("all"))
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.ALL_DATA, true);
                ViewBag.UserList = GetUserList(userId, unitId, Global.ALL_DATA, true);
                dataType = Global.ALL_DATA;
            }
            else if (dic.ContainsKey("family"))
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.FAMILY_DATA, true);
                ViewBag.UserList = GetUserList(userId, unitId, Global.FAMILY_DATA, true);
            }
            else if (dic.ContainsKey("unit"))
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.UNIT_DATA, true);
                ViewBag.UserList = GetUserList(userId, unitId, Global.UNIT_DATA, true);
                dataType = Global.UNIT_DATA;
            }
            else
            {
                ViewBag.UnitList = GetUnitList(unitId, Global.SELF_DATA, true);
                ViewBag.UserList = GetUserList(userId, unitId, Global.SELF_DATA, true);
                dataType = Global.SELF_DATA;
            }
            ViewBag.userId = userId.ToString();
            ViewBag.unitId = unitId.ToString();
            ViewBag.dataType = dataType.ToString();
            return View();
        }

        public ActionResult GetReportData(int type, int qKyUnit,int qCreator,string qKyDate1,string qKyDate2,int userId, int unitId, int dataType)
        {
            List<string> names = new List<string>();
            List<ChartSeriesModel> seriesList = new List<ChartSeriesModel>();
            List<int> nums = new List<int>();
            List<DataZoomModel> dzList = new List<DataZoomModel>();
            ChartSeriesModel csm = new ChartSeriesModel();
            string rsljson="";

            DateTime beginDate = Md5Helper.GMT2Local(qKyDate1);
            DateTime endDate = Md5Helper.GMT2Local(qKyDate2);

            var records = db.view_report.Where(p =>p.ky_date >= beginDate && p.ky_date <= endDate && p.record_state != 1000);   //
            if (dataType == Global.SELF_DATA)
            {
                records = records.Where(p => p.creater_id.Value == userId);
            }
            else if (qCreator != 0)
            {
                records = records.Where(p => p.creater_id == qCreator);
            }
            if (dataType == Global.UNIT_DATA)
            {
                records = records.Where(p => p.ky_unit == unitId);
            }
            else if (qKyUnit != 0)
            {
                records = records.Where(p => p.ky_unit == qKyUnit);
            }
            switch (type)
            {
                case 1:
                    var rslList = records.GroupBy(p => p.xz).ToList();
                    csm.name = "案件性质";
                    csm.type = "bar";
                    foreach (var rsl in rslList)
                    {
                        names.Add(rsl.Key);
                        nums.Add(rsl.Count());
                    }
                    csm.data = nums;
                    seriesList.Add(csm);
                    var option = new
                    {
                        title = new { text = "案件性质汇总统计" },
                        tooltip = new { },
                        xAxis = new
                        {
                            axisLabel = new { interval = "0", rotate = "30" },
                            data = names
                        },
                        yAxis = new { },
                        series = seriesList
                    };
                    rsljson = JsonConvert.SerializeObject(option);
                    break;
                case 2:
                    var rslList2 = records.GroupBy(p => p.bg_unit_name).ToList();
                    csm.name = "接警单位";
                    csm.type = "bar";
                    foreach (var rsl in rslList2)
                    {
                        names.Add(rsl.Key);
                        nums.Add(rsl.Count());
                    }
                    csm.data = nums;
                    seriesList.Add(csm);
                    var option2 = new
                    {
                        title = new { text = "接警单位汇总统计" },
                        tooltip = new { },
                        xAxis = new
                        {
                            axisLabel = new { interval = "0", rotate = "30" },
                            data = names
                        },
                        yAxis = new { },
                        series = seriesList
                    };
                    rsljson = JsonConvert.SerializeObject(option2);
                    break;
                case 3:
                    var rslList3 = records.GroupBy(p => p.jjr_name).ToList();
                    csm.name = "接警人";
                    csm.type = "bar";
                    foreach (var rsl in rslList3)
                    {
                        names.Add(rsl.Key);
                        nums.Add(rsl.Count());
                    }
                    csm.data = nums;
                    seriesList.Add(csm);
                    var option3 = new
                    {
                        title = new { text = "接警人汇总统计" },
                        tooltip = new { },
                        xAxis = new
                        {
                            axisLabel = new { interval = "0", rotate = "45" },
                            data = names
                        },
                        yAxis = new { },
                        series = seriesList
                    };
                    rsljson = JsonConvert.SerializeObject(option3);
                    break;
                case 4:
                    var rslList4 = records.OrderBy(p =>p.ky_date).GroupBy(p => p.ky_date).ToList();
                    csm.name = "接警日期";
                    csm.type = "line";
                    foreach (var rsl in rslList4)
                    {
                        names.Add(rsl.Key.Value.ToString("yyyy-MM-dd"));
                        nums.Add(rsl.Count());
                    }
                    csm.data = nums;
                    seriesList.Add(csm);

                    DataZoomModel dz = new DataZoomModel()
                    {
                        end = 100
                    };
                    dzList.Add(dz);
                    DataZoomModel dz1 = new DataZoomModel()
                    {
                        type = "inside"
                    };
                    dzList.Add(dz1);
                    var option4 = new
                    {
                        title = new { text = "日均案件统计" },
                        tooltip = new { },
                        xAxis = new
                        {                            
                            data = names
                        },
                        dataZoom = dzList,
                        yAxis = new { },
                        series = seriesList
                    };
                    rsljson = JsonConvert.SerializeObject(option4);
                    break;
                case 5:
                    var rslList5 = records.Select(n => new {record_id = n.record_id,ky_month=n.ky_date.Value.Year +"-"+n.ky_date.Value.Month })
                        .GroupBy(p => p.ky_month).ToList();
                    csm.name = "接警月";
                    csm.type = "line";
                    foreach (var rsl in rslList5)
                    {
                        names.Add(rsl.Key);
                        nums.Add(rsl.Count());
                    }
                    csm.data = nums;
                    seriesList.Add(csm);
                    DataZoomModel dz2 = new DataZoomModel()
                    {
                        end = 100
                    };
                    dzList.Add(dz2);
                    DataZoomModel dz3 = new DataZoomModel()
                    {
                        type = "inside"
                    };
                    dzList.Add(dz3);
                    var option5 = new
                    {
                        title = new { text = "月均案件统计" },
                        tooltip = new { },
                        xAxis = new
                        {
                            data = names
                        },
                        yAxis = new { },
                        dataZoom = dzList,
                        series = seriesList
                    };
                    rsljson = JsonConvert.SerializeObject(option5);
                    break;
            }

            return Content(rsljson);
        }

        public ActionResult MapReport(int qKyUnit, int qCreator, string qKyDate1, string qKyDate2, int userId, int unitId, int dataType)
        {
            ViewBag.qKyUnit = qKyUnit;
            ViewBag.qCreator = qCreator;
            ViewBag.qKyDate1 = qKyDate1;
            ViewBag.qKyDate2 = qKyDate2;
            ViewBag.userId = userId;
            ViewBag.unitId = unitId;
            ViewBag.dataType = dataType;
            
            return View();
        }

        public ActionResult GetMapData(int qKyUnit, int qCreator, string qKyDate1, string qKyDate2, int userId, int unitId, int dataType)
        {
            string rsljson = "";
            
            DateTime beginDate = Md5Helper.GMT2Local(qKyDate1);
            DateTime endDate = Md5Helper.GMT2Local(qKyDate2);

            var records = db.view_report.Where(p => p.ky_date >= beginDate && p.ky_date <= endDate && p.record_state != 1000);   //
            if (dataType == Global.SELF_DATA)
            {
                records = records.Where(p => p.creater_id.Value == userId);
            }
            else if (qCreator != 0)
            {
                records = records.Where(p => p.creater_id == qCreator);
            }
            if (dataType == Global.UNIT_DATA)
            {
                records = records.Where(p => p.ky_unit == unitId);
            }
            else if (qKyUnit != 0)
            {
                records = records.Where(p => p.ky_unit == qKyUnit);
            }
            var rslList = records.GroupBy(p => p.xz).ToList();
            
            
            //rsljson = JsonConvert.SerializeObject();
            return Content(rsljson);
        }
    }
}
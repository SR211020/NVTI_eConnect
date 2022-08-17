using eConnectV2.Extensions;
using eConnectV2.Services;
using eConnectV2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace eConnectV2.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ISaviorService _saviorService;

        public AttendanceController(IAttendanceService attendanceService, ISaviorService saviorService)
        {
            _attendanceService = attendanceService;
            _saviorService = saviorService;
        }

        #region IT Dashboard
        public IActionResult ITDashboard()
        {
            var model = new SaviorViewModel();
            var data = new AttendanceViewModel();
            DataTable dt = _attendanceService.usp_Attendance(new AttendanceViewModel { Activity = "IT_DASHBOARD" });

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dt.Rows)
                {
                    string strEmpCode = Convert.ToString(dataRow["EMPCODE"]).Trim();
                    string strEmpName = Convert.ToString(dataRow["EMPNAME"]).Trim();
                    string strContactNo = Convert.ToString(dataRow["CONTACTNO_O"]).Trim();
                    string strCategory = Convert.ToString(dataRow["CATEGORY"]).Trim();
                    byte[] bEmpImage = Convert.ToString(dataRow["PHOTO"]) != "" ? (byte[])(dataRow["PHOTO"]) : null;

                    Int32 iStatus = 0;
                    model.Activity = "GET_ATTENDENCE";
                    model.EmpCode = strEmpCode;
                    DataSet ds = _saviorService.usp_Reports(model);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        iStatus = Convert.ToInt32(ds.Tables[0].Rows[0]["STATUS"]);

                        if (iStatus == 1)
                        {
                            data.EmployeeList.Add(new AttendanceViewModel()
                            {
                                EmpCode = strEmpCode,
                                EmpName = strEmpName,
                                ContactNo = strContactNo,
                                Category = strCategory,
                                ImageName = bEmpImage != null ? "data:image/jpeg;base64," + Convert.ToBase64String(bEmpImage, 0, bEmpImage.Length) : ""
                            });
                        }
                    }
                }
            }
            return View(data);
        }
        public JsonResult ITBirthday()
        {
            var data = new List<AttendanceViewModel>();
            DataTable dt = _attendanceService.usp_Attendance(new AttendanceViewModel { Activity = "IT_BIRTHDAY" });
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dt.Rows)
                {
                    if (Convert.ToString(dataRow["PHOTO"]) != "")
                    {
                        byte[] bEmpImage = (byte[])(dataRow["PHOTO"]);
                        data.Add(new AttendanceViewModel()
                        {
                            EmpName = Convert.ToString(dataRow["EMPNAME"]),
                            ImageName = "data:image/jpeg;base64," + Convert.ToBase64String(bEmpImage, 0, bEmpImage.Length)
                        });
                    }
                }
            }
            return Json(data);
        }
         #endregion

        #region Production Dashboard
        public IActionResult ProdDashboard()
        {
            var model = new SaviorViewModel();
            var data = new AttendanceViewModel();
            DataTable dt = _attendanceService.usp_Attendance(new AttendanceViewModel { Activity = "PROD_DASHBOARD" });

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dt.Rows)
                {
                    string strEmpCode = Convert.ToString(dataRow["EMPCODE"]).Trim();
                    string strEmpName = Convert.ToString(dataRow["EMPNAME"]).Trim();
                    string strContactNo = Convert.ToString(dataRow["CONTACTNO_O"]).Trim();
                    byte[] bEmpImage = Convert.ToString(dataRow["PHOTO"]) != "" ? (byte[])(dataRow["PHOTO"]) : null;

                    Int32 iStatus = 0;

                    model.Activity = "GET_ATTENDENCE";
                    model.EmpCode = strEmpCode;
                    DataSet ds = _saviorService.usp_Reports(model);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        iStatus = Convert.ToInt32(ds.Tables[0].Rows[0]["STATUS"]);

                        if (iStatus == 1)
                        {
                            data.EmployeeList.Add(new AttendanceViewModel()
                            {
                                EmpCode = strEmpCode,
                                EmpName = strEmpName,
                                ContactNo = strContactNo,
                                ImageName = bEmpImage != null ? "data:image/jpeg;base64," + Convert.ToBase64String(bEmpImage, 0, bEmpImage.Length) : ""
                            });
                        }
                    }
                }
            }
            return View(data);
        }
        #endregion

        #region Fire Marshal Dashboard
        public IActionResult FireMarshal()
        {
            var model = new SaviorViewModel();
            var data = new AttendanceViewModel();
            DataTable dt = _attendanceService.usp_Attendance(new AttendanceViewModel { Activity = "FIRE_MARSHAL_DASHBOARD" });

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dt.Rows)
                {
                    string strEmpCode = Convert.ToString(dataRow["EMPCODE"]).Trim();
                    string strEmpName = Convert.ToString(dataRow["EMPNAME"]).Trim();
                    string strDeptName = Convert.ToString(dataRow["DEPTNAME"]).Trim();
                    string strContactNo = Convert.ToString(dataRow["CONTACTNO_O"]).Trim();
                    byte[] bEmpImage = Convert.ToString(dataRow["PHOTO"]) != "" ? (byte[])(dataRow["PHOTO"]) : null;

                    Int32 iStatus = 0;

                    model.Activity = "GET_ATTENDENCE";
                    model.EmpCode = strEmpCode;
                    DataSet ds = _saviorService.usp_Reports(model);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        iStatus = Convert.ToInt32(ds.Tables[0].Rows[0]["STATUS"]);

                        if (iStatus == 1)
                        {
                            data.EmployeeList.Add(new AttendanceViewModel()
                            {
                                EmpCode = strEmpCode,
                                EmpName = strEmpName,
                                Department = strDeptName,
                                ContactNo = strContactNo,
                                ImageName = bEmpImage != null ? "data:image/jpeg;base64," + Convert.ToBase64String(bEmpImage, 0, bEmpImage.Length) : ""
                            });
                        }
                    }
                }
            }
            return View(data);
        }
        #endregion

        #region My Team
        public JsonResult MyTeam()
        {
            var model = new SaviorViewModel();
            var data = new List<SaviorViewModel>();

            DataTable dt = _attendanceService.usp_Attendance(new AttendanceViewModel { Activity = "GET_TEAM_ATTENDANCE", Department = User.Identity.GetDeptCode() });

            foreach (DataRow dataRow in dt.Rows)
            {
                string strEmpCode = Convert.ToString(dataRow["EMPCODE"]).Trim();
                string strINPunch = "";
                string strOutPunch = "";

                model.Activity = "GET_PUNCH_TIME";
                model.EmpCode = strEmpCode;
                DataSet ds = _saviorService.usp_Reports(model);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    strINPunch = Convert.ToString(ds.Tables[0].Rows[0]["IN_PUNCH"]).Trim();
                    strOutPunch = Convert.ToString(ds.Tables[0].Rows[0]["OUT_PUNCH"]).Trim();
                }

                byte[] bEmpImage = Convert.ToString(dataRow["PHOTO"]) != "" ? (byte[])(dataRow["PHOTO"]) : null;

                data.Add(new SaviorViewModel()
                {
                    EmpCode = strEmpCode,
                    EmpName = Convert.ToString(dataRow["EMPNAME"]).Trim(),
                    ContactNo = Convert.ToString(dataRow["CONTACTNO_O"]).Trim(),
                    LandLineNo = Convert.ToString(dataRow["LANDLINE"]).Trim(),
                    DocName = bEmpImage != null ? "data:image/jpeg;base64," + Convert.ToBase64String(bEmpImage, 0, bEmpImage.Length) : "",
                    InPunch = strINPunch,
                    OutPunch = strOutPunch
                });
            }
            return Json(data);
        }
        #endregion

        #region My Attendance
        public IActionResult MyAttendance()
        {
            return View();
        }
        public JsonResult GetAttendance()
        {
            var model = new SaviorViewModel();
            var data = new List<SaviorViewModel>();

            model.Activity = "GET_ATTENDANCE_CALENDER";
            model.EmpCode = User.Identity.GetEmpCode();
            DataSet ds = _saviorService.usp_Reports(model);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dataRow in ds.Tables[0].Rows)
                {
                    data.Add(new SaviorViewModel()
                    {
                        EmpCode = Convert.ToString(dataRow["EMPCODE"]),
                        Date1 = Convert.ToString(dataRow["PUNCH_DATE"]),
                        Param1 = Convert.ToString(dataRow["IN_PUNCH"]).Trim(),
                        InPunch = Convert.ToString(dataRow["IN_TIME"]).Trim(),
                        Param2 = Convert.ToString(dataRow["OUT_PUNCH"]).Trim(),
                        OutPunch = Convert.ToString(dataRow["OUT_TIME"]).Trim(),
                    });
                }
            }
            return Json(data);
        }
        #endregion

        

    }
}

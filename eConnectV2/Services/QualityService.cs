using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace eConnectV2.Services
{
    public interface IQualityService
    {
        DataTable usp_DCN(QualityViewModel model);
        DataTable usp_Deviation(QualityViewModel model);
        DataTable usp_Calibration(QualityViewModel model);
        DataTable usp_SampleScrap(QualityViewModel model);
        DataTable usp_LAB(LabTestViewModel model);

    }

    public class QualityService : IQualityService
    {
        public ConnectionStrings _connectionStrings;
        private SqlConnection Con { get; set; }
        private SqlCommand Cmd { get; set; }
        public QualityService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }
        public DataTable usp_DCN(QualityViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_DCN", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@REQNO", model.RequestNo);
                Cmd.Parameters.AddWithValue("@EMPCODE", model.EmpCode);
                Cmd.Parameters.AddWithValue("@DEPT", model.DeptCode);
                Cmd.Parameters.AddWithValue("@REQTYPE", model.Type);
                Cmd.Parameters.AddWithValue("@REASON", model.Reason);
                Cmd.Parameters.AddWithValue("@DOCNO", model.DocNo);
                Cmd.Parameters.AddWithValue("@DOCNAME", model.DocName);
                Cmd.Parameters.AddWithValue("@DOCCHANGES", model.DocChanges);
                Cmd.Parameters.AddWithValue("@CHANGE_REQUIRE_IN", model.ChangeRequireIn);
                Cmd.Parameters.AddWithValue("@CHANGEDOCNO", model.ChangeDocNo);
                Cmd.Parameters.AddWithValue("@CHANGEDOCNAME", model.ChangeDocName);
                Cmd.Parameters.AddWithValue("@DCCDOCNO", model.DCCDocNo);
                Cmd.Parameters.AddWithValue("@DCCDOCNAME", model.DCCDocName);
                Cmd.Parameters.AddWithValue("@DCCREQTYPE", model.DCCReqType);
                Cmd.Parameters.AddWithValue("@DCCSTATUS", model.DCCStatus);
                Cmd.Parameters.AddWithValue("@REMARKS", model.Remarks);
                Cmd.Parameters.AddWithValue("@FILENAME", model.File_Name);
                Cmd.Parameters.AddWithValue("@STATUS", model.Status);
                Cmd.Parameters.AddWithValue("@STATUS_DESCR", model.StatusDescr);
                Cmd.Parameters.AddWithValue("@USERID", model.UserID);
                Cmd.Parameters.AddWithValue("@APPRID", model.ApprID);
                Cmd.Parameters.AddWithValue("@APPRTYPE", model.ApprType);
                Cmd.Parameters.AddWithValue("@HOD", model.DeptHOD);

                Cmd.Parameters.AddWithValue("@DATE1", model.Date1);
                Cmd.Parameters.AddWithValue("@DATE2", model.Date2);
                Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);


                SqlDataAdapter da = new(Cmd);
                DataTable dt = new();
                da.Fill(dt);
                return dt;
            }
            finally
            {
                Con.Close();
                Cmd.Dispose();
            }
        }

        public DataTable usp_Deviation(QualityViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_Deviation", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@REQNO", model.RequestNo);
                Cmd.Parameters.AddWithValue("@EMPCODE", model.EmpCode);
                Cmd.Parameters.AddWithValue("@DEPT", model.DeptCode);
                Cmd.Parameters.AddWithValue("@CONTACT", model.ContactNo);

                Cmd.Parameters.AddWithValue("@PLANT", model.Plant);
                Cmd.Parameters.AddWithValue("@MODEL", model.ModelName);
                Cmd.Parameters.AddWithValue("@PARTNO", model.PartNo);
                Cmd.Parameters.AddWithValue("@QTY", model.Qty);
                Cmd.Parameters.AddWithValue("@CUSTOMER", model.Customer);
                Cmd.Parameters.AddWithValue("@DEVIATION_TYPE", model.Type);
                Cmd.Parameters.AddWithValue("@FILENAME", model.DocName);
                Cmd.Parameters.AddWithValue("@NATURE_OF_ISSUE", model.NatureOfIssue);
                Cmd.Parameters.AddWithValue("@ACTION_PROPOSED", model.ActionProposed);
                Cmd.Parameters.AddWithValue("@ROOT_CAUSE", model.RootCause);
                Cmd.Parameters.AddWithValue("@CORRECTIVE_ACTION", model.CorrectiveAction);
                Cmd.Parameters.AddWithValue("@STATUS", model.Status);
                Cmd.Parameters.AddWithValue("@STATUS_DESCR", model.StatusDescr);
                Cmd.Parameters.AddWithValue("@REMARKS", model.Remarks);
                Cmd.Parameters.AddWithValue("@USERID", model.UserID);
                Cmd.Parameters.AddWithValue("@APPRID", model.ApprID);
                Cmd.Parameters.AddWithValue("@APPRTYPE", model.ApprType);

                Cmd.Parameters.AddWithValue("@DATE1", model.Date1);
                Cmd.Parameters.AddWithValue("@DATE2", model.Date2);
                Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);
                Cmd.Parameters.AddWithValue("@PARAM2", model.Param2);

                SqlDataAdapter da = new(Cmd);
                DataTable dt = new();
                da.Fill(dt);
                return dt;
            }
            finally
            {
                Con.Close();
                Cmd.Dispose();
            }
        }

        public DataTable usp_Calibration(QualityViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_Calibration", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@DOCNO", model.RequestNo);
                Cmd.Parameters.AddWithValue("@CATEGORY", model.Category);
                Cmd.Parameters.AddWithValue("@PLANT", model.Plant);
                Cmd.Parameters.AddWithValue("@DIVISION", model.Division);
                Cmd.Parameters.AddWithValue("@EQUIPID", model.EquipId);
                Cmd.Parameters.AddWithValue("@CERTI_STATUS", model.CertificateStatus);
                Cmd.Parameters.AddWithValue("@DESCR", model.Description);
                Cmd.Parameters.AddWithValue("@MAKE", model.Make);
                Cmd.Parameters.AddWithValue("@AGENCY", model.Agency);
                Cmd.Parameters.AddWithValue("@EQUIP_MODEL", model.EquipModel);
                Cmd.Parameters.AddWithValue("@TYPE", model.Type);
                Cmd.Parameters.AddWithValue("@LOCATION", model.Location);
                Cmd.Parameters.AddWithValue("@MAC_NAME", model.MacName);
                Cmd.Parameters.AddWithValue("@LEAST_CNT", model.LeastCnt);
                Cmd.Parameters.AddWithValue("@FULL_RANGE", model.FullRange);
                Cmd.Parameters.AddWithValue("@OPR_RANGE", model.OperatingRange);
                Cmd.Parameters.AddWithValue("@UNIT", model.Unit);
                Cmd.Parameters.AddWithValue("@REMARKS", model.Remarks);
                Cmd.Parameters.AddWithValue("@USERID", model.UserID);
                Cmd.Parameters.AddWithValue("@STATUS", model.Status);
                Cmd.Parameters.AddWithValue("@FILENAME", model.DocName);
                Cmd.Parameters.AddWithValue("@DATE1", model.Date1);
                Cmd.Parameters.AddWithValue("@DATE2", model.Date2);
                Cmd.Parameters.AddWithValue("@DATE3", model.Date3);
                Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);
                Cmd.Parameters.AddWithValue("@PARAM2", model.Param2);
                Cmd.Parameters.AddWithValue("@PARAM3", model.Param3);
                SqlDataAdapter da = new(Cmd);
                DataTable dt = new();
                da.Fill(dt);
                return dt;
            }
            finally
            {
                Con.Close();
                Cmd.Dispose();
            }
        }

        public DataTable usp_SampleScrap(QualityViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_SampleScrap", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@DOCNO", model.RequestNo);
                Cmd.Parameters.AddWithValue("@PLANT", model.Plant);
                Cmd.Parameters.AddWithValue("@DIVISION", model.Division);
                Cmd.Parameters.AddWithValue("@MODEL", model.ModelName);
                Cmd.Parameters.AddWithValue("@CUST_NAME", model.Customer);
                Cmd.Parameters.AddWithValue("@QTY", model.Quantity);
                Cmd.Parameters.AddWithValue("@ST_LOC", model.Location);
                Cmd.Parameters.AddWithValue("@PRODUCT", model.Product);
                Cmd.Parameters.AddWithValue("@PERIOD", model.Period);
                Cmd.Parameters.AddWithValue("@DESCR", model.Description);
                Cmd.Parameters.AddWithValue("@REMARKS", model.Remarks);
                Cmd.Parameters.AddWithValue("@USERID", model.UserID);
                Cmd.Parameters.AddWithValue("@STATUS", model.Status);
                Cmd.Parameters.AddWithValue("@STATUS_DESCR", model.StatusDescr);
                Cmd.Parameters.AddWithValue("@APPRID", model.ApprID);
                Cmd.Parameters.AddWithValue("@APPRTYPE", model.ApprType);
                Cmd.Parameters.AddWithValue("@NEXTAPPRID", model.NextApprID);
                Cmd.Parameters.AddWithValue("@NEXTAPPRTYPE", model.NextApprType);
                Cmd.Parameters.AddWithValue("@DATE1", model.Date1);
                Cmd.Parameters.AddWithValue("@DATE2", model.Date2);
                Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);
                Cmd.Parameters.AddWithValue("@PARAM2", model.Param2);
                SqlDataAdapter da = new(Cmd);
                DataTable dt = new();
                da.Fill(dt);
                return dt;
            }
            finally
            {
                Con.Close();
                Cmd.Dispose();
            }
        }

        public DataTable usp_LAB(LabTestViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_LAB", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Activity", model.Activity);
                Cmd.Parameters.AddWithValue("@Id", model.ID);
                Cmd.Parameters.AddWithValue("@Product", model.Product);
                Cmd.Parameters.AddWithValue("@ProductOther", model.ProductOther);
                Cmd.Parameters.AddWithValue("@ModelName", model.ModelName);
                Cmd.Parameters.AddWithValue("@PartNumber", model.PartNumber);
                Cmd.Parameters.AddWithValue("@SampleQty", model.SampleQty);
                Cmd.Parameters.AddWithValue("@TestName", model.TestName);
                Cmd.Parameters.AddWithValue("@TestConditionIfAny", model.TestConditionIfAny);
                Cmd.Parameters.AddWithValue("@Status", model.Status);
                Cmd.Parameters.AddWithValue("@Department", model.Department);
                Cmd.Parameters.AddWithValue("@EmpCode", model.EmpCode);
                Cmd.Parameters.AddWithValue("@TestStartDate", model.TestStartDate);
                Cmd.Parameters.AddWithValue("@Param1", model.Param1);
                Cmd.Parameters.AddWithValue("@Param2", model.Param2);
                Cmd.Parameters.AddWithValue("@Param3", model.Param3);
                SqlDataAdapter da = new(Cmd);
                DataTable dt = new();
                da.Fill(dt);
                return dt;
            }
            finally
            {
                Con.Close();
                Cmd.Dispose();
            }
        }
    }
}

using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace eConnectV2.Services
{
    public interface IITService
    {
        DataTable usp_ITHelpDesk(ITViewModel model);
        DataTable usp_ITAsset(ITViewModel model);
    }

    public class ITService : IITService
    {
        public ConnectionStrings _connectionStrings;
        private SqlConnection Con { get; set; }
        private SqlCommand Cmd { get; set; }
        public ITService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }

        public DataTable usp_ITHelpDesk(ITViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_ITHelpDesk", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@DOCNO", model.RequestNo);
                Cmd.Parameters.AddWithValue("@PLANT", model.Plant);
                Cmd.Parameters.AddWithValue("@EMPCODE", model.EmpCode);
                Cmd.Parameters.AddWithValue("@EMPCODE2", model.EmpCode2);
                Cmd.Parameters.AddWithValue("@NAME", model.EmpName);
                Cmd.Parameters.AddWithValue("@NAME2", model.EmpName2);
                Cmd.Parameters.AddWithValue("@DEPT", model.DeptCode);
                Cmd.Parameters.AddWithValue("@EMAIL", model.Email);
                Cmd.Parameters.AddWithValue("@CONTACT", model.ContactNo);
                Cmd.Parameters.AddWithValue("@EXTNO", model.ExtNo);
                Cmd.Parameters.AddWithValue("@PRIORITY", model.Priority);
                Cmd.Parameters.AddWithValue("@REQTYPE", model.Type);
                Cmd.Parameters.AddWithValue("@PROBCATG", model.ProbCatg);
                Cmd.Parameters.AddWithValue("@SUBCATG", model.SubCatg);
                Cmd.Parameters.AddWithValue("@FILENAME", model.DocName);
                Cmd.Parameters.AddWithValue("@SUBJECT", model.Subject);
                Cmd.Parameters.AddWithValue("@DESCR", model.Descr1);
                Cmd.Parameters.AddWithValue("@ROLES", model.Roles);
                Cmd.Parameters.AddWithValue("@REQ_FOR", model.ReqFor);
                Cmd.Parameters.AddWithValue("@ACCESS", model.EmailAccess);
                Cmd.Parameters.AddWithValue("@ADID", model.AdId);
                Cmd.Parameters.AddWithValue("@EMAILID", model.EmailId);
                Cmd.Parameters.AddWithValue("@BIS", model.BIS);
                Cmd.Parameters.AddWithValue("@MES", model.MES);
                Cmd.Parameters.AddWithValue("@SAP", model.SAP);
                Cmd.Parameters.AddWithValue("@ASSET", model.Asset);

                Cmd.Parameters.AddWithValue("@SOLUTION", model.Solution);
                Cmd.Parameters.AddWithValue("@REMARKS", model.Remarks);
                Cmd.Parameters.AddWithValue("@STATUS", model.Status);
                Cmd.Parameters.AddWithValue("@STATUS_DESCR", model.StatusDescr);
                Cmd.Parameters.AddWithValue("@USERID", model.UserID);
                Cmd.Parameters.AddWithValue("@APPRID", model.ApprID);
                Cmd.Parameters.AddWithValue("@APPRTYPE", model.ApprType);
                Cmd.Parameters.AddWithValue("@DATE1", model.Date1);
                Cmd.Parameters.AddWithValue("@DATE2", model.Date2);
                Cmd.Parameters.AddWithValue("@DATE3", model.Date3);
                Cmd.Parameters.AddWithValue("@DATE4", model.Date4);
                Cmd.Parameters.AddWithValue("@DATE5", model.Date5);
                Cmd.Parameters.AddWithValue("@DATE6", model.Date6);
                Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);
                Cmd.Parameters.AddWithValue("@PARAM2", model.Param2);
                Cmd.Parameters.AddWithValue("@PARAM3", model.Param3);
                Cmd.Parameters.AddWithValue("@PARAM4", model.Param4);
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
        public DataTable usp_ITAsset(ITViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_ITAsset", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@DOCNO", model.SrNo);
                Cmd.Parameters.AddWithValue("@REQNO", model.RequestNo);
                Cmd.Parameters.AddWithValue("@PLANT", model.Plant);
                Cmd.Parameters.AddWithValue("@EMPCODE", model.EmpCode);
                Cmd.Parameters.AddWithValue("@NAME", model.EmpName);
                Cmd.Parameters.AddWithValue("@DEPT", model.DeptCode);
                Cmd.Parameters.AddWithValue("@EMAIL", model.Email);
                Cmd.Parameters.AddWithValue("@CONTACT", model.ContactNo);
                Cmd.Parameters.AddWithValue("@ASSET_TYPE", model.Type);
                Cmd.Parameters.AddWithValue("@EQUIPMENT", model.EquipCatg);
                Cmd.Parameters.AddWithValue("@SUBJECT", model.Subject);
                Cmd.Parameters.AddWithValue("@DESCR", model.Descr1);
                Cmd.Parameters.AddWithValue("@MAKE", model.Make);
                Cmd.Parameters.AddWithValue("@MODEL", model.ModelName);
                Cmd.Parameters.AddWithValue("@SerialNo", model.DeviceSerialNo);
                Cmd.Parameters.AddWithValue("@Qty", model.Quantity);
                Cmd.Parameters.AddWithValue("@VENDOR", model.VendorName);
                Cmd.Parameters.AddWithValue("@INVOICE", model.InvoiceNo);
                Cmd.Parameters.AddWithValue("@PONO", model.PONo);
                Cmd.Parameters.AddWithValue("@AMOUNT", model.Amount);
                Cmd.Parameters.AddWithValue("@ASSETNO", model.PermanentAssetNo);
                Cmd.Parameters.AddWithValue("@PROCESSOR", model.Processor);
                Cmd.Parameters.AddWithValue("@CPU_SPEED", model.CPUSpeed);
                Cmd.Parameters.AddWithValue("@RAM", model.RAM);
                Cmd.Parameters.AddWithValue("@HDD", model.HDDType);
                Cmd.Parameters.AddWithValue("@CAPACITY", model.HDDCapacity);
                Cmd.Parameters.AddWithValue("@DVD", model.DVD);
                Cmd.Parameters.AddWithValue("@OS", model.OSVersion);
                Cmd.Parameters.AddWithValue("@REMARKS", model.Remarks);
                Cmd.Parameters.AddWithValue("@USERID", model.UserID);
                Cmd.Parameters.AddWithValue("@STATUS_DESCR", model.StatusDescr);
                Cmd.Parameters.AddWithValue("@APPRID", model.ApprID);
                //Cmd.Parameters.AddWithValue("@APPRTYPE", model.ApprType);
                Cmd.Parameters.AddWithValue("@STATUS", model.Status);
                Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);
                Cmd.Parameters.AddWithValue("@DATE1", model.Date1);
                Cmd.Parameters.AddWithValue("@DATE2", model.Date2);

                //Cmd.Parameters.AddWithValue("@EMPCODE", model.EmpCode);
                //Cmd.Parameters.AddWithValue("@NAME", model.EmpName);
                //Cmd.Parameters.AddWithValue("@DEPT", model.DeptCode);
                //Cmd.Parameters.AddWithValue("@EMAIL", model.Email);
                //Cmd.Parameters.AddWithValue("@CONTACT", model.ContactNo);
                //Cmd.Parameters.AddWithValue("@EXTNO", model.ExtNo);
                //Cmd.Parameters.AddWithValue("@PRIORITY", model.Priority);
                //Cmd.Parameters.AddWithValue("@REQTYPE", model.Type);
                //Cmd.Parameters.AddWithValue("@PROBCATG", model.ProbCatg);
                //Cmd.Parameters.AddWithValue("@SUBCATG", model.SubCatg);
                //Cmd.Parameters.AddWithValue("@FILENAME", model.DocName);
                //Cmd.Parameters.AddWithValue("@SUBJECT", model.Subject);
                //Cmd.Parameters.AddWithValue("@DESCR", model.Descr1);
                //Cmd.Parameters.AddWithValue("@SOLUTION", model.Solution);
                //Cmd.Parameters.AddWithValue("@REMARKS", model.Remarks);

                //Cmd.Parameters.AddWithValue("@STATUS", model.Status);
                //Cmd.Parameters.AddWithValue("@USERID", model.UserID);
                //Cmd.Parameters.AddWithValue("@APPRID", model.ApprID);
                //Cmd.Parameters.AddWithValue("@APPRTYPE", model.ApprType);

                //Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);
                //Cmd.Parameters.AddWithValue("@PARAM2", model.Param2);
                //Cmd.Parameters.AddWithValue("@PARAM3", model.Param3);
                //Cmd.Parameters.AddWithValue("@PARAM4", model.Param4);
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

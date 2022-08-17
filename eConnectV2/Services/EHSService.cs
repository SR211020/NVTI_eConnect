using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace eConnectV2.Services
{
    public interface IEHSService
    {
        DataTable usp_OHC(EHSViewModel model);
    }

    public class EHSService : IEHSService
    {
        public ConnectionStrings _connectionStrings;
        private SqlConnection Con { get; set; }
        private SqlCommand Cmd { get; set; }
        public EHSService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }

        public DataTable usp_OHC(EHSViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_OHC", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@REQNO", model.RequestNo);
                Cmd.Parameters.AddWithValue("@AADHAR", model.AadharNo);
                Cmd.Parameters.AddWithValue("@PLANT", model.Plant);
                Cmd.Parameters.AddWithValue("@EMPCODE", model.EmpCode);
                Cmd.Parameters.AddWithValue("@EMPNAME", model.EmpName);
                Cmd.Parameters.AddWithValue("@DEPT", model.DeptName);
                Cmd.Parameters.AddWithValue("@CONDITION", model.Condition);
                Cmd.Parameters.AddWithValue("@INJURY", model.Injury);
                Cmd.Parameters.AddWithValue("@INJURY_TYPE", model.InjuryType);
                Cmd.Parameters.AddWithValue("@INJURY_TYPE_OTHER", model.InjuryTypeOther);
                Cmd.Parameters.AddWithValue("@NURSHING_STAFF", model.NurshingStaff);
                Cmd.Parameters.AddWithValue("@QTY", model.Quantity);
                Cmd.Parameters.AddWithValue("@RESTGIVEN", model.RestGiven);
                Cmd.Parameters.AddWithValue("@REFER", model.Refer);
                Cmd.Parameters.AddWithValue("@TREATMENT", model.Treatment);
                Cmd.Parameters.AddWithValue("@REMARKS", model.Remarks);
                Cmd.Parameters.AddWithValue("@USERID", model.UserID);
                Cmd.Parameters.AddWithValue("@DATE1", model.Date1);
                Cmd.Parameters.AddWithValue("@DATE2", model.Date2);
                Cmd.Parameters.AddWithValue("@TIME1", model.Time1);
                Cmd.Parameters.AddWithValue("@TIME2", model.Time2);
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
    }

}

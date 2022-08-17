using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;


namespace eConnectV2.Services
{
    public interface IKaizenService
    {
        DataTable usp_Kaizen(KaizenViewModel model);
    }
    public class KaizenService : IKaizenService
    {
        public ConnectionStrings _connectionStrings;
        private SqlConnection Con { get; set; }
        private SqlCommand Cmd { get; set; }
        public KaizenService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }
        public DataTable usp_Kaizen(KaizenViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_KAIZEN", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@DOCNO", model.DocNo);
                Cmd.Parameters.AddWithValue("@KAIZEN_NAME", model.KaizenName);
                Cmd.Parameters.AddWithValue("@EMPCODE", model.EmpCode);
                Cmd.Parameters.AddWithValue("@EMPNAME", model.EmpName);
                Cmd.Parameters.AddWithValue("@DEPTCODE", model.DeptCode);
                Cmd.Parameters.AddWithValue("@PLANT", model.Plant);
                Cmd.Parameters.AddWithValue("@IMPACT", model.SelectedImpact);
                Cmd.Parameters.AddWithValue("@FINYEAR", model.FinYearCode);
                Cmd.Parameters.AddWithValue("@YR", model.Year);
                Cmd.Parameters.AddWithValue("@MON", model.Month);
                Cmd.Parameters.AddWithValue("@MACAREA", model.MacArea);
                Cmd.Parameters.AddWithValue("@HD_DONE", model.HDDone);
                Cmd.Parameters.AddWithValue("@CIRCLENO", model.CircleNo);
                Cmd.Parameters.AddWithValue("@RESP_PILLAR", model.RespPillar);
                Cmd.Parameters.AddWithValue("@KAIZEN_TYPE", model.KaizenType);
                Cmd.Parameters.AddWithValue("@MP_WORTHY", model.MPWorthy);
                Cmd.Parameters.AddWithValue("@COST_SAVING", model.CostSaving);
                Cmd.Parameters.AddWithValue("@FILENAME", model.DocName);
                Cmd.Parameters.AddWithValue("@REMARKS", model.Remarks);
                Cmd.Parameters.AddWithValue("@STATUS", model.Status);
                Cmd.Parameters.AddWithValue("@STATUS_DESCR", model.StatusDescr);
                Cmd.Parameters.AddWithValue("@USERID", model.UserId);
                Cmd.Parameters.AddWithValue("@APPRID", model.ApprId);
                Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);
                Cmd.Parameters.AddWithValue("@PARAM2", model.Param2);
                Cmd.Parameters.AddWithValue("@DATE1", model.Date1);
                Cmd.Parameters.AddWithValue("@DATE2", model.Date2);
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

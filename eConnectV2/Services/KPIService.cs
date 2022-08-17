using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace eConnectV2.Services
{
    public interface IKPIService
    {
        DataTable Usp_KPI(KPIViewModel model);
    }

    public class KPIService : IKPIService
    {
        public ConnectionStrings _connectionStrings;
        private SqlConnection Con { get; set; }
        private SqlCommand Cmd { get; set; }
        public KPIService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }

        public DataTable Usp_KPI(KPIViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_KPI", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);
                Cmd.Parameters.AddWithValue("@CONFIG_CATEGORY1", model.ConfigCategory1);
                Cmd.Parameters.AddWithValue("@EMPCODE", model.EmpCode);
                Cmd.Parameters.AddWithValue("@USERID", model.AdId);
                Cmd.Parameters.AddWithValue("@KPITYPE", model.KpiType);
                Cmd.Parameters.AddWithValue("@FINYEAR", model.FinYearCode);
                Cmd.Parameters.AddWithValue("@KPINO", model.KpiNo);
                Cmd.Parameters.AddWithValue("@KPINAME", model.KpiName);
                Cmd.Parameters.AddWithValue("@SRNO", model.SrNo);

                Cmd.Parameters.AddWithValue("@CATEGORY", model.CategoryCode);
                Cmd.Parameters.AddWithValue("@WEIGHTAGE", model.Weightage);
                Cmd.Parameters.AddWithValue("@RESPDEPT", model.RespDeptCode);
                Cmd.Parameters.AddWithValue("@ISAPPR", model.IsKPIApprove);
                Cmd.Parameters.AddWithValue("@APPR_CODE", model.ApproverCode);
                Cmd.Parameters.AddWithValue("@BOTTOMTGT", model.BottomTarget);
                Cmd.Parameters.AddWithValue("@BASICTGT", model.BasicTarget);
                Cmd.Parameters.AddWithValue("@CHALLENGETGT", model.ChallengeTarget);

                Cmd.Parameters.AddWithValue("@HODCODE", model.HodCode);
                Cmd.Parameters.AddWithValue("@STATUS", model.KpiStatusCode);

                Cmd.Parameters.AddWithValue("@DOC_ID", model.DocUId);
                Cmd.Parameters.AddWithValue("@DEPTCODE", model.DeptCode);

                Cmd.Parameters.AddWithValue("@Month", model.Month);

                Cmd.Parameters.AddWithValue("@INPUTVAL", model.InputVal);
                Cmd.Parameters.AddWithValue("@CALC", model.Calc);

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

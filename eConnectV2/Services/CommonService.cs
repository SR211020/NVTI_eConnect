using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace eConnectV2.Services
{
    public interface ICommonService
    {
        DataTable usp_LogIn(LoginViewModel model);
        DataTable usp_Master(CommonViewModel model);
    }

    public class CommonService : ICommonService
    {
        public ConnectionStrings _connectionStrings;
        private SqlConnection Con { get; set; }
        private SqlCommand Cmd { get; set; }
        public CommonService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }
        public DataTable usp_LogIn(LoginViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_Login", Con) { CommandType = CommandType.StoredProcedure };
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@USERID", model.UserId);
                Cmd.Parameters.AddWithValue("@NAME", model.Name);
                Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);
                Cmd.Parameters.AddWithValue("@PARAM2", model.Param2);
                Cmd.Parameters.AddWithValue("@PARAM3", model.Param3);
                Cmd.Parameters.AddWithValue("@PARAM4", model.Param4);
                Cmd.Parameters.AddWithValue("@PARAM5", model.Param5);
                SqlDataAdapter da = new(Cmd);
                DataTable dt = new();
                da.Fill(dt);
                Con.Close();
                return dt;
            }
            finally
            {
                Con.Close();
                Cmd.Dispose();
            }
        }

        public DataTable usp_Master(CommonViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_MASTER", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@DOCNO", model.DocNo);
                Cmd.Parameters.AddWithValue("@PLANT", model.Plant);
                Cmd.Parameters.AddWithValue("@DEPT", model.Dept);
                Cmd.Parameters.AddWithValue("@USERID", model.UserId);
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
    }
}

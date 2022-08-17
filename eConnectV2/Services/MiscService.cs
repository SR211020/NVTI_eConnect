using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace eConnectV2.Services
{
    public interface IMiscService
    {
        DataTable usp_Misc(MiscViewModel model);
    }

    public class MiscService : IMiscService
    {
        public ConnectionStrings _connectionStrings;
        private SqlConnection Con { get; set; }
        private SqlCommand Cmd { get; set; }
        public MiscService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }
        public DataTable usp_Misc(MiscViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_Misc", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@USERID", model.UserID);
                Cmd.Parameters.AddWithValue("@IMAGE", model.BImage);
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
    }
}

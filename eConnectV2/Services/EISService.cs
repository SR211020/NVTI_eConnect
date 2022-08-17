using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace eConnectV2.Services
{
    public interface IEISService
    {
        DataTable usp_EIS(EISViewModel model);
        DataTable usp_EIS_ADD(EISViewModel model);
    }

    public class EISService : IEISService
    {
        public ConnectionStrings _connectionStrings;
        private SqlConnection Con { get; set; }
        private SqlCommand Cmd { get; set; }
        public EISService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }

        public DataTable usp_EIS(EISViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_EIS", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@PLANT", model.Plant);
                Cmd.Parameters.AddWithValue("@DIVISION", model.Division);
                Cmd.Parameters.AddWithValue("@FROM_YR", model.FromYear);
                Cmd.Parameters.AddWithValue("@FROM_MON", model.FromMonth);
                Cmd.Parameters.AddWithValue("@TO_YR", model.ToYear);
                Cmd.Parameters.AddWithValue("@TO_MON", model.ToMonth);
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

        public DataTable usp_EIS_ADD(EISViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_EIS_ADD", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@DOCNO", model.DocNo);
                Cmd.Parameters.AddWithValue("@PLANT", model.Plant);
                Cmd.Parameters.AddWithValue("@DIVISION", model.Division);
                Cmd.Parameters.AddWithValue("@YEAR", model.Yr);
                Cmd.Parameters.AddWithValue("@MONTH", model.Mon);
                Cmd.Parameters.AddWithValue("@BOTTOM", model.Bottom);
                Cmd.Parameters.AddWithValue("@BASIC", model.Basic);
                Cmd.Parameters.AddWithValue("@CHALLENGE", model.Challenge);
                Cmd.Parameters.AddWithValue("@AMOUNT1", model.Amount);
                Cmd.Parameters.AddWithValue("@AMOUNT2", model.Amount2);

                Cmd.Parameters.AddWithValue("@USERID", model.UserId);
                Cmd.Parameters.AddWithValue("@DATE1", model.Date1);
                Cmd.Parameters.AddWithValue("@DATE2", model.Date2);
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

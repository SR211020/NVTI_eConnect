using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace eConnectV2.Services
{
    public interface IMESService
    {
        DataTable usp_Reports(MESViewModel model);
    }

    public class MESService : IMESService
    {
        public ConnectionStrings _connectionStrings;
        public MESService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }
        public DataTable usp_Reports(MESViewModel model)
        {
            OracleConnection con = new();
            OracleCommand cmd = new();
            try
            {
                con = new OracleConnection(_connectionStrings.MES_Connection);
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd = new OracleCommand("USP_REPORTS", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("a_ACTIVITY", OracleDbType.Varchar2).Value = model.Activity;
                cmd.Parameters.Add("a_PARAM1", OracleDbType.Varchar2).Value = model.Param1;
                cmd.Parameters.Add("a_PARAM2", OracleDbType.Varchar2).Value = model.Param2;
                cmd.Parameters.Add("a_PARAM3", OracleDbType.Varchar2).Value = model.Param3;
                cmd.Parameters.Add("a_USERID", OracleDbType.Varchar2).Value = model.UserId;
                cmd.Parameters.Add("a_DATE1", OracleDbType.Varchar2).Value = model.Date1;
                cmd.Parameters.Add("a_DATE2", OracleDbType.Varchar2).Value = model.Date2;
                cmd.Parameters.Add("a_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                OracleDataAdapter oda = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                oda.Fill(dt);
                return dt;
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
        }
    }
}


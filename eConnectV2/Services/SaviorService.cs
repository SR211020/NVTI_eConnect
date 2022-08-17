using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace eConnectV2.Services
{
    public interface ISaviorService
    {
        DataSet usp_Reports(SaviorViewModel model);
    }

    public class SaviorService : ISaviorService
    {
        public ConnectionStrings _connectionStrings;
        public SaviorService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }
        public DataSet usp_Reports(SaviorViewModel model)
        {
            OracleConnection con = new();
            OracleCommand cmd = new();
            try
            {
                con = new OracleConnection(_connectionStrings.Savior_Connection);
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd = new OracleCommand("USP_REPORTS", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("a_ACTIVITY", OracleDbType.Varchar2).Value = model.Activity;
                cmd.Parameters.Add("a_EMPCODE", OracleDbType.Varchar2).Value = model.EmpCode;
                cmd.Parameters.Add("a_PARAM1", OracleDbType.Varchar2).Value = model.Param1;
                cmd.Parameters.Add("a_PARAM2", OracleDbType.Varchar2).Value = model.Param2;
                cmd.Parameters.Add("a_DATE1", OracleDbType.Varchar2).Value = model.Date1;
                cmd.Parameters.Add("a_DATE2", OracleDbType.Varchar2).Value = model.Date2;
                cmd.Parameters.Add("a_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                OracleDataAdapter oda = new OracleDataAdapter(cmd);
                DataSet ds = new DataSet();
                oda.Fill(ds);
                return ds;
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
        }
    }
}

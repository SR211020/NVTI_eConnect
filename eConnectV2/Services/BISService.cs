using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace eConnectV2.Services
{
    public interface IBISService
    {
        DataTable usp_UPPH(BISViewModel model);
    }

    public class BISService : IBISService
    {
        public ConnectionStrings _connectionStrings;
        public BISService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }

        public DataTable usp_UPPH(BISViewModel model)
        {
            OracleConnection con = new();
            OracleCommand cmd = new();
            try
            {
                if (model.Plant == "MANESAR")
                {
                    con = new OracleConnection(_connectionStrings.BISManesar_Connection);
                }
                else
                {
                    con = new OracleConnection(_connectionStrings.BISBawal_Connection);
                }
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd = new OracleCommand("usp_UPPH", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("a_ACTIVITY", OracleDbType.Varchar2).Value = model.Activity;
                cmd.Parameters.Add("a_SRNO", OracleDbType.Int32).Value = model.DocNo;
                cmd.Parameters.Add("a_DIVISION", OracleDbType.Varchar2).Value = model.Division;
                cmd.Parameters.Add("a_CUSTOMER", OracleDbType.Varchar2).Value = model.CustomerName;
                cmd.Parameters.Add("a_LINENO", OracleDbType.Varchar2).Value = model.LineNo;
                cmd.Parameters.Add("a_LINETYPE", OracleDbType.Varchar2).Value = model.LineType;
                cmd.Parameters.Add("a_MODEL", OracleDbType.Varchar2).Value = model.ModelName;
                cmd.Parameters.Add("a_SHIFT", OracleDbType.Varchar2).Value = model.Shift;
                cmd.Parameters.Add("a_PLAN", OracleDbType.Int32).Value = model.Plan;
                cmd.Parameters.Add("a_UPH", OracleDbType.Int32).Value = model.UphTarget;
                cmd.Parameters.Add("a_STDHC", OracleDbType.Int32).Value = model.StdHC;
                cmd.Parameters.Add("a_ACTUALHC", OracleDbType.Int32).Value = model.ActualHC;
                cmd.Parameters.Add("a_UPPH", OracleDbType.Decimal).Value = model.UpphTarget;
                cmd.Parameters.Add("a_USERID", OracleDbType.Varchar2).Value = model.UserId;
                cmd.Parameters.Add("a_PARAM1", OracleDbType.Varchar2).Value = model.Param1;
                cmd.Parameters.Add("a_PARAM2", OracleDbType.Varchar2).Value = model.Param2;
                cmd.Parameters.Add("a_PARAM3", OracleDbType.Varchar2).Value = model.Param3;
                cmd.Parameters.Add("a_PARAM4", OracleDbType.Varchar2).Value = model.Param4;
                cmd.Parameters.Add("a_PARAM5", OracleDbType.Varchar2).Value = model.Param5;
                cmd.Parameters.Add("a_PARAM6", OracleDbType.Varchar2).Value = model.Param6;
                cmd.Parameters.Add("a_PARAM7", OracleDbType.Varchar2).Value = model.Param7;
                cmd.Parameters.Add("a_PARAM8", OracleDbType.Varchar2).Value = model.Param8;
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

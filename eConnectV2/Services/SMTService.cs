using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace eConnectV2.Services
{
    public interface ISMTService
    {
        DataTable usp_SMT(SMTViewModel model);
        DataTable usp_SMT_BAKING(SMTViewModel model);
    }
    public class SMTService : ISMTService
    {
        public ConnectionStrings _connectionStrings;
        private SqlConnection Con { get; set; }
        private SqlCommand Cmd { get; set; }
        public SMTService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }
        public DataTable usp_SMT(SMTViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_SMT", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);
                Cmd.Parameters.AddWithValue("@PARAM2", model.Param2);
                Cmd.Parameters.AddWithValue("@QRCODE", model.QRCode);
                Cmd.Parameters.AddWithValue("@CATEGORY", model.MaterialCategory);
                Cmd.Parameters.AddWithValue("@VENDORNAME", model.VendorName);
                Cmd.Parameters.AddWithValue("@MATERIALNO", model.MaterialNo);
                Cmd.Parameters.AddWithValue("@PARTNO", model.PartNo);
                Cmd.Parameters.AddWithValue("@SUBQRCODE", model.SubQRCode);
                Cmd.Parameters.AddWithValue("@SUBSEQNO", model.SubSeqNo);
                Cmd.Parameters.AddWithValue("@USERID", model.EmpAdId);
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

        public DataTable usp_SMT_BAKING(SMTViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_SMT_BAKING", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@DOCNO", model.DocNo);
                Cmd.Parameters.AddWithValue("@PARTNO", model.PartNo);
                Cmd.Parameters.AddWithValue("@MODEL", model.ModelName);
                Cmd.Parameters.AddWithValue("@USERID", model.UserId);

                Cmd.Parameters.AddWithValue("@DATE1", model.Date1);
                Cmd.Parameters.AddWithValue("@DATE2", model.Date2);
                Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);
                Cmd.Parameters.AddWithValue("@PARAM2", model.Param2);
                Cmd.Parameters.AddWithValue("@PARAM3", model.Param3);
                Cmd.Parameters.AddWithValue("@PARAM4", model.Param4);
                Cmd.Parameters.AddWithValue("@PARAM5", model.Param5);
                Cmd.Parameters.AddWithValue("@PARAM6", model.Param6);
                Cmd.Parameters.AddWithValue("@PARAM7", model.Param7);
                Cmd.Parameters.AddWithValue("@PARAM8", model.Param8);
                Cmd.Parameters.AddWithValue("@PARAM9", model.Param9);
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

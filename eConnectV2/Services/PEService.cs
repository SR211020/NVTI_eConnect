using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace eConnectV2.Services
{

    public interface IPEService
    {
        DataTable usp_PE(PEViewModel model);
    }

    public class PEService : IPEService
    {
        public ConnectionStrings _connectionStrings;
        private SqlConnection Con { get; set; }
        private SqlCommand Cmd { get; set; }
        public PEService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }

        public DataTable usp_PE(PEViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_PE", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@DOCNO", model.DocNo);
                Cmd.Parameters.AddWithValue("@CUSTOMER", model.Customer);
                Cmd.Parameters.AddWithValue("@MODEL", model.ModelName);
                Cmd.Parameters.AddWithValue("@LINE", model.Line);
                Cmd.Parameters.AddWithValue("@SCRAP_CATG", model.ScrapCatg);
                Cmd.Parameters.AddWithValue("@SCRAP_SUB_CATG", model.ScrapSubCatg);
                Cmd.Parameters.AddWithValue("@4M_CATG", model.MCatg);
                Cmd.Parameters.AddWithValue("@SHIFT", model.Shift);
                Cmd.Parameters.AddWithValue("@BARCODE", model.BarCode);
                Cmd.Parameters.AddWithValue("@SCRAP_DATE", model.ScrapDate);
                Cmd.Parameters.AddWithValue("@REMARKS", model.Remarks);
                Cmd.Parameters.AddWithValue("@QUANTITY", model.Quantity);
                Cmd.Parameters.AddWithValue("@PROD_DATE", model.ProdDate);
                Cmd.Parameters.AddWithValue("@USERID", model.UserId);
                Cmd.Parameters.AddWithValue("@DATE1", model.Date1);
                Cmd.Parameters.AddWithValue("@DATE2", model.Date2);
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

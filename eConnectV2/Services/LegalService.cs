using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace eConnectV2.Services
{
    public interface ILegalService
    {
        DataTable usp_Legal(LegalViewModel model);
    }
    public class LegalService : ILegalService
    {
        public ConnectionStrings _connectionStrings;
        private SqlConnection Con { get; set; }
        private SqlCommand Cmd { get; set; }
        public LegalService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }
        public DataTable usp_Legal(LegalViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_Legal", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ACTIVITY", model.Activity);
                Cmd.Parameters.AddWithValue("@DOCNO", model.DocNo);
                Cmd.Parameters.AddWithValue("@CONTRACT_NAME", model.ContractName);
                Cmd.Parameters.AddWithValue("@CONTRACT_NUMBER", model.ContractNumber);
                Cmd.Parameters.AddWithValue("@CONTRACT_VALUE", model.ContractValue);
                Cmd.Parameters.AddWithValue("@CURRENCY", model.Currency);
                Cmd.Parameters.AddWithValue("@CONTRACT_CATG", model.ContractCategory);
                Cmd.Parameters.AddWithValue("@CONTRACT_NATURE", model.ContractNature);
                Cmd.Parameters.AddWithValue("@VENDOR", model.Vendor);
                Cmd.Parameters.AddWithValue("@PLANT", model.Plant);
                Cmd.Parameters.AddWithValue("@DEPTCODE", model.DeptCode);
                Cmd.Parameters.AddWithValue("@EMAIL", model.Email);
                Cmd.Parameters.AddWithValue("@EXE_DATE", model.ExeDate);
                Cmd.Parameters.AddWithValue("@EFF_DATE", model.EffDate);
                if (model.ExpDate != null && model.ExpDate != "")
                {
                    Cmd.Parameters.Add("@EXP_DATE", SqlDbType.DateTime).Value = Convert.ToDateTime(model.ExpDate);
                }
                else
                {
                    Cmd.Parameters.Add("@EXP_DATE", SqlDbType.DateTime).Value = DBNull.Value;
                }                
                Cmd.Parameters.AddWithValue("@MASTER_CONTRACT", model.MasterContract);
                Cmd.Parameters.AddWithValue("@DOC_DRAFT", model.DocDraft);
                Cmd.Parameters.AddWithValue("@DOC_SIGNED", model.DocSigned);
                Cmd.Parameters.AddWithValue("@REMARK", model.Remark);
                Cmd.Parameters.AddWithValue("@SERVICE_DESC", model.ServiceDescription);
                Cmd.Parameters.AddWithValue("@USERID", model.UserId);
                Cmd.Parameters.AddWithValue("@PARAM1", model.Param1);
                Cmd.Parameters.AddWithValue("@PARAM2", model.Param2);
                if (model.Date1 != null && model.Date1 != "")
                {
                    Cmd.Parameters.Add("@DATE1", SqlDbType.DateTime).Value = Convert.ToDateTime(model.Date1);
                }
                else
                {
                    Cmd.Parameters.Add("@DATE1", SqlDbType.DateTime).Value = DBNull.Value;
                }
                if (model.Date2 != null && model.Date2 != "")
                {
                    Cmd.Parameters.Add("@DATE2", SqlDbType.DateTime).Value = Convert.ToDateTime(model.Date2);
                }
                else
                {
                    Cmd.Parameters.Add("@DATE2", SqlDbType.DateTime).Value = DBNull.Value;
                }
                if (model.Date3 != null && model.Date3 != "")
                {
                    Cmd.Parameters.Add("@DATE3", SqlDbType.DateTime).Value = Convert.ToDateTime(model.Date3);
                }
                else
                {
                    Cmd.Parameters.Add("@DATE3", SqlDbType.DateTime).Value = DBNull.Value;
                }
                if (model.Date4 != null && model.Date4 != "")
                {
                    Cmd.Parameters.Add("@DATE4", SqlDbType.DateTime).Value = Convert.ToDateTime(model.Date4);
                }
                else
                {
                    Cmd.Parameters.Add("@DATE4", SqlDbType.DateTime).Value = DBNull.Value;
                }
                if (model.Date5 != null && model.Date5 != "")
                {
                    Cmd.Parameters.Add("@DATE5", SqlDbType.DateTime).Value = Convert.ToDateTime(model.Date5);
                }
                else
                {
                    Cmd.Parameters.Add("@DATE5", SqlDbType.DateTime).Value = DBNull.Value;
                }
                if (model.Date6 != null && model.Date6 != "")
                {
                    Cmd.Parameters.Add("@DATE6", SqlDbType.DateTime).Value = Convert.ToDateTime(model.Date6);
                }
                else
                {
                    Cmd.Parameters.Add("@DATE6", SqlDbType.DateTime).Value = DBNull.Value;
                }
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

using eConnectV2.Helpers;
using eConnectV2.ViewModels;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace eConnectV2.Services
{
    public interface IDTService
    {
        DataTable Usp_DT(DTViewModel model);
    }

    public class DTService : IDTService
    {
        public ConnectionStrings _connectionStrings;
        private SqlConnection Con { get; set; }
        private SqlCommand Cmd { get; set; }
        public DTService(IOptions<ConnectionStrings> options)
        {
            _connectionStrings = options.Value;
        }

        public DataTable Usp_DT(DTViewModel model)
        {
            try
            {
                Con = new SqlConnection(_connectionStrings.SQL_Connection);
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                Cmd = new SqlCommand("usp_DT", Con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Activity", model.Activity);
                Cmd.Parameters.AddWithValue("@AadharNo", model.AadharNo);
                Cmd.Parameters.AddWithValue("@EmpName", model.EmpName);
                Cmd.Parameters.AddWithValue("@EmpCode", model.EmpCode);
                Cmd.Parameters.AddWithValue("@Qualification", model.Qualification);
                Cmd.Parameters.AddWithValue("@DOJ", model.DOJ);
                Cmd.Parameters.AddWithValue("@DOL", model.DOL);
                Cmd.Parameters.AddWithValue("@EmpType", model.EmpType);
                Cmd.Parameters.AddWithValue("@EmpStatus", model.EmpStatus);
                Cmd.Parameters.AddWithValue("@Grade", model.Grade);
                Cmd.Parameters.AddWithValue("@Reason", model.Reason);
                Cmd.Parameters.AddWithValue("@IsEmpReJoin", model.IsEmpReJoin);
                Cmd.Parameters.AddWithValue("@OldEmpCode", model.OldEmpCode);
                Cmd.Parameters.AddWithValue("@ADID", model.AdId);
                Cmd.Parameters.AddWithValue("@Station", model.Station);
                Cmd.Parameters.AddWithValue("@Que", model.Que);
                Cmd.Parameters.AddWithValue("@Option1", model.Option1);
                Cmd.Parameters.AddWithValue("@Option2", model.Option2);
                Cmd.Parameters.AddWithValue("@Option3", model.Option3);
                Cmd.Parameters.AddWithValue("@Option4", model.Option4);
                Cmd.Parameters.AddWithValue("@Ans", model.Ans);
                Cmd.Parameters.AddWithValue("@Action", model.Action);
                Cmd.Parameters.AddWithValue("@Id", model.Id);
                Cmd.Parameters.AddWithValue("@TrainingDate", model.TrainingDate);
                Cmd.Parameters.AddWithValue("@TrainingFor", model.TrainingFor);
                Cmd.Parameters.AddWithValue("@Trainer", model.Trainer);
                Cmd.Parameters.AddWithValue("@EmpCodes", model.EmpCodes);
                Cmd.Parameters.AddWithValue("@DocUId", model.DocUId);
                Cmd.Parameters.AddWithValue("@Customerid", model.CustomerId);
                Cmd.Parameters.AddWithValue("@CustomerName", model.CustomerName);
                Cmd.Parameters.AddWithValue("@PrimaryApprover", model.PrimaryApprover);
                Cmd.Parameters.AddWithValue("@Approver1", model.Approver1);
                Cmd.Parameters.AddWithValue("@Approver2", model.Approver2);
                Cmd.Parameters.AddWithValue("@Approver3", model.Approver3);
                Cmd.Parameters.AddWithValue("@Approver4", model.Approver4);
                Cmd.Parameters.AddWithValue("@Approver5", model.Approver5);
                Cmd.Parameters.AddWithValue("@Approver6", model.Approver6);
                Cmd.Parameters.AddWithValue("@TestScore", model.TestScore);
                Cmd.Parameters.AddWithValue("@TestScoreOutof", model.TestScoreOutof);
                Cmd.Parameters.AddWithValue("@SelectedAns", model.SelectedAnsName);
                Cmd.Parameters.AddWithValue("@IsAnsCorrect", model.IsAnsCorrect);

                Cmd.Parameters.AddWithValue("@Param1", model.Param1);
                Cmd.Parameters.AddWithValue("@Param2", model.Param2);
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

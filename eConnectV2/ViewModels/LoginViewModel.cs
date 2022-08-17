namespace eConnectV2.ViewModels
{
    public class LoginViewModel
    {
        #region param list
        public string Activity { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Param4 { get; set; }
        public string Param5 { get; set; }
        #endregion

        #region query result
        public string ADID { get; set; }
        public string EMPCODE { get; set; }
        public string EMPNAME { get; set; }
        public string DESIGNATION { get; set; }
        public string DEPTCODE { get; set; }
        public string DEPTNAME { get; set; }
        public string CONTACTNO_O { get; set; }
        public string EMAILID_O { get; set; }
        public string LANDLINE { get; set; }
        public bool ISACTIVE { get; set; }
        #endregion
    }
}

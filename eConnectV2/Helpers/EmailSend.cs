using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace eConnectV2.Helpers
{
    public class EmailSend
    {
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Attachment { get; set; }

        public bool SendEmail()
        {
            try
            {
                MailMessage mailMsg = new();
                if (To != null && To.Trim() != "")
                {
                    string[] ToMail = To.Split(',');
                    foreach (string strMultiId in ToMail)
                    {
                        mailMsg.To.Add(new MailAddress(strMultiId));
                    }
                }
                else
                {
                    return false;
                }
                if (CC != null && CC.Trim() != "")
                {
                    string[] ToCC = CC.Split(',');
                    foreach (string strMultiId in ToCC)
                    {
                        mailMsg.CC.Add(new MailAddress(strMultiId));
                    }
                }
                if (BCC != null && BCC.Trim() != "")
                {
                    string[] ToBCC = BCC.Split(',');
                    foreach (string strMultiId in ToBCC)
                    {
                        mailMsg.Bcc.Add(new MailAddress(strMultiId));
                    }
                }
                //if (Attachment != null && Attachment.Trim() != "")
                //{
                //    String[] ToAttachment = Attachment.Split(',');
                //    foreach (string strMultiAttachments in ToAttachment)
                //    {
                //        mailMsg.Attachments.Add(new Attachment(HttpContext.Current.Server.MapPath(strMultiAttachments)));
                //    }
                //}
                mailMsg.From = new MailAddress("nvti.notification@nvtpower.com");
                mailMsg.Subject = Subject;
                Body = "<font size='2' face='verdana'>" + Body + "</font>";
                Body += "<br/><br/><b><font size='2' color='blue' face='verdana'>Navitasys India Private Limited</font></b>";
                Body += "<br/><br/><font size='0.25' color='gray' face='verdana'>*Note : This is a Computer generated mail. Please do not reply. For more information kindly contact with NVTI IT Team.</font>";
                mailMsg.Body = Body;
                mailMsg.IsBodyHtml = true;

                SmtpClient Smtp = new SmtpClient
                {
                    Host = "mail.nvtpower.com",
                    Port = 587,
                    Credentials = new System.Net.NetworkCredential("nvti.notification@nvtpower.com", "Confirm@123")
                };
                //Smtp.EnableSsl = true;
                Smtp.Send(mailMsg);
                Smtp.Dispose();
                mailMsg.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                string str = ex.Message.ToString();
                return false;
            }
        }
    }
}

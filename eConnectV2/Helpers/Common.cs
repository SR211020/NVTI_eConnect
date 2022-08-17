using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace eConnectV2.Helpers
{
    public static class Common
    {
        public const string URL = "https://eConnect.nvtpower.com/";
        public const string URL_Test = "http://eConnect.nvtpower.com:8090/";
        public const string FilePath = @"D:\WebApplications\AppFiles";
        public const string SK_Plant = "_plant";
        public const string SK_ADId = "_adid";
        public const string SK_EmpCode = "_empcode";
        public const string SK_EmpName = "_empname";
        public const string SK_EmailId = "_emailId";
        public const string SK_Designation = "_designation";
        public const string SK_DeptCode = "_deptcode";
        public const string SK_DeptName = "_deptname";
        public const string SK_ContactNo = "_contactno";
        public const string SK_ExtNo = "_extno";
        public const string SomethingWentWrong = "Oops! Something went Wrong";
        public const string ResultSuccess = "1";
        public const string ResultError = "2";
        public const string ResultSuccessMessage = "Record saved successfully";
        public const string ResultUpdateMessage = "Record updated successfully";
        public const string RecordDeletedSuccess = "Record deleted successfully";
        public const string AlreadyExist = "Record already exists";
        public const string RecordAlreadyInUse = "Record already in use";
        public const string ResultUnauthorizedMessage = "Access denied! You are not Authorized";
        public const string SubMenuDirectAccessNotAllowed = "Direct access of subMenu page is not allowed. please go with using Goto SubMenu button.";
        public const string CodeDefDirectAccessNotAllowed = "Direct access of code def page is not allowed. please go with using Goto Code type button.";
        public const string BarcodeAlreadyExist = "Barcode already exists";

        public static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            List<T> lstItems = new();
            if (dt == null || dt.Rows.Count == 0)
            {
                return lstItems;
            }
            else
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        T item = GetItem<T>(row);
                        lstItems.Add(item);
                    }
                }
                else
                {
                    lstItems = null;
                }
                return lstItems;
            }
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name.ToUpper() == column.ColumnName.ToUpper())
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        public static List<SelectListItem> BindDropDown(DataTable dt)
        {
            var listItem = new List<SelectListItem>();
            foreach (DataRow dataRow in dt.Rows)
            {
                listItem.Add(new SelectListItem
                {
                    Value = dataRow["CODE"].ToString().Trim(),
                    Text = dataRow["NAME"].ToString().Trim()
                });
            }
            return listItem;
        }
        public static string Encrypt(string inputText)
        {
            string encryptionkey = "SAUW193BX628TD57";
            byte[] keybytes = Encoding.ASCII.GetBytes(encryptionkey.Length.ToString());
            RijndaelManaged rijndaelCipher = new();
            byte[] plainText = Encoding.Unicode.GetBytes(inputText);
            PasswordDeriveBytes pwdbytes = new(encryptionkey, keybytes);
            using (ICryptoTransform encryptrans = rijndaelCipher.CreateEncryptor(pwdbytes.GetBytes(32), pwdbytes.GetBytes(16)))
            {
                using (MemoryStream mstrm = new())
                {
                    using (CryptoStream cryptstm = new(mstrm, encryptrans, CryptoStreamMode.Write))
                    {
                        cryptstm.Write(plainText, 0, plainText.Length);
                        cryptstm.Close();
                        return Convert.ToBase64String(mstrm.ToArray());
                    }
                }
            }
        }
        public static string Decrypt(string encryptText)
        {
            string encryptionkey = "SAUW193BX628TD57";
            byte[] keybytes = Encoding.ASCII.GetBytes(encryptionkey.Length.ToString());
            RijndaelManaged rijndaelCipher = new();
            byte[] encryptedData = Convert.FromBase64String(encryptText.Replace(" ", "+"));
            PasswordDeriveBytes pwdbytes = new(encryptionkey, keybytes);
            using (ICryptoTransform decryptrans = rijndaelCipher.CreateDecryptor(pwdbytes.GetBytes(32), pwdbytes.GetBytes(16)))
            {
                using (MemoryStream mstrm = new(encryptedData))
                {
                    using (CryptoStream cryptstm = new(mstrm, decryptrans, CryptoStreamMode.Read))
                    {
                        byte[] plainText = new byte[encryptedData.Length];
                        int decryptedCount = cryptstm.Read(plainText, 0, plainText.Length);
                        return Encoding.Unicode.GetString(plainText, 0, decryptedCount);
                    }
                }
            }
        }
        public static string GenerateQrCodeByText(string qrCodeNo)
        {
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeNo, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(3);
            var msArray = BitmapToBytes(qrCodeImage);
            return "data:image/png;base64," + Convert.ToBase64String(msArray);
        }
        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using MemoryStream stream = new();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
        public static string GenerateBarCodeByText(string barCodeNo)
        {
            using MemoryStream ms = new();
            //The Image is drawn based on length of Barcode text.
            using Bitmap bitMap = new(barCodeNo.Length * 40, 80);
            //The Graphics library object is generated for the Image.
            using (Graphics graphics = Graphics.FromImage(bitMap))
            {
                //The installed Barcode font.
                Font oFont = new("IDAutomationHC39M Free Version", 16);
                PointF point = new(2f, 2f);

                //White Brush is used to fill the Image with white color.
                SolidBrush whiteBrush = new(Color.White);
                graphics.FillRectangle(whiteBrush, 0, 0, bitMap.Width, bitMap.Height);

                //Black Brush is used to draw the Barcode over the Image.
                SolidBrush blackBrush = new(Color.Black);
                graphics.DrawString("*" + barCodeNo + "*", oFont, blackBrush, point);
            }

            //The Bitmap is saved to Memory Stream.
            bitMap.Save(ms, ImageFormat.Png);

            //The Image is finally converted to Base64 string.
            return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
        }        
        public static bool SaveFile(IFormFile files, string[] extension, string path, string file)
        {
            if (files.Length > 0)
            {
                var fileName = Path.GetFileName(files.FileName);
                var fileName1 = file;
                var ext = fileName.Split(".")[1];
                foreach (string x in extension)
                {
                    if (x == ext)
                    {
                        var filepath = path + fileName1;
                        using (FileStream fs = File.Create(filepath))
                        {
                            files.CopyTo(fs);
                            fs.Flush();
                        }
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        public static bool SaveFile1(IFormFile files, string path, string file)
        {
            if (files.Length > 0)
            {
                var filepath = path + file;
                using (FileStream fs = File.Create(filepath))
                {
                    files.CopyTo(fs);
                    fs.Flush();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public static int CompareDate(string lesserThanDate, string greaterThanDate)
        {
            DateTime ldate = Convert.ToDateTime(lesserThanDate);
            DateTime gDate = Convert.ToDateTime(greaterThanDate);
            return DateTime.Compare(gDate, ldate);
        }
        public static int CompareDate(DateTime lesserThanDate, string greaterThanDate)
        {
            DateTime gdate = Convert.ToDateTime(greaterThanDate);
            return DateTime.Compare(gdate, lesserThanDate);
        }
        public static bool IsValidExtnForKpi(string extn)
        {
            string[] supportedTypes = new[] { ".txt", ".doc", ".docx", ".pdf", ".xls", ".xlsx", ".png", ".jpg", ".jpeg", ".gif", ".csv" };
            return supportedTypes.Contains(extn.ToLower());
        }
        public static bool IsValidExtnForDT(string extn)
        {
            string[] supportedTypes = new[] { ".pdf", ".png", ".jpg", ".jpeg", ".gif" };
            return supportedTypes.Contains(extn.ToLower());
        }
        public static string GetContentType(string path)
        {
            Dictionary<string, string> types = GetMimeTypes();
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        public static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.ms-excel"},
                //{".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}

using DVS.Common.Models;
using DVS.Common.SO;
using OfficeOpenXml;
using QRCoder;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DVS.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utils
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static async Task<bool> GetTaskExecuteLockAsync(string taskName)
        {
            return await RedisHelper.SetAsync(taskName, taskName, 2, CSRedis.RedisExistence.Nx);
        }

        /// <summary>
        /// 身份证号码处理
        /// </summary>
        /// <param name="IdCard"></param>
        /// <returns></returns>
        public static IdCardInfo ValidIdCard(string IdCard)
        {

            var result = new IdCardInfo();
            if (string.IsNullOrWhiteSpace(IdCard) || (IdCard.Length != 18 && IdCard.Length != 15))
            {

                return null;
            }

            //处理18位的身份证号码从号码中得到生日和性别代码
            if (IdCard.Length >= 18)
            {
                result.Birthday = DateTime.Parse(IdCard.Substring(6, 4) + "-" + IdCard.Substring(10, 2) + "-" + IdCard.Substring(12, 2));
                result.Sex = int.Parse(IdCard.Substring(14, 3));
            }
            else if (IdCard.Length == 15)
            {
                result.Birthday = DateTime.Parse("19" + IdCard.Substring(6, 2) + "-" + IdCard.Substring(8, 2) + "-" + IdCard.Substring(10, 2));
                result.Sex = int.Parse(IdCard.Substring(12, 3));
            }
            if (result.Sex % 2 == 0)
            {
                result.Sex = 2; // 女
            }
            else
            {
                result.Sex = 1; // 男
            }



            return result;
        }

        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length = 12, bool useNum = false, bool useLow = false, bool useUpp = false, bool useSpe = false, string custom = "")
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        public static string Decrypt(string ciphertext)
        {
            var content = BasicSO.Decrypt(ciphertext);
            return content;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plain"></param>
        /// <returns></returns>
        public static string Encrypt(string plain)
        {
            var content = BasicSO.Encrypt(plain);
            return content;
        }

        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string MD5(string content)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bytValue, bytHash;
                bytValue = System.Text.Encoding.UTF8.GetBytes(content);
                bytHash = md5.ComputeHash(bytValue);
                md5.Clear();
                string sTemp = "";
                for (int i = 0; i < bytHash.Length; i++)
                {
                    sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
                }
                content = sTemp.ToLower();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return content;
        }

        /// <summary>
        /// excel导入
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colCount"></param>
        /// <returns></returns>
        public static DataSet ImportExcel(Stream fileStream, int rowIndex, int colCount)
        {
            DataSet ds;

            try
            {
                //打开文件
                //FileStream fileStream = new FileStream(filePath, FileMode.Open);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //读取文件流
                ExcelPackage package = new ExcelPackage(fileStream);

                //获取 sheet 表
                ExcelWorksheets worksheets = package.Workbook.Worksheets;

                ExcelWorksheet worksheet = null;

                ds = new DataSet();
                DataTable dt = null;

                for (int i = 1; i <= worksheets.Count; i++)
                {
                    dt = new DataTable();
                    dt.TableName = "table" + i.ToString();

                    worksheet = worksheets[i - 1];

                    //获取行数
                    int rowCount = worksheet.Dimension.End.Row;

                    ////获取列数
                    //int colCount = worksheet.Dimension.End.Column;

                    ////起始行为 1 
                    //int rowIndex = worksheet.Dimension.Start.Row;

                    //起始列为 1 
                    int colIndex = worksheet.Dimension.Start.Column;

                    DataColumn dc = null;

                    for (int j = colIndex; j <= colCount; j++)
                    {
                        dc = new DataColumn(worksheet.Cells[rowIndex, j].Value?.ToString());
                        dt.Columns.Add(dc);
                    }

                    rowIndex++;

                    for (int k = rowIndex; k <= rowCount; k++)
                    {
                        DataRow dr = dt.NewRow();

                        for (int l = colIndex; l <= colCount; l++)
                        {
                            if (worksheet.GetValue(k, l) == null)
                            {
                                continue;
                            }

                            dr[l - 1] = worksheet.GetValue(k, l).ToString();
                        }

                        dt.Rows.Add(dr);
                    }

                    ds.Tables.Add(dt);
                }

                package.Dispose();

                worksheet = null;
                worksheets = null;
                package = null;

                fileStream.Close();
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return ds;
        }

        /// <summary>
        /// excel导入
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <param name="rowIndex">起始行数</param>
        /// <param name="colCount">列数</param>
        /// <returns></returns>
        public static DataSet ImportExcel(string filePath, int rowIndex, int colCount)
        {
            DataSet ds;
            try
            {
                //打开文件
                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                ds = ImportExcel(fileStream, rowIndex, colCount);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return ds;
        }

        const string KEY_64 = "Sun!@#$&";// 8个字符
        const string IV_64 = "Sun!@#$&";
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DESEncrypt(string data)
        {
            byte[] byKey = Encoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = Encoding.ASCII.GetBytes(IV_64);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DESDecrypt(string data)
        {
            byte[] byKey = Encoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = Encoding.ASCII.GetBytes(IV_64);
            byte[] byEnc = Convert.FromBase64String(data);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// URL生成二维码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetQRCodeImage(string url)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            Image image = new Bitmap(320, 320);
            Graphics graphics = Graphics.FromImage(image);
            graphics.DrawImage(qrCodeImage, new Rectangle(0, 0, 320, 320), 0, 0, qrCodeImage.Width, qrCodeImage.Height, GraphicsUnit.Pixel);

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
        }

        /// <summary>
        /// 拆分地址
        /// </summary>
        /// <param name="addressString"></param>
        /// <returns></returns>
        public static string[] SplitAddress(string addressString)
        {
            // String regex = "(?<province>[^省]+省|.+自治区)(?<city>[^自治州]+自治州|[^市]+市|[^盟]+盟|[^地区]+地区|.+区划)(?<county>[^市]+市|[^县]+县|[^旗]+旗|.+区)?(?<town>[^区]+区|.+镇)?(?<village>.*)";
            String regex = "(?<province>[^省]+省|.+自治区)(?<city>[^自治州]+自治州|[^市]+市|[^盟]+盟|[^地区]+地区|.+区划)(?<district>[^市]+市|[^县]+县|[^旗]+旗|.+区)?(?<address>.*)";
            var match = Regex.Match(addressString, regex);
            var province = match.Groups["province"];
            var city = match.Groups["city"];
            var district = match.Groups["district"];
            var address = match.Groups["address"];
            return new string[] { province.Value, city.Value, district.Value, address.Value };
        }


        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <returns>MD5值</returns>
        public static string GetMD5HashFromFile(string filePath)
        {
            try
            {
                FileStream stream = File.OpenRead(filePath);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                stream.Close();
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static string FilterKeyword(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Replace("'", "").Replace("\"", "").Replace("-", "").Replace("delete", "").Replace("update", "").Replace("set", "").Replace(",", "");
            }
            return keyword;
        }


       
    }
}

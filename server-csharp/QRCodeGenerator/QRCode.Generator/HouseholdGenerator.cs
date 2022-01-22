using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SLQRCode.Model;
using SLQRCode.Model.Common;
using QRCoder;
using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace SLQRCode.Generator
{
    public class HouseholdGenerator
    {
        IConfiguration configuration;
        public HouseholdGenerator(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public async Task<ResultInfo<List<FileCleanTask>>> CleanFileTask()
        {
            try
            {
                HttpClient http = new HttpClient();
                HttpResponseMessage message = await http.PostAsync(configuration["AppSettings:dvsServer"] + "/api/village/console/getQRCodeFileCleanTask", new StringContent(JsonConvert.SerializeObject(new { }), Encoding.UTF8, "application/json"));
                var value = await message.Content.ReadAsStringAsync();
                ResultInfo<List<FileCleanTask>> resultInfo = JsonConvert.DeserializeObject<ResultInfo<List<FileCleanTask>>>(value);
                return resultInfo;
            }
            catch (Exception ex)
            {
                return new ResultInfo<List<FileCleanTask>>().SetValue(null, ex.Message);
            }
        }

        public async Task NotifyFileDeleted(string taskId)
        {
            try
            {
                HttpClient http = new HttpClient();
                HttpResponseMessage message = await http.PostAsync(configuration["AppSettings:dvsServer"] + "/api/village/console/notifyFileCleanTaskCompleted", new StringContent(JsonConvert.SerializeObject(new { taskId }), Encoding.UTF8, "application/json"));
                var value = await message.Content.ReadAsStringAsync();
                JsonConvert.DeserializeObject<ResultInfo<List<FileCleanTask>>>(value);
            }
            catch (Exception ex)
            {

            }
        }

        #region 通知二维码生成完成
        public async Task<ResultInfo<int>> NotifyTaskComplete(int taskId, int fileId)
        {
            try
            {
                HttpClient http = new HttpClient();
                string zipFileName = taskId + ".zip";
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, zipFileName);
                HttpResponseMessage message = await http.PostAsync(configuration["AppSettings:dvsServer"] + "/api/village/console/notifyHouseholdQRCodeCompleted", new StringContent(JsonConvert.SerializeObject(new { taskId, fileId }), Encoding.UTF8, "application/json"));
                var value = await message.Content.ReadAsStringAsync();
                Console.WriteLine(value);
                ResultInfo<int> data = JsonConvert.DeserializeObject<ResultInfo<int>>(value);
                return data;
            }
            catch (Exception ex)
            {
                return new ResultInfo<int>().SetValue(0, ex.Message);
            }
        }
        #endregion

        public Stream GetHouseholdZipFile(int taskId)
        {
            string zipFileName = taskId + ".zip";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, zipFileName);
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            return fileStream;
        }


        #region 上传二维码
        public async Task<ResultInfo<int>> UploadQRCode(int taskId)
        {
            try
            {
                //压缩，通知主进程压缩完成
                string zipFileName = taskId + ".zip";
                HttpClient httpClient = new HttpClient();
                string notifyUrl = configuration["AppSettings:sunfsServer"] + "/api/sunfs/upload";
                MultipartFormDataContent multipartContent = new MultipartFormDataContent();
                //var formData = new StringContent(zipFileName, Encoding.UTF8, "text/plain");
                FileStream fileStream = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, zipFileName), FileMode.Open);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                ByteArrayContent fileContent = new ByteArrayContent(buffer);
                //添加文件
                multipartContent.Add(fileContent, "files", taskId + ".zip");
                //添加参数
                // multipartContent.Add(formData, "files");
                multipartContent.Add(new StringContent("qrcode", Encoding.UTF8), "src");
                HttpResponseMessage res = await httpClient.PostAsync(notifyUrl, multipartContent);
                string value = await res.Content.ReadAsStringAsync();
                fileStream.Close();
                if (File.Exists(zipFileName))
                {
                    File.Delete(zipFileName);
                }
                return new ResultInfo<int>().SetValue(JsonConvert.DeserializeObject<ResultInfo<List<QRCodeFileInfo>>>(value).Data[0].Id);
            }
            catch (Exception ex)
            {
                return new ResultInfo<int>().SetValue(0, ex.Message);
            }
        }
        #endregion

        #region 下载二维码模板
        public async Task<Image> DownloadTemplateImage(QRCodeTask task)
        {
            try
            {
                if (task.TaskType != 2)
                {
                    HttpClient httpClient = new HttpClient();
                    Stream imageStream = await httpClient.GetStreamAsync(task.Template.BackgroundUrl);
                    Image image = Image.FromStream(imageStream);
                    return image;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region 生成二维码
        public async Task<ResultInfo<bool>> GenerateQRCode(QRCodeTask data)
        {
            try
            {

                string qrCodeDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, data.TaskId.ToString());
                if (!Directory.Exists(qrCodeDir))
                {
                    Directory.CreateDirectory(qrCodeDir);
                }
                List<QRCodeContent> contents = data.QRCodeContents;
                if (contents.Count > 0)
                {
                    Image background = await DownloadTemplateImage(data);
                    foreach (QRCodeContent content in contents)
                    {
                        Image qrCodeImage = GetHouseholdCodeImage(content, data.Template, background,data.TaskType);
                        qrCodeImage.Save(qrCodeDir + "/" + content.AreaName + content.HouseName + content.HouseNumber + content.HouseholderName + ".jpg");
                    }
                    background?.Dispose();
                    string zipFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{data.TaskId}.zip");
                    if (File.Exists(zipFile))
                    {
                        File.Delete(zipFile);
                    }
                    ZipFile.CreateFromDirectory(qrCodeDir, zipFile);
                    Directory.Delete(qrCodeDir, true);
                    return new ResultInfo<bool>().SetValue(true);
                }
                else
                {
                    return new ResultInfo<bool>().SetValue(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ResultInfo<bool>().SetValue(false, ex.Message, -1);
            }
        }

        public Image GetQRCodeImage(QRCodeContent content)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content.Url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            Image img = new Bitmap(320, 320);
            Graphics graphics = Graphics.FromImage(img);

            graphics.DrawImage(qrCodeImage, new Rectangle(0, 0, 320, 320), 0, 0, qrCodeImage.Width, qrCodeImage.Height, GraphicsUnit.Pixel);
            return img;
        }


        public Image GetDefaultTemplateHouseholdCode(QRCodeContent content,int taskType)
        {
            // 二维码
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content.Url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            if (taskType == 2) {
                return qrCodeImage;
            }
            // 底图
            Image img = new Bitmap(680, 383);

            // 开始画图
            Graphics graphics = Graphics.FromImage(img);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.High;
            SolidBrush bgBrush = new SolidBrush(ColorTranslator.FromHtml("#123D8E"));
            graphics.FillRectangle(bgBrush, new Rectangle(0, 0, img.Width, img.Height));
            Pen linePen = new Pen(new SolidBrush(ColorTranslator.FromHtml("#FFFFFF")), 3);
            //圆角矩形
            int span = 34;
            graphics.DrawPath(linePen, CreateRoundedRectanglePath(new Rectangle(span, span, img.Width - span * 2, img.Height - span * 2), 10));

            //画分割线
            int splitLineY = 138;
            graphics.DrawLine(linePen, new Point(34, splitLineY), new Point(img.Width - 34, splitLineY));

            // 四个角的圈
            int padding = 18;
            float radius = 8;
            graphics.DrawEllipse(new Pen(new SolidBrush(ColorTranslator.FromHtml("#33ffffff")), 2), new RectangleF(padding, padding, radius, radius));
            graphics.DrawEllipse(new Pen(new SolidBrush(ColorTranslator.FromHtml("#33ffffff")), 2), new RectangleF(img.Width - padding - radius, padding, radius, radius));
            graphics.DrawEllipse(new Pen(new SolidBrush(ColorTranslator.FromHtml("#33ffffff")), 2), new RectangleF(img.Width - padding - radius, img.Height - padding - radius, radius, radius));
            graphics.DrawEllipse(new Pen(new SolidBrush(ColorTranslator.FromHtml("#33ffffff")), 2), new RectangleF(padding, img.Height - padding - radius, radius, radius));

            // 画二维码
            int qrCodeWidth = 150;
            float qrcodeLeft = span * 2 - 4;
            graphics.DrawImage(qrCodeImage, new Rectangle((int)qrcodeLeft, splitLineY + span-4, qrCodeWidth, qrCodeWidth), 0, 0, qrCodeImage.Width, qrCodeImage.Height, GraphicsUnit.Pixel);


            // 画houseName
            Font houseNameFont = new Font(new FontFamily("Alibaba PuHuiTi"), 52, FontStyle.Regular, GraphicsUnit.Point);
            string houseName = content.HouseName.Trim();
            int contentLength = houseName.Length;

            SizeF sizeHouseName = graphics.MeasureString(houseName, houseNameFont);

            bool addString = false;
            if (houseName.EndsWith("栋"))
            {
                addString = true;
            }
            if (addString)
            {// 字符一个一个画
                float householdNameWidth = 0;
                for (int i = 0; i < houseName.Length; i++)
                {
                    char name = houseName[i];
                    float width = graphics.MeasureString(name.ToString(), houseNameFont).Width;
                    householdNameWidth += width;
                }
                int houseNameX = (int)(img.Width / 2 - householdNameWidth / 2);
                for (int i = 0; i < houseName.Length; i++)
                {
                    char name = houseName[i];
                    float width = graphics.MeasureString(name.ToString(), houseNameFont).Width;
                    graphics.DrawString(name.ToString(), houseNameFont, new SolidBrush(Color.White), houseNameX, splitLineY - sizeHouseName.Height - 3);
                    houseNameX += (int)width;
                }
            }
            else
            {
                graphics.DrawString(houseName, houseNameFont, new SolidBrush(Color.White), img.Width / 2 - sizeHouseName.Width / 2, splitLineY - sizeHouseName.Height-3);
            }
            // 画houseNumber
            Font houseNumberFont = new Font(new FontFamily("Alibaba PuHuiTi"), 130, FontStyle.Bold, GraphicsUnit.Point);
            string houseNumber = content.HouseNumber;
            SizeF sizeHouseNumber = graphics.MeasureString(houseNumber, houseNumberFont);
            float houseNumberLeft = 432 - sizeHouseNumber.Width / 2;
            graphics.DrawString(houseNumber, houseNumberFont, new SolidBrush(Color.White), houseNumberLeft, splitLineY + span - sizeHouseNumber.Height / 5);
            // 画行政区域
            Font areaCodeFont = new Font(new FontFamily("Alibaba PuHuiTi"), 10, FontStyle.Regular, GraphicsUnit.Point);
            string areaCode = "No." + content.AreaCode.ToString();
            float sizeAreaCodeLeft = qrcodeLeft;
            graphics.DrawString(getQrNumber(content), areaCodeFont, new SolidBrush(Color.White), sizeAreaCodeLeft, splitLineY + span + qrCodeWidth );

            return img;

        }

        public Image GetDefaultHouseholdCodeImage(QRCodeContent content)
        {
            // 二维码
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content.Url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            // 底图
            Image img = new Bitmap(680, 269);

            // 开始画图
            Graphics graphics = Graphics.FromImage(img);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.High;
            SolidBrush bgBrush = new SolidBrush(ColorTranslator.FromHtml("#183e97"));
            graphics.FillRectangle(bgBrush, new Rectangle(0, 0, img.Width, img.Height));
            graphics.DrawPath(new Pen(new SolidBrush(ColorTranslator.FromHtml("#2472B9")), 4), CreateIntersetRoundRectanglePath(new Rectangle(32, 32, img.Width - 64, img.Height - 64), 22));

            int padding = 13;
            float radius = 8;
            graphics.DrawEllipse(new Pen(new SolidBrush(Color.White), 1), new RectangleF(padding, padding, radius, radius));
            graphics.DrawEllipse(new Pen(new SolidBrush(Color.White), 1), new RectangleF(img.Width - padding - radius, padding, radius, radius));
            graphics.DrawEllipse(new Pen(new SolidBrush(Color.White), 1), new RectangleF(img.Width - padding - radius, img.Height - padding - radius, radius, radius));
            graphics.DrawEllipse(new Pen(new SolidBrush(Color.White), 1), new RectangleF(padding, img.Height - padding - radius, radius, radius));

            // 画二维码
            int qrCodeWidth = 122;
            float qrcodeLeft = 122;
            graphics.DrawImage(qrCodeImage, new Rectangle((int)qrcodeLeft, 59, qrCodeWidth, qrCodeWidth), 0, 0, qrCodeImage.Width, qrCodeImage.Height, GraphicsUnit.Pixel);

            // 画houseName
            Font houseNameFont = new Font(new FontFamily("Alibaba PuHuiTi"), 36, FontStyle.Regular, GraphicsUnit.Point);
            string houseName = content.HouseName.Trim();
            int contentLength = houseName.Length;
            bool addString = false;
            if (houseName.EndsWith("栋"))
            {
                houseName += ".";
                addString = true;
            }
            SizeF sizeHouseName = graphics.MeasureString(houseName, houseNameFont);

            if (contentLength > 3)
            {
                float houseNameLeft = 448 - sizeHouseName.Width / 2;
                graphics.DrawString(houseName, houseNameFont, new SolidBrush(Color.White), houseNameLeft, 50);
                if (addString)
                {
                    graphics.FillRectangle(bgBrush, new RectangleF(houseNameLeft + sizeHouseName.Width - 12, 56, 10, sizeHouseName.Height - 6));
                }
            }
            else
            {
                float fontDist = 40;
                float houseNameWidth = 0;
                for (int i = 0; i < houseName.Length; i++)
                {
                    char name = houseName[i];
                    float width = graphics.MeasureString(name.ToString(), houseNameFont).Width;
                    if (i > 0)
                    {
                        houseNameWidth += fontDist;
                    }
                    houseNameWidth += width;
                }

                // 计算开始位置
                float beginPosition = 448 - houseNameWidth / 2 - 3 * houseName.Length;
                for (int i = 0; i < houseName.Length; i++)
                {
                    char name = houseName[i];
                    float width = graphics.MeasureString(name.ToString(), houseNameFont).Width;
                    graphics.DrawString(name.ToString(), houseNameFont, new SolidBrush(Color.White), beginPosition, 50);
                    beginPosition += width + fontDist;
                }

            }
            // 画houseNumber
            Font houseNumberFont = new Font(new FontFamily("Alibaba PuHuiTi"), 72, FontStyle.Bold, GraphicsUnit.Point);
            string houseNumber = content.HouseNumber;
            SizeF sizeHouseNumber = graphics.MeasureString(houseNumber, houseNumberFont);
            float houseNumberLeft = 448 - sizeHouseNumber.Width / 2;
            graphics.DrawString(houseNumber, houseNumberFont, new SolidBrush(Color.White), houseNumberLeft, sizeHouseName.Height + 36);
            // 画行政区域
            Font areaCodeFont = new Font(new FontFamily("Alibaba PuHuiTi"), 10, FontStyle.Regular, GraphicsUnit.Point);
            string areaCode = "No." + content.AreaCode.ToString();
            float sizeAreaCodeLeft = qrcodeLeft;
            graphics.DrawString(getQrNumber(content), areaCodeFont, new SolidBrush(Color.White), sizeAreaCodeLeft, qrCodeWidth + 64);

            return img;

        }

        /// <summary>
        /// 绘制二维码
        /// </summary>
        /// <param name="content"></param>
        /// <param name="task"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public Image GetHouseholdCodeImage(QRCodeContent content, QRCodeTemplate template, Image background,int taskType=1)
        {
            if (template == null)
            {
                return GetDefaultTemplateHouseholdCode(content,taskType);
                //return GetDefaultHouseholdCodeImage(content);
            }
            Bitmap qrCodeImage;
            // 二维码

            if (content.TemplateImage)
            {
                qrCodeImage = global::SLQRCode.Generator.Properties.Resources.QRCode;
            }
            else
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(content.Url, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                qrCodeImage = qrCode.GetGraphic(20);
            }

            if (taskType == 2) {
                return qrCodeImage;
            }
            // 底图
            Image img = background.Clone() as Image;
            

            // 开始画图
            Graphics graphics = Graphics.FromImage(img);
 
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.High;

            // 画二维码
            int qrCodeWidth = template.QrCodeWidth;
            int qrCodeHeight = template.QrCodeHeight;

            graphics.DrawImage(qrCodeImage, new Rectangle(template.QrCodeXaxis, template.QrCodeYaxis, qrCodeWidth, qrCodeHeight), 0, 0, qrCodeImage.Width, qrCodeImage.Height, GraphicsUnit.Pixel);


            // 画houseName
            Font houseNameFont = new Font(new FontFamily("Alibaba PuHuiTi"), template.HouseNameFontSize, FontStyle.Regular, GraphicsUnit.Point);
            string houseName = content.HouseName.Trim();

            int houseNameX = template.HouseNameXaxis;
            int houseNameY = template.HouseNameYaxis;

            bool addString = false;
            if (houseName.EndsWith("栋"))
            {
                addString = true;
            }
            if (addString)
            {// 字符一个一个画
                for (int i = 0; i < houseName.Length; i++)
                {
                    char name = houseName[i];
                    float width = graphics.MeasureString(name.ToString(), houseNameFont).Width;
                    graphics.DrawString(name.ToString(), houseNameFont, new SolidBrush(Color.White), houseNameX, houseNameY);
                    houseNameX += (int)width;
                }
            }
            else
            {
                graphics.DrawString(houseName, houseNameFont, new SolidBrush(Color.White), template.HouseNameXaxis, template.HouseNameYaxis);
            }
            // 画houseNumber
            Font houseNumberFont = new Font(new FontFamily("Alibaba PuHuiTi"), template.HouseNumberFontSize, FontStyle.Bold, GraphicsUnit.Point);
            string houseNumber = content.HouseNumber;
            SizeF sizeHouseNumber = graphics.MeasureString(houseNumber, houseNumberFont);
            graphics.DrawString(houseNumber, houseNumberFont, new SolidBrush(Color.White), template.HouseNumberXaxis, template.HouseNumberYaxis);
            // 画行政区域
            if (template.QrCodeNoShow == 1)
            {
                Font areaCodeFont = new Font(new FontFamily("Alibaba PuHuiTi"), template.QrCodeNoFontSize, FontStyle.Regular, GraphicsUnit.Point);
                string areaCode = "No." + content.AreaCode.ToString();
                graphics.DrawString(getQrNumber(content), areaCodeFont, new SolidBrush(Color.White), template.QrCodeNoXaxis, template.QrCodeNoYaxis);
            }
            
            return img;
        }

        private static string getQrNumber(QRCodeContent content)
        {
            if (content == null) {
                return "NO."+"".PadLeft(16,'0');


            }
            return "NO." + (content.AreaCode + content.HouseNumber.PadLeft(4, '0')).PadLeft(16, '0');
        }
        #endregion

        #region 获取生成任务
        public async Task<ResultInfo<QRCodeTask>> GetQRCodeTask()
        {
            try
            {
                HttpClient http = new HttpClient();
                HttpResponseMessage message = await http.PostAsync(configuration["AppSettings:dvsServer"] + "/api/village/console/getHouseholdGenTask", new StringContent(JsonConvert.SerializeObject(new { }), Encoding.UTF8, "application/json"));
                var value = await message.Content.ReadAsStringAsync();
                Console.WriteLine(value);
                ResultInfo<HouseholdeTask> data = JsonConvert.DeserializeObject<ResultInfo<HouseholdeTask>>(value);
                if (data == null || data.Data == null)
                {
                    return new ResultInfo<QRCodeTask>().SetValue(null);
                }
                QRCodeTask task = new QRCodeTask();
                task.Template = data.Data.Template;
                task.TaskId = data.Data.TaskId;
                task.TaskType = data.Data.TaskType;
                task.QRCodeContents = JsonConvert.DeserializeObject<List<QRCodeContent>>(data.Data.CodeData);
                return new ResultInfo<QRCodeTask>().SetValue(task);
            }
            catch (Exception ex)
            {
                return new ResultInfo<QRCodeTask>().SetValue(null, ex.Message);
            }
        }
        #endregion


        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;

        }

        private GraphicsPath CreateIntersetRoundRectanglePath(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X - cornerRadius, rect.Y - cornerRadius, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.X, rect.Y + cornerRadius, rect.X, rect.Y + rect.Height - cornerRadius);
            roundedRect.AddArc(rect.X - cornerRadius, rect.Y + rect.Height - cornerRadius, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y + rect.Height, rect.Right - cornerRadius, rect.Y + rect.Height);
            roundedRect.AddArc(rect.Right - cornerRadius, rect.Bottom - cornerRadius, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.Right, rect.Bottom - cornerRadius, rect.Right, rect.Y + cornerRadius);
            roundedRect.AddArc(rect.Right - cornerRadius, rect.Y - cornerRadius, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.CloseFigure();
            return roundedRect;
        }
    }
}

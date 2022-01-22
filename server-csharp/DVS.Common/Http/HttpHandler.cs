using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DVS.Common.Http
{
    public static class HttpHandler
    {

        public async static Task<HttpResponseMessage> GetAsync(this IHttpClientFactory factory, string url)
        {
            HttpClient client = factory.CreateClient();
            HttpResponseMessage message = await client.GetAsync(url);
            return message;
        }

        public async static Task<HttpResponseMessage> PostAsync(this IHttpClientFactory factory, string url, object data,Action<HttpRequestHeaders> addHeader=null)
        {
            if (data == null)
            {
                throw new NullReferenceException("请提供需要POST的数据");
            }
            HttpClient client = factory.CreateClient();
            addHeader?.Invoke(client.DefaultRequestHeaders);
            StringContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            HttpResponseMessage message = await client.PostAsync(url, content);
            return message;
        }
        public async static Task<Stream> GetStreamAsync(this IHttpClientFactory factory, string url)
        {
            HttpClient client = factory.CreateClient();
            Stream message = await client.GetStreamAsync(url);

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            return null;
        }


        public async static Task<HttpResponseMessage> PostAsync(this IHttpClientFactory factory, string url, string filePath)
        {
            try
            {
                HttpClient client = factory.CreateClient();
                MultipartFormDataContent content = new MultipartFormDataContent();
                string fileName = Path.GetFileName(filePath);
                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                byte[] buffer = new byte[fileStream.Length];
                await fileStream.ReadAsync(buffer, 0, buffer.Length);
                ByteArrayContent fileContent = new ByteArrayContent(buffer);
                content.Add(fileContent, "file", fileName);
                HttpResponseMessage message = await client.PostAsync(url, content);
                fileStream.Close();
                return message;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async static Task<HttpResponseMessage> UploadFilesAsync(this IHttpClientFactory factory, string url, string filePath,string src="")
        {
            try
            {
                HttpClient client = factory.CreateClient();
                MultipartFormDataContent content = new MultipartFormDataContent();
                string fileName = Path.GetFileName(filePath);
                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                byte[] buffer = new byte[fileStream.Length];
                await fileStream.ReadAsync(buffer, 0, buffer.Length);
                ByteArrayContent fileContent = new ByteArrayContent(buffer);
                content.Add(fileContent, "files", fileName);
                StringContent stringContent = new StringContent(src, Encoding.UTF8);
                content.Add(stringContent, "src");
                StringContent isMaterialContent = new StringContent("false", Encoding.UTF8);
                content.Add(isMaterialContent, "isMatermail");
                HttpResponseMessage message = await client.PostAsync(url, content);
                fileStream.Close();
                return message;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async static Task<HttpResponseMessage> PutAsync(this IHttpClientFactory factory, string url, object data, Action<HttpRequestHeaders> addHeader = null)
        {
            if (data == null)
            {
                throw new NullReferenceException("请提供需要POST的数据");
            }
            HttpClient client = factory.CreateClient();
            addHeader?.Invoke(client.DefaultRequestHeaders);
            StringContent content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            HttpResponseMessage message = await client.PutAsync(url, content);
            return message;
        }

        public async static Task<HttpResponseMessage> DeleteAsync(this IHttpClientFactory factory, string url, Action<HttpRequestHeaders> addHeader = null)
        {
            HttpClient client = factory.CreateClient();
            addHeader?.Invoke(client.DefaultRequestHeaders);
            HttpResponseMessage message = await client.DeleteAsync(url);
            return message;
        }
    }
}

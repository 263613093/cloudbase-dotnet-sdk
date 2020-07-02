using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CloudBase
{
    public class Request
    {
        private const string SdkVersion = "0.0.1";
        private const string DataVersion = "2019-06-01";
        private const string TcbWebUrl = "https://tcb-api.tencentcloudapi.com/web";

        private Core core;
        private HttpClient _client;

        public Request(Core core)
        {
            this.core = core;
        }

        public HttpClient GetClient()
        {
            if (this._client == null)
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                client.DefaultRequestHeaders.Add("user-agent", $"cloudbase-csharp-sdk/{SdkVersion}");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromMilliseconds(this.core.Timeout);
                _client = client;
            }
            return _client;
        }

        public T Post<T>(string action, Dictionary<string, object> data)
        {
            var task = this.PostAsync<T>(action, data);
            task.Wait();
            return task.GetAwaiter().GetResult();
        }

        public async Task<T> PostAsync<T>(string action, Dictionary<string, object> data)
        {
            if (this.core.Auth == null)
            {
                return await this.PostWithoutAuthAsync<T>(action, data);
            }

            string accessToken = await this.core.Auth.GetAccessTokenAsync();

            data.Add("action", action);
            data.Add("env", this.core.Env);
            data.Add("sdk_version", Request.SdkVersion);
            data.Add("dataVersion", Request.DataVersion);
            data.Add("access_token", accessToken);

            string json = JsonConvert.SerializeObject(data);
            var res = await this.HttpRequestAsync(json);

            return (T)Activator.CreateInstance(typeof(T), new object[] { res });
        }

        public async Task<T> PostWithoutAuthAsync<T>(string action, Dictionary<string, object> data)
        {
            data.Add("action", action);
            data.Add("env", this.core.Env);
            data.Add("sdk_version", Request.SdkVersion);
            data.Add("dataVersion", Request.DataVersion);

            string json = JsonConvert.SerializeObject(data);
            var res = await this.HttpRequestAsync(json);
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { res });
            return instance;
        }

        private async Task<object> HttpRequestAsync(string data)
        {
            var watch = new Stopwatch();
            watch.Start();
            HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage res = await this.GetClient().PostAsync($"{Request.TcbWebUrl}?env={this.core.Env}", content);
                res.EnsureSuccessStatusCode();
                string resString = await res.Content.ReadAsStringAsync();
                var dest = JsonConvert.DeserializeObject(resString);
                return dest;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            finally
            {
                watch.Stop();
                Console.WriteLine($"请求耗时:{watch.ElapsedMilliseconds} ms");
            }
        }

        public async Task<HttpResponseMessage> UploadAsync(string url, string filePath, Dictionary<string, string> data)
        {
            // .net core post form-data 不标准, 需要改造 https://www.mscto.com/aspnet/218955.html

            var form = new MultipartFormDataContent();

            foreach (var item in form.Headers.ContentType.Parameters)
            {
                if (item.Name == "boundary")
                {
                    item.Value = item.Value.Replace("\"", String.Empty);
                }
            }

            // 注入额外参数
            foreach (var item in data)
            {
                StringContent stringContent = new StringContent(item.Value);
                form.Add(stringContent, $"\"{item.Key}\"");
            }


            // 注入文件
            byte[] file = File.ReadAllBytes(filePath);
            ByteArrayContent fileContent = new ByteArrayContent(file);
            form.Add(fileContent, "\"file\"");

            HttpResponseMessage res = await this.GetClient().PostAsync($"{url}?env={this.core.Env}", form);

            return res;
        }

        public async Task DownloadAsync(string url, string filePath)
        {
            byte[] urlContents = await this.GetClient().GetByteArrayAsync(url);
            File.WriteAllBytes(filePath, urlContents);
        }
    }

}
using System;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Storage.Requests
{
    public class RepoRequest
    {
        private static HttpClient _client = new HttpClient();
        private static string _host;
        private static int _port;
        

        public static void SetClient(string host, int port)
        {
            _host = host;
            _port = port;
        }
        
        public static void LoadRepo(string saveId, string token)
        {
            Send($"http://{_host}:{_port}/api/repo/repo/load", saveId, token);
        } 
        
        public static void SaveRepo(string saveId, string token)
        {
            Send($"http://{_host}:{_port}/api/repo/repo/save", saveId, token);
        }

        private static void Send(string uri, string saveId, string token)
        {
            var body = new {Filename = saveId};
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Headers =
                {
                    {"Authorization", token ?? "null"},
                },
                Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.Default, "application/json"),
                RequestUri = new Uri(uri)
            };
            var result = _client.SendAsync(httpRequestMessage).Result;
            if (!result.IsSuccessStatusCode)
            {
                throw new RepoRequestException($"Request failed to repo from storage: {result.StatusCode}");
            }
        }


        public class RepoRequestException : Exception
        {
            public RepoRequestException(string message) : base(message)
            {
            }
        }
    }
}
using System;
using System.Net.Http;

namespace Generator.Requests
{
    public static class RepoRequest
    {
        private static readonly HttpClient Client = new HttpClient();
        private static string _host;
        private static int _port;
        private static string _token;
        

        public static void SetClient(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public static void SetToken(string token)
        {
            _token = token;
        }
        
        public static string GetModelAsString(string model)
        {
            return GetAsString($"http://{_host}:{_port}/api/repo/model/{model}");
        } 
        
        public static string GetElementAsString(string model, int id)
        {
            return GetAsString($"http://{_host}:{_port}/api/repo/element/{model}/{id}");
        }
        
        public static string GetEdgeAsString(string model, int id)
        {
            return GetAsString($"http://{_host}:{_port}/api/repo/element/{model}/{id}/asEdge");
        }

        private static string GetAsString(string uri)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                Headers =
                {
                    {"Authorization", _token ?? "null"},
                },
                RequestUri = new Uri(uri)
            };
            var result = Client.SendAsync(httpRequestMessage).Result;
            if (!result.IsSuccessStatusCode)
            {
                throw new RepoRequestException($"Request failed to repo from generator: {result.StatusCode}");
            }
            return result.Content.ReadAsStringAsync().Result;
        }


        public class RepoRequestException : Exception
        {
            public RepoRequestException(string message) : base(message)
            {
            }
        }
    }
}
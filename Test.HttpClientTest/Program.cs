using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Baltic.Node.BatchManager.Models;

namespace Test.HttpClientTest
{
    class Program
    {
        
        private static readonly HttpClient _client = Init();

        private static HttpClient Init()
        {
            HttpClient client = new HttpClient();
            // TODO set default headers here (if necessary)
            return client;
        }
        
        static void Main(string[] args)
        {
            /* XUnitQuery query = new XUnitQuery()
            {
                OnlyLastRelease = true
            };
            string body = JsonSerializer.Serialize(query);
            
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), "https://localhost:5001/app/list")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            var taskResult = _client.SendAsync(request).Result; */
            
            XTokensAck message = new XTokensAck()
            {
                MsgUids = new List<string>(),
                SenderUid = "xyz"
            };
            string body = JsonSerializer.Serialize(message);

            // string body = "{\"PinName\": \"MongoDBFileWriter\",\"SenderUid\": \"387d67a4-a060-49d1-9c27-5e4636ffdbb5\",\"Values\": {\"db_table_name\": \"images\",\"db_file_uid\": \"5ef62d11b591b4edd8020617\"},\"BaseInputMsgUid\": [],\"IsFinal\": true,\"BaseMsgUid\": \"a62ccac0-a0fa-4501-98e7-01617a85cdb6\"}"; /*JsonSerializer.Serialize(message);*/
            
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), "http://localhost:7000/token")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            var taskResult = _client.SendAsync(request).Result;
            Console.WriteLine(taskResult.Content.ReadAsStringAsync().Result);
            Console.WriteLine(taskResult.StatusCode);
            
            /*HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), "http://localhost:9101/uid?val="+"defgh1234");
            var taskResult = _client.SendAsync(request).Result;
            
            request = new HttpRequestMessage(new HttpMethod("GET"), "http://localhost:9101/uid");
            taskResult = _client.SendAsync(request).Result;
            var result = taskResult.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);*/
        }
    }
}
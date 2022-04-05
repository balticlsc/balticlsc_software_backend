using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Baltic.Node.ModuleManager.Controllers
{
    [Route("/")]
    public class ModuleManagerController : Controller
    {
        private string _batchManagerUrl;
        private IHttpClientFactory _clientFactory;
        private const int NumberOfRetrays = 5;
        private const int WaitTime = 5000; // [ms]
        
        public ModuleManagerController(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _batchManagerUrl = configuration["BatchManagerUrl"];
            _clientFactory = clientFactory;
        }
        
        
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return Ok("This is a BalticLSC ModuleManager");
        }
        
        [HttpGet]
        [Route("/uid/{moduleId}")]
        public async Task<IActionResult> UidGetFromModule([FromRoute]string moduleId)
        {
            Log.Information($"Received GET message on endpoint /uid/{moduleId} from host {Request.Host}");
            var client = _clientFactory.CreateClient("HttpClientWithSSLUntrusted");
            string url = "http://" + moduleId + "/uid";

            int k = 0;

            while (k < NumberOfRetrays)
            {
                try
                {
                    Log.Information($"Attempt to send GET message to {url}");
                    // var result = client.GetStringAsync(url).Result;
                    var result = await client.GetAsync(url);
                    var response = await result.Content.ReadAsStringAsync();
                    Log.Information($"Receive response {result}");
                    return StatusCode((int)result.StatusCode,response);    
                }
                catch (HttpRequestException)
                {
                    k++;
                    Log.Warning($"The server did not respond {k} times to call: {url}");
                    System.Threading.Thread.Sleep(WaitTime);
                }
                catch (TaskCanceledException)
                {
                    k++;
                    Log.Warning($"The server canceled request {k} times to call: {url}");
                    System.Threading.Thread.Sleep(WaitTime);
                }
                catch (Exception e)
                {
                    Log.Error($"Something unexpected have happen. Error stack: \n {e.Message}");
                    throw;
                }
            }
            
            var msg =
                $"A http request {url} could not be perform because the target computer is actively refusing connection."; 
            Log.Error(msg);
            throw new HttpRequestException(msg);
        }
        
        [HttpPost]
        [Route("/uid/{moduleId}")]
        public async Task<IActionResult> UidPostToModule([FromRoute]string moduleId, [FromQuery]string val)
        {
            Log.Information($"Received POST message on endpoint /uid/{moduleId}?val={val} from host {Request.Host}");
            var client = _clientFactory.CreateClient("HttpClientWithSSLUntrusted");
            var url = new Uri($"http://{moduleId}/uid?val={val}");

            int k = 0;
            while (k < NumberOfRetrays)
            {
                try
                {
                    Log.Information($"Attempt to send POST message to {url}");
                    var result = await client.PostAsync(url, new StringContent(""));
                    var response = await result.Content.ReadAsStringAsync();
                    Log.Information($"Receive response {result}");
            
                    return StatusCode((int)result.StatusCode,response);
                }
                catch (HttpRequestException)
                {
                    k++;
                    Log.Warning($"The server did not respond {k} times to call: {url}");
                    System.Threading.Thread.Sleep(WaitTime);
                }
                catch (TaskCanceledException)
                {
                    k++;
                    Log.Warning($"The server canceled request {k} times to call: {url}");
                    System.Threading.Thread.Sleep(WaitTime);
                }
                catch (Exception e)
                {
                    Log.Error($"Something unexpected have happen. Error stack: \n {e.Message}");
                    throw;
                }
            }
            
            var msg =
                $"A http request {url} could not be perform because the target computer is actively refusing connection."; 
            Log.Error(msg);
            throw new HttpRequestException(msg);
        }
        
        [HttpGet]
        [Route("/status/{moduleId}")]
        public async Task<IActionResult> StatusGetFromModule([FromRoute]string moduleId)
        {
            Log.Information($"Received POST message on endpoint /status/{moduleId} from host {Request.Host}");
            
            var client = _clientFactory.CreateClient("HttpClientWithSSLUntrusted");
            
            var url = new Uri($"http://{moduleId}/status");

            int k = 0;

            while (k < NumberOfRetrays)
            {
                try
                {
                    Log.Information($"Attempt to send GET message to {url} with no body");
                    // var response = client.GetAsync(url).Result;
                    var result = await client.GetAsync(url);
                    var response = await result.Content.ReadAsStringAsync();
                    Log.Information($"Receive response {response}");
            
                    return StatusCode((int)result.StatusCode,response);
                }
                catch (HttpRequestException)
                {
                    k++;
                    Log.Warning($"The server did not respond {k} times to call: {url}");
                    System.Threading.Thread.Sleep(WaitTime);
                }
                catch (TaskCanceledException)
                {
                    k++;
                    Log.Warning($"The server canceled request {k} times to call: {url}");
                    System.Threading.Thread.Sleep(WaitTime);
                }
                catch (Exception e)
                {
                    Log.Error($"Something unexpected have happen. Error stack: \n {e.Message}");
                    throw;
                }
            }
            
            var msg =
                $"A http request {url} could not be perform because the target computer is actively refusing connection."; 
            Log.Error(msg);
            throw new HttpRequestException(msg);
        }
        
        [HttpPost]
        [Route("/token/{moduleId}")]
        public async Task<IActionResult> TokenToModule([FromRoute]string moduleId, [FromBody]dynamic json)
        {
            Log.Information($"Received POST message on endpoint /token/{moduleId} from host {Request.Host}");
            Log.Information($"Message body: {json.ToString()}");
            var client = _clientFactory.CreateClient("HttpClientWithSSLUntrusted");

            var url = new Uri($"http://{moduleId}/token");
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            int k = 0;

            while (k < NumberOfRetrays)
            {
                try
                {
                    Log.Information($"Attempt to send POST message to {url} with same body");
                    var result = await client.PostAsync(url, content);
                    var response = await result.Content.ReadAsStringAsync();
                    Log.Information($"Receive response {result}");
            
                    return StatusCode((int)result.StatusCode,response);
                }
                catch (HttpRequestException)
                {
                    k++;
                    Log.Warning($"The server did not respond {k} times to call: {url}");
                    System.Threading.Thread.Sleep(WaitTime);
                }
                catch (TaskCanceledException)
                {
                    k++;
                    Log.Warning($"The server canceled request {k} times to call: {url}");
                    System.Threading.Thread.Sleep(WaitTime);
                }
                catch (Exception e)
                {
                    Log.Error($"Something unexpected have happen. Error stack: \n {e.Message}");
                    throw;
                }
            }
            var msg =
                $"A http request {url} could not be perform because the target computer is actively refusing connection."; 
            Log.Error(msg);
            throw new HttpRequestException(msg);
        }
        
        [HttpPost]
        [Route("/ack")]
        public async Task<IActionResult> AckToManager([FromRoute]string moduleId, [FromBody]dynamic json)
        {
            Log.Information($"Received POST message on endpoint /ack/{moduleId} from host {Request.Host}");
            Log.Information($"Message body: {json.ToString()}");
            var client = _clientFactory.CreateClient("HttpClientWithSSLUntrusted");
            
            var url = new Uri($"{_batchManagerUrl}/ack");
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage()
            {
                RequestUri = url,
                Method = HttpMethod.Post,
                Content = content,
                Version = HttpVersion.Version20
            };

            int k = 0;

            while (k < NumberOfRetrays)
            {
                try
                {
                    Log.Information($"Attempt to send POST message to {url} with same body");
                    var result = await client.SendAsync(request);
                    var response = await result.Content.ReadAsStringAsync();
                    Log.Information($"Receive response {result}");
            
                    return StatusCode((int)result.StatusCode,response);
                }
                catch (HttpRequestException)
                {
                    k++;
                    Log.Warning($"The server did not respond {k} times to call: {url}");
                    System.Threading.Thread.Sleep(WaitTime);
                }
                catch (TaskCanceledException)
                {
                    k++;
                    Log.Warning($"The server canceled request {k} times to call: {url}");
                    System.Threading.Thread.Sleep(WaitTime);
                }
                catch (Exception e)
                {
                    Log.Error($"Something unexpected have happen. Error stack: \n {e.Message}");
                    throw;
                }
            }
            var msg =
                $"A http request {url} could not be perform because the target computer is actively refusing connection."; 
            Log.Error(msg);
            throw new HttpRequestException(msg);
        }
        
        [HttpPost]
        [Route("/token")]
        public async Task<IActionResult> TokenToManager([FromBody]dynamic json)
        {
            Log.Information($"Received POST message on endpoint /token from host {Request.Host}");
            Log.Information($"Message body: {json.ToString()}");
            
            var client = _clientFactory.CreateClient("HttpClientWithSSLUntrusted");
            
            var url = new Uri($"{_batchManagerUrl}/token");
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage()
            {
                RequestUri = url,
                Method = HttpMethod.Post,
                Content = content,
                Version = HttpVersion.Version20
            };
            
            int k = 0;

            while (k < NumberOfRetrays)
            {
                try
                {
                    Log.Information($"Attempt to send POST message to {url} with same body");
                    var result = await client.SendAsync(request);
                    var response = await result.Content.ReadAsStringAsync();
                    Log.Information($"Receive response {result}");
            
                    return StatusCode((int)result.StatusCode,response);
                }
                catch (HttpRequestException)
                {
                    k++;
                    Log.Warning($"The server did not respond {k} times to call: {url}");
                    System.Threading.Thread.Sleep(WaitTime);
                }
                catch (TaskCanceledException)
                {
                    k++;
                    Log.Warning($"The server canceled request {k} times to call: {url}");
                    System.Threading.Thread.Sleep(WaitTime);
                }
                catch (Exception e)
                {
                    Log.Error($"Something unexpected have happen. Error stack: \n {e.Message}");
                    throw;
                }
            }
            var msg =
                $"A http request {url} could not be perform because the target computer is actively refusing connection."; 
            Log.Error(msg);
            throw new HttpRequestException(msg);
        }
    }
}
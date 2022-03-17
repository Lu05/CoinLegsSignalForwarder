using System.Globalization;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RestSharp;
using ILogger = NLog.ILogger;

namespace CoinLegsSignalForwarder.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : Microsoft.AspNetCore.Mvc.Controller
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _config;

        public NotificationController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [HttpPost("Listen")]
        public IActionResult Notify([FromBody] JsonObject content)
        {
            Task.Run(() =>
            {
                var input = content.ToString();
                Logger.Info($"Received {input}");
                var forwards = _config.GetSection("Forwards");
                if (forwards != null)
                {
                    foreach (var configurationSection in forwards.GetChildren())
                    {
                        Task.Run(() =>
                        {
                            //retry count = 5
                            for (int i = 0; i < 5; i++)
                            {
                                var restClient = new RestClient();
                                var value = int.Parse(configurationSection.Value, CultureInfo.InvariantCulture);
                                var request = new RestRequest($"http://localhost:{value}/api/notification/listen");
                                request.AddBody(content);
                                var response = restClient.ExecutePostAsync(request).Result;
                                if (!response.IsSuccessful)
                                {
                                    Logger.Error(response.ErrorException);
                                }
                                else
                                {
                                    Logger.Info($"successfully forwarded to {value}");
                                    break;
                                }
                            }
                        });
                    }
                }
            });

            return Ok();
        }
    }
}
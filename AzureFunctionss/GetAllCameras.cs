using System.Net;
using System.Text.Json;
using AzureFunction.Data;
using AzureFunction.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureFunction
{
    public class GetAllCameras
    {
        private readonly ILogger<GetAllCameras> _logger;
        private readonly IConfiguration _configuration;

        public GetAllCameras(ILogger<GetAllCameras> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Function("GetAllCameras")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "cameras")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to get all cameras.");

            var cameraData = new CameraData("Cameras", _configuration);
            var cameras = await cameraData.GetAllCameras();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(cameras);

            return response;
        }
    }
}
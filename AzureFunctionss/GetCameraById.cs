using System.Net;
using AzureFunction.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureFunction
{
    public class GetCameraById
    {
        private readonly ILogger<GetCameraById> _logger;
        private readonly IConfiguration _configuration;

        public GetCameraById(ILogger<GetCameraById> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Function("GetCameraById")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "camera/{id}")] HttpRequestData req,
            string id)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to get camera with ID: {id}");

            var cameraData = new CameraData("Cameras", _configuration);
            var camera = await cameraData.GetCameraById(id);

            var response = req.CreateResponse();

            if (camera == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                await response.WriteStringAsync($"Camera with ID '{id}' not found.");
                return response;
            }

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteAsJsonAsync(camera);

            return response;
        }
    }
}
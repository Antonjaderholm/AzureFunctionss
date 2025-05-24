using System.Net;
using AzureFunction.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureFunction
{
    public class DeleteCamera
    {
        private readonly ILogger<DeleteCamera> _logger;
        private readonly IConfiguration _configuration;

        public DeleteCamera(ILogger<DeleteCamera> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Function("DeleteCamera")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "camera/{id}")] HttpRequestData req,
            string id)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to delete camera with ID: {id}");

            var cameraData = new CameraData("Cameras", _configuration);
            var success = await cameraData.DeleteCamera(id);

            var response = req.CreateResponse();

            if (!success)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                await response.WriteStringAsync($"No camera found with ID '{id}'.");
                return response;
            }

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteStringAsync($"Camera with ID '{id}' was deleted.");

            return response;
        }
    }
}
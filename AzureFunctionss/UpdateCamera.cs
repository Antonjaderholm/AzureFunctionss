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
    public class UpdateCamera
    {
        private readonly ILogger<UpdateCamera> _logger;
        private readonly IConfiguration _configuration;

        public UpdateCamera(ILogger<UpdateCamera> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Function("UpdateCamera")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "camera/{id}")] HttpRequestData req,
            string id)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to update camera with ID: {id}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var camera = JsonSerializer.Deserialize<Camera>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var response = req.CreateResponse();

            if (camera == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid camera data.");
                return response;
            }

            var cameraData = new CameraData("Cameras", _configuration);
            var updatedCamera = await cameraData.UpdateCamera(id, camera);

            if (updatedCamera == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                await response.WriteStringAsync($"Camera with ID '{id}' not found.");
                return response;
            }

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteAsJsonAsync(updatedCamera);

            return response;
        }
    }
}
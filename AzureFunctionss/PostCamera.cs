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
    public class PostCamera
    {
        private readonly ILogger<PostCamera> _logger;
        private readonly IConfiguration _configuration;

        public PostCamera(ILogger<PostCamera> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Function("PostCamera")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "camera")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to create a new camera.");

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
            var createdCamera = await cameraData.CreateCamera(camera);

            response.StatusCode = HttpStatusCode.Created;
            await response.WriteAsJsonAsync(createdCamera);

            return response;
        }
    }
}
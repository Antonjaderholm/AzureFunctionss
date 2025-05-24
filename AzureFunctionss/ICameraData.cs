using AzureFunction.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureFunction.Data
{
    public interface ICameraData
    {
        Task<List<Camera>> GetAllCameras();
        Task<Camera?> GetCameraById(string id);
        Task<Camera> CreateCamera(Camera camera);
        Task<Camera?> UpdateCamera(string id, Camera cameraIn);
        Task<bool> DeleteCamera(string id);
    }
}
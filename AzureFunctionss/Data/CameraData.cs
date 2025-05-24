using AzureFunction.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureFunction.Data
{
    public class CameraData : ICameraData
    {
        private readonly IMongoCollection<Camera> _collection;
        private readonly ILogger<CameraData>? _logger;

        public CameraData(string collectionName, IConfiguration configuration, ILogger<CameraData>? logger = null)
        {
            _logger = logger;

            var connectionString = configuration["DefaultConnection"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }

            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("CameraDB");
                _collection = database.GetCollection<Camera>(collectionName);

                _logger?.LogInformation("Successfully connected to MongoDB database: CameraDB, collection: {CollectionName}", collectionName);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to connect to MongoDB");
                throw;
            }
        }

        public async Task<List<Camera>> GetAllCameras()
        {
            try
            {
                var cameras = await _collection.Find(_ => true).ToListAsync();
                _logger?.LogInformation("Retrieved {Count} cameras", cameras.Count);
                return cameras;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving all cameras");
                throw;
            }
        }

        public async Task<Camera?> GetCameraById(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                _logger?.LogWarning("Invalid camera ID provided: {Id}", id);
                return null;
            }

            try
            {
                var camera = await _collection.Find(camera => camera.Id == id).FirstOrDefaultAsync();
                if (camera == null)
                {
                    _logger?.LogWarning("Camera not found with ID: {Id}", id);
                }
                return camera;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving camera with ID: {Id}", id);
                throw;
            }
        }

        public async Task<Camera> CreateCamera(Camera camera)
        {
            if (camera == null)
            {
                throw new ArgumentNullException(nameof(camera));
            }

            try
            {
                await _collection.InsertOneAsync(camera);
                _logger?.LogInformation("Successfully created camera with ID: {Id}", camera.Id);
                return camera;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating camera");
                throw;
            }
        }

        public async Task<Camera?> UpdateCamera(string id, Camera cameraIn)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                _logger?.LogWarning("Invalid camera ID provided for update: {Id}", id);
                return null;
            }

            if (cameraIn == null)
            {
                throw new ArgumentNullException(nameof(cameraIn));
            }

            try
            {
                cameraIn.Id = id;
                var result = await _collection.ReplaceOneAsync(camera => camera.Id == id, cameraIn);

                if (result.ModifiedCount == 0)
                {
                    _logger?.LogWarning("No camera found to update with ID: {Id}", id);
                    return null;
                }

                _logger?.LogInformation("Successfully updated camera with ID: {Id}", id);
                return cameraIn;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating camera with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteCamera(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                _logger?.LogWarning("Invalid camera ID provided for deletion: {Id}", id);
                return false;
            }

            try
            {
                var result = await _collection.DeleteOneAsync(camera => camera.Id == id);
                var deleted = result.DeletedCount > 0;

                if (deleted)
                {
                    _logger?.LogInformation("Successfully deleted camera with ID: {Id}", id);
                }
                else
                {
                    _logger?.LogWarning("No camera found to delete with ID: {Id}", id);
                }

                return deleted;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error deleting camera with ID: {Id}", id);
                throw;
            }
        }
    }
}
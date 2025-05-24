using AzureFunction.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureFunction.Data
{
    public class CameraData
    {
        private readonly IMongoCollection<Camera> _collection;

        public CameraData(string collectionName, IConfiguration configuration)
        {
            var connectionString = configuration["DefaultConnection"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("CameraDB");
            _collection = database.GetCollection<Camera>(collectionName);
        }

        public async Task<List<Camera>> GetAllCameras()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Camera> GetCameraById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            return await _collection.Find(camera => camera.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Camera> CreateCamera(Camera camera)
        {
            await _collection.InsertOneAsync(camera);
            return camera;
        }

        public async Task<Camera> UpdateCamera(string id, Camera cameraIn)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            cameraIn.Id = id;
            var result = await _collection.ReplaceOneAsync(camera => camera.Id == id, cameraIn);

            if (result.ModifiedCount == 0)
            {
                return null;
            }

            return cameraIn;
        }

        public async Task<bool> DeleteCamera(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return false;
            }

            var result = await _collection.DeleteOneAsync(camera => camera.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
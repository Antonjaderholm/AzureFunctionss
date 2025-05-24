using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AzureFunction.Models
{
    public class Camera
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public decimal price { get; set; }
        public int megaPixels { get; set; }
    }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PomfSharpCdn.Dtos;
using PomfSharpCdn.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PomfSharpCdn.Managers
{
    public class FileManager
    {
        protected static IMongoClient _mongoClient;
        protected static IMongoDatabase _pomfSharpDatabase;

        public FileManager()
        {
            _mongoClient = new MongoClient();
            _pomfSharpDatabase = _mongoClient.GetDatabase("PomfSharp");
        }

        public FileDto GetFileData(string id)
        {
            if(CheckFileExists(id))
            {
                return GetFileDto(id);
            }
            else
            {
                return new FileDto
                {
                    FileId = "nope",
                    Name = "nope",
                    MappedLocation = "nope",
                    Type = "nope"
                };
            }
        }

        private bool CheckFileExists(string id)
        {
            var uploadedFilesCollection = _pomfSharpDatabase.GetCollection<BsonDocument>("uploadedFiles");
            var filter = Builders<BsonDocument>.Filter.Eq("fileid", id);
            return uploadedFilesCollection.Find(filter).ToList().Count == 1;
        }

        private FileDto GetFileDto(string id)
        {
            var uploadedFilesCollection = _pomfSharpDatabase.GetCollection<BsonDocument>("uploadedFiles");
            var filter = Builders<BsonDocument>.Filter.Eq("fileid", id);
            var data = uploadedFilesCollection.Find(filter).First();
            var file = BsonSerializer.Deserialize<UploadedFile>(data);
            return new FileDto
            {
                FileId = file.fileid,
                Name = file.name,
                MappedLocation = file.mappedlocation,
                Type = file.type
            };
        }
    }
}
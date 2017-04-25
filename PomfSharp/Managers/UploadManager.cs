using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PomfSharp.Managers
{
    public class UploadManager
    {
        protected static IMongoClient _mongoClient;
        protected static IMongoDatabase _pomfSharpDatabase;

        public UploadManager()
        {
            _mongoClient = new MongoClient();
            _pomfSharpDatabase = _mongoClient.GetDatabase("PomfSharp");
        }

        public void UploadFile(HttpPostedFileBase file, string path)
        {
            if (file.ContentLength > 0)
            {
                string newFileName = GenerateUniqueFileName(file.FileName);
                var fileLocation = Path.Combine(path, newFileName);
                file.SaveAs(fileLocation);
            }
        }

        private static string GenerateUniqueFileName(string fileName)
        {
            bool isUnique = false;
            var randomName = string.Empty;

            while (!isUnique)
            {
                var randomFilePath = Path.GetRandomFileName();
                var randomSplit = randomFilePath.Split('.');
                randomName = randomSplit[0];
                isUnique = CheckFileNameIsUnique(randomName);
            }

            SaveRandomName(randomName);

            var fileNameSplit = fileName.Split('.');
            var newFileName = $"{randomName}.{fileNameSplit.Last()}";
            return newFileName;
        }

        private static void SaveRandomName(string randomName)
        {
            var randomStringsCollection = _pomfSharpDatabase.GetCollection<BsonDocument>("randomStrings");
            var document = new BsonDocument
            {
                {"name", randomName}
            };
            randomStringsCollection.InsertOne(document);
        }

        private static bool CheckFileNameIsUnique(string randomString)
        {
            var randomStringsCollection = _pomfSharpDatabase.GetCollection<BsonDocument>("randomStrings");
            var filter = Builders<BsonDocument>.Filter.Eq("name", randomString);
            return randomStringsCollection.Find(filter).ToList().Count == 0;
        }
    }
}
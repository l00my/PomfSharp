using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
                var fileHash = string.Empty;
                var newFileName = GenerateUniqueFileName(file.FileName);
                var tempLocation = Path.Combine("C:\\temp\\", newFileName);
                file.SaveAs(tempLocation);
                using (var tempFile = File.OpenRead(tempLocation))
                {
                    if (CheckFileIsUnique(tempFile, ref fileHash))
                    {
                        var fileLocation = Path.Combine(path, newFileName);
                        SaveFile(file, newFileName, fileLocation, fileHash, tempLocation);
                    }
                    else
                    {
                        File.Delete(tempLocation);
                    }
                }
            }
        }

        private bool CheckFileIsUnique(FileStream file, ref string fileHash)
        {
            using (file)
            {
                var sha = new SHA256Managed();
                byte[] hashArray = sha.ComputeHash(file);
                fileHash = BitConverter.ToString(hashArray).Replace("-", string.Empty);
                //file.Position = 0;

                return CheckFileHashExists(fileHash);
            }
        }

        private bool CheckFileHashExists(string hash)
        {
            var uploadedFilesCollection = _pomfSharpDatabase.GetCollection<BsonDocument>("uploadedFiles");
            var filter = Builders<BsonDocument>.Filter.Eq("hash", hash);
            return uploadedFilesCollection.Find(filter).ToList().Count == 0;
        }

        private void SaveFile(HttpPostedFileBase file, string fileName, string fileLocation, string fileHash, string tempLocation)
        {
            var uploadedFilesCollection = _pomfSharpDatabase.GetCollection<BsonDocument>("uploadedFiles");

            var nameSplit = fileName.Split('.');
            var id = nameSplit[0];
            var document = new BsonDocument
            {
                {"id",  id},
                {"name", fileName },
                {"location", fileLocation },
                {"hash", fileHash }
            };

            uploadedFilesCollection.InsertOne(document);

            File.Move(tempLocation, fileLocation);
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
using MongoDB.Bson;

namespace PomfSharpCdn.Objects
{
    public class UploadedFile
    {
        public ObjectId _id { get; set; }
        public string fileid { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string type { get; set; }
        public string hash { get; set; }
    }
}
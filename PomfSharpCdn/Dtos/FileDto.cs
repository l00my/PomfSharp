using MongoDB.Bson;

namespace PomfSharpCdn.Dtos
{
    public class FileDto
    {
        public string FileId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
    }
}
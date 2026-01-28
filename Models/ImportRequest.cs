namespace RestAPI.Models
{
    public class ImportRequest
    {
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
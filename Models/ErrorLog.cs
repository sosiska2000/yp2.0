namespace RestAPI.Models
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
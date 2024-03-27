namespace webapi.Model
{
    public class ConvertorResponse
    {
        public string Output { get; set; }
        public string Message { get; set; }

        public ConvertorResponse()
        {
            Output = String.Empty;
            Message = String.Empty;
        }
    }
}

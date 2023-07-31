namespace StoreAPI.ResponseModule
{
    public class ApiExcption : ApiResponse
    {
        public ApiExcption(int statusCode,string details = null, string message = null) 
            : base(statusCode, message)
        {
            Details = details;
        }
        public string Details { get; set; }
    }
}

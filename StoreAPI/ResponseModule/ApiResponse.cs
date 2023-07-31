namespace StoreAPI.ResponseModule
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode,string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefultMessageForStatusCode(statusCode);
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        private string GetDefultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "You made a bad request",
                401 => "You are not authorized",
                404 => "Resourse not found",
                500 => "Internal Server Error",
                _ => null
            };
        }
    }
}

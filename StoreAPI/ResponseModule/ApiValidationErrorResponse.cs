namespace StoreAPI.ResponseModule
{
    public class ApiValidationErrorResponse : ApiExcption
    {
        public ApiValidationErrorResponse() : base(400)
        {

        }
        public IEnumerable<string> Errors { get; set; }
    }
}

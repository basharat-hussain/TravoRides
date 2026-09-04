namespace TravoRides.Application.Common.Responses
{
    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        public ApiResponse() { }

        public ApiResponse(bool isSuccess, string message, T? data)
            : base(isSuccess, message)
        {
            Data = data;
        }
    }
}

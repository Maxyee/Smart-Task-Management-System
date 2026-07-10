namespace SmartTaskManagement.Application.Common
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static Response<T> SuccessResponse(T data, string message = "Operation successful")
        {
            return new Response<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = 200
            };
        }

        public static Response<T> FailureResponse(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new Response<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
        }
    }
}
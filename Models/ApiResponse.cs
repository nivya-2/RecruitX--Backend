namespace RecruitX.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public T? Data { get; set; }
        public object? Meta { get; set; } // Optional metadata

        public ApiResponse(T? data, int statusCode, string? message = null, object? meta = null)
        {
            Data = data;
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Success = statusCode >= 200 && statusCode < 300;
            Meta = meta;
        }

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "OK",
                201 => "Created",
                204 => "No Content",
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => null
            } ?? string.Empty;
        }
    }
}

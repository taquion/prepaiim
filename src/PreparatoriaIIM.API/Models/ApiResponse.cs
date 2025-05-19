namespace PreparatoriaIIM.API.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public IEnumerable<string> Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message ?? "Operación exitosa",
                Data = data,
                Errors = null
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, IEnumerable<string> errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message ?? "Ha ocurrido un error",
                Data = default,
                Errors = errors
            };
        }
    }
}

namespace EmployeeLeaveManagementSystem.Services
{
    /// <summary>
    /// Lightweight outcome wrapper so services can report validation/business
    /// errors back to controllers without throwing exceptions for normal,
    /// expected failure cases (e.g. "not enough leave balance").
    /// </summary>
    public class ServiceResult
    {
        public bool Succeeded { get; protected set; }
        public List<string> Errors { get; protected set; } = new();

        public static ServiceResult Success() => new() { Succeeded = true };

        public static ServiceResult Failure(string error) => new()
        {
            Succeeded = false,
            Errors = new List<string> { error }
        };

        public static ServiceResult Failure(IEnumerable<string> errors) => new()
        {
            Succeeded = false,
            Errors = errors.ToList()
        };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; private set; }

        public static ServiceResult<T> Success(T data) => new()
        {
            Succeeded = true,
            Data = data
        };

        public static new ServiceResult<T> Failure(string error) => new()
        {
            Succeeded = false,
            Errors = new List<string> { error }
        };

        public static new ServiceResult<T> Failure(IEnumerable<string> errors) => new()
        {
            Succeeded = false,
            Errors = errors.ToList()
        };
    }
}

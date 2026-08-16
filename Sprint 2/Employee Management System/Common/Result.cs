using System;

namespace Employee_Management_System.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public T? Data { get; }

        private Result(bool isSuccess, string message, T? data)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }

        public static Result<T> Success(T data, string message = "Operation succeeded.")
        {
            return new Result<T>(true, message, data);
        }

        public static Result<T> Failure(string message, T? data = default)
        {
            return new Result<T>(false, message, data);
        }
    }
}

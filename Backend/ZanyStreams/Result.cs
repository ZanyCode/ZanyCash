using System;
using System.Collections.Generic;
using System.Text;

namespace ZanyStreams
{
    public class Result<T>
    {
        public Result(T value)
        {
            Value = value;
        }

        public Result(Error error)
        {
            Error = error;
        }

        public T Value { get; set; }

        public bool HasError { get => Error != null; }

        public Error Error { get; set; }
    }

    public class Result : Result<object>
    {
        public Result(object value) : base(value)
        {
        }

        public Result(Error error) : base(error)
        {
        }
    }

    public class Error
    {
        public Error(string message, int code = 0)
        {
            Message = message;
            Code = code;
        }

        public string Message { get; set; }

        public int Code { get; set; }
    }
}

namespace DetailedLogSwitcher
{
    public class Result
    {
        public bool IsError { get; internal set; }

        public static Ok<T> Ok<T>(T value) => new Ok<T>(value);

        public static Error Error(string message) => new Error(message);
    }

    public class Ok<T> : Result
    {
        public T Value { get; private set; }

        internal Ok(T value) : base() => Value = value;
    }

    public class Error : Result
    {
        public string Message { get; private set; }

        internal Error(string message) : base()
        {
            IsError = true;
            Message = message;
        }
    }
}
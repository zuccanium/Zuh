namespace Zuh.Compiler.Utils {
    /// <summary>
    /// 🦀
    /// </summary>
    public record Result<TError> {
        public TError? Error { get; init; }
        
        public bool IsSuccess
            => Error is null;

        public static Result<TError> Success()
            => new();

        public static Result<TError> Failure(TError error)
            => new() { Error = error };
    }
    
    public record Result<TValue, TError> : Result<TError> {
        public TValue? Value { get; init; }

        public static Result<TValue, TError> Success(TValue value)
            => new() { Value = value };
    }
}
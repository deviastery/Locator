using System.Text.Json.Serialization;

namespace Shared;

public record Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }
    public string? InvalidField { get; }

    [JsonConstructor]
    private Error(string code, string message, ErrorType type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public static Error NotFound(string? code, string message, Guid? id) => 
        new(code ?? "record.not.found", message, ErrorType.NOT_FOUND);
    
    public static Error Validation(string? code, string message, string? invalidField = null) => 
        new(code ?? "value.is.invalid", message, ErrorType.VALIDATION, invalidField);
    
    public static Error Conflict(string? code, string message) => 
        new(code ?? "value.is.conflict", message, ErrorType.CONFLICT);
    
    public static Error Failure(string? code, string message) => 
        new(code ?? "failure", message, ErrorType.FAILURE);
    
    public Failure ToFailure() => this;
}

public static class ErrorExtensions
{
    public static Failure ToFailure(this IEnumerable<Error> errors) => new(errors.ToArray());
}

public enum ErrorType
{
    /// <summary>
    /// Error validation
    /// </summary>
    VALIDATION,

    /// <summary>
    /// Error not found
    /// </summary>
    NOT_FOUND,

    /// <summary>
    /// Error failure
    /// </summary>
    FAILURE,

    /// <summary>
    /// Error conflict
    /// </summary>
    CONFLICT,
}
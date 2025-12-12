using System.Net;

namespace LichessSharp.Exceptions;

/// <summary>
///     Base exception for all Lichess API errors.
/// </summary>
public class LichessException : Exception
{
    /// <summary>
    ///     Creates a new LichessException.
    /// </summary>
    public LichessException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Creates a new LichessException with an inner exception.
    /// </summary>
    public LichessException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Creates a new LichessException with HTTP status information.
    /// </summary>
    public LichessException(string message, HttpStatusCode statusCode, string? lichessError = null)
        : base(message)
    {
        StatusCode = statusCode;
        LichessError = lichessError;
    }

    /// <summary>
    ///     Creates a new LichessException with HTTP status information and inner exception.
    /// </summary>
    public LichessException(string message, HttpStatusCode statusCode, string? lichessError, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        LichessError = lichessError;
    }

    /// <summary>
    ///     The HTTP status code returned by the API, if available.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    ///     The error message returned by the Lichess API, if available.
    /// </summary>
    public string? LichessError { get; }
}

/// <summary>
///     Exception thrown when the API rate limit is exceeded (HTTP 429).
/// </summary>
public class LichessRateLimitException : LichessException
{
    /// <summary>
    ///     Creates a new LichessRateLimitException.
    /// </summary>
    public LichessRateLimitException(string message, TimeSpan? retryAfter = null)
        : base(message, HttpStatusCode.TooManyRequests)
    {
        RetryAfter = retryAfter;
    }

    /// <summary>
    ///     The amount of time to wait before retrying, if provided by the API.
    /// </summary>
    public TimeSpan? RetryAfter { get; }
}

/// <summary>
///     Exception thrown when authentication fails (HTTP 401).
/// </summary>
public class LichessAuthenticationException : LichessException
{
    /// <summary>
    ///     Creates a new LichessAuthenticationException.
    /// </summary>
    public LichessAuthenticationException(string message, string? lichessError = null)
        : base(message, HttpStatusCode.Unauthorized, lichessError)
    {
    }
}

/// <summary>
///     Exception thrown when access is forbidden (HTTP 403).
///     Typically indicates missing OAuth scopes.
/// </summary>
public class LichessAuthorizationException : LichessException
{
    /// <summary>
    ///     Creates a new LichessAuthorizationException.
    /// </summary>
    public LichessAuthorizationException(string message, string? requiredScope = null, string? lichessError = null)
        : base(message, HttpStatusCode.Forbidden, lichessError)
    {
        RequiredScope = requiredScope;
    }

    /// <summary>
    ///     The OAuth scope that may be required for this operation.
    /// </summary>
    public string? RequiredScope { get; }
}

/// <summary>
///     Exception thrown when a requested resource is not found (HTTP 404).
/// </summary>
public class LichessNotFoundException : LichessException
{
    /// <summary>
    ///     Creates a new LichessNotFoundException.
    /// </summary>
    public LichessNotFoundException(string message, string? lichessError = null)
        : base(message, HttpStatusCode.NotFound, lichessError)
    {
    }
}

/// <summary>
///     Exception thrown when request validation fails (HTTP 400).
/// </summary>
public class LichessValidationException : LichessException
{
    /// <summary>
    ///     Creates a new LichessValidationException.
    /// </summary>
    public LichessValidationException(string message, string? lichessError = null)
        : base(message, HttpStatusCode.BadRequest, lichessError)
    {
    }

    /// <summary>
    ///     Creates a new LichessValidationException with validation errors.
    /// </summary>
    public LichessValidationException(string message, IReadOnlyDictionary<string, string[]> validationErrors,
        string? lichessError = null)
        : base(message, HttpStatusCode.BadRequest, lichessError)
    {
        ValidationErrors = validationErrors;
    }

    /// <summary>
    ///     Validation errors returned by the API, if available.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }
}
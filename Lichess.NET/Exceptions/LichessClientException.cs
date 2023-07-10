using System.Runtime.Serialization;

namespace Lichess.NET.Exceptions;

public class LichessClientException : Exception
{
    public LichessClientException()
    {
    }

    public LichessClientException(string? message) : base(message)
    {
    }

    public LichessClientException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected LichessClientException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

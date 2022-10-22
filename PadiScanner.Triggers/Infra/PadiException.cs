using System;
using System.Runtime.Serialization;

namespace PadiScanner.Triggers.Infra;

public class PadiException : Exception
{
    public PadiException()
    {
    }

    public PadiException(string? message) : base(message)
    {
    }

    public PadiException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected PadiException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
using System;

namespace SharpChannels.Core.Channels
{
    public class ExceptionEventArgs
    {
        public Exception Exception { get; }

        public ExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
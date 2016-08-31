using System;

namespace SharpChannels.Core.Channels
{
    public class ErrorCreatingChannelEventArgs
    {
        public Exception Exception { get; }

        public ErrorCreatingChannelEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
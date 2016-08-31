using System;

namespace SharpChannels.Core.Channels
{
    public class DataTransferException : Exception
    {
        public DataTransferException(string message, DataTransferErrorCode code): base(message)
        {
            Code = code;
        }

        public DataTransferException(string message, DataTransferErrorCode code, Exception innerException) 
            : base (message, innerException)
        {
            Code = code;
        }

        public DataTransferErrorCode Code { get; }
    }
}
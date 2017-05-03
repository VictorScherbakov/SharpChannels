using System;
using System.IO;
using System.Threading;

namespace SharpChannels.Core.Channels.Intradomain
{
    internal class IntradomainStream : Stream
    {
        private readonly TimeSpan _waitTimeout;
        internal object SyncRoot = new object();
        internal MemoryStream Stream;
        internal long ReadPosition;
        internal long WritePosition;

        internal AutoResetEvent DataUpdated = new AutoResetEvent(false);

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            bool needToWaitData = false;
            lock (Partner.SyncRoot)
            {
                if (Partner.Stream.Length == ReadPosition)
                {
                    needToWaitData = true;

                    // here we reset DataUpdated to prevent reaction to old update signals
                    Partner.DataUpdated.Reset(); 
                }
            }

            if (needToWaitData)
            {
                if (!Partner.DataUpdated.WaitOne(_waitTimeout))
                    throw new TimeoutException();
            }

            lock (Partner.SyncRoot)
            {
                Partner.Stream.Position = ReadPosition;
                var bytesRead = Partner.Stream.Read(buffer, offset, count);
                ReadPosition += bytesRead;

                if (ReadPosition == Partner.Stream.Length)
                {
                    Partner.Reset();
                    ReadPosition = 0;
                }
                return bytesRead;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (SyncRoot)
            {
                Stream.Position = WritePosition;
                Stream.Write(buffer, offset, count);
                WritePosition = Stream.Position;
                DataUpdated.Set();
            }
        }

        private void Reset()
        {
            Stream.Position = 0;
            Stream.SetLength(0);
            WritePosition = 0;
        }

        public override void Flush()
        {
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public IntradomainStream Partner { get; set; }

        public IntradomainStream(TimeSpan waitTimeout)
        {
            _waitTimeout = waitTimeout;
            Stream = new MemoryStream();
        }
    }
}
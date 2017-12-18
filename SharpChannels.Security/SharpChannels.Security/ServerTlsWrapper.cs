using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using SharpChannels.Core.Security;

namespace SharpChannels.Security
{
    public class ServerTlsWrapper : ISecurityWrapper
    {
        private readonly X509Certificate _serverCertificate;
        private readonly bool _checkCertificateRevocation;
        private readonly RemoteCertificateValidationCallback _certificateValidationCallback;

        public Stream Wrap(Stream stream)
        {
            var wrappedStream = new SslStream(stream, false, _certificateValidationCallback);
            wrappedStream.AuthenticateAsServer(_serverCertificate, true, SslProtocols.Tls12, _checkCertificateRevocation);
            return wrappedStream;
        }

        public ServerTlsWrapper(X509Certificate serverCertificate,
                                bool checkCertificateRevocation,
                                RemoteCertificateValidationCallback certificateValidationCallback)
        {
            _serverCertificate = serverCertificate;
            _checkCertificateRevocation = checkCertificateRevocation;
            _certificateValidationCallback = certificateValidationCallback;
        }
    }
}
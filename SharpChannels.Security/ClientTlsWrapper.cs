using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using SharpChannels.Core.Security;

namespace SharpChannels.Security
{
    public class ClientTlsWrapper : ISecurityWrapper
    {
        private readonly string _targetHost;
        private readonly X509Certificate[] _clientCertificates;
        private readonly bool _checkCertificateRevocation;
        private readonly RemoteCertificateValidationCallback _certificateValidationCallback;

        public Stream Wrap(Stream stream)
        {
            var wrappedStream = new SslStream(stream, false, _certificateValidationCallback);

            wrappedStream.AuthenticateAsClient(_targetHost, 
                                               new X509CertificateCollection(_clientCertificates), 
                                               SslProtocols.Tls12, 
                                               _checkCertificateRevocation);

            return wrappedStream;
        }

        public ClientTlsWrapper(string targetHost, 
                                X509Certificate[] clientCertificates, 
                                bool checkCertificateRevocation,
                                RemoteCertificateValidationCallback certificateValidationCallback)
        {
            _targetHost = targetHost;
            _clientCertificates = clientCertificates;
            _checkCertificateRevocation = checkCertificateRevocation;
            _certificateValidationCallback = certificateValidationCallback;
        }
    }
}
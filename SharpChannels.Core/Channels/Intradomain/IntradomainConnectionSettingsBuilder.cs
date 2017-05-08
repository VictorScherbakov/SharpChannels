using System;

namespace SharpChannels.Core.Channels.Intradomain
{
    public class IntradomainConnectionSettingsBuilder
    {
        private readonly IntradomainConnectionSettings _settings = GetDefaultSettings();

        public static IntradomainConnectionSettings GetDefaultSettings()
        {
            return new IntradomainConnectionSettings
            {
                SendTimeout = TimeSpan.FromMilliseconds(1000),
                ReceiveTimeout = TimeSpan.FromMilliseconds(1000),
                ConnectTimeout = TimeSpan.FromMilliseconds(1000)
            };
        }

        public IntradomainConnectionSettingsBuilder UsingConnectTimeout(TimeSpan connectTimeout)
        {
            _settings.ConnectTimeout = connectTimeout;
            return this;
        }

        public IntradomainConnectionSettingsBuilder UsingSendTimeout(TimeSpan sendTimeout)
        {
            _settings.SendTimeout = sendTimeout;
            return this;
        }

        public IntradomainConnectionSettingsBuilder UsingReceiveTimeout(TimeSpan receiveTimeout)
        {
            _settings.ReceiveTimeout = receiveTimeout;
            return this;
        }

        public IntradomainConnectionSettings Build()
        {
            return _settings;
        }
    }
}
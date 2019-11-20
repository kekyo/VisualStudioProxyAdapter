using System;
using System.Linq;
using System.Net;

namespace ProxyAdapter
{
    public sealed class FromEnvironment : IWebProxy
    {
        private static readonly EnvironmentVariableTarget[] targets =
        {
            EnvironmentVariableTarget.Process, EnvironmentVariableTarget.User, EnvironmentVariableTarget.Machine
        };

        private static readonly string[] names =
        {
            "HTTPS_PROXY", "HTTP_PROXY", "ALL_PROXY"
        };

        private static Uri GetProxyUrl() =>
            targets.SelectMany(target => names.Select(name => Environment.GetEnvironmentVariable(name, target))).
            Where(value => !string.IsNullOrEmpty(value)).
            Select(value => Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var result) ? result : null).
            Where(url => url != null).
            FirstOrDefault();

        public ICredentials Credentials
        {
            get
            {
                if (GetProxyUrl()?.UserInfo.Split(':') is string[] userInfo)
                {
                    if (userInfo.ElementAtOrDefault(0) is string user &&
                        !string.IsNullOrEmpty(user))
                    {
                        if (userInfo.ElementAtOrDefault(1) is string password &&
                            !string.IsNullOrEmpty(password))
                        {
                        }
                        else
                        {
                            password = null;
                        }
                        return new NetworkCredential(user, password);
                    }
                }
                return null;
            }
            set => throw new NotImplementedException();
        }

        public Uri GetProxy(Uri destination)
        {
            var proxyUrl = GetProxyUrl();
            return new Uri($"{proxyUrl.Scheme}://{proxyUrl.DnsSafeHost}:{proxyUrl.Port}");
        }

        public bool IsBypassed(Uri host) =>
            false;
    }
}

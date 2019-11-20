using ProxyAdapter;
using System;
using System.Net;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxyAdapter = new FromEnvironment();

            Console.WriteLine(proxyAdapter.GetProxy(null));

            var credential = proxyAdapter.Credentials.GetCredential(null, null);
            Console.WriteLine($"{credential.UserName}:{credential.Password}");
        }
    }
}

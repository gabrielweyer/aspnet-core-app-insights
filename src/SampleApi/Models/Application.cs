using System.Diagnostics;
using System.Net;

namespace SampleApi.Models
{
    public class Application
    {
        public Application(string name)
        {
            Name = name;
            Build = FileVersionInfo.GetVersionInfo(typeof(Startup).Assembly.Location).ProductVersion;
            Host = Dns.GetHostName();
        }

        public string Name { get; }

        public string Build { get; }

        public string Host { get; }
    }
}

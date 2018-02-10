using System;
using System.Diagnostics;
using System.Net;

namespace SimpleInstrumentation.Models
{
    public class Application
    {
        public Application(string name, string build)
        {
            Name = name;
            Build = build;
            Host = Dns.GetHostName();
        }

        public string Name { get; }

        public string Build { get; }

        public string Host { get; }

        public static string GetAssemblyInformationalversion(Type type)
        {
            return FileVersionInfo.GetVersionInfo(type.Assembly.Location).ProductVersion;
        }
    }
}

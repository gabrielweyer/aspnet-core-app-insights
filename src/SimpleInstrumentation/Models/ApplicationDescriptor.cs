using System;
using System.Diagnostics;
using System.Net;

namespace SimpleInstrumentation.Models
{
    public class ApplicationDescriptor
    {
        public ApplicationDescriptor(string name, string version)
        {
            Name = name;
            Version = version;
            Instance = Environment.MachineName;
        }

        public string Name { get; }

        public string Version { get; }

        public string Instance { get; }

        public static string GetAssemblyInformationalVersion(Type type)
        {
            return FileVersionInfo.GetVersionInfo(type.Assembly.Location).ProductVersion;
        }
    }
}

using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;

namespace SampleWorker.Infrastructure
{
    public class ContainerJobActivator : IJobActivator
    {
        private readonly IServiceProvider _service;

        public ContainerJobActivator(IServiceProvider service)
        {
            _service = service;
        }

        public T CreateInstance<T>()
        {
            return _service.GetService<T>();
        }
    }
}

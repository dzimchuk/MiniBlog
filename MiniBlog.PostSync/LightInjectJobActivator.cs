﻿using LightInject;
using Microsoft.Azure.WebJobs.Host;

namespace MiniBlog.PostSync
{
    internal class LightInjectJobActivator : IJobActivator
    {
        private readonly IServiceContainer container;

        public LightInjectJobActivator(IServiceContainer container)
        {
            this.container = container;
        }

        public T CreateInstance<T>()
        {
            return container.GetInstance<T>();
        }
    }
}
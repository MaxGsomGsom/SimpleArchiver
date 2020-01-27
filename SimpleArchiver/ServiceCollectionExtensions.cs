using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleArchiver
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterArchiverServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}

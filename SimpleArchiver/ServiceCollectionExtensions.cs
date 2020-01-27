using Microsoft.Extensions.DependencyInjection;
using SimpleArchiver.Contracts;
using SimpleArchiver.Services;

namespace SimpleArchiver
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterArchiverServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<IOperationComposer, OperationComposer>()
                .AddTransient<IOperationExecutor, CompressOperationExecutor>()
                .AddTransient<IOperationExecutor, DecompressOperationExecutor>();
        }
    }
}

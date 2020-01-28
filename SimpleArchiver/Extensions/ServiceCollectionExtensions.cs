using Microsoft.Extensions.DependencyInjection;
using SimpleArchiver.Contracts;
using SimpleArchiver.Services;

namespace SimpleArchiver.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterArchiverServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<IOperationComposer, OperationComposer>()
                .AddTransient<IOperationExecutor, CompressOperationExecutor>()
                .AddTransient<IOperationExecutor, DecompressOperationExecutor>()
                .AddTransient<IBufferPool, BufferPool>()
                .AddSingleton<IThreadPool, ThreadPool>()
                .AddTransient<IBlockStreamWriter, BlockStreamWriter>()
                .AddSingleton<ILogger, DebugConsoleLogger>();
        }
    }
}

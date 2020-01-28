using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SimpleArchiver.Contracts;
using SimpleArchiver.Extensions;

namespace SimpleArchiver
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterArchiverServices();
            using var services = serviceCollection.BuildServiceProvider();

            try
            {
                var composer = services.GetRequiredService<IOperationComposer>();
                var parameters = composer.Compose(args);

                var executor = services.GetServices<IOperationExecutor>().Single(e => e.Type == parameters.Type);
                executor.Execute(parameters);
            }
            catch (Exception e)
            {
                var logger = services.GetRequiredService<ILogger>();
                logger.Info(e.Message);
                return 1;
            }

            return 0;
        }
    }
}

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SimpleArchiver.Contracts;

namespace SimpleArchiver
{
    class Program
    {
        static int Main(string[] args)
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
                Console.WriteLine(e.Message);
                return 1;
            }

            return 0;
        }
    }
}

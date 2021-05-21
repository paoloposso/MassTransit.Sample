using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Components.Consumers;
using Sample.Components.StateMachines;
using System.Configuration;  

namespace Sample.Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) => {
                    config.AddJsonFile("appsettings.json", false).Build();
                    config.AddEnvironmentVariables();

                    if (args != null) {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostingContext, services) => { 
                    services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
                    
                    services.AddMassTransit(x => {
                        x.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();

                        x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                            .RedisRepository(s => {
                                s.DatabaseConfiguration("localhost");
                            });

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("localhost", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.ConfigureEndpoints(context);
                        });
                    });

                    services.AddHostedService<MassTransitConsoleHostedService>();

                }).ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });
                
            if (isService)
                await builder.UseWindowsService().Build().RunAsync();
            else
                await builder.RunConsoleAsync();
        }
    }
}

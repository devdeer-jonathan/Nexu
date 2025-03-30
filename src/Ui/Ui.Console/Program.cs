using Logic.Core;
using Logic.Interfaces;
using Logic.Visualizers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Ui.Console;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (_, services) =>
        {
            // TODO add your service dependencies here
            services.AddTransient<IDependencyAnalyzerLogic, DependencyAnalyzerLogic>();
            services.AddTransient<IGraphVisualizationLogic, GraphVisualizationLogic>();
            services.AddSingleton<App>();
        });
var app = builder.Build();
return await app.Services.GetRequiredService<App>()
    .StartAsync(Environment.GetCommandLineArgs());
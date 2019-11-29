using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Migrations;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Repositories;
using BezorgDirect.BezorgersApplicatie.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

[assembly: FunctionsStartup(typeof(BezorgDirect.BezorgersApplicatie.Api.Startup))]

namespace BezorgDirect.BezorgersApplicatie.Api
{
    /* This class was written by Lennart de Waart (563079) */
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Add dependency injection           
            builder.Services.AddSingleton<ILogger>(x => new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File($".../../../../../wwwroot/logs/Log-.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}",
                    rollingInterval: RollingInterval.Day)
                    .CreateLogger());
            builder.Services.AddTransient<IAvailabilitiesRepository, AvailabilitiesRepository>();
            builder.Services.AddTransient<IDeliveriesRepository, DeliveriesRepository>();
            builder.Services.AddTransient<IDeliverersRepository, DeliverersRepository>();
            builder.Services.AddTransient<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddTransient<ILocationsRepository, LocationsRepository>();
            builder.Services.AddTransient<INotificationsRepository, NotificationsRepository>();  
            builder.Services.AddSingleton<Scheduler.Scheduler>();
            builder.Services.AddDbContext<Context>(
               options => options.UseSqlServer(Environment.GetEnvironmentVariable("BezorgersApplicatieConnection", EnvironmentVariableTarget.Process)));

            // Enable logging
            builder.Services.AddLogging();

            // Register Swagger services
            builder.Services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "1.0.0";
                    document.Info.Title = "Bezorg Direct Bezorgersapp API";
                    document.Info.Description = "An Azure Funtions API for the deliverers application";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "De Wolkenridders",
                        Email = "563079@student.inholland.nl",
                        Url = "https://bezorg.direct"
                    };
                };
            });
        }
    }
}

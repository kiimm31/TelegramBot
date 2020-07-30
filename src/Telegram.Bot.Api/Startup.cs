using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Api.Handlers.Commands;
using Telegram.Bot.Api.Handlers.Exceptions;
using Telegram.Bot.Api.Models;
using Telegram.Bot.Api.Options;
using Telegram.Bot.Api.Services;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
namespace Telegram.Bot.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (Configuration.GetSection(nameof(BotSettings)).Exists())
            {
                services.Configure<BotSettings>(Configuration.GetSection(nameof(BotSettings)));
            }
            services.AddSimpleBot(Configuration);
            services.AddHttpContextAccessor();
            services.AddCustomCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseTelegramBotLongPolling<MessageBot>(ConfigureBot(), Configuration, startAfter: TimeSpan.FromSeconds(2));
            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseMvc();
        }
        private IBotBuilder ConfigureBot() =>
            new BotBuilder()
                .Use<ExceptionHandler>()
                .UseCommand<StartCommand>("start")
                .UseWhen<SubscribeHandler>(When.CorrectPokemon)
            .UseWhen<SubscribeChannelHandler>(When.ChannelPost)
            ;
    }
    internal static class ServiceExtensions
    {
        internal static IServiceCollection AddSimpleBot(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetSection(nameof(BotSettings)).Exists())
            {
                services.AddTransient<MessageBot>()
                    .Configure<BotOptions<MessageBot>>(configuration.GetSection(nameof(BotSettings)))
                    .AddScoped<ExceptionHandler>()
                    .AddScoped<SubscribeHandler>()
                    .AddScoped<SubscribeChannelHandler>()
                    .AddScoped<StartCommand>();
              
            };
            if (configuration.GetSection(nameof(WebHookOptions)).Exists())
            {
                services.Configure<WebHookOptions>(configuration.GetSection(nameof(WebHookOptions)));
            };
            services.AddTransient<KeepAliveService>();
            return services;
        }
        internal static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials()
                .Build());
            });
            return services;
        }
    }
    static class AppStartupExtensions
    {
        public static IApplicationBuilder UseTelegramBotLongPolling<TBot>(
            this IApplicationBuilder app,
            IBotBuilder botBuilder,
            IConfiguration configuration,
            TimeSpan startAfter = default,
            CancellationToken cancellationToken = default
        )
            where TBot : BotBase
        {
            if (startAfter == default)
            {
                startAfter = TimeSpan.FromSeconds(2);
            }
            var updateManager = new UpdatePollingManager<TBot>(botBuilder, new BotServiceProvider(app));
            Task.Run(async () =>
            {
                await Task.Delay(startAfter, cancellationToken);
                await updateManager.RunAsync(cancellationToken: cancellationToken);
            }, cancellationToken)
            .ContinueWith(t =>
            {// ToDo use logger
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(t.Exception);
                Console.ResetColor();
                using(var scope = app.ApplicationServices.CreateScope())
                {
                    var bot = scope.ServiceProvider.GetRequiredService<TBot>();
                    var debugGroupId = configuration.GetValue<string>("DebugGroup", "-368260554");
                    var siteName = configuration.GetValue<string>("SiteName", "TelegramBotApi");
                    var restartService = scope.ServiceProvider.GetRequiredService<KeepAliveService>();
                    Task.Run(() => restartService.DoRestart(bot.Client, debugGroupId, t.Exception, siteName));
                }
                throw t.Exception;
            }, TaskContinuationOptions.OnlyOnFaulted);
            return app;
        }
        public static IApplicationBuilder EnsureWebhookSet<TBot>(
            this IApplicationBuilder app
        )
            where TBot : IBot
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                var bot = scope.ServiceProvider.GetRequiredService<TBot>();
                var options = scope.ServiceProvider.GetRequiredService<IOptions<BotOptions<MessageBot>>>();
                var extOptions = scope.ServiceProvider.GetRequiredService<IOptions<WebHookOptions>>();
                var url = new Uri(new Uri(extOptions.Value.WebHookDomain), options.Value.WebhookPath);
                logger.LogInformation("Setting webhook for bot \"{0}\" to URL \"{1}\"", typeof(TBot).Name, url);
                bot.Client.SetWebhookAsync(url.AbsoluteUri)
                    .GetAwaiter().GetResult();
            }
            return app;
        }
    }
}

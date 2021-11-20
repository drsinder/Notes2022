using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Notes2022Preview.Data;
using System;
using System.Net.Http;

namespace Notes2022Preview
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .RegisterBlazorMauiWebView()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .Host
                .ConfigureAppConfiguration((app, config) =>
                {
                    string dir = Environment.ProcessPath;
                    int index = dir.LastIndexOf('\\');
                    dir = dir.Substring(0, index);
                    var Provider = new PhysicalFileProvider(dir);
                    config.AddJsonFile(Provider, "appsettings.json", optional: false, false);
                });

            string baseAddress = builder.Configuration["BaseAddress"];

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

            //.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            //builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Notes2022.ServerAPI"));


            builder.Services.AddBlazorWebView();
            //builder.Services.AddSingleton<WeatherForecastService>();

            return builder.Build();
        }
    }
}
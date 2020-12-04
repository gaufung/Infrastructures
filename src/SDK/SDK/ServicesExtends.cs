﻿using Aiursoft.Scanner;
using Aiursoft.Scanner.Tools;
using Aiursoft.SDK.Services;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tools;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Aiursoft.SDK
{
    public static class ServicesExtends
    {
        public static IServiceCollection AddAiurMvc(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddAiurAPIMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            return services;
        }

        public static IMvcBuilder AddAiurAPIMvc(this IServiceCollection services)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            return services
                .AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
        }

        public static IServiceCollection AddAiursoftSDK(this IServiceCollection services,
            Assembly assembly = null,
            params Type[] abstracts)
        {
            services.AddHttpClient();
            services.AddMemoryCache();
            var abstractsArray = abstracts.AddWith(typeof(IHostedService)).ToArray();
            if (EntryExtends.IsProgramEntry())
            {
                services.AddScannedDependencies(abstractsArray);
            }
            else if (assembly != null)
            {
                services.AddAssemblyDependencies(assembly, abstractsArray);
            }
            else
            {
                services.AddAssemblyDependencies(Assembly.GetCallingAssembly(), abstractsArray);
            }
            return services;
        }

        public static IServiceCollection UseBlacklistFromAddress(this IServiceCollection services, string address)
        {
            AsyncHelper.TryAsync(async () =>
            {
                var list = await new WebClient().DownloadStringTaskAsync(address);
                var provider = new BlackListPorivder(list.Split('\n'));
                services.AddSingleton(provider);
            }, 3);
            return services;
        }

        public static IServiceCollection AddDbContextWithCache<T>(this IServiceCollection services, string connectionString) where T : DbContext
        {
            if (EntryExtends.IsInUT())
            {
                services.AddDbContext<T>((optionsBuilder) =>
                    optionsBuilder.UseInMemoryDatabase("inmemory"));
            }
            else
            {
                services.AddDbContextPool<T>((serviceProvider, optionsBuilder) =>
                        optionsBuilder
                            .UseSqlServer(connectionString)
                            .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));
            }
            services.AddEFSecondLevelCache(options =>
            {
                options.UseMemoryCacheProvider().DisableLogging(true);
                options.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(30));
            });
            return services;
        }
    }
}

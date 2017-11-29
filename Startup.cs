﻿using AspNetCoreApplication.Repository;
using AspNetCoreApplication.Repository.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AspNetCoreApplication {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices (IServiceCollection services) {
            services.AddDbContext<AspNetCoreApplicationDbContext> (options => {
                options.UseSqlServer (Configuration.GetConnectionString ("ApplicationConnection"));
            });
            services.AddMvc ()
                .AddJsonOptions (opt => {
                    opt.SerializerSettings.ReferenceLoopHandling =
                        ReferenceLoopHandling.Ignore;
                });
            services.AddAutoMapper ();
            services.AddScoped<ICampaignRepository, CampaignRepository> ();
        }

        public void Configure (IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole ();
            loggerFactory.AddDebug (LogLevel.Information);
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseExceptionHandler (appBuilder => {
                    appBuilder.Run (async context => {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature> ();
                        if (exceptionHandlerFeature != null) {
                            var logger = loggerFactory.CreateLogger ("Global exception logger");
                            logger.LogError (500,
                                exceptionHandlerFeature.Error,
                                exceptionHandlerFeature.Error.Message);
                        }
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync ("An unexpected fault happened. Try again later.");

                    });
                });
            }
            app.UseCors ((corsPolicyBuilder) => {
                corsPolicyBuilder.AllowAnyOrigin ();
                corsPolicyBuilder.AllowAnyMethod ();
                corsPolicyBuilder.AllowAnyHeader ();
            });
            app.UseMvc ();
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace CityInfo.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
                     //the below is added to make the response from the request be formatted exactly the way it is in your class as opposed to the camelcase format..
                     //.AddJsonOptions(o =>
                     //{
                     //    if (o.SerializerSettings.ContractResolver != null)
                     //    {
                     //        var castedResolver = o.SerializerSettings.ContractResolver
                     //            as DefaultContractResolver;
                     //        castedResolver.NamingStrategy = null;
                     //    }
                     //});

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
                app.UseExceptionHandler();
            }

            app.UseStatusCodePages();//this was added so when an error occurs it is dispalyed on browser 
            app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello Worldkfkfkf!");
            //});
        }
    }
}

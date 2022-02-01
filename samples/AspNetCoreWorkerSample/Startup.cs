//   Copyright 2020-present Etherna Sagl
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Etherna.ServicesClient.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Etherna.ServicesClient.AspSampleClient
{
    public class Startup
    {
        // Constructor.
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Properties.
        public IConfiguration Configuration { get; }

        // Methods.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            // Register client.
            var ethernaServiceClientBuilder = services.AddEthernaCreditClientForServices(
                new Uri(Configuration["SampleConfig:ServiceBaseUrl"]),
                new Uri(Configuration["SampleConfig:SsoBaseUrl"]),
                Configuration["SampleConfig:ClientId"],
                Configuration["SampleConfig:ClientSecret"]);

            var clientCredentialTask = ethernaServiceClientBuilder.GetClientCredentialsTokenRequestAsync();
            clientCredentialTask.Wait();
            var clientCredential = clientCredentialTask.Result;

            // Register token manager.
            services.AddAccessTokenManagement(options =>
            {
                options.Client.Clients.Add(ethernaServiceClientBuilder.ClientName, clientCredential);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}

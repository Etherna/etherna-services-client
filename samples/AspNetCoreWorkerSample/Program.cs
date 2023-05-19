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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Etherna.ServicesClient.AspSampleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Register client.
            var ethernaServiceClientBuilder = builder.Services.AddEthernaCreditClientForServices(
                new Uri(builder.Configuration["SampleConfig:ServiceBaseUrl"]!),
                new Uri(builder.Configuration["SampleConfig:SsoBaseUrl"]!),
                builder.Configuration["SampleConfig:ClientId"]!,
                builder.Configuration["SampleConfig:ClientSecret"]!);

            var clientCredentialTask = ethernaServiceClientBuilder.GetClientCredentialsTokenRequestAsync();
            clientCredentialTask.Wait();
            var clientCredential = clientCredentialTask.Result;

            // Register token manager.
            builder.Services.AddAccessTokenManagement(options =>
            {
                options.Client.Clients.Add(ethernaServiceClientBuilder.ClientName, clientCredential);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
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

            app.MapRazorPages();

            app.Run();
        }
    }
}
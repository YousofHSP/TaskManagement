﻿using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.DataInitializer;

namespace WebFramework.Configuration;

public static class ApplicationBuilderException
{
    public static void UseHsts(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app.UseHsts();
        }
    }

    public static async Task DataSeeder(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>()!; // service locator
        // dbContext.Database.EnsureCreated();
        await dbContext.Database.MigrateAsync();

        var dataInitializers = scope.ServiceProvider.GetServices<IDataInitializer>();
        foreach (var dataInitializer in dataInitializers)
            await dataInitializer.InitializerData();
    }
}
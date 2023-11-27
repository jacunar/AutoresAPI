﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace AutoresAPI; 
public class Startup {
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services) {
        services.AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
            .AddNewtonsoftJson();

        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiAutores", Version = "v1" });
        });

        services.AddAutoMapper(typeof(Startup));
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });
    }
}
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace AutoresAPI;
public class Startup {
    public IConfiguration Configuration { get; }
    
    public Startup(IConfiguration configuration) {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services) {
        services.AddControllers(o => {
            o.Conventions.Add(new SwaggerGroupByVersion());
        }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
          .AddNewtonsoftJson();

        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt => opt.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["llavejwt"] ?? "")),
                ClockSkew = TimeSpan.Zero
            });

        services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Autores", Version = "v1",
                Description = "Web API Autores y Libros"
            });
            c.SwaggerDoc("v2", new OpenApiInfo { Title = "API Autores", Version = "v2" });
            c.OperationFilter<AgregarParametroHATEOAS>();
            c.OperationFilter<AddParameterXVersion>();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {   
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                        }
                    }, new string[]{} 
                }
            });

            var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXML);
            c.IncludeXmlComments(rutaXML);
        });

        services.AddAutoMapper(typeof(Startup));
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthorization(o => {
            o.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
            o.AddPolicy("EsVendedor", politica => politica.RequireClaim("esVendedor"));
        });        

        services.AddDataProtection();
        services.AddTransient<HashService>();

        services.AddCors(op => {
            op.AddDefaultPolicy(builder => {
                builder.WithOrigins("").AllowAnyMethod().AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "cantidadTotalRegistros" });
            });
        });

        services.AddTransient<GeneradorEnlaces>();
        services.AddTransient<HATEOASAutorFilterAttribute>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "API Autores v1");
                c.SwaggerEndpoint("../swagger/v2/swagger.json", "API Autores v2");
            });
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });
    }
}
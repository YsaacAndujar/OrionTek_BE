using Data;
using Logic.Utils;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using OrionTek.Middlewares;
using System.Reflection;

namespace OrionTek
{
    public class Startup
    {
        private readonly IConfiguration Configuration;
        private readonly string AllowAllCors = "AllowAll";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowAllCors,
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin()
                                            .AllowAnyMethod()
                                            .AllowAnyHeader()
                                            .WithExposedHeaders("totalPages", "totalEntities");
                                  });
            });
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            services.AddDbContext<OrionTekDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("Data");
                })
            );
            services.AddHttpContextAccessor();
            DependencyInjector.ConfigureDependecies(services, Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors(AllowAllCors);
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCustomExceptionMiddleware();

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    if (exceptionHandlerPathFeature?.Error is CustomException customException)
                    {
                        context.Response.StatusCode = customException.StatusCode;
                        context.Response.ContentType = "application/json";

                        var errorResponse = new { message = customException.Message, statusCode = customException.StatusCode };
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
                    }
                    else
                    {
                        var ex = exceptionHandlerPathFeature?.Error is not null ? exceptionHandlerPathFeature?.Error : null;
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";

                        var innerExMessage = ex.InnerException?.Message;
                        var innerExSource = ex.InnerException?.Source;

                        var errorResponse = new { message = ex.Message, inner_exception = innerExMessage, source = innerExSource };
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
                    }
                });
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}

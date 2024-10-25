using Logic.Interfaces;
using Logic.Repositories;
using Logic.Services;
using System.Reflection;

namespace OrionTek
{
    public class DependencyInjector
    {
        public static void ConfigureDependecies(IServiceCollection services, IConfiguration Configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(Logic.Utils.AutoMapperProfiles).Assembly);
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IEmailSender, SendgridService>();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddSingleton(provider =>
            {
                var apiKey = Configuration["SendGrid:Key"];
                return new SendGrid.SendGridClient(apiKey);
            });
        }
    }
}

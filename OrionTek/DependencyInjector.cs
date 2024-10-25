using Logic.Interfaces;
using Logic.Repositories;
using System.Reflection;

namespace OrionTek
{
    public class DependencyInjector
    {
        public static void ConfigureDependecies(IServiceCollection services, IConfiguration Configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(Logic.Utils.AutoMapperProfiles).Assembly);
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
        }
    }
}

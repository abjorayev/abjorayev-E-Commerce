using E_Commerce.Repository;

namespace E_Commerce.DependencyInjections
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IECommerceRepository<>), typeof(ECommerceRepository<>));
            return services;
        }
    }
}

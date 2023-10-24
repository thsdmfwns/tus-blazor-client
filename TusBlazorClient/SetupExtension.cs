using Microsoft.Extensions.DependencyInjection;

namespace TusBlazorClient;

public static class SetupExtension
{
    public static IServiceCollection AddTusBlazorClient(this IServiceCollection services)
    {
        services.AddSingleton<TusClient>();
        return services;
    }
}
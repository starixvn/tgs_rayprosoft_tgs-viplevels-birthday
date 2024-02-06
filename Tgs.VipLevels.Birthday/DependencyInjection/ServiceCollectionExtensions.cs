using Microsoft.EntityFrameworkCore;
using Tgs.PlayerService.Client;
using Tgs.VipLevels.Birthday;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddVipLevelsBirthday(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Action<RedisOptions, IServiceProvider> redisOptions,
		Action<PlayerServiceClientOptions, IServiceProvider> playerServiceClientOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisOptions)
		.Services
		.AddSingleton<RedisConnectionManager>()
		.AddVipLevelServiceClient((httpClientBuilder) =>
		httpClientBuilder.ConfigureHttpClient((sp, http) =>
		{
			var configuration = sp.GetRequiredService<IConfiguration>();
			http.BaseAddress = configuration.GetValue<Uri>("VipLevelConnectionStrings:VipLevelServiceClientBaseUrl")!;
		}))
		.AddPlayerServiceClient(playerServiceClientOptions)
		.AddVipLevelsBirthdayCore(
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database)
		.AddVipLevelsBirthdayRepositories(
			dbContextOptions: dbContextOptions);
}

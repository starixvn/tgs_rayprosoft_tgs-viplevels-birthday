using Microsoft.EntityFrameworkCore;
using Tgs.VipLevels.Birthday.Core;
using Tgs.VipLevels.Birthday.Entities;
using Tgs.VipLevels.Birthday.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddVipLevelsBirthdayRepositories(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions)
		=> services
		.AddDbContext<TgsContext>(dbContextOptions)
		.AddTransient<IBrandRepository, BrandRepository>();
}

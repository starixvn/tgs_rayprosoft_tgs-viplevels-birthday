using Hangfire.Dashboard;

namespace Tgs.VipLevels.Birthday;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}

using System.Text.Json.Nodes;
using StackExchange.Redis;

namespace Tgs.VipLevels.Birthday.Core;

internal class HangfireLock(
	IDatabase database) : IHangfireLock
{
	public Task<bool> LockingAsync(string application, CancellationToken cancellationToken = default)
			=> database.StringSetAsync(
			key: GetKey(application),
			value: new JsonObject
			{
				["MachineName"] = Environment.MachineName,
				["Application"] = application,
				["LabelTime"] = DateTimeOffset.Now
			}.ToJsonString(),
			expiry: TimeSpan.FromSeconds(59),
			when: When.NotExists);

	private static string GetKey(string application)
		=> string.Concat("tgs-viplevels-birthday-locking:hangfire:", application);
}

using Tgs.VipLevels.Birthday.Core;

namespace Tgs.VipLevels.Birthday.Core;

internal class SystemClock : ISystemClock
{
	public DateTimeOffset CurrentTime => DateTimeOffset.UtcNow;

	public DateTimeOffset GetTimeZone(int timeSpanMinutes = 480)
	{
		var timeSpan = TimeSpan.FromMinutes(timeSpanMinutes);
		return CurrentTime.ToOffset(timeSpan);
	}
}

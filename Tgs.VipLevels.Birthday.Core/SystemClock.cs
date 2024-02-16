using Tgs.VipLevels.Birthday.Core;

namespace Tgs.VipLevels.Birthday.Core;

internal class SystemClock : ISystemClock
{
	public DateTimeOffset CurrentTime => DateTimeOffset.UtcNow;
}

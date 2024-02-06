namespace Tgs.VipLevels.Birthday.Core;

public interface ISystemClock
{
	DateTimeOffset CurrentTime { get; }

	DateTimeOffset GetTimeZone(int timeSpanMinutes = 480);
}

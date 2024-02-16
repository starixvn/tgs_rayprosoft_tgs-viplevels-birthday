namespace Tgs.VipLevels.Birthday.Core;

public interface ISystemClock
{
	DateTimeOffset CurrentTime { get; }
}

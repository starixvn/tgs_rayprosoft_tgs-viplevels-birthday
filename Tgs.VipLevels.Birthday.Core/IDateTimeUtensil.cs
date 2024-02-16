namespace Tgs.VipLevels.Birthday.Core;

public interface IDateTimeUtensil
{
	DateTimeOffset GetTimeZone(DateTimeOffset source, int timeSpanMinutes = 480);
}

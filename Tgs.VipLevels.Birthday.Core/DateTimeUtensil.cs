using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.VipLevels.Birthday.Core;
internal class DateTimeUtensil : IDateTimeUtensil
{
	public DateTimeOffset GetTimeZone(DateTimeOffset source, int timeSpanMinutes = 480)
	{
		var timeSpan = TimeSpan.FromMinutes(timeSpanMinutes);
		var utcDateTime = source.UtcDateTime;
		return new DateTimeOffset(utcDateTime.Year, utcDateTime.Month, utcDateTime.Day, utcDateTime.Hour, utcDateTime.Minute, utcDateTime.Second, TimeSpan.Zero).ToOffset(timeSpan);
	}
}

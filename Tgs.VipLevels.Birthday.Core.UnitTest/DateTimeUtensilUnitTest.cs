using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.VipLevels.Birthday.Core.UnitTest;
public class DateTimeUtensilUnitTest
{
	[Fact]
	public void Get_TimeZone_480()
	{
		var source = new DateTimeOffset(year: 2024, month: 02, day: 01, hour: 00, minute: 00, second: 00, offset: TimeSpan.FromMinutes(-240));
		var sut = new DateTimeUtensil();

		var actual = sut.GetTimeZone(source, 480);

		var expected = new DateTimeOffset(year: 2024, month: 02, day: 01, hour: 12, minute: 00, second: 00, offset: TimeSpan.FromMinutes(480));

		Assert.Equal(expected, actual);
		Assert.Equal(expected.Offset, actual.Offset);
	}

	[Fact]
	public void Get_TimeZone_330()
	{
		var source = new DateTimeOffset(year: 2024, month: 02, day: 01, hour: 00, minute: 00, second: 00, offset: TimeSpan.FromMinutes(-240));
		var sut = new DateTimeUtensil();

		var actual = sut.GetTimeZone(source, 330);

		var expected = new DateTimeOffset(year: 2024, month: 02, day: 01, hour: 09, minute: 30, second: 00, offset: TimeSpan.FromMinutes(330));

		Assert.Equal(expected, actual);
		Assert.Equal(expected.Offset, actual.Offset);
	}

	[Fact]
	public void Get_TimeZone_480_V2()
	{
		var source = new DateTimeOffset(year: 2024, month: 02, day: 15, hour: 04, minute: 00, second: 00, offset: TimeSpan.Zero);
		var sut = new DateTimeUtensil();

		var actual = sut.GetTimeZone(source, 480);

		var expected = new DateTimeOffset(year: 2024, month: 02, day: 15, hour: 12, minute: 00, second: 00, offset: TimeSpan.FromMinutes(480));

		Assert.Equal(expected, actual);
		Assert.Equal(expected.Offset, actual.Offset);
	}
}

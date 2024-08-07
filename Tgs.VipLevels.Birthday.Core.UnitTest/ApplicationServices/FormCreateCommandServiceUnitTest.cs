using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.PlayerService.Client;
using Tgs.VipLevels.Birthday.Core.ApplicationServices;
using Tgs.VipLevelService.Clients;

namespace Tgs.VipLevels.Birthday.Core.UnitTest.ApplicationServices;

public class FormCreateCommandServiceUnitTest
{
	private static readonly string[] _SourceArray = ["4ucmqplxb029"];

	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<FormCreateCommandService>.Instance;
		var fakeDateTimeUtensil = Substitute.For<IDateTimeUtensil>();
		var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
		var fakeVipLevelServiceClient = Substitute.For<IVipLevelServiceClient>();

		var command = new FormCreateCommand
		(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demon",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			TimeSpanMinutes: 480,
			CurrentTime: new DateTimeOffset(2024, 02, 15, 04, 00, 00, TimeSpan.Zero)
		);

		var timeZone = new DateTimeOffset(2024, 02, 15, 12, 00, 00, TimeSpan.FromMinutes(command.TimeSpanMinutes));
		_ = fakeDateTimeUtensil.GetTimeZone(
			source: Arg.Is(command.CurrentTime),
			timeSpanMinutes: Arg.Is(command.TimeSpanMinutes))
			.Returns(timeZone);

		var birthday = DateOnly.FromDateTime(timeZone.Date);

		_ = fakePlayerServiceClient.QueryPlayerBirthdayAsync(
			brandId: Arg.Is(command.BrandId),
			birthday: Arg.Is(birthday),
			page: Arg.Any<int>(),
			rows: Arg.Any<int>(),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(_SourceArray.ToAsyncEnumerable(), Enumerable.Empty<string>().ToAsyncEnumerable());

		_ = fakeVipLevelServiceClient.PlayerVipLevelBirthdayFormExistAsync(
			playerCode: Arg.Is("4ucmqplxb029"),
			birthday: Arg.Is(birthday),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var id = Guid.NewGuid();
		_ = fakeVipLevelServiceClient.PlayerVipLevelBirthdayFormCreateAsync(
			playerCode: Arg.Is("4ucmqplxb029"),
			birthday: Arg.Is(birthday),
			operatorUserId: Arg.Is(Guid.Empty),
			operatorUsername: Arg.Is("system"),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var sut = new FormCreateCommandService(
			fakeLog,
			fakeDateTimeUtensil,
			fakePlayerServiceClient,
			fakeVipLevelServiceClient);

		await sut.ExecuteAsync(command);
	}

	[Fact]
	public async Task This_player_VIP_level_benefits_have_been_distributed()
	{
		var fakeLog = NullLogger<FormCreateCommandService>.Instance;
		var fakeDateTimeUtensil = Substitute.For<IDateTimeUtensil>();
		var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
		var fakeVipLevelServiceClient = Substitute.For<IVipLevelServiceClient>();

		var command = new FormCreateCommand
		(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demon",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			TimeSpanMinutes: 480,
			CurrentTime: new DateTimeOffset(2024, 02, 15, 04, 00, 00, TimeSpan.Zero)
		);

		var timeZone = new DateTimeOffset(2024, 02, 15, 12, 00, 00, TimeSpan.FromMinutes(command.TimeSpanMinutes));
		_ = fakeDateTimeUtensil.GetTimeZone(
			source: Arg.Is(command.CurrentTime),
			timeSpanMinutes: Arg.Is(command.TimeSpanMinutes))
			.Returns(timeZone);

		var birthday = DateOnly.FromDateTime(timeZone.Date);

		_ = fakePlayerServiceClient.QueryPlayerBirthdayAsync(
			brandId: Arg.Is(command.BrandId),
			birthday: Arg.Is(birthday),
			page: Arg.Any<int>(),
			rows: Arg.Any<int>(),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(_SourceArray.ToAsyncEnumerable(), Enumerable.Empty<string>().ToAsyncEnumerable());

		_ = fakeVipLevelServiceClient.PlayerVipLevelBirthdayFormExistAsync(
			playerCode: Arg.Is("4ucmqplxb029"),
			birthday: Arg.Is(birthday),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new FormCreateCommandService(
			fakeLog,
			fakeDateTimeUtensil,
			fakePlayerServiceClient,
			fakeVipLevelServiceClient);

		await sut.ExecuteAsync(command);

		_ = fakeVipLevelServiceClient
			.Received(0)
			.PlayerVipLevelBirthdayFormCreateAsync(
			playerCode: Arg.Is("4ucmqplxb029"),
			birthday: Arg.Is(birthday),
			operatorUserId: Arg.Is(Guid.Empty),
			operatorUsername: Arg.Is("system"),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Enumerable_Empty()
	{
		var fakeLog = NullLogger<FormCreateCommandService>.Instance;
		var fakeDateTimeUtensil = Substitute.For<IDateTimeUtensil>();
		var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
		var fakeVipLevelServiceClient = Substitute.For<IVipLevelServiceClient>();

		var command = new FormCreateCommand
		(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demon",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			TimeSpanMinutes: 480,
			CurrentTime: new DateTimeOffset(2024, 02, 15, 04, 00, 00, TimeSpan.Zero)
		);

		var timeZone = new DateTimeOffset(2024, 02, 15, 12, 00, 00, TimeSpan.FromMinutes(command.TimeSpanMinutes));
		_ = fakeDateTimeUtensil.GetTimeZone(
			source: Arg.Is(command.CurrentTime),
			timeSpanMinutes: Arg.Is(command.TimeSpanMinutes))
			.Returns(timeZone);

		var birthday = DateOnly.FromDateTime(timeZone.Date);

		_ = fakePlayerServiceClient.QueryPlayerBirthdayAsync(
			brandId: Arg.Is(command.BrandId),
			birthday: Arg.Is(birthday),
			page: Arg.Any<int>(),
			rows: Arg.Any<int>(),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Enumerable.Empty<string>().ToAsyncEnumerable());

		var sut = new FormCreateCommandService(
			fakeLog,
			fakeDateTimeUtensil,
			fakePlayerServiceClient,
			fakeVipLevelServiceClient);

		await sut.ExecuteAsync(command);

		_ = fakeVipLevelServiceClient
			.Received(0)
			.PlayerVipLevelBirthdayFormExistAsync(
			playerCode: Arg.Is("4ucmqplxb029"),
			birthday: Arg.Is(birthday),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeVipLevelServiceClient
			.Received(0)
			.PlayerVipLevelBirthdayFormCreateAsync(
			playerCode: Arg.Is("4ucmqplxb029"),
			birthday: Arg.Is(birthday),
			operatorUserId: Arg.Is(Guid.Empty),
			operatorUsername: Arg.Is("system"),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}

using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.PlayerService.Client;
using Tgs.VipLevels.Birthday.Common.Protos;
using Tgs.VipLevels.Birthday.Core.ApplicationServices;
using Tgs.VipLevelService.Clients;
using Valhalla.MessageQueue;

namespace Tgs.VipLevels.Birthday.Core.UnitTest.ApplicationServices;

public class FormCreateCommandServiceUnitTest
{
	private static readonly string[] _SourceArray = ["4ucmqplxb029"];

	[Fact]
	public async void Normal_Process()
	{
		var fakeLog = NullLogger<FormCreateCommandService>.Instance;
		var fakeSystemClock = Substitute.For<ISystemClock>();
		var fakeDateTimeUtensil = Substitute.For<IDateTimeUtensil>();
		var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
		var fakeVipLevelServiceClient = Substitute.For<IVipLevelServiceClient>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new FormCreateCommand
		(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demon",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			TimeSpanMinutes: 480
		);

		var currentTime = new DateTimeOffset(2024, 02, 15, 04, 00, 00, TimeSpan.Zero);
		_ = fakeSystemClock.CurrentTime
			.Returns(currentTime);

		var timeZone = new DateTimeOffset(2024, 02, 15, 12, 00, 00, TimeSpan.FromMinutes(command.TimeSpanMinutes));
		_ = fakeDateTimeUtensil.GetTimeZone(
			currentTime,
			command.TimeSpanMinutes)
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
			fakeSystemClock,
			fakeDateTimeUtensil,
			fakePlayerServiceClient,
			fakeVipLevelServiceClient,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(VipLevelsBirthdayNatsConst.Queue),
			data: Arg.Is(new VipLevelsBirthdayFormMessage
			{
				Id = id
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async void This_player_VIP_level_benefits_have_been_distributed()
	{
		var fakeLog = NullLogger<FormCreateCommandService>.Instance;
		var fakeSystemClock = Substitute.For<ISystemClock>();
		var fakeDateTimeUtensil = Substitute.For<IDateTimeUtensil>();
		var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
		var fakeVipLevelServiceClient = Substitute.For<IVipLevelServiceClient>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new FormCreateCommand
		(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demon",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "GPI",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			TimeSpanMinutes: 480
		);

		var currentTime = new DateTimeOffset(2024, 02, 15, 04, 00, 00, TimeSpan.Zero);
		_ = fakeSystemClock.CurrentTime
			.Returns(currentTime);

		var timeZone = new DateTimeOffset(2024, 02, 15, 12, 00, 00, TimeSpan.FromMinutes(command.TimeSpanMinutes));
		_ = fakeDateTimeUtensil.GetTimeZone(
			currentTime,
			command.TimeSpanMinutes)
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
			fakeSystemClock,
			fakeDateTimeUtensil,
			fakePlayerServiceClient,
			fakeVipLevelServiceClient,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(VipLevelsBirthdayNatsConst.Queue),
			data: Arg.Any<VipLevelsBirthdayFormMessage>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}

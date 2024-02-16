using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.VipLevels.Birthday.Common.Protos;
using Tgs.VipLevels.Birthday.Core.ApplicationServices;
using Tgs.VipLevelService.Clients;
using Tgs.VipLevelService.Common.ViewModels;
using Valhalla.MessageQueue;

namespace Tgs.VipLevels.Birthday.Core.UnitTest.ApplicationServices;

public class FormQueueCommandServiceUnitTest
{
	[Fact]
	public async void Normal_Process()
	{
		var fakeLog = NullLogger<FormQueueCommandService>.Instance;
		var fakeVipLevelServiceClient = Substitute.For<IVipLevelServiceClient>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new FormQueueCommand
		(
			Id: Guid.NewGuid()
		);

		var form = new PlayerVipLevelBirthdayFormViewModel(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 2,
			Birthday: DateOnly.Parse("2024-02-15"),
			State: VipLevelService.Common.PlayerVipLevelBirthdayFormState.Create,
			Amount: 99m,
			ActualAmount: 0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.UtcNow,
			CreatedUserId: Guid.Empty,
			CreatedUsername: string.Empty,
			UpdatedOn: DateTimeOffset.UtcNow,
			UpdatedUserId: Guid.Empty,
			UpdatedUsername: string.Empty);
		_ = fakeVipLevelServiceClient.PlayerVipLevelBirthdayFormGetAsync(
			id: Arg.Is(command.Id),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new FormQueueCommandService(
			fakeLog,
			fakeVipLevelServiceClient,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(VipLevelsBirthdayNatsConst.Approved),
			data: Arg.Is(new VipLevelsBirthdayFormMessage
			{
				Id = form.Id
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async void This_form_status_is_not_created()
	{
		var fakeLog = NullLogger<FormQueueCommandService>.Instance;
		var fakeVipLevelServiceClient = Substitute.For<IVipLevelServiceClient>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new FormQueueCommand
		(
			Id: Guid.NewGuid()
		);

		var form = new PlayerVipLevelBirthdayFormViewModel(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 2,
			Birthday: DateOnly.Parse("2024-02-15"),
			State: VipLevelService.Common.PlayerVipLevelBirthdayFormState.Approved,
			Amount: 99m,
			ActualAmount: 99m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.UtcNow,
			CreatedUserId: Guid.Empty,
			CreatedUsername: string.Empty,
			UpdatedOn: DateTimeOffset.UtcNow,
			UpdatedUserId: Guid.Empty,
			UpdatedUsername: string.Empty);
		_ = fakeVipLevelServiceClient.PlayerVipLevelBirthdayFormGetAsync(
			id: Arg.Is(command.Id),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new FormQueueCommandService(
			fakeLog,
			fakeVipLevelServiceClient,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(VipLevelsBirthdayNatsConst.Approved),
			data: Arg.Is(new VipLevelsBirthdayFormMessage
			{
				Id = form.Id
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async void This_form_does_not_exist()
	{
		var fakeLog = NullLogger<FormQueueCommandService>.Instance;
		var fakeVipLevelServiceClient = Substitute.For<IVipLevelServiceClient>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new FormQueueCommand
		(
			Id: Guid.NewGuid()
		);

		_ = fakeVipLevelServiceClient.PlayerVipLevelBirthdayFormGetAsync(
			id: Arg.Is(command.Id),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelBirthdayFormViewModel?)null);

		var sut = new FormQueueCommandService(
			fakeLog,
			fakeVipLevelServiceClient,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(VipLevelsBirthdayNatsConst.Approved),
			data: Arg.Any<VipLevelsBirthdayFormMessage>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}

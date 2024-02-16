using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.VipLevels.Birthday.Core.ApplicationServices;
using Tgs.VipLevelService.Clients;
using Tgs.VipLevelService.Common.ViewModels;

namespace Tgs.VipLevels.Birthday.Core.UnitTest.ApplicationServices;

public class FormPendingCommandServiceUnitTest
{
	[Fact]
	public async void Normal_Process()
	{
		var fakeLog = NullLogger<FormPendingCommandService>.Instance;
		var fakeVipLevelServiceClient = Substitute.For<IVipLevelServiceClient>();

		var command = new FormPendingCommand
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

		var sut = new FormPendingCommandService(
			fakeLog,
			fakeVipLevelServiceClient);

		await sut.ExecuteAsync(command);

		_ = fakeVipLevelServiceClient
			.Received(1)
			.PlayerVipLevelBirthdayFormPendingAsync(
			id: Arg.Is(form.Id),
			operatorUserId: Arg.Is(Guid.Empty),
			operatorUsername: Arg.Is("system"),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async void This_form_status_is_not_created()
	{
		var fakeLog = NullLogger<FormPendingCommandService>.Instance;
		var fakeVipLevelServiceClient = Substitute.For<IVipLevelServiceClient>();

		var command = new FormPendingCommand
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

		var sut = new FormPendingCommandService(
			fakeLog,
			fakeVipLevelServiceClient);

		await sut.ExecuteAsync(command);

		_ = fakeVipLevelServiceClient
			.Received(0)
			.PlayerVipLevelBirthdayFormPendingAsync(
			id: Arg.Is(form.Id),
			operatorUserId: Arg.Is(Guid.Empty),
			operatorUsername: Arg.Is("system"),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async void This_form_does_not_exist()
	{
		var fakeLog = NullLogger<FormPendingCommandService>.Instance;
		var fakeVipLevelServiceClient = Substitute.For<IVipLevelServiceClient>();

		var command = new FormPendingCommand
		(
			Id: Guid.NewGuid()
		);

		_ = fakeVipLevelServiceClient.PlayerVipLevelBirthdayFormGetAsync(
			id: Arg.Is(command.Id),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelBirthdayFormViewModel?)null);

		var sut = new FormPendingCommandService(
			fakeLog,
			fakeVipLevelServiceClient);

		await sut.ExecuteAsync(command);

		_ = fakeVipLevelServiceClient
			.Received(0)
			.PlayerVipLevelBirthdayFormPendingAsync(
			id: Arg.Any<Guid>(),
			operatorUserId: Arg.Is(Guid.Empty),
			operatorUsername: Arg.Is("system"),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}

using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Clients;

namespace Tgs.VipLevels.Birthday.Core.ApplicationServices;
internal class FormApprovedCommandService(
	ILogger<FormApprovedCommandService> logger,
	IVipLevelServiceClient vipLevelServiceClient) : ICommandService<FormApprovedCommand>
{
	public async ValueTask ExecuteAsync(FormApprovedCommand command, CancellationToken cancellationToken = default)
	{
		var form = await vipLevelServiceClient.PlayerVipLevelBirthdayFormGetAsync(
			id: command.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (form is null)
		{
			logger.LogWarning("LogOn:{logOn} Message:This form does not exist.({id})",
				DateTime.Now.ToString("u"),
				command.Id);
			return;
		}

		if (form.State != VipLevelService.Common.PlayerVipLevelBirthdayFormState.Create)
		{
			logger.LogWarning("LogOn:{logOn} Message:This form({id}) status is not created.({state})",
				DateTime.Now.ToString("u"),
				form.Id,
				form.State);
			return;
		}

		await vipLevelServiceClient.PlayerVipLevelBirthdayFormApprovedAsync(
			id: form.Id,
			amount: form.Amount,
			remark: "This system automatically passes the review",
			operatorUserId: Guid.Empty,
			operatorUsername: string.Empty,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}


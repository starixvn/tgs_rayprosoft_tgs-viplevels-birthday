using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevels.Birthday.Common.Protos;
using Tgs.VipLevelService.Clients;
using Valhalla.MessageQueue;

namespace Tgs.VipLevels.Birthday.Core.ApplicationServices;

internal class FormQueueCommandService(
	ILogger<FormQueueCommandService> logger,
	IVipLevelServiceClient vipLevelServiceClient,
	IMessageSender messageSender) : ICommandService<FormQueueCommand>
{
	public async ValueTask ExecuteAsync(FormQueueCommand command, CancellationToken cancellationToken = default)
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

		/*
		 * 判斷是否自動還是人工
		 */
		if (true)
		{
			var message = new VipLevelsBirthdayFormMessage
			{
				Id = form.Id
			};
			await messageSender.PublishAsync(
				subject: VipLevelsBirthdayNatsConst.Approved,
				data: message,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
				DateTime.Now.ToString("u"),
				VipLevelsBirthdayNatsConst.Approved,
				message);
		}
	}
}

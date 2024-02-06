using Microsoft.Extensions.Logging;

using Tgs.ServiceUtility;
using Tgs.VipLevels.Birthday.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.VipLevels.Birthday.Core.JobApplicationServices;

internal class JobExecuteCommandService(
	ILogger<JobExecuteCommandService> logger,
	IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
	public async ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
	{
		var message = new VipLevelsBirthdayBrandMessage
		{
			ResellerId = command.ResellerId,
			ResellerCode = command.ResellerCode,
			LicenseeId = command.LicenseeId,
			LicenseeCode = command.LicenseeCode,
			BrandId = command.BrandId,
			BrandCode = command.BrandCode,
			TimeSpanMinutes = 480
		};
		await messageSender.PublishAsync(
			subject: VipLevelsBirthdayNatsConst.Create,
			data: message,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
			DateTime.Now.ToString("u"),
			VipLevelsBirthdayNatsConst.Create,
			message);
	}
}

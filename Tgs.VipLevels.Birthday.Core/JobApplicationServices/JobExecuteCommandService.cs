using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

using Tgs.ServiceUtility;
using Tgs.VipLevels.Birthday.Common;
using Tgs.VipLevels.Birthday.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.VipLevels.Birthday.Core.JobApplicationServices;

internal class JobExecuteCommandService(
	ILogger<JobExecuteCommandService> logger,
	TimeProvider timeProvider,
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
			TimeSpanMinutes = command.TimeSpanMinutes,
			CurrentTime = timeProvider.GetUtcNow().ToTimestamp()
		};
		await messageSender.PublishAsync(
			subject: VipLevelsBirthdayNatsConst.Create,
			data: message,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = VipLevelsBirthdayNatsConst.Create,
				Data = new
				{
					message.ResellerCode,
					message.LicenseeCode,
					message.BrandCode,
					message.TimeSpanMinutes,
					CurrentTime = message.CurrentTime.ToDateTimeOffset()
				},
				Message = "This message has been published."
			}));
	}
}

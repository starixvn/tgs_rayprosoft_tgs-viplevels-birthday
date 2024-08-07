using System.Text.Json;
using Tgs.ServiceUtility;
using Tgs.VipLevels.Birthday.Common.Protos;
using Tgs.VipLevels.Birthday.Core.ApplicationServices;
using Tgs.VipLevels.Birthday.Entities;
using Valhalla.MessageQueue;

namespace Tgs.VipLevels.Birthday.Messages;

internal class FormCreateMessageHandler(
	ILogger<FormCreateMessageHandler> logger,
	ICommandService<FormCreateCommand> service) : IMessageHandler<VipLevelsBirthdayBrandMessage>
{
	public ValueTask HandleAsync(string subject, VipLevelsBirthdayBrandMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					data.ResellerCode,
					data.LicenseeCode,
					data.BrandCode,
					data.TimeSpanMinutes,
					CurrentTime = data.CurrentTime.ToDateTimeOffset()
				},
				Message = "This receive message handler."
			}));

		return service.ExecuteAsync(
			command: new FormCreateCommand(
				ResellerId: data.ResellerId.ToGuid(),
				ResellerCode: data.ResellerCode,
				LicenseeId: data.LicenseeId.ToGuid(),
				LicenseeCode: data.LicenseeCode,
				BrandId: data.BrandId.ToGuid(),
				BrandCode: data.BrandCode,
				TimeSpanMinutes: data.TimeSpanMinutes,
				CurrentTime: data.CurrentTime.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}

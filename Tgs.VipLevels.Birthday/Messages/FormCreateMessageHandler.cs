using Tgs.ServiceUtility;
using Tgs.VipLevels.Birthday.Common.Protos;
using Tgs.VipLevels.Birthday.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.VipLevels.Birthday.Messages;

internal class FormCreateMessageHandler(
	ILogger<FormCreateMessageHandler> logger,
	ICommandService<FormCreateCommand> service) : IMessageHandler<VipLevelsBirthdayBrandMessage>
{
	public ValueTask HandleAsync(string subject, VipLevelsBirthdayBrandMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return service.ExecuteAsync(
			command: new FormCreateCommand(
				ResellerId: data.ResellerId.ToGuid(),
				ResellerCode: data.ResellerCode,
				LicenseeId: data.LicenseeId.ToGuid(),
				LicenseeCode: data.LicenseeCode,
				BrandId: data.BrandId.ToGuid(),
				BrandCode: data.BrandCode,
				TimeSpanMinutes: data.TimeSpanMinutes),
			cancellationToken: cancellationToken);
	}
}

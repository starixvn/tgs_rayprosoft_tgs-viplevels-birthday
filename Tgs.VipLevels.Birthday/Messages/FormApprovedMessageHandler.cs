using Tgs.ServiceUtility;
using Tgs.VipLevels.Birthday.Common.Protos;
using Tgs.VipLevels.Birthday.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.VipLevels.Birthday.Messages;

internal class FormApprovedMessageHandler(
	ILogger<FormPendingMessageHandler> logger,
	ICommandService<FormApprovedCommand> service) : IMessageHandler<VipLevelsBirthdayFormMessage>
{
	public ValueTask HandleAsync(string subject, VipLevelsBirthdayFormMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return service.ExecuteAsync(
			command: new FormApprovedCommand(
				Id: data.Id.ToGuid()),
			cancellationToken: cancellationToken);
	}
}

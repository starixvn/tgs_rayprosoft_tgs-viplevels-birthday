using Tgs.ServiceUtility;
using Tgs.VipLevels.Birthday.Common.Protos;
using Tgs.VipLevels.Birthday.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.VipLevels.Birthday.Messages;

internal class FormPendingMessageHandler(
	ILogger<FormPendingMessageHandler> logger,
	ICommandService<FormPendingCommand> service) : IMessageHandler<VipLevelsBirthdayFormMessage>
{
	public ValueTask HandleAsync(string subject, VipLevelsBirthdayFormMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return service.ExecuteAsync(
			command: new FormPendingCommand(
				Id: data.Id.ToGuid()),
			cancellationToken: cancellationToken);
	}
}

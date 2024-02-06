using Microsoft.Extensions.Logging;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;
using Tgs.VipLevels.Birthday.Common.Protos;
using Tgs.VipLevelService.Clients;
using Valhalla.MessageQueue;

namespace Tgs.VipLevels.Birthday.Core.ApplicationServices;
internal class FormCreateCommandService(
	ILogger<FormCreateCommandService> logger,
	ISystemClock systemClock,
	IPlayerServiceClient playerServiceClient,
	IVipLevelServiceClient vipLevelServiceClient,
	IMessageSender messageSender) : ICommandService<FormCreateCommand>
{
	public async ValueTask ExecuteAsync(FormCreateCommand command, CancellationToken cancellationToken = default)
	{
		IEnumerable<string> playerCodes;
		var currentTime = systemClock.GetTimeZone(command.TimeSpanMinutes);
		var birthday = DateOnly.FromDateTime(currentTime.Date);
		var page = 1;
		var rows = 100;
		do
		{
			playerCodes = await playerServiceClient.QueryPlayerBirthdayAsync(
				brandId: command.BrandId,
				birthday: birthday,
				page: page,
				rows: rows,
				cancellationToken: cancellationToken)
				.ToArrayAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? Enumerable.Empty<string>();
			foreach (var playerCode in playerCodes)
			{
				if (await vipLevelServiceClient.PlayerVipLevelBirthdayFormExistAsync(
					playerCode: playerCode,
					birthday: birthday,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false))
				{
					logger.LogWarning("LogOn:{logOn} Message:This player ({playerCode}) has already received birthday gifts this year({year}).",
						DateTime.Now.ToString("u"),
						playerCode,
						birthday.Year);
					continue;
				}

				var id = await vipLevelServiceClient.PlayerVipLevelBirthdayFormCreateAsync(
					playerCode: playerCode,
					birthday: birthday,
					operatorUserId: Guid.Empty,
					operatorUsername: string.Empty,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				var message = new VipLevelsBirthdayFormMessage
				{
					Id = id
				};
				await messageSender.PublishAsync(
					subject: VipLevelsBirthdayNatsConst.Queue,
					data: message,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
					DateTime.Now.ToString("u"),
					VipLevelsBirthdayNatsConst.Queue,
					message);
			}

			page++;

		} while (playerCodes.Any());
	}
}

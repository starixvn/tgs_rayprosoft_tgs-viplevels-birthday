using Microsoft.Extensions.Logging;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Clients;

namespace Tgs.VipLevels.Birthday.Core.ApplicationServices;

internal class FormCreateCommandService(
	ILogger<FormCreateCommandService> logger,
	ISystemClock systemClock,
	IDateTimeUtensil dateTimeUtensil,
	IPlayerServiceClient playerServiceClient,
	IVipLevelServiceClient vipLevelServiceClient) : ICommandService<FormCreateCommand>
{
	public async ValueTask ExecuteAsync(FormCreateCommand command, CancellationToken cancellationToken = default)
	{
		IEnumerable<string> playerCodes;
		var currentTime = systemClock.CurrentTime;
		var timeZone = dateTimeUtensil.GetTimeZone(currentTime, command.TimeSpanMinutes);
		var birthday = DateOnly.FromDateTime(timeZone.Date);
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
					operatorUsername: "system",
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				logger.LogInformation("LogOn:{logOn}  Message:This player VIP level form was created successfully.({id})",
					DateTime.Now.ToString("u"),
					id);
			}

			page++;
		} while (playerCodes.Any());
	}
}

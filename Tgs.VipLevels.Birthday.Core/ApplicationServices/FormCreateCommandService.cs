using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Clients;

namespace Tgs.VipLevels.Birthday.Core.ApplicationServices;

internal class FormCreateCommandService(
	ILogger<FormCreateCommandService> logger,
	IDateTimeUtensil dateTimeUtensil,
	IPlayerServiceClient playerServiceClient,
	IVipLevelServiceClient vipLevelServiceClient) : ICommandService<FormCreateCommand>
{
	public async ValueTask ExecuteAsync(FormCreateCommand command, CancellationToken cancellationToken = default)
	{
		IEnumerable<string> playerCodes;
		var timeZoneCurrentTime = dateTimeUtensil.GetTimeZone(command.CurrentTime, command.TimeSpanMinutes);
		var birthday = DateOnly.FromDateTime(timeZoneCurrentTime.Date);

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Data = new
				{
					command.ResellerCode,
					command.LicenseeCode,
					command.BrandCode,
					command.TimeSpanMinutes,
					command.CurrentTime,
					TimeZoneCurrentTime = timeZoneCurrentTime,
					Birthday = birthday
				},
				Message = "Process information in this birthday."
			}));

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
					logger.LogWarning("{logWarning}",
						JsonSerializer.Serialize(new
						{
							LogOn = DateTime.Now.ToString("u"),
							Data = new
							{
								command.ResellerCode,
								command.LicenseeCode,
								command.BrandCode,
								PlayerCode = playerCode,
								birthday.Year,
								Birthday = birthday,
							},
							Message = "This player has already received birthday gifts this year."
						}));
					continue;
				}

				var id = await vipLevelServiceClient.PlayerVipLevelBirthdayFormCreateAsync(
					playerCode: playerCode,
					birthday: birthday,
					operatorUserId: Guid.Empty,
					operatorUsername: "system",
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				logger.LogInformation("{logInformation}",
					JsonSerializer.Serialize(new
					{
						LogOn = DateTime.Now.ToString("u"),
						Data = new
						{
							command.ResellerCode,
							command.LicenseeCode,
							command.BrandCode,
							PlayerCode = playerCode,
							birthday.Year,
							Birthday = birthday,
							Id = id
						},
						Message = "This player VIP level form was created successfully."
					}));
			}

			page++;
		} while (playerCodes.Any());
	}
}

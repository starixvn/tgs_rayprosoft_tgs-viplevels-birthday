using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Tgs.VipLevels.Birthday.Common;
using Tgs.VipLevels.Birthday.Common.Protos;
using Tgs.VipLevels.Birthday.Core;
using Tgs.VipLevels.Birthday.ViewModels;
using Valhalla.MessageQueue;

namespace Tgs.VipLevels.Birthday.Controllers;

[Route("form")]
[ApiController]
public class PlayerVipLevelBirthdayFormController(
	ILogger<PlayerVipLevelBirthdayFormController> logger) : ControllerBase
{
	/// <summary>
	/// Publishes the asynchronous.
	/// </summary>
	/// <param name="repository">The repository.</param>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///		當前時間 2024-01-01T00:00:00+08:00觸發生日福利
	///		週期時間 2024-01-01
	///			{
	///				"brandCode": "GPI",
	///				"currentTime": "2024-01-01T00:00:00+08:00"
	///			}
	/// </remarks>
	[HttpPost("publish", Name = "PlayerVipLevelBirthday_Form_Create")]
	public async ValueTask PublishAsync(
			[FromServices] IBrandRepository repository,
			[FromServices] IMessageSender messageSender,
			[FromBody] FormCreateViewModel source)
	{
		var brandInfo = await repository.GetAsync(
			code: source.BrandCode,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		var message = new VipLevelsBirthdayBrandMessage
		{
			ResellerId = brandInfo.ResellerId,
			ResellerCode = brandInfo.ResellerCode,
			LicenseeId = brandInfo.LicenseeId,
			LicenseeCode = brandInfo.LicenseeCode,
			BrandId = brandInfo.BrandId,
			BrandCode = brandInfo.BrandCode,
			TimeSpanMinutes = 480,
			CurrentTime = source.CurrentTime.ToTimestamp()
		};
		await messageSender.PublishAsync(
			subject: VipLevelsBirthdayNatsConst.Create,
			data: message,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = VipLevelsBirthdayNatsConst.Create,
				Data = new
				{
					ResellerId = message.ResellerId.ToGuid(),
					message.ResellerCode,
					LicenseeId = message.LicenseeId.ToGuid(),
					message.LicenseeCode,
					BrandId = message.BrandId.ToGuid(),
					message.BrandCode,
					message.TimeSpanMinutes,
					CurrentTime = message.CurrentTime.ToDateTimeOffset()
				},
				Message = "This message has been published."
			}));
	}
}

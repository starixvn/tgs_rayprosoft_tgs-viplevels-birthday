namespace Tgs.VipLevels.Birthday.Core.ApplicationServices;
public record FormCreateCommand(
	Guid ResellerId,
	string ResellerCode,
	Guid LicenseeId,
	string LicenseeCode,
	Guid BrandId,
	string BrandCode,
	int TimeSpanMinutes);

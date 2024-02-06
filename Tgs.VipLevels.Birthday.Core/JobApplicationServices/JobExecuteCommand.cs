namespace Tgs.VipLevels.Birthday.Core.JobApplicationServices;

public record JobExecuteCommand(
    Guid ResellerId,
    string ResellerCode,
    Guid LicenseeId,
    string LicenseeCode,
    Guid BrandId,
    string BrandCode,
    int TimeSpanMinutes);

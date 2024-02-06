using Tgs.VipLevels.Birthday.Core.JobApplicationServices;

namespace Tgs.VipLevels.Birthday.Core;

public interface IHangfireUtensil
{
    public Task JobCheckAsync();

    public Task JobExecuteAsync(JobExecuteCommand source);

    public Task JobInitAsync();
}

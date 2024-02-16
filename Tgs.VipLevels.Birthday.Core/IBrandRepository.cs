using Tgs.VipLevels.Birthday.Core.Models;

namespace Tgs.VipLevels.Birthday.Core;

public interface IBrandRepository
{
	IAsyncEnumerable<BrandInfo> QueryAsync(CancellationToken cancellationToken = default);
}

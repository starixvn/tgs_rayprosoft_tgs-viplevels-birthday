using Tgs.VipLevels.Birthday.Core.Models;

namespace Tgs.VipLevels.Birthday.Core;

public interface IBrandRepository
{
	ValueTask<BrandInfo> GetAsync(string code, CancellationToken cancellationToken = default);

	IAsyncEnumerable<BrandInfo> QueryAsync(CancellationToken cancellationToken = default);
}

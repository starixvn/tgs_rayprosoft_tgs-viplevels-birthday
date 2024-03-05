using Microsoft.EntityFrameworkCore;
using Tgs.VipLevels.Birthday.Core;
using Tgs.VipLevels.Birthday.Core.Models;
using Tgs.VipLevels.Birthday.Entities;

namespace Tgs.VipLevels.Birthday.Repositories;

internal class BrandRepository(TgsContext context) : IBrandRepository
{
	public async ValueTask<BrandInfo> GetAsync(string code, CancellationToken cancellationToken = default)
		=> await context.Brands.AsQueryable()
		.Join(
		   context.Licensees,
		   inner => inner.LicenseeId,
		   outer => outer.Id,
		   (brand, licensee) => new
		   {
			   licensee.ResellerId,
			   LicenseeId = licensee.Id,
			   LicenseeCode = licensee.Code,
			   LicenseeStatus = licensee.Status,
			   BrandId = brand.Id,
			   BrandCode = brand.Code,
			   BrandStatus = brand.Status,
			   TimeSpanMinutes = brand.TimeSpanMinutes.GetValueOrDefault(0)
		   })
		.Join(
			context.Resellers,
			inner => inner.ResellerId,
			outer => outer.Id,
			(combined, reseller) => new
			{
				ResellerId = reseller.Id,
				ResellerCode = reseller.Code,
				ResellerStatus = reseller.Status,
				combined.LicenseeId,
				combined.LicenseeCode,
				combined.LicenseeStatus,
				combined.BrandId,
				combined.BrandCode,
				combined.BrandStatus,
				combined.TimeSpanMinutes
			})
		.Where(predicate: combined => combined.BrandCode == code)
		.Select(selector: combined => new BrandInfo(
			combined.ResellerId,
			combined.ResellerCode,
			combined.LicenseeId,
			combined.LicenseeCode,
			combined.BrandId,
			combined.BrandCode,
			combined.TimeSpanMinutes))
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(code);

	public IAsyncEnumerable<BrandInfo> QueryAsync(CancellationToken cancellationToken = default)
		=> context.Brands.AsQueryable()
		.Join(
		   context.Licensees,
		   inner => inner.LicenseeId,
		   outer => outer.Id,
		   (brand, licensee) => new
		   {
			   licensee.ResellerId,
			   LicenseeId = licensee.Id,
			   LicenseeCode = licensee.Code,
			   LicenseeStatus = licensee.Status,
			   BrandId = brand.Id,
			   BrandCode = brand.Code,
			   BrandStatus = brand.Status,
			   TimeSpanMinutes = brand.TimeSpanMinutes.GetValueOrDefault(0)
		   })
		.Join(
			context.Resellers,
			inner => inner.ResellerId,
			outer => outer.Id,
			(combined, reseller) => new
			{
				ResellerId = reseller.Id,
				ResellerCode = reseller.Code,
				ResellerStatus = reseller.Status,
				combined.LicenseeId,
				combined.LicenseeCode,
				combined.LicenseeStatus,
				combined.BrandId,
				combined.BrandCode,
				combined.BrandStatus,
				combined.TimeSpanMinutes
			})
		.Where(predicate: combined => combined.ResellerStatus != 2 && combined.LicenseeStatus != 2 && combined.BrandStatus != 2)
		.Select(selector: combined => new BrandInfo(
			combined.ResellerId,
			combined.ResellerCode,
			combined.LicenseeId,
			combined.LicenseeCode,
			combined.BrandId,
			combined.BrandCode,
			combined.TimeSpanMinutes))
		.AsAsyncEnumerable();
}

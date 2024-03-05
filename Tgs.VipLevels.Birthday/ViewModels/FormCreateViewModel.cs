using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevels.Birthday.ViewModels;

public record FormCreateViewModel
{
	[Required]
	public string BrandCode { get; init; } = default!;
	[Required]
	public DateTimeOffset CurrentTime { get; init; }
}

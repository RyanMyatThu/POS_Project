using YaungMel_POS.Domain.DTOs;
using YaungMel_POS.Shared.Responses;

namespace YaungMel_POS.Domain.Features.Inventory
{
    public interface IInventoryService
    {
        Task<Result<List<ProductDTO>>> GetLowStockAlertsAsync(int lowStock);
    }
}

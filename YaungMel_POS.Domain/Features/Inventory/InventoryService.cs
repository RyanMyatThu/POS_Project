using Microsoft.EntityFrameworkCore;
using Serilog.Core;
using YaungMel_POS.Database.Data;
using YaungMel_POS.Database.Models;
using YaungMel_POS.Domain.DTOs;
using YaungMel_POS.Shared.Responses;

namespace YaungMel_POS.Domain.Features.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly POSDbContext _db;

        public InventoryService(POSDbContext db)
        {
            _db = db;
        }

        private IQueryable<Tbl_Product> ActiveProduct => _db.Products.Where(p => !p.DeleteFlag);

        #region get low stock alert
        public async Task<Result<List<ProductDTO>>> GetLowStockAlertsAsync(int lowStock = 5)
        {
            try
            {
                var products = await _db.Products
                    .AsNoTracking()
                    .Where(p => !p.DeleteFlag && p.StockQuantity <= lowStock)
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        StockQuantity = p.StockQuantity,
                        CategoryId = p.CategoryId,
                        DeleteFlag = p.DeleteFlag,
                        IsActive = p.IsActive
                    })
                    .ToListAsync();

                return Result<List<ProductDTO>>.Success(products, "Low stock products retrieved.");
            }
            catch (Exception ex)
            {
                return Result<List<ProductDTO>>.SystemError(ex.Message);
            }
        }
        #endregion

    }
}

using Microsoft.EntityFrameworkCore;
using POSSampleOWN.Data;
using POSSampleOWN.DTOs;
using POSSampleOWN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSampleOWN.domain.Features.Search
{
    public class SearchService : ISearchService
    {
        private readonly POSDbContext _db;

        public SearchService(POSDbContext db)
        {
            _db = db;
        }
        private IQueryable<Tbl_Product> ActiveProductQuery => _db.Products
            .AsNoTracking()
            .Where(p => !p.DeleteFlag);

        public async Task<List<ProductDTO>> SearchProductsAsync(SearchRequestDTO searchRequest)
        {
            var query = ActiveProductQuery.AsQueryable();

            if(searchRequest.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == searchRequest.CategoryId.Value);

            if(searchRequest.MinPrice.HasValue)
                query = query.Where(p => p.Price >= searchRequest.MinPrice.Value);

            if(searchRequest.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= searchRequest.MaxPrice.Value);

            if(searchRequest.StartDate.HasValue)
                query = query.Where(p => p.CreatedAt >= searchRequest.StartDate.Value);

            if(searchRequest.EndDate.HasValue)
                query = query.Where(p => p.CreatedAt <= searchRequest.EndDate.Value);

            if(searchRequest.MinStockQuantity.HasValue)
                query = query.Where(p => p.StockQuantity >= searchRequest.MinStockQuantity.Value);

            if(searchRequest.MaxStockQuantity.HasValue)
                query = query.Where(p => p.StockQuantity <= searchRequest.MaxStockQuantity.Value);

            if (!string.IsNullOrEmpty(searchRequest.Name))
                query = query.Where(p => p.Name.Contains(searchRequest.Name));

            query = searchRequest.SortBy switch
            {
                "Name" => searchRequest.IsDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "Price" => searchRequest.IsDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "CreatedAt" => searchRequest.IsDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                _ => query
            };

            var products = await query
                .Skip((searchRequest.PageNumber - 1) * searchRequest.PageSize)
                .Take(searchRequest.PageSize)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    DeleteFlag = p.DeleteFlag
                }).ToListAsync();

            return products;
        }
    }
}

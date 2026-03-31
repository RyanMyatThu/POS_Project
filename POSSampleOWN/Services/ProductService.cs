using Microsoft.EntityFrameworkCore;
using POSSampleOWN.Data;
using POSSampleOWN.DTOs;
using POSSampleOWN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POSSampleOWN.Services
{
    public class ProductService : IProductService
    {
        private readonly POSDbContext _db;

        public ProductService(POSDbContext db)
        {
            _db = db;
        }

        private IQueryable<Product> ActiveProductQuery => _db.Products
            .AsNoTracking()
            .Where(p => !p.DeleteFlag);

        public async Task<List<ProductDTO>> GetAllProductsAsync()
        {
            return await _db.Products
                .AsNoTracking()
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();
        }

        public async Task<ProductResponseDTO> GetByIdAsync(int id)
        {
            var product = await ActiveProductQuery
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return new ProductResponseDTO
                {
                    IsSuccess = false,
                    Message = "Product not found."
                };
            }

            return new ProductResponseDTO
            {
                IsSuccess = true,
                Message = "Product retrieved successfully.",
                Data = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    CategoryId = product.CategoryId
                }
            };
        }

        public async Task<List<ProductDTO>> GetAvailableProductsAsync()
        {
            return await ActiveProductQuery
                .Where(p => p.StockQuantity > 0)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();
        }

        public async Task<ProductResponseDTO> CreateAsync(CreateProductDTO request)
        {
            var newProduct = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            await _db.Products.AddAsync(newProduct);
            var result = await _db.SaveChangesAsync() > 0;

            return new ProductResponseDTO
            {
                IsSuccess = result,
                Message = result ? "Product created successfully." : "Failed to create product."
            };
        }

        public async Task<ProductResponseDTO> UpdateAsync(int id, UpdateProductDTO request)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null)
            {
                return new ProductResponseDTO
                {
                    IsSuccess = false,
                    Message = "Product not found."
                };
            }

            if (request.Name != null) product.Name = request.Name;
            if (request.Description != null) product.Description = request.Description;
            if (request.Price > 0) product.Price = request.Price;
            if (request.StockQuantity >= 0) product.StockQuantity = request.StockQuantity;
            if (request.CategoryId != 0) product.CategoryId = request.CategoryId;
            if (request.DeleteFlag.HasValue) product.DeleteFlag = request.DeleteFlag.Value;

            product.UpdatedAt = DateTime.UtcNow;

            var result = await _db.SaveChangesAsync() > 0;

            return new ProductResponseDTO
            {
                IsSuccess = result,
                Message = result ? "Product updated successfully!" : "Failed to update product."
            };
        }

        public async Task<ProductResponseDTO> DeleteAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null)
            {
                return new ProductResponseDTO
                {
                    IsSuccess = false,
                    Message = "Product not found."
                };
            }

            product.DeleteFlag = true;
            product.UpdatedAt = DateTime.UtcNow;

            var result = await _db.SaveChangesAsync() > 0;

            return new ProductResponseDTO
            {
                IsSuccess = result,
                Message = result ? "Product deleted successfully!" : "Failed to delete product."
            };
        }
    }
}

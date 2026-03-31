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
    public class CategoryService : ICategoryService
    {
        private readonly POSDbContext _db;

        public CategoryService(POSDbContext db)
        {
            _db = db;
        }

        public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
        {
            return await _db.Categories
                .AsNoTracking()
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();
        }

        public async Task<CategoryResponseDTO> GetByIdAsync(int id)
        {
            var category = await _db.Categories
                .AsNoTracking()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
            {
                return new CategoryResponseDTO
                {
                    IsSuccess = false,
                    Message = "Category not found."
                };
            }

            return new CategoryResponseDTO
            {
                IsSuccess = true,
                Message = "Category retrieved successfully.",
                Data = new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                }
            };
        }

        public async Task<CategoryResponseDTO> CreateAsync(CreateCategoryDTO request)
        {
            var newCategory = new Category
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _db.Categories.AddAsync(newCategory);
            var result = await _db.SaveChangesAsync() > 0;

            return new CategoryResponseDTO
            {
                IsSuccess = result,
                Message = result ? "Category created successfully." : "Failed to create category.",
                Data = result ? new CategoryDTO
                {
                    Id = newCategory.Id,
                    Name = newCategory.Name,
                    Description = newCategory.Description
                } : null
            };
        }

        public async Task<CategoryResponseDTO> UpdateAsync(int id, UpdateCategoryDTO request)
        {
            var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
            {
                return new CategoryResponseDTO
                {
                    IsSuccess = false,
                    Message = "Category not found."
                };
            }

            if (!string.IsNullOrEmpty(request.Name))
                category.Name = request.Name;

            if (request.Description != null)
                category.Description = request.Description;

            var result = await _db.SaveChangesAsync() > 0;

            return new CategoryResponseDTO
            {
                IsSuccess = result,
                Message = result ? "Category updated successfully." : "No changes were made."
            };
        }

        public async Task<CategoryResponseDTO> DeleteAsync(int id)
        {
            var category = await _db.Categories
                        .Include(c => c.Products)
                        .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
            {
                return new CategoryResponseDTO
                {
                    IsSuccess = false,
                    Message = "Category not found."
                };
            }

            if (category.Products != null && category.Products.Any(p => !p.DeleteFlag))
            {
                return new CategoryResponseDTO
                {
                    IsSuccess = false,
                    Message = "Category cannot be deleted because it contains active products."
                };
            }

            category.DeleteFlag = true;
            var success = await _db.SaveChangesAsync() > 0;

            return new CategoryResponseDTO
            {
                IsSuccess = success,
                Message = success ? "Category deleted successfully." : "Failed to delete category."
            };
        }
    }
}

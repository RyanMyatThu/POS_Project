using System.Collections.Generic;
using System.Threading.Tasks;
using POSSampleOWN.DTOs;

namespace POSSampleOWN.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryResponseDTO> GetByIdAsync(int id);
        Task<CategoryResponseDTO> CreateAsync(CreateCategoryDTO request);
        Task<CategoryResponseDTO> UpdateAsync(int id, UpdateCategoryDTO request);
        Task<CategoryResponseDTO> DeleteAsync(int id);
    }
}

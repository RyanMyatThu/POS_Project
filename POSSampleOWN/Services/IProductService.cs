using System.Collections.Generic;
using System.Threading.Tasks;
using POSSampleOWN.DTOs;

namespace POSSampleOWN.Services
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllProductsAsync();
        Task<ProductResponseDTO> GetByIdAsync(int id);
        Task<List<ProductDTO>> GetAvailableProductsAsync();
        Task<ProductResponseDTO> CreateAsync(CreateProductDTO request);
        Task<ProductResponseDTO> UpdateAsync(int id, UpdateProductDTO request);
        Task<ProductResponseDTO> DeleteAsync(int id);
    }
}

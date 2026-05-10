using YaungMel_POS.Domain.DTOs;

namespace YaungMel_POS.Domain.Features.Search
{
    public interface ISearchService
    {
        Task<ProductSearchResponseDTO> SearchProductsAsync(SearchProductRequestDTO searchRequest);
        
        Task<List<CategoryDTO>> SearchCategoryAsync(SearchCategoryRequestDTO searchRequest);
    }
}

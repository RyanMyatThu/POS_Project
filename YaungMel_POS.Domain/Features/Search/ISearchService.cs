using YaungMel_POS.Domain.DTOs;
using YaungMel_POS.Shared;

namespace YaungMel_POS.Domain.Features.Search
{
    public interface ISearchService
    {
        Task<PagedResult<ProductDTO>> SearchProductsAsync(SearchProductRequestDTO request);

        Task<Result<List<CategoryDTO>>> SearchCategoryAsync(SearchCategoryRequestDTO request);
    }
}

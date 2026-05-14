using YaungMel_POS.Domain.DTOs;
using YaungMel_POS.Shared;

namespace YaungMel_POS.Domain.Features.Summary
{
    public interface ISummaryService
    {
        Task<Result<SummaryDTO>> CreateSummaryAsync();
        Task<Result<SummaryDetailDto>> GetSummaryByDateAsync(DateTime date);
        Task<PagedResult<SummaryDetailDto>> GetSummaryByPagination(PaginationRequest request);
        Task<Result<List<SummaryDTO>>> GetSummaryByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
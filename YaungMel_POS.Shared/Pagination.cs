using System.Text.Json.Serialization;

namespace YaungMel_POS.Shared;

public class Pagination
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;

    [JsonConstructor]
    public Pagination(int pageNumber, int pageSize, int totalCount, int totalPages)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
    }

    public Pagination(int pageNumber, int pageSize, int totalCount)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}

public class PagedResult<T> : Result
{
    public List<T> Data { get; init; }
    public Pagination Pagination { get; init; }

    [JsonConstructor]
    public PagedResult(bool isSuccess, string message, EnumRespType type, List<T> data, Pagination pagination)
        : base(isSuccess, message, type)
    {
        Data = data;
        Pagination = pagination;
    }

    public static PagedResult<T> Success(List<T> data, Pagination pagination, string message = "Success")
        => new(true, message, EnumRespType.Success, data, pagination);

    public static PagedResult<T> DeleteSuccess(string message = "Deleted successfully!")
        => new(true, message, EnumRespType.Success, new(), new Pagination(0, 0, 0, 0));

    public new static PagedResult<T> Failure(string message, EnumRespType type = EnumRespType.SystemError)
        => new(false, message, type, new(), new Pagination(0, 0, 0, 0));

    public static PagedResult<T> ValidationError(string message, List<T>? data = default)
        => new(false, message, EnumRespType.ValidationError, data ?? new(), new Pagination(0, 0, 0, 0));

    public static PagedResult<T> SystemError(string message, List<T>? data = default)
        => new(false, message, EnumRespType.SystemError, data ?? new(), new Pagination(0, 0, 0, 0));

    public static PagedResult<T> NotFound(string message, List<T>? data = default)
        => new(false, message, EnumRespType.NotFound, data ?? new(), new Pagination(0, 0, 0, 0));
}

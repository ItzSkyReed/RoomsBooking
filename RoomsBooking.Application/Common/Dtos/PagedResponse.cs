namespace RoomsBooking.Application.Common.Dtos;

public record PagedResponse<T>(List<T> Items, int TotalCount, int PageNumber, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
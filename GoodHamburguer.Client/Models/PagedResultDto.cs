namespace GoodHamburguer.Client.Models;

public record PagedResultDto<T>(
    IEnumerable<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);


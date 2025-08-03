using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
public static class QueryableExtensions
{
    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, QueryParameters queryParameters)
    {
        if (queryParameters.SortBy == null) return query;

        string order = queryParameters.SortOrder?.ToLower() == "desc" ? "descending" : "ascending";

        query = query.OrderBy($"{queryParameters.SortBy} {order}");

        return query;
    }

    public static async Task<myCharacter.DTOs.PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, QueryParameters queryParameters)
    {
        var totalCount = await query.CountAsync();
        var items = await query.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize).Take(queryParameters.PageSize).ToListAsync();

        return new myCharacter.DTOs.PagedResult<T>(
            items,
            queryParameters.PageNumber,
            queryParameters.PageSize,
            totalCount);
    }
}
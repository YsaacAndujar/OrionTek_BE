using Logic.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace Logic.Utils
{
    public static class HttpContextAccessorExtensions
    {
        public async static Task<IQueryable<T>> Paginate<T>(this IHttpContextAccessor contextAccessor,
            IQueryable<T> queryable, GenericFilterDto filter
            )
        {
            var httpContext = contextAccessor.HttpContext;
            double quantity = await queryable.CountAsync();
            double totalPages = Math.Ceiling(quantity / filter.EntitiesPerPage);
            httpContext.Response.Headers.Add("totalPages", totalPages.ToString());
            httpContext.Response.Headers.Add("totalEntities", quantity.ToString());
            return queryable.Skip((filter.Page - 1) * filter.EntitiesPerPage)
                .Take((filter.EntitiesPerPage));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UserRoleMgtApi.Models;
using UserRoleMgtApi.Models.Dtos;

namespace UserRoleMgtApi.Helpers
{
    public class PagedList<T>
    {
        public static PaginatedListDto<T> GetPagedData(List<T> paginatedList, int page, int perPage, int total)
        {
            var total_pages = total % perPage == 0 ? total / perPage : total / perPage + 1;

            var pageMeta = new PageMeta
            {
                Page = page,
                PerPage = perPage,
                Total = total,
                TotalPages = total_pages
            };

            return new PaginatedListDto<T>
            {
                MetaData = pageMeta,
                Data = paginatedList
            };
        }

        public static IEnumerable<T> Paginate(List<T> source, int page, int perPage)
        {
            page = page < 1 ? 1 : page;

            return source.Skip((page - 1) * perPage).Take(perPage);
        }
    }
}

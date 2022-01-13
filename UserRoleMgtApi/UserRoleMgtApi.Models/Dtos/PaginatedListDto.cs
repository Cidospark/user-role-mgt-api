using System;
using System.Collections.Generic;

namespace UserRoleMgtApi.Models.Dtos
{
    public class PaginatedListDto<T>
    {
        public PageMeta MetaData { get; set; }
        public IEnumerable<T> Data { get; set; }

        public PaginatedListDto()
        {
            Data = new List<T>();
        }
    }
}

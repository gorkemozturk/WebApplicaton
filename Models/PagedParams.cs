using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class PagedParams
    {
        [Range(1, 100000, ErrorMessage = "Offset have to be greater than 0 less than 100.000.")]
        public int? Offset { get; set; }

        [Range(1, 100, ErrorMessage = "Limit have to be greater than 0 less than 100.")]
        public int? Limit { get; set; }

        public PagedParams Replace(PagedParams pagedParams)
        {
            return new PagedParams
            {
                Offset = pagedParams.Offset ?? this.Offset,
                Limit = pagedParams.Limit ?? this.Limit
            };
        }
    }
}

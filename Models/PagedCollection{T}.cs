using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class PagedCollection<T> : Collection<T>
    {
        public static PagedCollection<T> Create(Link self, T[] items, int size, PagedParams pagedParams)
        {
            return new PagedCollection<T>
            {
                Self = self,
                Value = items,
                Size = size,
                Offset = pagedParams.Offset.Value,
                Limit = pagedParams.Limit.Value,
                First = self,
                Last = GetLastLink(self, size, pagedParams),
                Next = GetNextLink(self, size, pagedParams),
                Previous = GetPreviousLink(self, size, pagedParams)
            };
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Offset { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Limit { get; set; }
        public int Size { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Link First { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Link Last { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Link Next { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Link Previous { get; set; }

        private static Link GetNextLink(Link self, int size, PagedParams pagedParams)
        {
            if (pagedParams?.Limit == null || pagedParams?.Offset == null)
            {
                return null;
            }

            var limit = pagedParams.Limit.Value;
            var offset = pagedParams.Offset.Value;
            var next = offset + limit;

            if (next >= size)
            {
                return null;
            }

            var parameters = new RouteValueDictionary(self.RouteValues)
            {
                ["limit"] = limit,
                ["offset"] = next
            };

            return ToCollection(self.RouteName, parameters);
        }

        private static Link GetPreviousLink(Link self, int size, PagedParams pagedParams)
        {
            if (pagedParams?.Limit == null || pagedParams?.Offset == null)
            {
                return null;
            }

            var limit = pagedParams.Limit.Value;
            var offset = pagedParams.Offset.Value;

            if (offset == 0)
            {
                return null;
            }

            if (offset > size)
            {
                return GetLastLink(self, size, pagedParams);
            }

            var previousPage = Math.Max(offset - limit, 0);

            if (previousPage <= 0)
            {
                return self;
            }

            var parameters = new RouteValueDictionary(self.RouteValues)
            {
                ["limit"] = limit,
                ["offset"] = previousPage
            };

           return ToCollection(self.RouteName, parameters);
        }

        private static Link GetLastLink(Link self, int size, PagedParams pagedParams)
        {
            if (pagedParams?.Limit == null)
            {
                return null;
            }

            var limit = pagedParams.Limit.Value;

            if (size <= limit)
            {
                return null;
            }

            var offset = Math.Ceiling((size - (double)limit) / limit) * limit;

            var parameters = new RouteValueDictionary(self.RouteValues)
            {
                ["limit"] = limit,
                ["offset"] = offset
            };

            return ToCollection(self.RouteName, parameters);
        }
    }
}

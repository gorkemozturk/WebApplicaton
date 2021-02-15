using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApplication.Helpers;
using WebApplication.Models;

namespace WebApplication.Filters
{
    public class RewritedLinkFilter : IAsyncResultFilter
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public RewritedLinkFilter(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var result = context.Result as ObjectResult;
            var skipped = result?.StatusCode >= 400 || result?.Value == null || result?.Value as Resource == null;

            if (skipped)
            {
                return next();
            }

            var rewriter = new LinkHelper(_urlHelperFactory.GetUrlHelper(context));
            RewriteLinks(result.Value, rewriter);

            return next();
        }

        private static void RewriteLinks(object value, LinkHelper rewriter)
        {
            if (value == null)
            {
                return;
            }

            var properties = value
                .GetType()
                .GetTypeInfo()
                .GetProperties()
                .Where(p => p.CanRead)
                .ToArray();

            var links = properties.Where(p => p.CanWrite && p.PropertyType == typeof(Link));

            foreach (var item in links)
            {
                var rewritten = rewriter.Rewrite(item.GetValue(value) as Link);

                if (rewritten == null)
                {
                    continue;
                }

                item.SetValue(value, rewritten);

                if (item.Name == nameof(Resource.Self))
                {
                    properties.FirstOrDefault(p => p.Name == nameof(Resource.Href))
                        ?.SetValue(value, rewritten.Href);

                    properties.FirstOrDefault(p => p.Name == nameof(Resource.Method))
                        ?.SetValue(value, rewritten.Method);

                    properties.FirstOrDefault(p => p.Name == nameof(Resource.Relations))
                        ?.SetValue(value, rewritten.Relations);
                }
            }

            var ar = properties.Where(p => p.PropertyType.IsArray);
            RewriteLinksInArrays(ar, value, rewriter);

            var objects = properties.Except(links).Except(ar);
            RewriteLinksInNestedObjects(objects, value, rewriter);
        }

        private static void RewriteLinksInNestedObjects(IEnumerable<PropertyInfo> objects, object value, LinkHelper rewriter)
        {
            foreach (var item in objects)
            {
                if (item.PropertyType == typeof(string))
                {
                    continue;
                }

                var info = item.PropertyType.GetTypeInfo();

                if (info.IsClass)
                {
                    RewriteLinks(item.GetValue(value), rewriter);
                }
            }
        }

        private static void RewriteLinksInArrays(IEnumerable<PropertyInfo> ar, object value, LinkHelper rewriter)
        {
            foreach (var item in ar)
            {
                var array = item.GetValue(value) as Array ?? new Array[0];

                foreach (var element in array)
                {
                    RewriteLinks(element, rewriter);
                }
            }
        }
    }
}

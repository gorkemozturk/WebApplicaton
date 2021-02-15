using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Helpers
{
    public class LinkHelper
    {
        private readonly IUrlHelper _urlHelper;

        public LinkHelper(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public Link Rewrite(Link link)
        {
            if (link == null)
            {
                return null;
            }

            return new Link
            {
                Href = _urlHelper.Link(link.RouteName, link.RouteValues),
                Method = link.Method,
                Relations = link.Relations
            };
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Attributes;

namespace WebApplication.Models
{
    public class Room : Resource
    {
        [Sortable]
        [SearchableString]
        public string Name { get; set; }

        [Sortable(Default = true)]
        [SearchableDecimal]
        public decimal Rate { get; set; }

        [Sortable]
        [SearchableDateTime]
        public DateTimeOffset CreatedAt { get; set; }
    }
}

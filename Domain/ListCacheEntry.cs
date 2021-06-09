using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ondato.Domain
{
    public sealed class ListCacheEntry
    {
        public ListCacheEntry() { }
        public ListCacheEntry(string key, List<byte[]> values, TimeSpan slidingExpiration)
        {
            this.Key = key;
            this.Values = values;
            this.SlidingExpiration = slidingExpiration;
        }

        [Required]
        public string? Key { get; set; }

        [Required]
        public List<byte[]>? Values { get; set; }

        public TimeSpan SlidingExpiration { get; set; }
    }
}

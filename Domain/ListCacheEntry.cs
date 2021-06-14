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
            this.SlidingExpiration = slidingExpiration.TotalMilliseconds;
        }

        [Required]
        public string? Key { get; set; }

        [Required]
        public List<byte[]>? Values { get; set; }

        public double SlidingExpiration { get; set; }

        public TimeSpan GetSlidingExpiration() => TimeSpan.FromMilliseconds(this.SlidingExpiration);
    }
}

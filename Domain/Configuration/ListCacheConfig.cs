using System;

namespace Ondato.Domain.Configuration
{
    public sealed class ListCacheConfig
    {
        public TimeSpan SlidingExpiration { get; set; }
    }
}

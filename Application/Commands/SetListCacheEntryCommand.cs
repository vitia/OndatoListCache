using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Ondato.Common;
using Ondato.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Ondato.Domain.Configuration;

namespace Ondato.Application.Commands
{
    public sealed record SetListCacheEntryCommand(IDistributedCache Cache, IOptions<ListCacheConfig> Configuration)
    {
        public async Task<ListCacheEntry> InvokeAsync(string key, List<byte[]> values, TimeSpan slidingExpiration, CancellationToken ct)
        {
            var data = ObjSerializer.Serialize(values);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = slidingExpiration == TimeSpan.Zero
                    ? Configuration.Value.SlidingExpiration
                    : slidingExpiration
            };
            await Cache.SetAsync(key, data, options, ct);
            return new ListCacheEntry(key, values, slidingExpiration);
        }
    }
}

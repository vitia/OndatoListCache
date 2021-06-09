using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Ondato.Common;
using Microsoft.Extensions.Caching.Distributed;

namespace Ondato.Application.Commands
{
    public sealed record GetListCacheEntryCommand(IDistributedCache Cache)
    {
        public async Task<List<byte[]>?> InvokeAsync(string key, CancellationToken ct)
        {
            var data = await Cache.GetAsync(key, ct);
            return data is null ? null : (List<byte[]>)ObjSerializer.Deserialize(data);
        }
    }
}

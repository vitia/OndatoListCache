using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Caching.Distributed;

namespace Ondato.Application.Commands
{
    public sealed record RemoveListCacheEntryCommand(IDistributedCache Cache)
    {
        public async Task InvokeAsync(string key, CancellationToken ct)
        {
            await Cache.RemoveAsync(key, ct);
        }
    }
}

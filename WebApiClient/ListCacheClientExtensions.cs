using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ondato.Common;

namespace Ondato.WebApiClient
{
    public static class ListCacheClientExtensions
    {
        public static async Task<List<object>> GetListByKeyAsync(this ListCacheClient client, string key, CancellationToken ct)
        {
            var values = await client.GetByKeyAsync(key, ct);
            return ToObjectList(values);
        }

        public static async Task CreateListAsync(this ListCacheClient client, string key, List<object> list, TimeSpan slidingExpiration, CancellationToken ct)
        {
            var values = ToValues(list);
            await client.CreateAsync(new() { Key = key, Values = values, SlidingExpiration = slidingExpiration }, ct);
        }

        public static async Task UpdateListAsync(this ListCacheClient client, string key, List<object> list, TimeSpan slidingExpiration, CancellationToken ct)
        {
            var values = ToValues(list);
            await client.UpdateAsync(key, new() { Key = key, Values = values, SlidingExpiration = slidingExpiration }, ct);
        }

        public static async Task DeleteListAsync(this ListCacheClient client, string key, CancellationToken ct)
        {
            await client.DeleteAsync(key, ct);
        }

        private static List<byte[]> ToValues(IEnumerable<object> list)
        {
            return list.Select(o => ObjSerializer.Serialize(o)).ToList();
        }
        private static List<object> ToObjectList(IEnumerable<byte[]> list)
        {
            return list.Select(o => ObjSerializer.Deserialize(o)).ToList();
        }
    }
}

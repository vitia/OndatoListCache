using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Ondato.WebApiClient;

namespace Ondato.WebApiClientTests
{
    [TestFixture]
    public class ListCacheClientTest
    {
        // https://localhost:44395/swagger/v1/swagger.json
        private const string ServiceUrl = "https://localhost:44395/ListCache/";
        private const string ServiceApiKey = "test123";

        [Test]
        public void Unauthorized_access_should_fail()
        {
            var ct = CancellationToken.None;
            var client = CreateClient("badApiKey");
            Assert.ThrowsAsync<ApiException>(async () => await client.GetListByKeyAsync("key1", ct));
        }

        [Test]
        public void Get_missing_key_should_fail()
        {
            var ct = CancellationToken.None;
            var client = CreateClient();
            Assert.ThrowsAsync<ApiException<ProblemDetails>>(async () => await client.GetListByKeyAsync("bad1", ct));
        }

        [Test]
        public async Task Authorized_create_update_get_delete_should_succeed()
        {
            var ct = CancellationToken.None;
            var client = CreateClient();
            var key = "k1";
            var values1 = new List<object> { 101, "str1" };
            var values2 = new List<object> { 201, "str2" };
            var longExpiration = TimeSpan.FromSeconds(500);
            var shortExpiration = TimeSpan.FromMilliseconds(500);

            await client.CreateListAsync(key, values1, longExpiration, ct);

            var resCreated = await client.GetListByKeyAsync(key, ct);
            CollectionAssert.AreEqual(resCreated, values1);

            await client.UpdateListAsync(key, values2, longExpiration, ct);

            var resAppended = await client.GetListByKeyAsync(key, ct);
            CollectionAssert.AreEqual(new List<object> { 101, "str1", 201, "str2" }, resAppended);

            await client.DeleteListAsync(key, ct);

            var resDeleted = await client.GetListByKeyAsync(key, ct);
            Assert.IsNull(resDeleted);

            await client.UpdateListAsync(key, values2, shortExpiration, ct);

            var resUpdated = await client.GetListByKeyAsync(key, ct);
            Assert.IsNull(resUpdated);

            await Task.Delay(shortExpiration);

            var resExpired = await client.GetListByKeyAsync(key, ct);
            Assert.IsNull(resExpired);
        }

        [Test]
        public void Delete_missing_key_should_succeed()
        {
            //curl -X DELETE "https://localhost:44395/ListCache/bad1" -H "accept: */*" -H "X-API-KEY: test123"
            var ct = CancellationToken.None;
            var client = CreateClient();
            Assert.DoesNotThrowAsync(async () => await client.DeleteListAsync("bad1", ct));
        }

        private ListCacheClient CreateClient(string apiKey = ServiceApiKey)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
            return new ListCacheClient(ServiceUrl, httpClient);
        }
    }
}
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
        private const string ServiceUrl = "https://localhost:44395";
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

            void AssertMissing() => Assert.ThrowsAsync<ApiException<ProblemDetails>>(async () => await client.GetListByKeyAsync(key, ct));
            async void AssertValues(List<object> expected) => CollectionAssert.AreEqual(expected, await client.GetListByKeyAsync(key, ct));

            AssertMissing();

            await client.CreateListAsync(key, values1, longExpiration, ct);
            AssertValues(values1);

            await client.UpdateListAsync(key, values2, longExpiration, ct);
            AssertValues(new List<object> { 101, "str1", 201, "str2" });

            await client.DeleteListAsync(key, ct);
            AssertMissing();

            await client.UpdateListAsync(key, values2, shortExpiration, ct);
            AssertValues(values2);

            await Task.Delay(shortExpiration);
            AssertMissing();
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
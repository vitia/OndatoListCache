using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ondato.Application.Commands;
using Ondato.Domain;
using Ondato.Domain.Configuration;

namespace Ondato.WebApi.Controllers
{
    [ApiController]
    //[ApiConventionType(typeof(DefaultApiConventions))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("[controller]")]
    public class ListCacheController : ControllerBase
    {
        private readonly ILogger<ListCacheController> _logger;
        private readonly IOptions<ListCacheConfig> _configuration;
        private readonly IDistributedCache _cache;

        public ListCacheController(ILogger<ListCacheController> logger, IOptions<ListCacheConfig> configuration, IDistributedCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        [HttpGet("{key}", Name = nameof(GetByKey))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<byte[]>>> GetByKey(string key, CancellationToken ct)
        {
            var values = await new GetListCacheEntryCommand(_cache).InvokeAsync(key, ct);
            if (values is null)
            {
                return NotFound();
            }

            return values;
        }

        [HttpPost(Name = nameof(Create))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(ListCacheEntry entry, CancellationToken ct)
        {
            Debugger.Launch();
            if (entry?.Key is null)
            {
                return BadRequest("Key is required.");
            }
            if (entry?.Values is null)
            {
                return BadRequest("Values are required.");
            }

            var createdEntry = await new SetListCacheEntryCommand(_cache, _configuration).InvokeAsync(entry.Key, entry.Values, entry.SlidingExpiration, ct);
            return CreatedAtAction(nameof(GetByKey), new { key = entry.Key }, null);
        }

        [HttpPut("{key}", Name = nameof(Update))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(string key, ListCacheEntry entry, CancellationToken ct)
        {
            Debugger.Launch();
            if (key != entry?.Key)
            {
                return BadRequest("Key missmatch.");
            }
            if (entry?.Values is null)
            {
                return BadRequest("Values are required.");
            }

            var values = await new GetListCacheEntryCommand(_cache).InvokeAsync(key, ct);
            if (values is null)
            {
                var createdEntry = await new SetListCacheEntryCommand(_cache, _configuration).InvokeAsync(key, entry.Values, entry.SlidingExpiration, ct);
                return CreatedAtAction(nameof(GetByKey), new { key }, null);
            }

            values.AddRange(entry.Values);
            await new SetListCacheEntryCommand(_cache, _configuration).InvokeAsync(key, values, entry.SlidingExpiration, ct);
            return Ok();
        }

        [HttpDelete("{key}", Name = nameof(Delete))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(string key, CancellationToken ct)
        {
            Debugger.Launch();
            await new RemoveListCacheEntryCommand(_cache).InvokeAsync(key, ct);
            return NoContent();
        }
    }
}

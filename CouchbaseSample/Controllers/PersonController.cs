using System;
using System.Net;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.N1QL;
using CouchbaseSample.Models;
using Microsoft.AspNetCore.Mvc;

namespace CouchbaseSample.Controllers
{
    [Route("api/")]
    public class PersonController : Controller
    {
        private readonly IBucket _bucket;
		//private readonly IBucketContext _ctx;

	    public PersonController(IBucketProvider bucketProvider)
		{
            _bucket = bucketProvider.GetBucket("workshop");
			//_ctx = context;
		}

        [HttpGet]
        [Route("get/{id?}")]
        public async Task<IActionResult> Get(string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Missing or empty 'id' query string parameter");
            }

            var result = await _bucket.GetAsync<Person>(id);
            if (!result.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, result.Exception?.Message ?? result.Message);
            }

            result.Value.Id = id;

            return Ok(result.Value);
        }

        [HttpGet]
        [Route("getAll")]
        public async Task<IActionResult> Getall()
        {
            var query = new QueryRequest()
                .Statement("SELECT META().id, b.* FROM `workshop` b WHERE type = $1")
                .AddPositionalParameter(typeof(Person).Name.ToLower())
                .ScanConsistency(ScanConsistency.RequestPlus);

            var result = await _bucket.QueryAsync<Person>(query);
            if (!result.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, result.Exception?.Message ?? result.Message);
            }

            return Ok(result.Rows);
        }

		//[HttpGet]
		//[Route("getAll2")]
		//public async Task<IActionResult> GetAll2()
		//{
		//	var res = _ctx.Query<Person>()
		//		.ScanConsistency(ScanConsistency.RequestPlus)
		//	    .ToList();

		//	return Ok(res);
		//}

		[HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save([FromBody] Person person)
        {
            if (person == null || !person.IsValid())
            {
                return BadRequest("Missing or invalid body content");
            }

            if (string.IsNullOrEmpty(person.Id))
            {
                person.Id = Guid.NewGuid().ToString();
            }

            var result = await _bucket.UpsertAsync(person.Id, person);
            if (!result.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, result.Exception?.Message ?? result.Message);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromBody] Person person)
        {
            if (string.IsNullOrEmpty(person.Id))
            {
                return BadRequest("Missing or invalid 'document_id' body parameter");
            }

            var result = await _bucket.RemoveAsync(person.Id);
            if (!result.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, result.Exception?.Message ?? result.Message);
            }

            return Ok(result);
        }
    }
}

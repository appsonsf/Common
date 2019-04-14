using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommonTestWeb.DTOs;
using AppsOnSF.Common.BaseServices;
using Microsoft.AspNetCore.Mvc;

namespace CommonTestWeb.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ISimpleKeyValueService _simpleKeyValueService;

        public ValuesController(ISimpleKeyValueService simpleKeyValueService)
        {
            _simpleKeyValueService = simpleKeyValueService;
        }

        const string ContainerName = "CommonTestWeb.Controllers.ValuesController";
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var result = await _simpleKeyValueService.GetAll(ContainerName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // GET api/values/5
        [HttpGet("{key}")]
        public async Task<IActionResult> GetAsync(string key)
        {
            var result = await _simpleKeyValueService.Get(ContainerName, key);

            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]ValueInput input)
        {
            await _simpleKeyValueService.Add(ContainerName, input.Key, input.Value);
            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{key}")]
        public async Task<IActionResult> PutAsync(string key, [FromBody]ValueInput input)
        {
            var result = await _simpleKeyValueService.Update(ContainerName, key, input.Value);
            if (result) return Ok();
            return NoContent();
        }

        // DELETE api/values/5
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteAsync(string key)
        {
            var result = await _simpleKeyValueService.Remove(ContainerName, key);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}

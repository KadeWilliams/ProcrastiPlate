using Microsoft.AspNetCore.Mvc;
// using Microsoft.IdentityModel.Tokens;
using ProcrastiPlate.API.Services.Interface;

// using ProcrastiPlate.API.Services.Interfaces;

namespace ProcrastiPlate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IProcrastiPlateService _service;
        public AuthorController(IProcrastiPlateService service)
        {
            _service = service;
        }
        // GET: api/<AuthorController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AuthorController>/5
        [HttpGet("{id}")]
        public IActionResult GetAuthor(int id)
        {
            return Ok(_service.GetAuthor(id));
        }

        // POST api/<AuthorController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AuthorController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuthorController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
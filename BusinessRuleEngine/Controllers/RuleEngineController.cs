using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BusinessRuleEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuleEngineController : ControllerBase
    {
        // GET: api/<RuleEngine>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello", "This API works!" };
        }

        /*
        // GET api/<RuleEngine>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RuleEngine>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<RuleEngine>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RuleEngine>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */
    }
}

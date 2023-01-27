using BusinessRuleEngine.Entities;
using BusinessRuleEngine.Repositories;
using Microsoft.AspNetCore.Mvc;

/*
 * As of this moment, execute rule only retreives data that is stored locally, upon running the project that is
 * what will be displayed.
 */

namespace BusinessRuleEngine.Controllers
{
    [Route("api/ExecuteRule")]
    [ApiController]
    public class ExecuteRuleController : ControllerBase
    {
        // this will store the class with the data saved locally
        private readonly RuleInterface memoryRepo;

        // populate the class above with data
        public ExecuteRuleController(RuleInterface repo)
        {
            this.memoryRepo = repo;
        }

        // GET: api/<RuleEngine>
        [HttpGet]
        public IEnumerable<Rule> Get()
        {
            var rules = memoryRepo.GetRules();
            return rules;
        }
    }
}

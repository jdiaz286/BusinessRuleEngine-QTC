using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Mvc;
using BusinessRuleEngine.Entities; // import the Rule class from the entities folder
using BusinessRuleEngine.DTO;
using System.Data.SqlClient;
using System.Data;
using Rule = BusinessRuleEngine.Entities.Rule;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BusinessRuleEngine.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AddRuleController : ControllerBase
    {
        // this variable will store the configuration that we pass it in
        public readonly IConfiguration _configuration;

        // populate class with all data needed
        public AddRuleController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }


        // GET: api/<AddRuleController>
        // possibly remove this???
        [HttpGet]
        [Route("GetAllRules")]
        // returns a json formatted result of the rules saved on an sql database specified under "appsettings.json"
        public string Get()
        {
            // create an instance of Response to return any possible errors
            Response response = new Response();

            // establish connection to sql database
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString());

            // create a query to retreive all the rules from the database
            string query = "Select * FROM RuleTable";
            SqlDataAdapter data = new SqlDataAdapter(query, conn);

            // create a data table and populate it with data above
            DataTable dt = new DataTable();
            data.Fill(dt);

            // create an arraylist to save the rules
            List<Rule> rulesList = new List<Rule>();

            // if we have at least 1 row of rules, retreive it and print 
            if (dt.Rows.Count > 0)
            {
                // loop through all the rows in the data table
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    // create a new rule and populate it
                    Rule currentRule = new Rule
                    {
                        RuleID = Convert.ToString(dt.Rows[i]["ruleID"]),
                        RuleName = Convert.ToString(dt.Rows[i]["ruleName"]),
                        ExpressionID = Convert.ToString(dt.Rows[i]["expressionId"]),
                        PositiveAction = Convert.ToString(dt.Rows[i]["positiveAction"]),
                        PositiveValue = Convert.ToString(dt.Rows[i]["positiveValue"]),
                        NegativeAction = Convert.ToString(dt.Rows[i]["negativeAction"]),
                        NegativeValue = Convert.ToString(dt.Rows[i]["negativeValue"])
                    };

                    // add the current rule to the arraylist
                    rulesList.Add(currentRule);
                }
            }

            // if the arraylist has at least 1 item, then return it as a json object
            if (rulesList.Count > 0)
            {
                Console.WriteLine(JsonConvert.SerializeObject(rulesList).GetType());
                return JsonConvert.SerializeObject(rulesList);
                //return JObject.Parse(rulesList);
            }
            else
            {
                response.StatusCode = 100;
                response.ErrorMessage = "No data was found in the database";
                return JsonConvert.SerializeObject(response);
            }


        }


        // PUT api/<AddRuleController>
        // add a rule based on what the user has sent (ruleID and expressionID are generated randomly)

        [HttpPut]
        [Route("AddRule")]
        public void CreateRule(CreateRuleDTO ruleDTO)
        {
            // check if the rule already exists in the database

            // get all the elements needed to create a rule
            Rule newRule = new Rule
            {
                //RuleID = new Guid(),
                RuleName = ruleDTO.RuleName,
                //ExpressionID = new Guid(),
                PositiveAction = ruleDTO.PositiveAction,
                PositiveValue = ruleDTO.PositiveValue,
                NegativeAction = ruleDTO.NegativeAction,
                NegativeValue = ruleDTO.NegativeValue
            };

            Console.WriteLine("The values in body: "+newRule);
            //return CreatedAtAction()
        }

        
    }
}

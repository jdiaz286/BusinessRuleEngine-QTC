using Microsoft.AspNetCore.Mvc;
using BusinessRuleEngine.Entities; // import the Rule class from the entities folder
using BusinessRuleEngine.DTO;
using Rule = BusinessRuleEngine.Entities.Rule;
using BusinessRuleEngine.Repositories; // import the repositories folder from the project
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace BusinessRuleEngine.Controllers
{
    //[Route("/[controller]")]
    [ApiController]
    public class AddRuleController : ControllerBase
    {
        #region variables
        // used to import the sql repository to read all the rules from
        private readonly SQLRepository sqlRepo;

        // used to read the info from appsettings.json
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public AddRuleController(IConfiguration configuration)
        {
            this._configuration = configuration; // retrieves configuration passed in (appsettings.json)
            this.sqlRepo = new SQLRepository(_configuration); // pass in data retrieved from server to instance of SQLRepository
        }
        #endregion

        #region GetAllRules
        // returns a json formatted result of the rules saved on an sql database specified under "appsettings.json"
        // GET: /<GetAllRules>
        [HttpGet]
        [Route("GetAllRules")] // change this to change name on swaggerUI
        public IEnumerable<Rule> Get()
        {
            // instantiate the JsonObject that will be returned to the user
            JsonObject message = new JsonObject();

            // get the rules from the sql repository and save as a variable
            var rulesList = sqlRepo.getAllRules();

            // if the arraylist has at least 1 item, then return it as a json object letting the user know
            if (rulesList.Count == 0)
            {
                message.Add("Message", "No Rules found in database");
                //return message;
            }

            // Debug.WriteLine("size of the list: " +info.Count);
            return rulesList;
            
        }
        #endregion


        #region AddRule
        // PUT /AddRule
        // add a rule based on what the user has sent (ruleID and expressionID are generated randomly)
        [HttpPut]
        [Route("AddRule")]
        public JsonObject CreateRule(CreateRuleDTO ruleDTO)
        {
            JsonObject message = new JsonObject() { };

            // get the name of the rule that is going to be added
            string nameOfNewRule = ruleDTO.RuleName;

            // check if the rule already exists in the database
            if (sqlRepo.ruleExists(nameOfNewRule))
            {
                // add message letting the user know they cannot add the rule 
                message.Add("Status", "Cannot add rule '" + nameOfNewRule + "' because it already exists");

                // retreive existing rule in database
                Rule existingRule = sqlRepo.getRule(nameOfNewRule);

                var ruleValues = new JsonArray() { };

                ruleValues.Add(new JsonObject()
                {
                    ["Rule ID"] = existingRule.RuleID,
                    ["Rule Name"] = existingRule.RuleName,
                    ["Expression ID"] = existingRule.ExpressionID,
                    ["Positive Action"] = existingRule.PositiveAction,
                    ["Positive Value"] = existingRule.PositiveValue,
                    ["Negative Action"] = existingRule.NegativeAction,
                    ["Positive Value"] = existingRule.NegativeValue
                });

                // to add to JsonArray (nameOfValue, valueToInsert)
                message.Add("Existing rule values", ruleValues);

            }
            else
            {
                // get all the elements needed to create a rule
                Rule newRule = new Rule
                {
                    RuleID = Guid.NewGuid().ToString(),
                    RuleName = ruleDTO.RuleName,
                    ExpressionID = ruleDTO.ExpressionID,
                    PositiveAction = ruleDTO.PositiveAction,
                    PositiveValue = ruleDTO.PositiveValue,
                    NegativeAction = ruleDTO.NegativeAction,
                    NegativeValue = ruleDTO.NegativeValue
                };

                // if invalid expression, let the user know
                if (!sqlRepo.expressionExists(newRule.ExpressionID))
                {
                    message.Add("Error", "Could not find expression id '" + newRule.ExpressionID+ "', please type in a valid expression id");
                }
                else
                {
                    sqlRepo.addRule(newRule);

                    message.Add("Status", "Successfully added rule '" + newRule.RuleName + "' with ID: " + newRule.RuleID);
                }
            }

            return message;
        }
        #endregion

        #region EditRule
        [HttpPut]
        [Route("EditRule")]
        public JsonObject EditRule(CreateRuleDTO ruleDTO, string ruleID)
        {
            // create a JsonObject to let the user know if there are any errors
            JsonObject message = new JsonObject() { };

            // if the rule doesn't exist, let the user know
            if (!sqlRepo.ruleExistsWithID(ruleID))
            {
                // add message letting the user know they cannot remove the rule 
                message.Add("Status", "There is no rule with ID '" + ruleID + "' that exists, please type in an existing rule ID.");
            }
            else
            {
                // if invalid expression, let the user know
                if (!sqlRepo.expressionExists(ruleDTO.ExpressionID))
                {
                    message.Add("Error", "Could not find expression id '" + ruleDTO.ExpressionID + "', please type in a valid expression id");
                }
                else
                {
                    // edit the rule on the sql repository
                    sqlRepo.editRule(ruleDTO, ruleID);

                    // let the user know the change has been executed successfully
                    message.Add("Status", "Successfully updated rule with ID '" + ruleID + "'.");
                }
            }

            return message;
        }
        #endregion

        #region DeleteRule
        // /DeleteRule
        // delete a rule given the rule id
        [HttpDelete]
        [Route("DeleteRule")]
        public JsonObject DeleteRule(string ruleName)
        {
            JsonObject message = new JsonObject() { };

            // if the rule name does not exist then let the user know
            if (!sqlRepo.ruleExists(ruleName))
            {
                // add message letting the user know they cannot remove the rule 
                message.Add("Status", "There is no rule named '" + ruleName + "' that exists, please type in an existing rule name");
            }
            else
            {
                // delete the rule from the repository given 
                sqlRepo.deleteRule(ruleName);
                message.Add("Status", "Successfully deleted rule named '" + ruleName + "'");
            }

            return message;
        }
        #endregion

    }
}

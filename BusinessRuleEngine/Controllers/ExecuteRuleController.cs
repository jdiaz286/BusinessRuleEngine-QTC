using BusinessRuleEngine.Entities;
using BusinessRuleEngine.Model;
using BusinessRuleEngine.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

// TODO: Start to make the rule engine work
namespace BusinessRuleEngine.Controllers
{
    [Route("api/RuleEngine")]
    [ApiController]
    public class ExecuteRuleController : ControllerBase
    {
        #region variables
        // used to import the sql repository to read all the rules from
        private SQLRepository sqlRepo;

        // used to read the info from appsettings.json
        private readonly IConfiguration _configuration;
        #endregion

        #region constructor
        public ExecuteRuleController(IConfiguration configuration)
        {
            this._configuration = configuration; // retrieves configuration passed in (appsettings.json)
            this.sqlRepo = new SQLRepository(_configuration); // pass in data retrieved from server to instance of SQLRepository
        }
        #endregion

        #region start rule
        // GET: api/<RuleEngine>
        [Route("/ExecuteRule")]
        [HttpGet]
        public JsonObject startRule(string ruleName, [FromBody] JsonArray userParameters)
        {
            // instantiate the JsonObject that will be returned to the user
            JsonObject result = new JsonObject();

            // loop through all the items in JsonArray that the user passed in parameter and pass in values as a JsonObject
            for (int objectIndex = 0; objectIndex < userParameters.Count; objectIndex++)
            {
                // pass in rule name, body parameters, and reference to result array to ExecuteRule() method
                ExecuteRule(ruleName, userParameters[objectIndex], ref result, objectIndex);
            }

            // return the result of the Json array 
            return result;
        }
        #endregion

        #region rule execution
        // this method will recursively keep on executing rules until we reach an output or we find an error
        private void ExecuteRule(string ruleName, JsonNode userParameters, ref JsonObject result, int currentIndexInParameters)
        {
            // get the rule given the rule name
            Rule currentRuleInfo = sqlRepo.getRule(ruleName);

            // if the rule isn't found, let the user know and don't continue any further
            if (currentRuleInfo == null)
            {
                result.Add("Entry " + currentIndexInParameters + " output", "Error, rule '" + ruleName + "' not found");
                return;
            }

            // get the expression given the expression id provided in the rule
            Expression currentExpressionInfo = sqlRepo.getExpression(currentRuleInfo.ExpressionID);

            // create an instance of Expression Evaluator and pass in json node vals and expression to evaluate
            // for more info on Expression, check under Model Folder, ExpressionEvaluator.cs
            ExpressionEvaluator exEval = new ExpressionEvaluator(currentExpressionInfo, userParameters, result, currentIndexInParameters, sqlRepo);

            // evaluate the expression of the given instance and return a boolean value, pass in 0 to represent count of nested expressions
            int expressionEvaluation = exEval.evaluateExpression(0);

            // if evaluation yields 1, execute the positive action 
            if (expressionEvaluation == 1)
            {
                // if the rule positiveAction is "ExecuteRule" then call this uri to recurse onto the next rule
                if (currentRuleInfo.PositiveAction.Equals("ExecuteRule"))
                {
                    // go on to execute the ruleName under the current rule positiveValue
                    ExecuteRule(currentRuleInfo.PositiveValue, userParameters, ref result, currentIndexInParameters);
                }
                // if it is not execute rule then just return the output value
                else
                {
                    result.Add("Entry " + currentIndexInParameters + " output", currentRuleInfo.PositiveValue);
                }
            }
            // if the evaluation is 0, execute negative action
            else if (expressionEvaluation == 0)
            {
                // if the rule positiveAction is "ExecuteRule" then call this uri to recurse onto the next rule
                if (currentRuleInfo.NegativeAction.Equals("ExecuteRule"))
                {
                    // go on to execute the ruleName under the current rule negativeValue
                    ExecuteRule(currentRuleInfo.NegativeValue, userParameters, ref result, currentIndexInParameters);
                }
                // if it is not execute rule then just return the output value
                else
                {
                    result.Add("Entry " + currentIndexInParameters + " output", currentRuleInfo.NegativeValue);
                }
            }
            // if the evaluation is -2, the operator was not recognized, let the user know
            else if (expressionEvaluation == -2)
            {
                //result.Add("Entry " + currentIndexInParameters + " output", "Expression operator '"+e+"' is not a valid operator");
                Debug.WriteLine("operator not valid");
            }

            // if did not belong in any category, then let the user know of error
            else
            {
                result.Add("Entry " + currentIndexInParameters + " output", "The input did not satisfy any of the expressions");
            }
        }
        #endregion
    }
}
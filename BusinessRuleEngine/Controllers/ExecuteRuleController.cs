using BusinessRuleEngine.Entities;
using BusinessRuleEngine.Model;
using BusinessRuleEngine.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Text.Json.Nodes;

// TODO: Start to make the rule engine work
namespace BusinessRuleEngine.Controllers
{
    [Route("api/RuleEngine")]
    [ApiController]
    public class ExecuteRuleController : ControllerBase
    {
        // used to import the sql repository to read all the rules from
        private readonly SQLRepository sqlRepo;

        // used to read the info from appsettings.json
        private readonly IConfiguration _configuration;

        // populate the class
        public ExecuteRuleController(IConfiguration configuration)
        {
            this._configuration = configuration; // retrieves configuration passed in (appsettings.json)
            this.sqlRepo = new SQLRepository(_configuration); // pass in data retrieved from server to instance of SQLRepository
        }

        // GET: api/<RuleEngine>
        [Route("/ExecuteRule")]
        [HttpGet]
        public JsonObject startRule(string ruleName, [FromBody] JsonArray userParameters)
        {
            // the line below is an example of how to retreive 'gender' in the first json object
            //Debug.WriteLine("userParameters['gender']: " + userParameters[0]["gender"]);

            Debug.WriteLine("startRule() Executed!!!");

            // instantiate the JsonObject that will be returned to the user
            JsonObject result = new JsonObject();

            // use the lines below to reference how to add values to the result
            /*
            // create the JSONArray that will return the evaluations or errors to the user
            JsonObject result = new JsonObject()
            {
            };
            result.Add("Second value added", "some other value");

            var OutputValues = new JsonArray() { };

            OutputValues.Add(new JsonObject()
            {
                ["nested val 1"] = "test val",
                ["nest 2"] = "rand value here",
            });

            // to add to JsonArray (nameOfValue, valueToInsert)
            result.Add("array values",OutputValues);

            Debug.WriteLine("result['Second value added']: " + result["Second value added"]);
            */

            // loop through all the items in JsonArray that the user passed in parameter and pass in values as a JsonObject
            for (int objectIndex = 0; objectIndex<userParameters.Count; objectIndex++)
            {
                // pass in rule name, body parameters, and reference to result array to ExecuteRule() method
                ExecuteRule(ruleName, userParameters[objectIndex], ref result, objectIndex);
            }


            // return the result of the Json array 
            return result;
        }
    
        // this method will recursively keep on executing rules until we reach an output or we find an error
        private void ExecuteRule(string ruleName, JsonNode userParameters, ref JsonObject result, int currentIndexInParameters)
        {
            /*
            Debug.WriteLine("\nExecuteRule() Executed!!!");

            // get the rule given the rule name
            Rule currentRuleInfo = sqlRepo.getRule(ruleName);

            // get the expression given the expression id provided in the rule
            Expression currentExpressionInfo = sqlRepo.getExpression(currentRuleInfo.ExpressionID);

            // create an instance of Expression Evaluator and pass in json node vals and expression to evaluate
            // for more info on Expression, check under Model Folder, ExpressionEvaluator.cs
            ExpressionEvaluator exEval = new ExpressionEvaluator(currentExpressionInfo, userParameters,result);

            // evaluate the expression of the given instance and return a boolean value
            bool expressionEvaluation = exEval.evaluateExpression();

            Debug.WriteLine("value of expressionEvaluation: " + expressionEvaluation);

            // if true evaluation, execute the positive action 
            if (expressionEvaluation)
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
                    Debug.WriteLine("positive rule value added to jsonNode: " + currentRuleInfo.PositiveValue);
                    result.Add("output"+currentIndexInParameters, currentRuleInfo.PositiveValue);
                }
            }
            // if the evaluation is false, execute negative action
            if (!expressionEvaluation)
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
                    Debug.WriteLine("negative rule value added to jsonNode: "+ currentRuleInfo.NegativeValue);
                    result.Add("output"+ currentIndexInParameters, currentRuleInfo.NegativeValue);
                }
            }*/
        }

    }
}
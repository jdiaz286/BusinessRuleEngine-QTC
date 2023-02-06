﻿using BusinessRuleEngine.Entities;
using BusinessRuleEngine.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;

/*
 * As of this moment, execute rule only retreives data that is stored locally, upon running the project that is
 * what will be displayed.
 */

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
            this.sqlRepo = new SQLRepository(_configuration, "RuleTable"); // pass in data retrieved from server to instance of SQLRepository
        }

        // GET: api/<RuleEngine>
        [Route("/ExecuteRule")]
        [HttpGet]
        public string startRule(string ruleID, [FromBody] JsonContent userParameters)
        {
            var output = "";

            // get the rule given the rule ID
            Rule currentRuleInfo = sqlRepo.getRule(ruleID);

            // get the expression given the expression id provided in the rule
            Expression currentExpressionInfo = sqlRepo.getExpression(currentRuleInfo.ExpressionID);

            bool expressionEvaluation = evaluateExpression(currentExpressionInfo, userParameters);

            // if true evaluation, execute the positive action 
            if (expressionEvaluation = true)
            {
                // if the rule positiveAction is "ExecuteRule" then call this uri to recurse onto the next rule
                if (currentRuleInfo.PositiveAction.Equals("ExecuteRule"))
                {
                    var nextRuleName = currentRuleInfo.PositiveValue;
                    startRule(sqlRepo.getRuleID(nextRuleName), userParameters);
                }
                // if it is not execute rule then just return the output value
                else
                {
                    return currentRuleInfo.PositiveValue;
                }
            }
            // if the evaluation is false, execute negative action
            else if(expressionEvaluation == false) 
            {
                // if the rule positiveAction is "ExecuteRule" then call this uri to recurse onto the next rule
                if (currentRuleInfo.PositiveAction.Equals("ExecuteRule"))
                {
                    var nextRuleName = currentRuleInfo.NegativeValue;
                    startRule(sqlRepo.getRuleID(nextRuleName), userParameters);
                }
                // if it is not execute rule then just return the output value
                else
                {
                    return currentRuleInfo.NegativeValue;
                }
            }

            return "something went wrong";
        }

        // method to evaluate the expression and return either positive (true) or negative (false)
        private bool evaluateExpression(Expression expression, JsonContent userParameters)
        {
            // create a boolean variable to track the amount of time 
            bool expressionEvaluation = false;

            // save the left and right operand types alongside their corresponding values
            var leftOperandType = expression.LeftOperandType;
            var leftOperandValue = expression.LeftOperandValue;

            var rightOperandType = expression.RightOperandType;
            var rightOperandValue = expression.RightOperandValue;

            // check to make sure both operand types are the same, if so then determine which method should be used to evaluate
            if (leftOperandType.ToLower().Equals(rightOperandType.ToLower()))
            {
                // TODO: take into account more objects besides strings and integers
                switch (leftOperandType.ToLower())
                {
                    case "string":
                        Console.WriteLine("Evaluating string expression.");
                        expressionEvaluation = evaluateString(leftOperandValue, rightOperandValue ,expression.Operator, userParameters);
                        break;
                    case "integer":
                        Console.WriteLine("Evaluating integer expression.");
                        expressionEvaluation = evaluateInteger(leftOperandValue, rightOperandValue, expression.Operator, userParameters);
                        break;
                    default:
                        Console.WriteLine("Object type not yet programmed.");
                        break;
                }
            }

            // return the evaluation of the expression
            return expressionEvaluation;
        }

        /* 
         * Methods below will be used to evaluate expressions
         */

        private bool evaluateString(string leftOperandVal, string rightOperandVal, string expressionOperator, JsonContent userParameters)
        {
            bool evaluation = false;

            // TODO: Take into account more scenarios besides just "="
            // if there is an equals sign in front of the operator, then determine if the strings equal each other
            if (expressionOperator[0].Equals("="))
            {
                // save the rest of the operator that isn't a "="
                string targetValue = expressionOperator.Substring(0);

                // if the string version of userParameters contains the target value, return true to execute positive rule
                if (userParameters.ToString().Contains(targetValue))
                {
                    return true;
                }
            }

            return false;
        }

        private bool evaluateInteger(string leftOperandVal, string rightOperandVal, string expressionOperator, JsonContent userParameters)
        {
            // if there is a "<" as the first character then return the evaluate as <
            if (expressionOperator[0].Equals("<"))
            {
                // save the rest of the operator that isn't a "<"
                string remainingString = expressionOperator.Substring(0);
                int targetValue = Int32.Parse(remainingString);

                // TODO: Change the 30 to find the target value that the user wants
                // if the string version of userParameters contains the target value, return true to execute positive rule
                if (30 < targetValue)
                {
                    return true;
                }
            }

            return false;
        }


        
    }
}

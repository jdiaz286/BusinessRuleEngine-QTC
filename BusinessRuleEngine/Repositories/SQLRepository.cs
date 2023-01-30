using System.Data;
using System.Data.SqlClient;
using BusinessRuleEngine.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rule = BusinessRuleEngine.Entities.Rule;

namespace BusinessRuleEngine.Repositories
{
    /*
     * This class is inteneded to connect to an sql server and retreive all the rules/expressions stored
     */
    public class SQLRepository
    {
        // create an arraylist to save the rules
        List<Rule> rulesList = new List<Rule>();
        HashSet<string> namesOfRules = new HashSet<string>(); // this is to easily retreive the name of a rule to see if a rule is in db

        // create an arraylist to save the expressions
        List<Expression> expressionsList = new List<Expression>();
        HashSet<string> namesOfExpressions = new HashSet<string>(); // this is to easily retreive the name of a expression to see if a expression is in db

        public SQLRepository(IConfiguration _configuration, string tableName) {

            // create an instance of Response to return any possible errors
            Response response = new Response();

            // establish connection to sql database
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString());

            // create a query to retreive all the rules from the database
            string query = "Select * FROM "+tableName;
            SqlDataAdapter data = new SqlDataAdapter(query, conn);

            // create a data table and populate it with data above
            DataTable dt = new DataTable();
            data.Fill(dt);

            // if we have at least 1 row of rules, retreive it and print 
            if (dt.Rows.Count > 0)
            {
                // loop through all the rows in the data table
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (tableName.Equals("RuleTable"))
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

                        // add the current rule name to the hashset
                        namesOfRules.Add(currentRule.RuleName);
                    }
                    
                    else if (tableName.Equals("ExpressionTable"))
                    {
                        // create a new expression and populate it
                        Expression currentExpression = new Expression
                        {
                            ExpressionID = Convert.ToString(dt.Rows[i]["expressionID"]),
                            LeftOperandType = Convert.ToString(dt.Rows[i]["leftOperandType"]),
                            LeftOperandValue = Convert.ToString(dt.Rows[i]["leftOperandValue"]),
                            RightOperandType = Convert.ToString(dt.Rows[i]["rightOperandType"]),
                            RightOperandValue = Convert.ToString(dt.Rows[i]["rightOperandValue"]),
                            Operator = Convert.ToString(dt.Rows[i]["operator"])
                        };

                        // add the current rule to the arraylist
                        expressionsList.Add(currentExpression);

                        // add the current expression id to the hashset
                        namesOfRules.Add(currentExpression.ExpressionID);
                    
                    }
                    
                }
            }

        }

        // method to retrieve all info rules from the "rulesList" arraylist
        public List<Rule> getAllRules()
        {
            return rulesList;
        }
       
        // given a rule name, determine if it was found in the rules table
        public bool ruleExists(string ruleName)
        {   
            return namesOfRules.Contains(ruleName); // returns true if the rule exists, if not then false
        }
    }
}

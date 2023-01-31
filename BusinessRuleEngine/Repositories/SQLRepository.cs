using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using BusinessRuleEngine.Entities;
using Rule = BusinessRuleEngine.Entities.Rule;

namespace BusinessRuleEngine.Repositories
{
    /*
     * This class is inteneded to connect to an sql server and retreive all the rules/expressions stored
     */
    public class SQLRepository
    {
        IConfiguration _configuration;
        // create an arraylist to save the rules
        List<Rule> rulesList = new List<Rule>();
        HashSet<string> namesOfRules = new HashSet<string>(); // this is to easily retreive the name of a rule to see if a rule is in db

        // create an arraylist to save the expressions
        List<Expression> expressionsList = new List<Expression>();
        HashSet<string> namesOfExpressions = new HashSet<string>(); // this is to easily retreive the name of a expression to see if a expression is in db

        // the contstructor for this file is designed to retrieve data from rules and expressions tables
        public SQLRepository(IConfiguration _configuration, string tableName) {
            this._configuration = _configuration;

            // create an instance of Response to return any possible errors
            Response response = new Response();

            // TODO: surround the connection in the constructor with a try catch
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
            conn.Close();
        }

        /*
         * The methods below are meant to add rules and Expressions to the database and datastructures above
         */
        public void addRule(Rule ruleToAdd)
        {
            try
            {
                // query that we want to execute to insert into rule table
                string query = "INSERT INTO RuleTable (ruleID, ruleName, expressionID, positiveAction, positiveValue, negativeAction, negativeValue)";
                query += " VALUES ('"+ ruleToAdd.RuleID +"', '"+ruleToAdd.RuleName+"', '"+ruleToAdd.ExpressionID+"', '"+ruleToAdd.PositiveAction+"', '"+ruleToAdd.PositiveValue+"', '"+ruleToAdd.NegativeAction+"', '"+ruleToAdd.NegativeValue+"')";

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
                using (var command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    command.ExecuteNonQuery(); // use ExecuteNonQuery because we don't expect to return anything

                    rulesList.Add(ruleToAdd);
                    namesOfRules.Add(ruleToAdd.RuleName);

                    Debug.WriteLine("rule added: "+ruleToAdd);
                }
            }
            catch (Exception ex)
            {
                // Handle any exception.
                Debug.WriteLine(ex.ToString());
            }
        }

        // method to add an expression to the database
        public void addExpression(Expression expressionToAdd)
        {
            try
            {
                // query that we want to execute to insert into expression table
                string query = "INSERT INTO ExpressionTable (expressionID, leftOperandType, leftOperandValue, rightOperandType, rightOperandValue, operator)";
                query += " VALUES ('" + expressionToAdd.ExpressionID + "', '" + expressionToAdd.LeftOperandType + "', '" + expressionToAdd.LeftOperandValue + "', '" + expressionToAdd.RightOperandType + "', '" + expressionToAdd.RightOperandValue + "', '" + expressionToAdd.Operator + "')";

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
                using (var command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    command.ExecuteNonQuery(); // use ExecuteNonQuery because we don't expect to return anything

                    expressionsList.Add(expressionToAdd);
                    namesOfExpressions.Add(expressionToAdd.ExpressionID); // TODO: possibly remove this as it would be redundant to chcek if a expression exists, or maybe check existing expression another way?

                    Debug.WriteLine("rule added: " + expressionToAdd);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        /*
         * The methods below are meant to retreive data from the 
         */
        // method to retrieve all rules from the "rulesList" arraylist
        public List<Rule> getAllRules()
        {
            return rulesList;
        }
       
        // given a rule name, determine if it was found in the rules table
        public bool ruleExists(string ruleName)
        {   
            return namesOfRules.Contains(ruleName); // returns true if the rule exists, if not then false
        }

        // method to retreive all expressions from the "expressionsList" arraylist
        public List<Expression> getAllExpressions()
        {
            return expressionsList;
        }

        // TODO Possibly remove expressionExists()?
        // given a expression id, determine if it was fround from expressions table
        public bool expressionExists(string expressionID)
        {
            return namesOfExpressions.Contains(expressionID);
        }
    }
}

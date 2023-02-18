using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using BusinessRuleEngine.DTO;
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
        Dictionary<string, Rule> namesWithRules = new Dictionary<string, Rule>(); // store rule id and given rule <rule id, Rule object>
        Dictionary<string, string> namesWithRuleID = new Dictionary<string, string>();

        // create an arraylist to save the expressions
        List<Expression> expressionsList = new List<Expression>();
        HashSet<string> namesOfExpressions = new HashSet<string>(); // this is to easily retreive the name of a expression to see if a expression is in db
        Dictionary<string, Expression> idsOfExpressions = new Dictionary<string, Expression>();

        // the contstructor for this file is designed to retrieve data from rules and expressions tables
        public SQLRepository(IConfiguration _configuration, string tableName)
        {
            this._configuration = _configuration;

            // create an instance of Response to return any possible errors
            //Response response = new Response();

            try
            {
                // create a query to retreive all the rules from the database
                string ruleQuery = "Select * FROM RuleTable";

                // create a query to retreive all the expressions from the database
                string expressionQuery = "Select * FROM ExpressionTable";

                // get data for the rule table
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
                using (SqlDataAdapter ruleData = new SqlDataAdapter(ruleQuery, conn))
                {
                    // create a data table and populate it with data above
                    DataTable ruleDT = new DataTable();
                    ruleData.Fill(ruleDT);

                    // if we have at least 1 row of rules, retreive it and print 
                    if (ruleDT.Rows.Count > 0)
                    {
                        // loop through all the rows in the data table
                        for (int i = 0; i < ruleDT.Rows.Count; i++)
                        {
                            // create a new rule and populate it
                            Rule currentRule = new Rule
                            {
                                RuleID = Convert.ToString(ruleDT.Rows[i]["ruleID"]),
                                RuleName = Convert.ToString(ruleDT.Rows[i]["ruleName"]),
                                ExpressionID = Convert.ToString(ruleDT.Rows[i]["expressionId"]),
                                PositiveAction = Convert.ToString(ruleDT.Rows[i]["positiveAction"]),
                                PositiveValue = Convert.ToString(ruleDT.Rows[i]["positiveValue"]),
                                NegativeAction = Convert.ToString(ruleDT.Rows[i]["negativeAction"]),
                                NegativeValue = Convert.ToString(ruleDT.Rows[i]["negativeValue"])
                            };

                            // add the current rule to the arraylist
                            rulesList.Add(currentRule);

                            // add the current rule name to the hashset
                            namesOfRules.Add(currentRule.RuleName);

                            namesWithRules.Add(currentRule.RuleName, currentRule);
                            namesWithRuleID.Add(currentRule.RuleName, currentRule.RuleID);
                        }
                    }
                }

                // get data for the expression table
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
                using (SqlDataAdapter expressionData = new SqlDataAdapter(expressionQuery, conn))
                {
                    // create a data table and populate it with data above
                    DataTable expressionDT = new DataTable();
                    expressionData.Fill(expressionDT);
                    // if we have at least 1 row of rules, retreive it and print 
                    if (expressionDT.Rows.Count > 0)
                    {
                        // loop through all the rows in the data table
                        for (int i = 0; i < expressionDT.Rows.Count; i++)
                        {
                            // create a new expression and populate it
                            Expression currentExpression = new Expression
                            {
                                ExpressionID = Convert.ToString(expressionDT.Rows[i]["expressionID"]),
                                LeftOperandType = Convert.ToString(expressionDT.Rows[i]["leftOperandType"]),
                                LeftOperandValue = Convert.ToString(expressionDT.Rows[i]["leftOperandValue"]),
                                RightOperandType = Convert.ToString(expressionDT.Rows[i]["rightOperandType"]),
                                RightOperandValue = Convert.ToString(expressionDT.Rows[i]["rightOperandValue"]),
                                Operator = Convert.ToString(expressionDT.Rows[i]["operator"])
                            };

                            // add the current rule to the arraylist
                            expressionsList.Add(currentExpression);

                            // add the current expression id to the hashset
                            namesOfExpressions.Add(currentExpression.ExpressionID);

                            idsOfExpressions.Add(currentExpression.ExpressionID, currentExpression);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

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
                //query += " VALUES (, + ruleToAdd.ExpressionID + "', '" + ruleToAdd.PositiveAction + "', '" + ruleToAdd.PositiveValue + "', '" + ruleToAdd.NegativeAction + "', '" + ruleToAdd.NegativeValue + "')";
                query += "VALUES (@rID, @rName, @eID, @posAct, @posVal, @negAct, @negVal)";

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
                using (var command = new SqlCommand(query, conn))
                {
                    SqlParameter[] sqlParams = new SqlParameter[7];

                    // store all the parameters in sqlParams
                    sqlParams[0] = new SqlParameter("@rID", ruleToAdd.RuleID);
                    sqlParams[1] = new SqlParameter("@rName", ruleToAdd.RuleName);
                    sqlParams[2] = new SqlParameter("@eID", ruleToAdd.ExpressionID);
                    sqlParams[3] = new SqlParameter("@posAct", ruleToAdd.PositiveAction);
                    sqlParams[4] = new SqlParameter("@posVal", ruleToAdd.PositiveValue);
                    sqlParams[5] = new SqlParameter("@negAct", ruleToAdd.NegativeAction);
                    sqlParams[6] = new SqlParameter("negVal", ruleToAdd.NegativeValue);

                    // add all 7 sqlParams to the command
                    for (int i = 0; i < sqlParams.Length; i++)
                    {
                        command.Parameters.Add(sqlParams[i]);
                    }

                    conn.Open();
                    command.ExecuteNonQuery(); // use ExecuteNonQuery because we don't expect to return anything

                    rulesList.Add(ruleToAdd);
                    namesOfRules.Add(ruleToAdd.RuleName);

                    Debug.WriteLine("rule added: " + ruleToAdd);
                }
            }
            catch (Exception ex)
            {
                // Handle any exception.
                Debug.WriteLine(ex.ToString());
            }
        }

        public void deleteRule(string ruleToDelete)
        {
            try
            {
                // query that we want to execute to insert into rule table
                string query = "DELETE FROM RuleTable WHERE ruleID= '";
                //query += ruleToDelete + "'";
                query += " VALUES (@rName)";

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
                using (var command = new SqlCommand(query, conn))
                {
                    SqlParameter[] sqlParams = new SqlParameter[1];

                    // store all the parameters in sqlParams
                    sqlParams[0] = new SqlParameter("@rName", ruleToDelete);

                    // add all sqlParams to the command
                    for (int i = 0; i < sqlParams.Length; i++)
                    {
                        command.Parameters.Add(sqlParams[i]);
                    }
                    conn.Open();
                    command.ExecuteNonQuery(); // use ExecuteNonQuery because we don't expect to return anything

                    Debug.WriteLine("rule deleted: " + ruleToDelete);
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
                //query += " VALUES ('" + expressionToAdd.ExpressionID + "', '" + expressionToAdd.LeftOperandType + "', '" + expressionToAdd.LeftOperandValue + "', '" + expressionToAdd.RightOperandType + "', '" + expressionToAdd.RightOperandValue + "', '" + expressionToAdd.Operator + "')";
                query += " VALUES (@eID, @lot, @lov, @rot, @rov, @op)";

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
                using (var command = new SqlCommand(query, conn))
                {
                    SqlParameter[] sqlParams = new SqlParameter[6];

                    // store all the parameters in sqlParams
                    sqlParams[0] = new SqlParameter("@eID", expressionToAdd.ExpressionID);
                    sqlParams[1] = new SqlParameter("@lot", expressionToAdd.LeftOperandType);
                    sqlParams[2] = new SqlParameter("@lov", expressionToAdd.LeftOperandValue);
                    sqlParams[3] = new SqlParameter("@rot", expressionToAdd.RightOperandType);
                    sqlParams[4] = new SqlParameter("@rov", expressionToAdd.RightOperandValue);
                    sqlParams[5] = new SqlParameter("@op", expressionToAdd.Operator);

                    // add all 6 sqlParams to the command
                    for (int i = 0; i < sqlParams.Length; i++)
                    {
                        command.Parameters.Add(sqlParams[i]);
                    }

                    conn.Open();
                    command.ExecuteNonQuery(); // use ExecuteNonQuery because we don't expect to return anything

                    expressionsList.Add(expressionToAdd);
                    namesOfExpressions.Add(expressionToAdd.ExpressionID); // TODO: possibly remove this as it would be redundant to chcek if a expression exists, or maybe check existing expression another way?

                    Debug.WriteLine("Expression added: " + expressionToAdd);
                }
            }
            catch (Exception ex)
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

        // given a rule namee, return the Rule with all information
        public Rule getRule(string ruleName)
        {
            return namesWithRules[ruleName];
        }

        public string getRuleID(string ruleName)
        {
            return namesWithRuleID["ruleName"];
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

        public Expression getExpression(string expressionID)
        {
            return idsOfExpressions[expressionID];
        }
    }
}
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using BusinessRuleEngine.DTO;
using BusinessRuleEngine.Entities;
using Expression = BusinessRuleEngine.Entities.Expression;
using Rule = BusinessRuleEngine.Entities.Rule;

namespace BusinessRuleEngine.Repositories
{
    /*
     * This class is inteneded to connect to an sql server and retreive all the rules/expressions stored
     */
    public class SQLRepository
    {
        #region variables
        IConfiguration _configuration;

        List<Rule> rulesList = new List<Rule>(); // List of rules to store all expressions retreived from the db
        //HashSet<string> namesOfRules = new HashSet<string>(); // this is to easily retreive the name of a rule to see if a rule is in db
        Dictionary<string, Rule> ruleGivenName = new Dictionary<string, Rule>(); // store rule id and given rule <rule id, Rule>
        Dictionary<string, string> ruleIDGivenName = new Dictionary<string, string>(); // store rule name alongside rule id <rule name, rule id>

        List<Expression> expressionsList = new List<Expression>(); // List of expressions to store all expressions retreived from the db
        Dictionary<string, Expression> expressionGivenID = new Dictionary<string, Expression>(); // store Expression given expression id <expression ID, Expression>
        #endregion

        // the contstructor for this file is designed to retrieve data from rules and expressions tables
        #region constructor
        public SQLRepository(IConfiguration _configuration)
        {
            this._configuration = _configuration;

            // establish connection to sql database
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString());

            #region Rule retreival from db
            // create a query to retreive all the rules from the database
            string ruleQuery = "Select * FROM RuleTable";
            SqlDataAdapter ruleData = new SqlDataAdapter(ruleQuery, conn);

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

                    ruleGivenName.Add(currentRule.RuleName, currentRule);
                    ruleIDGivenName.Add(currentRule.RuleName, currentRule.RuleID);
                }
            }
            #endregion

            #region Expression retrieval from db
            // create a query to retreive all the expressions from the database
            string expressionQuery = "Select * FROM ExpressionTable";
            SqlDataAdapter expressionData = new SqlDataAdapter(expressionQuery, conn);

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
                        Operator = Convert.ToString(expressionDT.Rows[i]["operator"]),
                        LeftOperandName = Convert.ToString(expressionDT.Rows[i]["LeftOperandName"]),
                        RightOperandName = Convert.ToString(expressionDT.Rows[i]["RightOperandName"])
                    };

                    // add the current rule to the arraylist
                    expressionsList.Add(currentExpression);

                    expressionGivenID.Add(currentExpression.ExpressionID, currentExpression);
                }
            }
            #endregion
            conn.Close();
        }
        #endregion

        /*
         * The methods below are meant to manipulate Rules 
         */
        #region AddRule
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
                    sqlParams[6] = new SqlParameter("@negVal", ruleToAdd.NegativeValue);

                    // add all 7 sqlParams to the command
                    for (int i = 0; i < sqlParams.Length; i++)
                    {
                        command.Parameters.Add(sqlParams[i]);
                    }

                    conn.Open();
                    command.ExecuteNonQuery(); // use ExecuteNonQuery because we don't expect to return anything

                    rulesList.Add(ruleToAdd);
                    ruleIDGivenName.Add(ruleToAdd.RuleName, ruleToAdd.RuleID);
                }
            }
            catch (Exception ex)
            {
                // Handle any exception.
                Debug.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region EditRule
        public void editRule(CreateRuleDTO ruleToEdit,string ruleID)
        {
            try
            {
                // query that we want to execute to insert into rule table
                string query = "UPDATE dbo.RuleTable SET ruleName=@rName, expressionID=@eID, positiveAction=@pA, positiveValue=@pV, negativeAction=@nA, negativeValue=@nV WHERE ruleID=@rID";
                /*query += "SET ruleName='" + ruleToEdit.RuleName +"', expressionID='" + ruleToEdit.ExpressionID +"', " +
                    "positiveAction='" + ruleToEdit.PositiveAction + "', positiveValue='" + ruleToEdit.PositiveValue + "', " +
                    "negativeAction='" + ruleToEdit.NegativeAction + "', negativeValue='" + ruleToEdit.NegativeValue + "' WHERE " +
                    "ruleID='" + ruleToEdit.RuleID + "';";*/

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
                using (var command = new SqlCommand(query, conn))
                {
                    SqlParameter[] sqlParams = new SqlParameter[7];

                    // store all the parameters in sqlParams
                    sqlParams[0] = new SqlParameter("@rID", ruleID);
                    sqlParams[1] = new SqlParameter("@rName", ruleToEdit.RuleName);
                    sqlParams[2] = new SqlParameter("@eID", ruleToEdit.ExpressionID);
                    sqlParams[3] = new SqlParameter("@pA", ruleToEdit.PositiveAction);
                    sqlParams[4] = new SqlParameter("@pV", ruleToEdit.PositiveValue);
                    sqlParams[5] = new SqlParameter("@nA", ruleToEdit.NegativeAction);
                    sqlParams[6] = new SqlParameter("@nV", ruleToEdit.NegativeValue);

                    // add all 7 sqlParams to the command
                    for (int i = 0; i < sqlParams.Length; i++)
                    {
                        command.Parameters.Add(sqlParams[i]);
                    }

                    conn.Open();
                    command.ExecuteNonQuery(); // use ExecuteNonQuery because we don't expect to return anything
                }
            }
            catch (Exception ex)
            {
                // Handle any exception.
                Debug.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region DeleteRule
        public void deleteRule(string ruleName)
        {
            try
            {
                // query that we want to execute to insert into rule table
                string query = "DELETE FROM RuleTable WHERE ruleName=(@rName)";

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
                using (var command = new SqlCommand(query, conn))
                {
                    SqlParameter[] sqlParams = new SqlParameter[1];

                    // store all the parameters in sqlParams
                    sqlParams[0] = new SqlParameter("@rName", ruleName);

                    // add all sqlParams to the command
                    for (int i = 0; i < sqlParams.Length; i++)
                    {
                        command.Parameters.Add(sqlParams[i]);
                    }
                    conn.Open();
                    command.ExecuteNonQuery(); // use ExecuteNonQuery because we don't expect to return anything
                }
            }
            catch (Exception ex)
            {
                // Handle any exception.
                Debug.WriteLine(ex.ToString());
            }
        }
        #endregion

        /*
         * The methods below are meant to manipulate Expressions 
         */
        #region AddExpression
        // method to add an expression to the database
        public void addExpression(Expression expressionToAdd)
        {
            try
            {
                // query that we want to execute to insert into expression table
                string query = "INSERT INTO ExpressionTable (expressionID, leftOperandType, leftOperandValue, rightOperandType, rightOperandValue, operator, rightOperandName, leftOperandName)";
                //query += " VALUES ('" + expressionToAdd.ExpressionID + "', '" + expressionToAdd.LeftOperandType + "', '" + expressionToAdd.LeftOperandValue + "', '" + expressionToAdd.RightOperandType + "', '" + expressionToAdd.RightOperandValue + "', '" + expressionToAdd.Operator + "')";
                query += " VALUES (@eID, @lot, @lov, @rot, @rov, @op, @lon, @ron)";

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
                using (var command = new SqlCommand(query, conn))
                {
                    SqlParameter[] sqlParams = new SqlParameter[8];

                    // store all the parameters in sqlParams
                    sqlParams[0] = new SqlParameter("@eID", expressionToAdd.ExpressionID);
                    sqlParams[1] = new SqlParameter("@lot", expressionToAdd.LeftOperandType);
                    sqlParams[2] = new SqlParameter("@lov", expressionToAdd.LeftOperandValue);
                    sqlParams[3] = new SqlParameter("@rot", expressionToAdd.RightOperandType);
                    sqlParams[4] = new SqlParameter("@rov", expressionToAdd.RightOperandValue);
                    sqlParams[5] = new SqlParameter("@op", expressionToAdd.Operator);
                    sqlParams[6] = new SqlParameter("@lon", expressionToAdd.LeftOperandName);
                    sqlParams[7] = new SqlParameter("@ron", expressionToAdd.RightOperandName);

                    // add all 6 sqlParams to the command
                    for (int i = 0; i < sqlParams.Length; i++)
                    {
                        command.Parameters.Add(sqlParams[i]);
                    }

                    conn.Open();
                    command.ExecuteNonQuery(); // use ExecuteNonQuery because we don't expect to return anything

                    expressionsList.Add(expressionToAdd);
                    //idsOfExpressions.Add(expressionToAdd.ExpressionID); // TODO: possibly remove this as it would be redundant to chcek if a expression exists, or maybe check existing expression another way?
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region EditExpression
        public void editExpression(EditExpressionDTO expressionToEdit, string expressionID)
        {
            try
            {
                // query that we want to execute to insert into rule table
                string query = "UPDATE dbo.ExpressionTable SET ";
                query += "leftOperandType=@lot, leftOperandValue=@lov, rightOperandType=@rot, rightOperandValue=@rov, operator=@op, leftOperandName=@lon, rightOperandName=@ron WHERE expressionID=@eid";

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
                using (var command = new SqlCommand(query, conn))
                {
                    SqlParameter[] sqlParams = new SqlParameter[8];

                    // store all the parameters in sqlParams
                    sqlParams[0] = new SqlParameter("@lot", expressionToEdit.LeftOperandType);
                    sqlParams[1] = new SqlParameter("@lov", expressionToEdit.LeftOperandValue);
                    sqlParams[2] = new SqlParameter("@rot", expressionToEdit.RightOperandType);
                    sqlParams[3] = new SqlParameter("@rov", expressionToEdit.RightOperandValue);
                    sqlParams[4] = new SqlParameter("@op", expressionToEdit.Operator);
                    sqlParams[5] = new SqlParameter("@lon", expressionToEdit.LeftOperandName);
                    sqlParams[6] = new SqlParameter("@ron", expressionToEdit.RightOperandName);
                    sqlParams[7] = new SqlParameter("@eid", expressionID);

                    // add all 6 sqlParams to the command
                    for (int i = 0; i < sqlParams.Length; i++)
                    {
                        command.Parameters.Add(sqlParams[i]);
                    }

                    conn.Open();
                    command.ExecuteNonQuery(); // use ExecuteNonQuery because we don't expect to return anything
                }
            }
            catch (Exception ex)
            {
                // Handle any exception.
                Debug.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region DeleteExpression
        public void deleteExpression(string expressionToDelete)
        {
            try
            {
                // query that we want to execute to insert into expression table
                string query = "DELETE FROM dbo.ExpressionTable WHERE expressionID=@eid";

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
                using (var command = new SqlCommand(query, conn))
                {
                    SqlParameter[] sqlParams = new SqlParameter[1];

                    // store all the parameters in sqlParams
                    sqlParams[0] = new SqlParameter("@eid", expressionToDelete);

                    // add all 6 sqlParams to the command
                    for (int i = 0; i < sqlParams.Length; i++)
                    {
                        command.Parameters.Add(sqlParams[i]);
                    }

                    conn.Open();
                    command.ExecuteNonQuery(); // use ExecuteNonQuery because we don't expect to return anything
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        #endregion

        /*
         * The methods below are meant to retreive data from the sql database
         */
        #region Rule Getters
        // method to retrieve all rules from the "rulesList" arraylist
        public List<Rule> getAllRules()
        {
            return rulesList;
        }

        // given a rule name, determine if it was found in the rules table
        public bool ruleExists(string ruleName)
        {
            return ruleIDGivenName.ContainsKey(ruleName); // returns true if the rule exists, if not then false
        }

        public bool ruleExistsWithID(string ruleID)
        {
            return ruleIDGivenName.ContainsValue(ruleID);
        }

        // given a rule namee, return the Rule with all information
        public Rule getRule(string ruleName)
        {
            return ruleGivenName[ruleName];
        }

        public string getRuleWithID(string ruleID)
        {
            return ruleIDGivenName[ruleID];
        }
        #endregion

        #region Expression Getters
        // method to get expression given expression ID
        public Expression getExpression(string expressionID)
        {
            return expressionGivenID[expressionID];
        }
        // method to check if an expression exists
        public bool expressionExists(CreateExpressionDTO expression)
        {
            // query that we want to execute to insert into rule table
            string query = "SELECT * FROM ExpressionTable WHERE (leftOperandType=@lot AND leftOperandValue=@lov AND rightOperandType=@rot AND rightOperandValue=@rov AND operator=@op AND leftOperandName=@lon AND rightOperandName=@ron) ";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
            using (var command = new SqlCommand(query, conn))
            {
                SqlParameter[] sqlParams = new SqlParameter[7];

                // store all the parameters in sqlParams
                sqlParams[0] = new SqlParameter("@lot", expression.LeftOperandType);
                sqlParams[1] = new SqlParameter("@lov", expression.LeftOperandValue);
                sqlParams[2] = new SqlParameter("@rot", expression.RightOperandType);
                sqlParams[3] = new SqlParameter("@rov", expression.RightOperandValue);
                sqlParams[4] = new SqlParameter("@op", expression.Operator);
                sqlParams[5] = new SqlParameter("@lon", expression.LeftOperandName);
                sqlParams[6] = new SqlParameter("@ron", expression.RightOperandName);

                // add all 6 sqlParams to the command
                for (int i = 0; i < sqlParams.Length; i++)
                {
                    command.Parameters.Add(sqlParams[i]);
                }

                conn.Open();

                // get all the rows from the command call
                var populatedTable = command.ExecuteReader();

                // if the table has a row, that means there is a duplicate value in the database, return true
                if (populatedTable.HasRows)
                {
                    return true;
                }

                // if the above condition isn't satisfied then there are other expressions with same values
                return false;
            }
        }

        // method to retreive all expressions from the "expressionsList" arraylist
        public List<Expression> getAllExpressions()
        {
            return expressionsList;
        }
        // given a expression id, determine if it was fround from expressions table
        public bool expressionExists(string expressionID)
        {
            return expressionGivenID.ContainsKey(expressionID); // check if dictonary contains the key with the expression ID
        }

        // given everything but an expressionID, return the expression ID of expression object
        public string getExpressionID(CreateExpressionDTO expression)
        {
            // query that we want to execute to insert into rule table
            string query = "SELECT * FROM ExpressionTable WHERE ";
            query += "leftOperandType=@lot AND leftOperandValue=@lov AND rightOperandType=@rot AND rightOperandValue=@rov AND operator=@op AND leftOperandName=@lon AND rightOperandName=@ron";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString()))
            using (var command = new SqlCommand(query, conn))
            {
                SqlParameter[] sqlParams = new SqlParameter[7];

                // store all the parameters in sqlParams
                sqlParams[0] = new SqlParameter("@lot", expression.LeftOperandType);
                sqlParams[1] = new SqlParameter("@lov", expression.LeftOperandValue);
                sqlParams[2] = new SqlParameter("@rot", expression.RightOperandType);
                sqlParams[3] = new SqlParameter("@rov", expression.RightOperandValue);
                sqlParams[4] = new SqlParameter("@op", expression.Operator);
                sqlParams[5] = new SqlParameter("@lon", expression.LeftOperandName);
                sqlParams[6] = new SqlParameter("@ron", expression.RightOperandName);

                // add all 6 sqlParams to the command
                for (int i = 0; i < sqlParams.Length; i++)
                {
                    command.Parameters.Add(sqlParams[i]);
                }

                conn.Open();

                // get all the rows from the command call
                var populatedTable = command.ExecuteReader();

                string expressionID = "";

                while (populatedTable.Read())
                {
                    expressionID = populatedTable["expressionID"].ToString();
                }
                return expressionID;
            }
        }
        #endregion
    }
}
    
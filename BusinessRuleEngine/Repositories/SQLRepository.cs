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

        // create an arraylist to save the expressions
        List<Expression> expressionsList = new List<Expression>();

        public SQLRepository() {
            // create an instance of Response to return any possible errors
            Response response = new Response();

            // establish connection to sql database
            //SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QTC-Server").ToString());
        }
       
    }
}

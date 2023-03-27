using Microsoft.AspNetCore.Mvc;
using BusinessRuleEngine.DTO;
using BusinessRuleEngine.Entities;
using BusinessRuleEngine.Repositories;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BusinessRuleEngine.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AddExpressionController : ControllerBase
    {
        #region variables
        // used to import the sql repository to read all the rules from
        private readonly SQLRepository sqlRepo;

        // used to read the info from appsettings.json
        private readonly IConfiguration _configuration;
        #endregion

        #region constructor
        public AddExpressionController(IConfiguration configuration)
        {
            this._configuration = configuration; // retrieves configuration passed in (appsettings.json)
            this.sqlRepo = new SQLRepository(_configuration); // pass in data retrieved from server to instance of SQLRepository
        }
        #endregion

        #region GetAllExpressions
        // returns a json formatted result of the rules saved on an sql database specified under "appsettings.json"
        // GET: /GetAllExpressions
        [HttpGet]
        [Route("GetAllExpressions")]
        public IEnumerable<Expression> Get()
        {
            // get the rules from the sql repository and save as a variable
            var expressionsList = sqlRepo.getAllExpressions();

            // Debug.WriteLine("size of the list: " +info.Count);
            return expressionsList;
        }
        #endregion

        #region AddExpression
        // PUT /AddExpression
        [HttpPut]
        [Route("AddExpression")]
        public JsonObject createExpression(CreateExpressionDTO expressionDTO)
        {
            JsonObject message = new JsonObject() { };

            if (sqlRepo.expressionExists(expressionDTO))
            {
                message.Add("Status", "Cannot add Expression because it already exists with id: "+sqlRepo.getExpressionID(expressionDTO));
            }
            else
            {
                // get all the elements needed to create an Expression
                Expression newExpression = new Expression
                {
                    ExpressionID = Guid.NewGuid().ToString(),
                    LeftOperandType = expressionDTO.LeftOperandType,
                    LeftOperandValue = expressionDTO.LeftOperandValue,
                    RightOperandType = expressionDTO.RightOperandType,
                    RightOperandValue = expressionDTO.RightOperandValue,
                    Operator = expressionDTO.Operator,
                    LeftOperandName = expressionDTO.LeftOperandName,
                    RightOperandName = expressionDTO.RightOperandName
                };

                sqlRepo.addExpression(newExpression);

                message.Add("Status","Successfully added expression!");
            }
            
            return message;
        }
        #endregion

        #region EditExpression
        [HttpPut]
        [Route("EditExpression")]
        public JsonObject EditExpression(EditExpressionDTO expressionDTO, string expressionID)
        {
            JsonObject message = new JsonObject() { };

            // get the name of the rule that is going to be added
            string expressionIDofExpressionToEdit = expressionID;

            // check if the the expression id exists in the database, if not then let user know
            if (!sqlRepo.expressionExists(expressionID))
            {
                // add message letting the user know they cannot edit the expression
                message.Add("Status","The expression id " + expressionIDofExpressionToEdit + " does not exist. Please type in a valid expression ID");
            }
            else
            {
                // edit the expression
                sqlRepo.editExpression(expressionDTO, expressionID);

                // let the user know they successfully updated the rule
                message.Add("Status", "Successfully updated expression with ID '" + expressionID + "'.");
            }

            return message;
        }
        #endregion

        #region DeleteExpression
        [HttpDelete]
        [Route("DeleteExpression")]
        public JsonObject deleteExpression(string expressionID)
        {
            JsonObject message = new JsonObject() { };
            // get all the elements needed to create an Expression

            if (!sqlRepo.expressionExists(expressionID))
            {
                // add message letting the user know they cannot remove the rule 
                message.Add("Status", "There is no expression with id '" + expressionID + "' that exists, please type in an existing expression id");
            }
            else
            {
                sqlRepo.deleteExpression(expressionID);
                message.Add("Status", "Successfully deleted expression with id '" + expressionID + "'");
            }

            return message;
        }
        #endregion
    }
}

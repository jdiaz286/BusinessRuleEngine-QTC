using Microsoft.AspNetCore.Mvc;
using BusinessRuleEngine.DTO;
using BusinessRuleEngine.Entities;
using BusinessRuleEngine.Repositories;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

// TODO: Add the same functionallity to the expression controller as rule controller

// TODO: Create DTO's for Expression
namespace BusinessRuleEngine.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AddExpressionController : ControllerBase
    {
        // used to import the sql repository to read all the rules from
        private readonly SQLRepository sqlRepo;

        // used to read the info from appsettings.json
        private readonly IConfiguration _configuration;

        public AddExpressionController(IConfiguration configuration)
        {
            this._configuration = configuration; // retrieves configuration passed in (appsettings.json)
            this.sqlRepo = new SQLRepository(_configuration, "ExpressionTable"); // pass in data retrieved from server to instance of SQLRepository
        }

        // returns a json formatted result of the rules saved on an sql database specified under "appsettings.json"

        // GET: /GetAllExpressions
        [HttpGet]
        [Route("GetAllExpressions")]
        public IEnumerable<Expression> Get()
        {
            // create an instance of Response to return any possible errors
            Response response = new Response();

            // get the rules from the sql repository and save as a variable
            var expressionsList = sqlRepo.getAllExpressions();

            // Debug.WriteLine("size of the list: " +info.Count);
            return expressionsList;
        }


        // PUT /AddExpression
        [HttpPut]
        [Route("AddExpression")]
        public void createExpression(CreateExpressionDTO expressionDTO)
        {
            // TODO: Verify that the expression doesn't already exist

            // get all the elements needed to create an Expression
            Expression newExpression = new Expression
            {
                ExpressionID = Guid.NewGuid().ToString(),
                LeftOperandType = expressionDTO.LeftOperandType,
                LeftOperandValue = expressionDTO.LeftOperandValue,
                RightOperandType = expressionDTO.RightOperandType,
                RightOperandValue = expressionDTO.RightOperandValue,
                Operator = expressionDTO.Operator
            };
            

            sqlRepo.addExpression(newExpression);

            Debug.WriteLine("The values in body: " + newExpression);
            //return CreatedAtAction()
        }

    }
}

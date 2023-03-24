﻿using Microsoft.AspNetCore.Mvc;
using BusinessRuleEngine.Entities; // import the Rule class from the entities folder
using BusinessRuleEngine.DTO;
using Rule = BusinessRuleEngine.Entities.Rule;
using BusinessRuleEngine.Repositories; // import the repositories folder from the project
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace BusinessRuleEngine.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AddRuleController : ControllerBase
    {
        #region variables
        // used to import the sql repository to read all the rules from
        private readonly SQLRepository sqlRepo;

        // used to read the info from appsettings.json
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public AddRuleController(IConfiguration configuration)
        {
            this._configuration = configuration; // retrieves configuration passed in (appsettings.json)
            this.sqlRepo = new SQLRepository(_configuration); // pass in data retrieved from server to instance of SQLRepository
        }
        #endregion

        #region GetAllRules
        // returns a json formatted result of the rules saved on an sql database specified under "appsettings.json"
        // GET: /<GetAllRules>
        [HttpGet]
        [Route("GetAllRules")] // change this to change name on swaggerUI
        public IEnumerable<Rule> Get()
        {
            // instantiate the JsonObject that will be returned to the user
            JsonObject message = new JsonObject();

            // get the rules from the sql repository and save as a variable
            var rulesList = sqlRepo.getAllRules();

            // if the arraylist has at least 1 item, then return it as a json object letting the user know
            if (rulesList.Count == 0)
            {
                message.Add("Message", "No Rules found in database");
                //return message;
            }

            // Debug.WriteLine("size of the list: " +info.Count);
            return rulesList;
            
        }
        #endregion


        #region AddRule
        // PUT /AddRule
        // add a rule based on what the user has sent (ruleID and expressionID are generated randomly)
        [HttpPut]
        [Route("AddRule")]
        public void CreateRule(CreateRuleDTO ruleDTO)
        {
            // get the name of the rule that is going to be added
            string nameOfNewRule = ruleDTO.RuleName;

            // check if the rule already exists in the database
            if (sqlRepo.ruleExists(nameOfNewRule))
            {
                Debug.WriteLine("The rule named " + nameOfNewRule + " already exists");
            }

            // TODO: if the rule does not exist, make sure that the expressionID is valid
            

            // get all the elements needed to create a rule
            Rule newRule = new Rule
            {
                RuleID =  Guid.NewGuid().ToString(),
                RuleName = ruleDTO.RuleName,
                ExpressionID = ruleDTO.ExpressionID,
                PositiveAction = ruleDTO.PositiveAction,
                PositiveValue = ruleDTO.PositiveValue,
                NegativeAction = ruleDTO.NegativeAction,
                NegativeValue = ruleDTO.NegativeValue
            };

            sqlRepo.addRule(newRule);

            Debug.WriteLine("The values in body: "+newRule);
            //return CreatedAtAction()
        }
        #endregion

        #region EditRule
        [HttpPut]
        [Route("EditRule")]
        public void EditRule(EditRuleDTO ruleDTO)
        {
            // get the name of the rule that is going to be added
            string ruleIDofRuleToEdit = ruleDTO.RuleID;

            //TODO: Needs to check if rule doesn't exist and handle accordingly
            if (sqlRepo.ruleExists(ruleIDofRuleToEdit))
            {
                Debug.WriteLine("The rule named " + ruleIDofRuleToEdit + " already exists");
            }

            // TODO: if the rule does not exist, make sure that the expressionID is valid
            sqlRepo.editRule(ruleDTO);

            Debug.WriteLine("The values in body: " + ruleIDofRuleToEdit);
            //return CreatedAtAction()
        }
        #endregion

        #region DeleteRule
        // TODO: Add functionallity to remove rule from database
        [HttpDelete]
        [Route("DeleteRule")]
        //TODO: Make it input json file either be only rule name or rule id
        public void DeleteRule(DeleteRuleDTO ruleDTO)
        {
            // get the name of the rule that is going to be added
            string nameOfNewRule = ruleDTO.RuleID;

            //TODO: Needs to check if rule doesn't exist and handle accordingly
            if (sqlRepo.ruleExists(nameOfNewRule))
            {
                Debug.WriteLine("The rule named " + nameOfNewRule + " already exists");
            }

            // TODO: if the rule does not exist, make sure that the expressionID is valid
            sqlRepo.deleteRule(nameOfNewRule);

            Debug.WriteLine("The values in body: " + nameOfNewRule);
            //return CreatedAtAction()
        }
        #endregion

    }
}

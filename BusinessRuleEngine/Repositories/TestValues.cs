namespace BusinessRuleEngine.Repositories;
using BusinessRuleEngine.Entities; // import the Rule class from the entities folder
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Collections.Generic; // import module to use lists in this file


public class TestValues : RuleInterface
{
    private readonly List<Rule> listOfRules = new()
    {
        new Rule {RuleID = "rand ID", RuleName = "test rule 1", ExpressionID =  "rand ID", PositiveAction = "positiveA", PositiveValue = "positiveV", NegativeAction = "negativeA", NegativeValue = "negativeA" },
        new Rule {RuleID =  "rand ID", RuleName = "test rule 2", ExpressionID =  "rand ID", PositiveAction = "positiveA", PositiveValue = "positiveV", NegativeAction = "negativeA", NegativeValue = "negativeA" },
        new Rule {RuleID =  "rand ID", RuleName = "test rule 3", ExpressionID =  "rand ID", PositiveAction = "positiveA", PositiveValue = "positiveV", NegativeAction = "negativeA", NegativeValue = "negativeA" },
    };

    public IEnumerable<Rule> GetRules()
    {
        return listOfRules;
    }

    public Rule GetRule(Guid id)
    {
        return listOfRules.Where(rule => rule.RuleID.Equals(id)).SingleOrDefault();
    }
}

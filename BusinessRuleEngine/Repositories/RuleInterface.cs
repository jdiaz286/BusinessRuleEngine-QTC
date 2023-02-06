using BusinessRuleEngine.Entities;
// TODO: DELETE THIS FILE!!!
namespace BusinessRuleEngine.Repositories
{
    public interface RuleInterface
    {
        Rule GetRule(Guid id);

        IEnumerable<Rule> GetRules();
    }
}

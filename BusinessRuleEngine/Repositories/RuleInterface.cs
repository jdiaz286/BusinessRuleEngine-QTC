using BusinessRuleEngine.Entities;

namespace BusinessRuleEngine.Repositories
{
    public interface RuleInterface
    {
        Rule GetRule(Guid id);

        IEnumerable<Rule> GetRules();
    }
}

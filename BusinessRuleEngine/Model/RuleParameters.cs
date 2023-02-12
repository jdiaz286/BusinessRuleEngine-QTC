using System.Text.Json.Nodes;

/*
 * This file is intended to hold all the values needed to pass to ExecuteRule() function
 */
namespace BusinessRuleEngine.Model
{
    public class RuleParameters
    {
        public string ruleName { get; init; }

        public JsonArray userParameters { get; init; }

        public JsonObject resultObject { get; init; }
    }
}

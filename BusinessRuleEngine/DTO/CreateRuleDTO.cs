namespace BusinessRuleEngine.DTO

/*
 * This class is meant to be used on the AddRule controller to retreive a new rule to add 
 * TODO: Still needs to change the actions and values datatype
 */

{
    public record CreateRuleDTO
    {
        public string RuleName { get; init; }

        // TODO: Possibly change from init to set because ExpressionID should already exist in rule table
        public string ExpressionID { get; init; }

        public string PositiveAction { get; init; }

        public string PositiveValue { get; init; }

        public string NegativeAction { get; init; }

        public string NegativeValue { get; init; }
    }
}

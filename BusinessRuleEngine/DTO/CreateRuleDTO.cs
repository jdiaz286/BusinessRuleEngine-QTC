namespace BusinessRuleEngine.DTO

/*
 * This class is meant to be used on the AddRule controller to retreive a new rule to add 
 * NOTE: Still needs to change the actions and values datatype
 */

{
    public record CreateRuleDTO
    {
        public string RuleName { get; init; }

        public string ExpressionID { get; init; }

        public string PositiveAction { get; init; }

        public string PositiveValue { get; init; }

        public string NegativeAction { get; init; }

        public string NegativeValue { get; init; }
    }
}

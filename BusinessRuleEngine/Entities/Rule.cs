namespace BusinessRuleEngine.Entities
{
    public record Rule
    {
        public String RuleID { get; init; }

        public string RuleName { get; init; }

        public string ExpressionID { get; init; }

        public string PositiveAction { get; init; }

        public string PositiveValue { get; init; }

        public string NegativeAction { get; init; }

        public string NegativeValue { get; init; }
    }
}

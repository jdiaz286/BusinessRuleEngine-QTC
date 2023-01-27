namespace BusinessRuleEngine.DTO
{
    /*
     * DTO = Data Transfer Object
     */
    public record RuleDTO
    {
        public string RuleID { get; init; }

        public string RuleName { get; init; }

        public string ExpressionID { get; init; }

        public string PositiveAction { get; init; }

        public string PositiveValue { get; init; }

        public string NegativeAction { get; init; }

        public string NegativeValue { get; init; }
    }
}





namespace BusinessRuleEngine.DTO

/*
 * This class is meant to be used on the AddRule controller to retreive a new rule to add 
 * TODO: Still needs to change the actions and values datatype
 */

{
    public record EditExpressionDTO
    {
        public string ExpressionID { get; init; }

        // TODO: Possibly change from init to set because ExpressionID should already exist in rule table

        public string LeftOperandType { get; init; }

        public string LeftOperandValue { get; init; }

        public string RightOperandType { get; init; }

        public string RightOperandValue { get; init; }

        public string Operator { get; init; }
    }
}

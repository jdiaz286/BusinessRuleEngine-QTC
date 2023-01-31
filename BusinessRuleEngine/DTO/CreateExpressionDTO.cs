namespace BusinessRuleEngine.DTO
{
    /*
     * This class is meant to be used on the AddExpression controller to retreive a new expression to add 
     * TODO: Still needs to change the actions and values datatype
     */

    public class CreateExpressionDTO
    {
        public string LeftOperandType { get; init; }

        public string LeftOperandValue { get; init; }

        public string RightOperandType { get; init; }

        public string RightOperandValue { get; init; }

        public string Operator { get; init; }
    }
}

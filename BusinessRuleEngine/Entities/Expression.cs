namespace BusinessRuleEngine.Entities
{
    public class Expression
    {
        public string ExpressionID { get; init; }

        public string LeftOperandType { get; init; }

        public string LeftOperandValue { get; init; }
    
        public string RightOperandType { get; init; }

        public string RightOperandValue { get; init; }

        public string Operator { get; init; }
    }
}

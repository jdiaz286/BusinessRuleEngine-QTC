namespace BusinessRuleEngine.DTO
{
    /*
     * DTO = Data Transfer Object
     */
    public record ExpressionDTO
    {
        public string ExpressionID { get; init; }

        public string LeftOperandType { get; init; }
        
        public string LeftOperandValue { get; init; }

        public string RightOperandType { get; init;}

        public string RightOperandValue { get; init;}

        public string Operator { get; init; }

        public string LeftOperandName { get; init; }
        public string RightOperandName { get; init; }
    }
}

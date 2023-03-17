namespace BusinessRuleEngine.DTO

/*
 * This class is meant to be used on the AddRule controller to retreive a rule to delete 
 * TODO: Still needs to change the actions and values datatype
 */

{
    public record DeleteRuleDTO
    {
        public string RuleID { get; init; }

    }
}

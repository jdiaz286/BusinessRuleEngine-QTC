/*
 * This class is meant to indicate wheather an error was found
 */

namespace BusinessRuleEngine.Entities
{
    public class Response
    {
        public int StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}

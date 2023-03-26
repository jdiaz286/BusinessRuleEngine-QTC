using BusinessRuleEngine.Entities;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace BusinessRuleEngine.Model
{
    /*
     This class is designed to take in the values of an Expression and evaluate the expression, returns a bool value to represent evaluation
     */

    public class ExpressionEvaluator
    {
        // save the Expression that is going to be evaluated
        Expression express;

        // save the JsonNode of the current value that we want to evaluate
        JsonNode jsonVals;
        JsonObject result;

        // constructor to pass in and instantiate the expression in the class
        public ExpressionEvaluator(Expression expression, JsonNode jsonOject, JsonObject result)
        {
            this.express = expression;
            this.jsonVals = jsonOject;
            this.result = result;
        }

        // method to evaluate the expression and return either positive (true) or negative (false)
        public bool evaluateExpression()
        {
            // create a boolean variable to track the amount of time 
            bool expressionEvaluation = false;

            // if both operand types are equal to each other and we don't have any nestedExpressions
            if (express.LeftOperandType.ToLower().Equals(express.RightOperandType.ToLower()))
            {
                // split both left and right operandTypes into individual strings, remove the "-"
                string[] leftOperandTypeSplit = express.LeftOperandType.Split("-");
                string[] rightOperandTypeSplit = express.RightOperandType.Split("-");

                // TODO: take into account more objects besides strings and integers
                switch (leftOperandTypeSplit[0].ToLower())
                {
                    case "string":
                        expressionEvaluation = evaluateString(leftOperandTypeSplit, rightOperandTypeSplit);
                        break;
                    case "integer":
                        expressionEvaluation = evaluateInteger(leftOperandTypeSplit, rightOperandTypeSplit);
                        break;
                    // TODO: finish adding the boolean expression (reference lines above for an example)
                    case "boolean":
                        Debug.WriteLine("Evaluating integer expression.");
                        break;
                    default:
                        Console.WriteLine("Object type not yet programmed.");
                        break;
                }
            }

            // return the evaluation of the expression
            return expressionEvaluation;
        }

        /* 
         * Methods below will be used to evaluate expressions
         */

        public bool evaluateString(string[] leftOperandTypeSplit, string[] rightOperandTypeSplit)
        {
            bool evaluation = false;
            Debug.WriteLine("\tLeft Operand type: " + leftOperandTypeSplit[0]);
            Debug.WriteLine("\tLeft Operand json target val: " + leftOperandTypeSplit[1]);
            Debug.WriteLine("\tRight Operand type: " + rightOperandTypeSplit[0]);
            Debug.WriteLine("\tleft Operand json target val: " + rightOperandTypeSplit[1]);

            // TODO: Take into account more scenarios besides just "="
            // if there is an equals sign in front of the operator, then determine if the strings equal each other
            if (express.Operator.Equals("=") || express.Operator.ToLower().Equals("equals"))
            {
                // TODO: Create a way to check if the operand type
                // if the string version of userParameters contains the target value, return true to execute positive rule
                
                if (jsonVals[leftOperandTypeSplit[1]].ToString().ToLower().Equals(express.LeftOperandValue.ToLower()))
                {
                    Debug.WriteLine("Returns True");
                    evaluation = true;
                    Debug.WriteLine("value of evaluation = "+evaluation);
                }
            }

            return evaluation;
        }

        public bool evaluateInteger(string[] leftOperandTypeSplit, string[] rightOperandTypeSplit)
        {
            bool evaluation = false;
            //Debug.WriteLine(jsonVals[leftOperandTypeSplit[1]].ToString());
            float floatValue;
            int intValue;/*
            if (jsonVals[leftOperandTypeSplit[1]].ToString().Contains(".") && float.TryParse(jsonVals[leftOperandTypeSplit[1]].ToString(), out floatValue))
            {
                jsonVals[leftOperandTypeSplit[1]] = (int)float.Parse(jsonVals[leftOperandTypeSplit[1]].ToString());
 
            }
            if (Int32.TryParse(jsonVals[leftOperandTypeSplit[1]].ToString(), out intValue))
            {
                jsonVals[leftOperandTypeSplit[1]] = Int32.Parse(jsonVals[leftOperandTypeSplit[1]].ToString());
            }
            else
            {
                //TODO: Create some sort of error object or system to indicate thru json obects that the incorrect input type has 
                //been inputted.
                string errorMessage = "Error: Value 'age' is supposed to be an integer, actual current type is string";
                result.Add("Error Message", errorMessage);
                return evaluation;
            }

            // if there is a "<" as the first character then return the evaluate as <
            if (express.Operator.Equals("<"))
            {
                // if the condition below is true, return true. (parse everything as int)
                if (intValue < Int32.Parse(express.LeftOperandValue.ToString()))
                {
                    Debug.WriteLine("Returns True");
                    evaluation = true;
                    Debug.WriteLine("value of evaluation = " + evaluation);
                }
            }
            // if there is a "<" as the first character then return the evaluate as >=
            if (express.Operator.Equals(">="))
            {
                if (intValue >= Int32.Parse(express.LeftOperandValue.ToString()))
                {
                    Debug.WriteLine("Returns True");
                    evaluation = true;
                    Debug.WriteLine("value of evaluation = " + evaluation);
                }// if the condition below is true, return true. (parse everything as int)
            }
*/
            return evaluation;
        }
    }

}

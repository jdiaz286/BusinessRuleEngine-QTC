﻿using BusinessRuleEngine.Entities;
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
        #region variables
        // save the Expression that is going to be evaluated
        Expression express;

        int currentItemIndex;

        // save the JsonNode of the current value that we want to evaluate
        JsonNode jsonVals;
        JsonObject result;
        #endregion

        #region constructor
        // constructor to pass in and instantiate the expression in the class
        public ExpressionEvaluator(Expression expression, JsonNode jsonOject, JsonObject result, int currentItemIndex)
        {
            this.express = expression;
            this.jsonVals = jsonOject;
            this.result = result;
            this.currentItemIndex = currentItemIndex;
        }
        #endregion

        // method to evaluate the expression and return either positive (true) or negative (false)
        public int evaluateExpression()
        {
            // create a boolean variable to track the amount of time 
            int expressionEvaluation = -1;

            // check the left expression if nested

            // check the right expression if nested

            if (express.LeftOperandType.ToLower().Equals(express.RightOperandType.ToLower()))
            {
                // TODO: take into account more objects besides strings and integers
                switch (express.LeftOperandType.ToLower())
                {
                    case "string":
                        expressionEvaluation = evaluateString();
                        break;
                    case "integer":
                        expressionEvaluation = evaluateInteger();
                        break;
                    case "boolean":
                        Debug.WriteLine("Evaluating integer expression.");
                        break;
                    default:
                        result.Add("", "Oject type '" + express.LeftOperandType + "' has not been implemented yet.");
                        break;
                }
            }
            

            // return the evaluation of the expression
            return expressionEvaluation;
        }

        /* 
         * Methods below will be used to evaluate expressions
         */
        #region string evaluation
        public int evaluateString()
        {
            int evaluation = -1;

            // if there is an equals sign in front of the operator, then determine if the strings equal each other
            if (express.Operator.Equals("=") || express.Operator.ToLower().Equals("equals"))
            {
                // check the left/negative side, if evaluates to true, return 0
                if (jsonVals[express.LeftOperandName].ToString().ToLower().Equals(express.LeftOperandValue.ToLower()))
                {
                    evaluation = 0;
                }
                // check the right/positive side, if evaluates to true, return 1
                else if (jsonVals[express.RightOperandName].ToString().ToLower().Equals(express.RightOperandValue.ToLower()))
                {
                    evaluation = 1;
                }
            }
            // if the operator is not recognized, return a message letting the user know
            else
            {
                result.Add("Entry " + currentItemIndex + " output", "Expression operator '" + express.Operator + "' is not a valid operator for type 'string'");
                return -2;
            }

            return evaluation;
        }
        #endregion

        public int evaluateInteger()
        {
            int evaluation = -1;

            float leftFloatValue;
            float rightFloatValue;

            int leftIntValue;
            int rightIntValue;

            #region parse value as integer
            // boolean variables to handle integer saved as float
            bool leftIsFloat = (jsonVals[express.LeftOperandName].ToString().Contains(".") && float.TryParse(jsonVals[express.LeftOperandName].ToString(), out leftFloatValue));
            bool rightIsFloat = (jsonVals[express.RightOperandName].ToString().Contains(".") && float.TryParse(jsonVals[express.RightOperandName].ToString(), out rightFloatValue));

            // if the user typed in a float, parse it as an integer
            if (leftIsFloat && rightIsFloat)
            {
                jsonVals[express.LeftOperandName] = (int)float.Parse(jsonVals[express.LeftOperandName].ToString());
                jsonVals[express.RightOperandName] = (int)float.Parse(jsonVals[express.RightOperandName].ToString());
            }

            // boolean variables to check if values can be saved as ints
            bool leftIsInt = Int32.TryParse(jsonVals[express.LeftOperandName].ToString(), out leftIntValue);
            bool rightIsInt = Int32.TryParse(jsonVals[express.RightOperandName].ToString(), out rightIntValue);

            // check if the user can save integers, if so then save
            if (leftIsInt && rightIsInt)
            {
                jsonVals[express.LeftOperandName] = Int32.Parse(jsonVals[express.LeftOperandName].ToString());
                jsonVals[express.RightOperandName] = Int32.Parse(jsonVals[express.RightOperandName].ToString());
            }
            // if there was an exception, let the user know
            else
            {
                // if one of the values is an integer but not the other then tell the user
                if ( (leftIsInt && !rightIsInt) || (!leftIsInt && rightIsInt) )
                {
                    result.Add("Error Message", "Error: One of the values is an integer but not the other");
                }
                // if one of the values is a float but not the other then tell the user
                else if ((leftIsFloat && !rightIsFloat) || (!leftIsFloat && rightIsFloat))
                {
                    result.Add("Error Message", "Error: One of the values is a float but not the other");
                }
                else
                {
                    result.Add("Error Message", "Error: Value "+express.LeftOperandValue+" is supposed to be an integer, actual current type is string");
                }
            
                evaluation = -2;

                return evaluation;
            }
            #endregion

            // if there is a "<" as the first character then return the evaluate as <
            if (express.Operator.Equals("<"))
            {
                bool leftEvaluation = jsonVals[express.LeftOperandName] < ;

                // check the left/negative side, if evaluates to true, return 0
                if (jsonVals[express.LeftOperandName].ToString().ToLower().Equals(express.LeftOperandValue.ToLower()))
                {
                    evaluation = 0;
                }
                // check the right/positive side, if evaluates to true, return 1
                else if (jsonVals[express.RightOperandName].ToString().ToLower().Equals(express.RightOperandValue.ToLower()))
                {
                    evaluation = 1;
                }

                /*// if the condition below is true, return true. (parse everything as int)
                if (intValue < Int32.Parse(express.LeftOperandValue.ToString()))
                {
                    evaluation = true;
                }*/
            }

            // if there is a "<" as the first character then return the evaluate as >=
            if (express.Operator.Equals(">="))
            {
                if (intValue >= Int32.Parse(express.LeftOperandValue.ToString()))
                {
                    evaluation = true;
                }// if the condition below is true, return true. (parse everything as int)
            }

            return evaluation;
        }
    }

}

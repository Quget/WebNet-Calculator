using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WebNet_Calculator.Calculator
{
    /// <summary>
    /// Does calculator things!
    /// </summary>
    public class Calculator
    {

        private const string inputValidatorRegex = @"([\/?\d?\.?\+?\-?\*?\(?\)?\^?\√?]+)";
        private const string operationValidatorRegex = @"([\-|\+|\*|\/|\^|\√])";
        private const string parenthesesRegex = @"([(][^()]*[)])";

        private const string errorMessage = "Invalid input error";

        /// <summary>
        /// Calculate a calculateable string
        /// </summary>
        /// <param name="calcString">input string</param>
        /// <returns>The result</returns>
        public string Calculate(string calcString)
        {
            MatchCollection matches = Regex.Matches(calcString, inputValidatorRegex);
            if (matches.Count != 1)
                return errorMessage;

            //To avoid freezing up the applcation, it just loops maximum of 99 times
            int maxTries = 99;
            int tries = 0;

            string calcIn = MatchReplaceParentheses(calcString);
            //while max tries has not been reached and there are still +-/* in it.
            while (tries < maxTries && Regex.IsMatch(calcIn, operationValidatorRegex))
            {
                tries++;
                //Start out with * and / then + and - 
                //Most left goes first!
                string result = OrderOfOperation(calcIn);

                //No more new result! Done or invalid input.
                if (result == null)
                    break;

                calcIn = result;
            }
            //if in is still the same then the input was most likely invalid
            //Invalid result when there are multiple + or - or even contains * or /
            if (calcIn == calcString || Regex.Matches(calcIn, operationValidatorRegex).Count > 1)
                calcIn = errorMessage;

            return calcIn;
        }

        /// <summary>
        /// Calculate a calculateable string. Uses DataTable().Compute.
        /// </summary>
        /// <param name="calculateAbleString"></param>
        /// <returns>A result or empty. Unless it is an error.</returns>
        public string CalculateSimple(string calcString)
        {
            if (string.IsNullOrWhiteSpace(calcString))
            {
                return "";
            }
            else
            {
                //https://learn.microsoft.com/en-us/dotnet/api/system.data.datatable.compute?view=net-7.0&redirectedfrom=MSDN#System_Data_DataTable_Compute_System_String_System_String_
                //Todo validate input. Can also try and catch for now.
                try
                {
                    return $"{Convert.ToDouble(new DataTable().Compute(calcString, null))}";
                }
                catch
                {
                    return errorMessage;
                }
            }
        }

        /// <summary>
        /// Order of operation! * / ^ and √ first then  + and -
        /// </summary>
        /// <param name="calcIn">Current calculateable string</param>
        /// <returns>new calc string or final result. Can also be null.</returns>
        private string OrderOfOperation(string calcIn)
        {
            string result = FindFirstOperationByOperationsArray(calcIn, new char[] { '*', '/', '^', '√' });
            if (result == null)
            {
                result = FindFirstOperationByOperationsArray(calcIn, new char[] { '+', '-' });
            }
            return result;
        }

        /// <summary>
        /// calculate the first operation found that's in de operations array
        /// </summary>
        /// <param name="calcIn">Current calculateable string</param>
        /// <param name="operations">Array of operations like *-+/</param>
        /// <returns>new calc string or final result. Can also be null.</returns>
        private string FindFirstOperationByOperationsArray(string calcIn, char[] operations)
        {
            string result;
            //Index as in position inside calcIn
            int closestOperationIndex = -1;
            for (int i = 0; i < operations.Length; i++)
            {
                int currentOperationIndex = calcIn.IndexOf(operations[i]);
                if (currentOperationIndex == 0)
                    currentOperationIndex = calcIn.IndexOf(operations[i], 1);

                if (currentOperationIndex != -1 && (closestOperationIndex == -1 || closestOperationIndex > currentOperationIndex))
                    closestOperationIndex = currentOperationIndex;
            }

            if (closestOperationIndex == -1)
                return null;


            result = MatchReplace(calcIn, calcIn[closestOperationIndex]);
            return result;

        }

        /// <summary>
        /// Get result depending on math operation
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private double Operation(char operation, double value, double value2)
        {
            switch (operation)
            {
                case '+':
                    return Operations.Add(value, value2);

                case '*':
                    return Operations.Multiply(value, value2);

                case '-':
                    return Operations.Subtract(value, value2);

                case '/':
                    return Operations.Divide(value, value2);

                case '^':
                    return Operations.xPower(value, value2);

                case '√':
                    return Operations.xRoot(value, value2);

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Calculate everything inside parentheses.
        /// </summary>
        /// <param name="input">Math string</param>
        /// <returns>Everything inside parentheses calculated.</returns>
        private string MatchReplaceParentheses(string input)
        {
            string output = input;
            MatchCollection matches = Regex.Matches(input, parenthesesRegex);
            for(int i = 0; i < matches.Count; i++)
            {
                string toOperate = matches[i].Value.Substring(1, matches[i].Value.Length - 2);
                string result = OrderOfOperation(toOperate);
                output = output.Replace(matches[i].Value, result);
            }
            return output;
        }

        /// <summary>
        /// Replace a matched math operation with the result using Regular Expression(Regex)
        /// </summary>
        /// <param name="input">Math string</param>
        /// <param name="operation">Math operation</param>
        /// <returns>String where a result replaced the first operation</returns>
        private string MatchReplace(string input, char operation)
        {

            //([-]?\d*\.?\d+)[%*]([-]?\d*\.?\d+)
            //Group 1 & 2; Match a number may have + or - in front or have a floating point. (-1.1, 1 are valid matches)
            string regexIn = @$"([-]?\d*\.?\d+)[%{operation}]([-]?\d*\.?\d+)";

            Match match = Regex.Match(input, regexIn);
            //Needs 3 groups!
            if (match == null || match.Groups.Count < 3)
                return null;

            double result = 0;
            double value1 = -1;
            double value2 = -1;
            Double.TryParse(match.Groups[1].Value, out value1);
            Double.TryParse(match.Groups[2].Value, out value2);
            result = Operation(operation, value1, value2);
            return input.Replace(match.Value, result.ToString());
        }
    }
}

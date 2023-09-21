using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.RegularExpressions;

//https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-7.0&tabs=visual-studio
namespace WebNet_Calculator.Controllers
{
    /// <summary>
    /// Calculator controller class!
    /// </summary>
    public class CalculatorController : Controller
    {
        /// <summary>
        /// Calculator page, accepts a calculateable string as input!
        /// </summary>
        /// <param name="calcString">calculateable string</param>
        /// <returns>Calculator/Index.cshtml</returns>
        public IActionResult Index(string calcString = "")
        {
            if(calcString == "")
                return View();

            string result = Calculate(calcString);//CalculateSimple(calcString);
            ViewData["CalcResult"] = result;
            return View();
        }


        /// <summary>
        /// Calculate a calculateable string
        /// </summary>
        /// <param name="calcString">input string</param>
        /// <returns>The result</returns>
        private string Calculate(string calcString)
        {
            //([\/?\d?\.?\+?\-?\*?]+)
            MatchCollection matches = Regex.Matches(calcString, @"([\/?\d?\.?\+?\-?\*?]+)");
            if (matches.Count != 1)
                return "User has been playing! Wrong input.";

            //To avoid freezing up the applcation, it just loops maximum of 99 times
            int maxTries = 99;
            int tries = 0;

            string calcIn = calcString;
            //while max tries has not been reached and there are still +-/* in it.
            while (tries < maxTries && Regex.IsMatch(calcIn, @"[,\/*\+\-]"))
            {
                tries++;
                //Start out with * and / then + and - 
                //Most left goes first!
                string result = Operation(calcIn, '*', '/');
                if (result == null)
                {
                    result = Operation(calcIn, '+', '-');
                }

                //No more new result! Done or invalid input.
                if (result == null)
                    break;

                calcIn = result;
            }
            //if in is still the same then the input was most likely invalid
            //Invalid result when there are multiple + or - or even contains * or /
            if (calcIn == calcString || Regex.Matches(calcIn, @"([\-|\+|\*|\/])").Count > 1)
                calcIn = "Invalid input";

            return calcIn;
        }

        /// <summary>
        /// Multiplay and devide first then add and substract from left to right. Order of operations
        /// </summary>
        /// <param name="calcIn">Current calculateable string</param>
        /// <param name="operation1"></param>
        /// <param name="operation2"></param>
        /// <returns>new calc string or final result. Can also be null.</returns>
        private string Operation(string calcIn, char operation1, char operation2)
        {
            string result;
            int indexFirst = calcIn.IndexOf(operation1);
            if (indexFirst == 0)
                indexFirst = calcIn.IndexOf(operation1, 1);


            int indexSecond = calcIn.IndexOf(operation2);
            if (indexSecond == 0)
                indexSecond = calcIn.IndexOf(operation2, 1);


            if (indexFirst != -1 && (indexSecond == -1 || indexFirst < indexSecond))
            {
                result = MatchReplace(calcIn, operation1);
            }
            else
            {
                result = MatchReplace(calcIn, operation2);
            }
            return result;
        }

        /// <summary>
        /// Replace a matched math operation with the result using Regular Expression(Regex)
        /// </summary>
        /// <param name="input">Math string</param>
        /// <param name="operation">Math operation</param>
        /// <returns>String where a result replaced the first operation</returns>
        private string MatchReplace(string input, char operation)
        {

            //([+-]?\d*\.?\d+)[%*]([+-]?\d*\.?\d+)
            //Group 1 & 2; Match a number may have + or - in front or have a floating point. (-1.1, 1 are valid matches)
            string regexIn = @"([+-]?\d*\.?\d+)[%" + operation + @"]([+-]?\d*\.?\d+)";

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
                    return Add(value, value2);

                case '*':
                    return Multiplay(value, value2);

                case '-':
                    return Substract(value, value2);

                case '/':
                    return Divide(value, value2);

                default:
                    return 0;
            }
        }

        private double Multiplay(double value, double value2)
        {
            return value * value2;
        }

        private double Add(double value, double value2)
        {
            return value + value2;
        }

        private double Substract(double value, double value2)
        {
            return value - value2;
        }

        private double Divide(double value, double value2)
        {
            return value / value2;
        }

        /// <summary>
        /// Calculate a calculateable string. Uses DataTable().Compute.
        /// </summary>
        /// <param name="calculateAbleString"></param>
        /// <returns>A result or empty. Unless it is an error.</returns>
        private string CalculateSimple(string calcString)
        {
            if(string.IsNullOrWhiteSpace(calcString))
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
                    return $"Invalid input.";
                }
            }
        }

        /// <summary>
        /// Another action that's called! Just for testing
        /// </summary>
        /// <returns></returns>
        public string OtherAction()
        {
            return "Test!";
        }
    }
}

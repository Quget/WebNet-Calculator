using Microsoft.AspNetCore.Mvc;
using System.Data;

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
            string result = Calculate(calcString);
            ViewData["CalcResult"] = result;
            return View();
        }
        
        /// <summary>
        /// Calculate a calculateable string. Uses DataTable().Compute.
        /// </summary>
        /// <param name="calculateAbleString"></param>
        /// <returns>A result or empty. Unless it is an error.</returns>
        private string Calculate(string calcString)
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

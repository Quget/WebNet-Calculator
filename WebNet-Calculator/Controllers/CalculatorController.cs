using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.RegularExpressions;
using WebNet_Calculator.Calculator;
//https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-7.0&tabs=visual-studio
namespace WebNet_Calculator.Controllers
{
    /// <summary>
    /// Calculator controller class!
    /// </summary>
    public class CalculatorController : Controller
    {

        private Calculator.Calculator calculator = new Calculator.Calculator();
        /// <summary>
        /// Calculator page, accepts a calculateable string as input!
        /// </summary>
        /// <param name="calcString">calculateable string</param>
        /// <returns>Calculator/Index.cshtml</returns>
        public IActionResult Index(string calcString = "")
        {
            if(calcString == "")
                return View();

            string result = calculator.Calculate(calcString);//calculator.CalculateSimple(calcString);
            ViewData["CalcResult"] = result;
            return View();
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

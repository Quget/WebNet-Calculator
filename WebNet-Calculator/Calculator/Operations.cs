namespace WebNet_Calculator.Calculator
{
    /// <summary>
    /// Holds different Math things! 
    /// </summary>
    public static class Operations
    {
        public static double Multiply(double value, double value2)
        {
            return value * value2;
        }

        public static double Add(double value, double value2)
        {
            return value + value2;
        }

        public static double Subtract(double value, double value2)
        {
            return value - value2;
        }

        public static double Divide(double value, double value2)
        {
            return value / value2;
        }

        public static double xPower(double value, double power)
        {
            return Math.Pow(value, power);
        }

        public static double xRoot(double value, double root)
        {
            return xPower(value, 1.0 / root);
        }
    }
}

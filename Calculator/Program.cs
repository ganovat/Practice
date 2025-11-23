
namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
    public class Calculations
    {
        public int Add(int a, int b)
        {
            return 0;

        }

       
        public double Divide(double a, double b)
        {
            if (b == 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            return a / b;
        }

        public double Subtract(int v1, int v2)
        {

            return v1 - v2;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CalculatorApp
{
    /// <summary>
    /// کلاس اصلی برنامه ماشین حساب پیشرفته
    /// این کلاس شامل تمام عملیات های ریاضی و منطق برنامه است
    /// </summary>
    class AdvancedCalculator
    {
        // تاریخچه عملیات های انجام شده
        private static List<string> calculationHistory = new List<string>();

        // ثابت های ریاضی
        private const double PI = Math.PI;
        private const double E = Math.E;

        static void Main(string[] args)
        {
            Console.Title = "Advanced Scientific Calculator"; // تنظیم عنوان کنسول

            // تنظیم encoding کنسول برای نمایش کاراکترهای خاص
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            DisplayWelcomeMessage(); // نمایش پیغام خوشآمدگویی

            bool continueCalculating = true;

            while (continueCalculating)
            {
                try
                {
                    DisplayMainMenu(); // نمایش منوی اصلی

                    string choice = GetUserInput("Enter your choice (1-14): ").Trim();

                    // پردازش انتخاب کاربر
                    switch (choice)
                    {
                        case "1":
                            PerformBasicOperation("+");
                            break;
                        case "2":
                            PerformBasicOperation("-");
                            break;
                        case "3":
                            PerformBasicOperation("*");
                            break;
                        case "4":
                            PerformBasicOperation("/");
                            break;
                        case "5":
                            CalculatePower();
                            break;
                        case "6":
                            CalculateSquareRoot();
                            break;
                        case "7":
                            CalculateTrigonometricFunction();
                            break;
                        case "8":
                            CalculateLogarithm();
                            break;
                        case "9":
                            CalculateFactorial();
                            break;
                        case "10":
                            EvaluateExpression();
                            break;
                        case "11":
                            ShowCalculationHistory();
                            break;
                        case "12":
                            ClearHistory();
                            break;
                        case "13":
                            ShowConstants();
                            break;
                        case "14":
                            continueCalculating = false;
                            Console.WriteLine("\nThank you for using Advanced Calculator!");
                            Console.WriteLine("Goodbye! 👋");
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine("Please try again.");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        /// <summary>
        /// نمایش پیغام خوشآمدگویی به کاربر
        /// </summary>
        static void DisplayWelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===============================================");
            Console.WriteLine("      ADVANCED SCIENTIFIC CALCULATOR          ");
            Console.WriteLine("===============================================");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// نمایش منوی اصلی برنامه با دسته‌بندی های مختلف
        /// </summary>
        static void DisplayMainMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("MAIN MENU:");
            Console.ResetColor();

            Console.WriteLine("Basic Operations:");
            Console.WriteLine("  1. Addition (+)");
            Console.WriteLine("  2. Subtraction (-)");
            Console.WriteLine("  3. Multiplication (×)");
            Console.WriteLine("  4. Division (÷)");

            Console.WriteLine("\nScientific Operations:");
            Console.WriteLine("  5. Power (x^y)");
            Console.WriteLine("  6. Square Root (√)");
            Console.WriteLine("  7. Trigonometric Functions (sin, cos, tan)");
            Console.WriteLine("  8. Logarithm (log, ln)");
            Console.WriteLine("  9. Factorial (n!)");

            Console.WriteLine("\nAdvanced Features:");
            Console.WriteLine("  10. Evaluate Mathematical Expression");
            Console.WriteLine("  11. Show Calculation History");
            Console.WriteLine("  12. Clear History");
            Console.WriteLine("  13. Show Mathematical Constants");

            Console.WriteLine("\nExit:");
            Console.WriteLine("  14. Exit Calculator");
            Console.WriteLine();
        }

        /// <summary>
        /// انجام عملیات پایه ریاضی (جمع، تفریق، ضرب، تقسیم)
        /// </summary>
        /// <param name="operation">عملگر ریاضی</param>
        static void PerformBasicOperation(string operation)
        {
            Console.WriteLine($"\n--- {GetOperationName(operation)} Operation ---");

            double num1 = GetNumber("Enter the first number: ");
            double num2 = GetNumber("Enter the second number: ");

            double result = 0;
            string operationSymbol = operation;

            switch (operation)
            {
                case "+":
                    result = num1 + num2;
                    break;
                case "-":
                    result = num1 - num2;
                    break;
                case "*":
                    result = num1 * num2;
                    break;
                case "/":
                    if (num2 == 0)
                    {
                        Console.WriteLine("Error: Division by zero is not allowed.");
                        return;
                    }
                    result = num1 / num2;
                    break;
            }

            string calculation = $"{num1} {operation} {num2} = {result}";
            calculationHistory.Add(calculation);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Result: {calculation}");
            Console.ResetColor();
        }

        /// <summary>
        /// محاسبه توان یک عدد
        /// </summary>
        static void CalculatePower()
        {
            Console.WriteLine("\n--- Power Operation ---");
            double baseNum = GetNumber("Enter the base number: ");
            double exponent = GetNumber("Enter the exponent: ");

            double result = Math.Pow(baseNum, exponent);
            string calculation = $"{baseNum}^{exponent} = {result}";
            calculationHistory.Add(calculation);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Result: {calculation}");
            Console.ResetColor();
        }

        /// <summary>
        /// محاسبه ریشه دوم یک عدد
        /// </summary>
        static void CalculateSquareRoot()
        {
            Console.WriteLine("\n--- Square Root Operation ---");
            double number = GetNumber("Enter the number: ");

            if (number < 0)
            {
                Console.WriteLine("Error: Cannot calculate square root of a negative number.");
                return;
            }

            double result = Math.Sqrt(number);
            string calculation = $"√{number} = {result}";
            calculationHistory.Add(calculation);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Result: {calculation}");
            Console.ResetColor();
        }

        /// <summary>
        /// محاسبه توابع مثلثاتی (سینوس، کسینوس، تانژانت)
        /// </summary>
        static void CalculateTrigonometricFunction()
        {
            Console.WriteLine("\n--- Trigonometric Functions ---");
            Console.WriteLine("1. Sine (sin)");
            Console.WriteLine("2. Cosine (cos)");
            Console.WriteLine("3. Tangent (tan)");

            string choice = GetUserInput("Enter your choice (1-3): ").Trim();

            double angle = GetNumber("Enter the angle in degrees: ");
            // تبدیل درجه به رادیان
            double radians = angle * PI / 180;

            double result = 0;
            string functionName = "";

            switch (choice)
            {
                case "1":
                    result = Math.Sin(radians);
                    functionName = "sin";
                    break;
                case "2":
                    result = Math.Cos(radians);
                    functionName = "cos";
                    break;
                case "3":
                    // بررسی آیا زاویه برابر با 90+180k درجه است (تانژانت تعریف نشده)
                    if (Math.Abs(Math.Cos(radians)) < 1E-10)
                    {
                        Console.WriteLine("Error: Tangent is undefined for this angle.");
                        return;
                    }
                    result = Math.Tan(radians);
                    functionName = "tan";
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }

            string calculation = $"{functionName}({angle}°) = {result}";
            calculationHistory.Add(calculation);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Result: {calculation}");
            Console.ResetColor();
        }

        /// <summary>
        /// محاسبه لگاریتم (طبیعی و معمولی)
        /// </summary>
        static void CalculateLogarithm()
        {
            Console.WriteLine("\n--- Logarithmic Functions ---");
            Console.WriteLine("1. Natural Logarithm (ln)");
            Console.WriteLine("2. Base-10 Logarithm (log)");

            string choice = GetUserInput("Enter your choice (1-2): ").Trim();

            double number = GetNumber("Enter the number: ");

            if (number <= 0)
            {
                Console.WriteLine("Error: Logarithm is only defined for positive numbers.");
                return;
            }

            double result = 0;
            string functionName = "";

            switch (choice)
            {
                case "1":
                    result = Math.Log(number);
                    functionName = "ln";
                    break;
                case "2":
                    result = Math.Log10(number);
                    functionName = "log";
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }

            string calculation = $"{functionName}({number}) = {result}";
            calculationHistory.Add(calculation);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Result: {calculation}");
            Console.ResetColor();
        }

        /// <summary>
        /// محاسبه فاکتوریل یک عدد
        /// </summary>
        static void CalculateFactorial()
        {
            Console.WriteLine("\n--- Factorial Calculation ---");
            int number = (int)GetNumber("Enter a non-negative integer: ");

            if (number < 0)
            {
                Console.WriteLine("Error: Factorial is not defined for negative numbers.");
                return;
            }

            if (number > 20) // جلوگیری از overflow
            {
                Console.WriteLine("Error: Number is too large for factorial calculation.");
                return;
            }

            long result = CalculateFactorialRecursive(number);
            string calculation = $"{number}! = {result}";
            calculationHistory.Add(calculation);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Result: {calculation}");
            Console.ResetColor();
        }

        /// <summary>
        /// تابع بازگشتی برای محاسبه فاکتوریل
        /// </summary>
        /// <param name="n">عدد ورودی</param>
        /// <returns>فاکتوریل عدد</returns>
        static long CalculateFactorialRecursive(int n)
        {
            if (n == 0 || n == 1)
                return 1;
            return n * CalculateFactorialRecursive(n - 1);
        }

        /// <summary>
        /// ارزیابی یک عبارت ریاضی پیچیده
        /// </summary>
        static void EvaluateExpression()
        {
            Console.WriteLine("\n--- Expression Evaluation ---");
            Console.WriteLine("Enter a mathematical expression (e.g., 2+3*4, sin(45), 2^3+sqrt(4)):");
            Console.WriteLine("Supported operators: +, -, *, /, ^, sqrt(), sin(), cos(), tan(), log(), ln()");

            string expression = GetUserInput("Expression: ").ToLower().Trim();

            try
            {
                // این یک ارزیابی ساده است - در نسخه پیشرفته‌تر می‌توان از کتابخانه‌هایی مثل NCalc استفاده کرد
                double result = EvaluateSimpleExpression(expression);
                string calculation = $"{expression} = {result}";
                calculationHistory.Add(calculation);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Result: {calculation}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error evaluating expression: {ex.Message}");
            }
        }

        /// <summary>
        /// ارزیابی ساده عبارات ریاضی (برای نمایش قابلیت)
        /// در نسخه واقعی باید از پارسر پیشرفته‌تری استفاده شود
        /// </summary>
        /// <param name="expression">عبارت ریاضی</param>
        /// <returns>نتیجه محاسبه</returns>
        static double EvaluateSimpleExpression(string expression)
        {
            // این یک پیاده‌سازی ساده است و فقط عبارات پایه را پشتیبانی می‌کند
            // برای نسخه تولیدی، استفاده از یک کتابخانه ارزیابی عبارت توصیه می‌شود

            // حذف فضاهای خالی
            expression = expression.Replace(" ", "");

            // بررسی و اجرای توابع خاص
            if (expression.Contains("sin("))
            {
                // پیاده‌سازی ساده برای نمایش قابلیت
                int start = expression.IndexOf("sin(") + 4;
                int end = expression.IndexOf(')', start);
                string inner = expression.Substring(start, end - start);
                double value = double.Parse(inner);
                double radians = value * PI / 180;
                double result = Math.Sin(radians);

                return result;
            }
            else if (expression.Contains("sqrt("))
            {
                int start = expression.IndexOf("sqrt(") + 5;
                int end = expression.IndexOf(')', start);
                string inner = expression.Substring(start, end - start);
                double value = double.Parse(inner);

                return Math.Sqrt(value);
            }

            // اگر عبارت ساده بود، از DataTable برای محاسبه استفاده می‌کنیم
            try
            {
                var dataTable = new System.Data.DataTable();
                // جایگزینی کاراکترهای توان و غیره با معادل‌های قابل فهم برای DataTable
                string formattedExpression = expression.Replace('^', '~')
                                                    .Replace("π", PI.ToString())
                                                    .Replace("e", E.ToString());
                var result = dataTable.Compute(formattedExpression, "");
                return Convert.ToDouble(result);
            }
            catch
            {
                throw new ArgumentException("Invalid expression format.");
            }
        }

        /// <summary>
        /// نمایش تاریخچه محاسبات
        /// </summary>
        static void ShowCalculationHistory()
        {
            Console.WriteLine("\n--- Calculation History ---");

            if (calculationHistory.Count == 0)
            {
                Console.WriteLine("No calculations yet.");
                return;
            }

            for (int i = 0; i < calculationHistory.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {calculationHistory[i]}");
            }
        }

        /// <summary>
        /// پاک کردن تاریخچه محاسبات
        /// </summary>
        static void ClearHistory()
        {
            calculationHistory.Clear();
            Console.WriteLine("\nCalculation history cleared.");
        }

        /// <summary>
        /// نمایش ثابت‌های ریاضی مهم
        /// </summary>
        static void ShowConstants()
        {
            Console.WriteLine("\n--- Mathematical Constants ---");
            Console.WriteLine($"π (Pi) = {PI}");
            Console.WriteLine($"e (Euler's number) = {E}");
            Console.WriteLine($"φ (Golden ratio) = {1.618033988749895}");
            Console.WriteLine($"√2 = {Math.Sqrt(2)}");
            Console.WriteLine($"√3 = {Math.Sqrt(3)}");
        }

        /// <summary>
        /// دریافت یک عدد از کاربر با اعتبارسنجی
        /// </summary>
        /// <param name="message">پیغام درخواست عدد</param>
        /// <returns>عدد وارد شده توسط کاربر</returns>
        static double GetNumber(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine().Trim();

                // اجازه دادن به کاربر برای استفاده از ثابت‌ها
                if (input.ToLower() == "pi")
                    return PI;
                if (input.ToLower() == "e")
                    return E;

                if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                {
                    return number;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
        }

        /// <summary>
        /// دریافت ورودی از کاربر
        /// </summary>
        /// <param name="message">پیغام درخواست ورودی</param>
        /// <returns>ورودی کاربر</returns>
        static string GetUserInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }

        /// <summary>
        /// تبدیل عملگر به نام کامل آن
        /// </summary>
        /// <param name="operation">عملگر ریاضی</param>
        /// <returns>نام کامل عملگر</returns>
        static string GetOperationName(string operation)
        {
            switch (operation)
            {
                case "+": return "Addition";
                case "-": return "Subtraction";
                case "*": return "Multiplication";
                case "/": return "Division";
                default: return "Unknown";
            }
        }
    }
}
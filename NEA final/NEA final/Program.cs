using System;
using System.IO;
using System.Collections.Generic;

namespace NEA_Technical_Solution
{
    class Program
    {
        static void Main(string[] args)
        {
            Polynomial f = null; // no function loaded
            if (!File.Exists("SavedFunctions"))
                File.Create("SavedFunctions");
            HelpMenu();
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
            Console.Clear();
            bool readInputs = true;
            while (readInputs)
            {
                Console.Write("\nLoaded function: ");
                if (f != null)
                    f.Print();
                else Console.WriteLine("No function loaded");
                string[] arguments;
                while (true)
                {
                    arguments = ParseCommand(Console.ReadLine()); // user enters command
                    if (arguments.Length > 0)
                        break;
                    Console.WriteLine("Enter a command - try \\help");
                }
                switch (arguments[0])
                {
                    case "\\help":
                        HelpMenu();
                        break;
                    case "\\exit":
                        readInputs = false;
                        break;
                    case "\\quit":
                        readInputs = false;
                        break;
                    case "\\clear":
                        Console.Clear();
                        break;
                    case "\\newfunc":
                        if (arguments.Length != 1)
                        {
                            string formula = "";
                            for (int i = 1; i < arguments.Length; i++) // in case user puts spaces in between terms of function
                                formula += arguments[i];
                            if (IsPolynomial(formula))
                                f = new Polynomial(formula);
                            else
                            {
                                Console.WriteLine("\nInvalid arguments - formula must be a polynomial.");
                                Console.WriteLine("Polynomials must:\n - have only one variable");
                                Console.WriteLine(" - involve only addition, subtraction, multiplication, and positive integer powers of variables");
                                Console.WriteLine(" - written in form ax^n e.g. x^3 + 2x - 2");
                            }
                        }
                        else Console.WriteLine("Invalid arguments - must include formula");
                        break;
                    case "\\loadfunc":
                        if (arguments.Length != 1)
                            f = LoadFunction(arguments[1]);
                        else Console.WriteLine("Invalid arguments - must include name of the file to load function from");
                        break;
                    case "\\listfuncs":
                        ListSavedFunctions();
                        break;
                    case "\\savefunc":
                        if (arguments.Length != 1)
                        {
                            if (arguments.Length == 2) // command and name scenario
                            {
                                if (f != null)
                                    SaveFunction(arguments[1], f);
                                else Console.WriteLine("No function loaded");
                            }
                            else // command, name and function scenario
                            {
                                string formula = "";
                                for (int i = 2; i < arguments.Length; i++) // in case user puts spaces in between terms of function
                                    formula += arguments[i];
                                SaveFunction(arguments[1], new Polynomial(formula));
                            }
                        }
                        else Console.WriteLine("Invalid arguments - must include name and optionally formula \nExample use: \\savefunc f1 x^2-4");
                        break;
                    case "\\deletefunc":
                        if (arguments.Length != 1)
                            DeleteFile(arguments[1]);
                        else Console.WriteLine("Invalid arguments - must include name of function to be deleted");
                        break;
                    case "\\eval":
                        if (f != null)
                        {
                            if (arguments.Length != 1)
                            {
                                if (decimal.TryParse(arguments[1], out _))
                                    Console.WriteLine("f({0}) = {1}", arguments[1], f.Evaluate(Convert.ToDecimal(arguments[1])));
                                else Console.WriteLine("Invalid arguments - argument must be a real number");
                            }
                            else Console.WriteLine("Invalid arguments - must include value to evaluate the function at");
                        }
                        else Console.WriteLine("No function loaded");
                        break;
                    case "\\dif":
                        if (f != null)
                        {
                            if (arguments.Length == 1) // dif loaded function scenario
                                Console.WriteLine("f'({0}) = {1}", f.Variable, f.Derivative().Formula);
                            else if (int.TryParse(arguments[1], out int n)) // find nth derivative of loaded function scenario
                            {
                                Console.Write("f");
                                for (int i = 0; i < n; i++)
                                    Console.Write("'");
                                Console.Write("({0}) = ", f.Variable);
                                Console.WriteLine(f.Derivative(n).Formula);
                            }
                            else if (arguments.Length == 2) // dif with respect to variable scenario
                                Console.WriteLine("d/d{0}({1}) = {2}", arguments[1], f.Formula, f.Derivative(Convert.ToChar(arguments[1])).Formula);
                        }
                        else Console.WriteLine("No function loaded");
                        break;
                    case "\\int":
                        if (f != null)
                        {
                            if (arguments.Length == 1) // indefinite
                                Console.WriteLine("INT({0})d{1} = {2}", f.Formula, f.Variable, f.Integral(f.Variable).Formula);
                            else if (arguments.Length == 3) // definite
                            {
                                decimal a = Convert.ToDecimal(arguments[1]);
                                decimal b = Convert.ToDecimal(arguments[2]);
                                // , a, b, f.Formula, f.Variable, f.Integral(a, b, f.Variable)
                                Console.WriteLine("INT({0})d{1} | {1}={2} to {1}={3} = {4}", f.Formula, f.Variable, a, b, f.Integral(a, b, f.Variable));
                            }
                            else Console.WriteLine("Invalid arguments - must include upper and lower bound");
                        }
                        else Console.WriteLine("No function loaded");
                        break;
                    case "\\area":
                        if (f != null && arguments.Length == 3)
                        {
                            decimal a = Convert.ToDecimal(arguments[1]);
                            decimal b = Convert.ToDecimal(arguments[2]);
                            Console.WriteLine("INTAREA({0})d{1} | {1}={2} to {1}={3} = {4}", f.Formula, f.Variable, a, b, f.IntegralArea(a, b, f.Variable));
                        }
                        else if (f == null)
                            Console.WriteLine("No function loaded");
                        else Console.WriteLine("Invalid arguments - must include upper and lower bound");
                        break;
                    case "\\roots":
                        if (f != null)
                        {
                            List<decimal> roots = f.Roots(-200, 200);
                            if (roots.Count > 0)
                            {
                                Console.WriteLine("Roots of function at:");
                                foreach (var root in roots)
                                    Console.WriteLine("     ({0:0.###}, 0)", root);
                            }
                            else if (roots.Count == 0)
                                Console.WriteLine("Function has no roots");
                        }
                        else Console.WriteLine("No function loaded");
                        break;
                    case "\\turningpoints":
                        if (f != null)
                        {
                            if (arguments.Length == 1 || (arguments[1] != "max" && arguments[1] != "min"))
                                TurningPoints(f);
                            else if (arguments[1] == "max")
                                TurningPoints(f, "max");
                            else if (arguments[1] == "min")
                                TurningPoints(f, "min");
                        }
                        else Console.WriteLine("No function loaded");
                        break;
                    case "\\tangent":
                        if (f != null)
                        {
                            if (arguments.Length == 1)
                                Console.WriteLine("Invalid arguments - must include x-axis value of point to find tangent at");
                            else
                            {
                                if (decimal.TryParse(arguments[1], out _))
                                    f.Tangent(Convert.ToDecimal(arguments[1])).Print();
                                else
                                    Console.WriteLine("Invalid arguments - argument must be a number");
                            }
                        }
                        else Console.WriteLine("No function Loaded");
                        break;
                    case "\\normal":
                        if (f != null)
                        {
                            if (arguments.Length == 1)
                                Console.WriteLine("Invalid arguments - must include x-axis value of point to find normal at");
                            else
                            {
                                if (decimal.TryParse(arguments[1], out _))
                                    f.Normal(Convert.ToDecimal(arguments[1])).Print();
                                else
                                    Console.WriteLine("Invalid arguments - argument must be a number");
                            }
                        }
                        else Console.WriteLine("No function Loaded");
                        break;
                    case "\\evaleqn":
                        Equation();
                        break;
                    case "\\kinematics":
                        KinematicsMenu();
                        break;
                    case "\\optimisation":
                        OptimisationMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid input command - try \\help");
                        break;
                }
            }
        }
        public static decimal RoundToSF(decimal value, int n)
        {
            if (value == 0)
                return value;
            bool neg = value < 0;
            if (neg) value = -value;
            double m10 = Math.Log10((double)value);
            decimal scale = (decimal)Math.Pow(10, Math.Floor(m10) - n + 1);
            value = Decimal.Round(value / scale) * scale;
            if (neg)
                value = -value;
            return value;
        }
        private static void HelpMenu()
        {
            Console.WriteLine();
            Console.WriteLine("\\help");
            Console.WriteLine("\\exit");
            Console.WriteLine("\\clear");
            Console.WriteLine("\\newfunc [function]         - loads a function from the given formula");
            Console.WriteLine("\\loadfunc [name]            - loads a function from given file name");
            Console.WriteLine("\\listfuncs                  - lists all saved functions");
            Console.WriteLine("\\savefunc [name]            - saves the loaded function to a file under given name");
            Console.WriteLine("\\savefunc [name] [function] - saves a given function to a file under given name");
            Console.WriteLine("\\deletefunc [name]          - deletes a function saved in a file");
            Console.WriteLine();
            Console.WriteLine("\\eval [input]               - evaluates function at given input value");
            Console.WriteLine("\\dif                        - finds the derivative of the loaded function");
            Console.WriteLine("\\dif [n]                    - finds the nth derivative of the loaded function");
            Console.WriteLine("\\dif [variable]             - finds the derivative, relative to the given variable, of the loaded function");
            Console.WriteLine("\\int                        - finds the integral of the loaded function");
            Console.WriteLine("\\int [lower] [upper]        - finds the definite integral of the loaded function between given limits");
            Console.WriteLine("\\area [lower] [upper]       - finds the area between the curve of the loaded function and the x-axis between limits");
            Console.WriteLine("\\roots                      - finds all roots of the loaded function");
            Console.WriteLine("\\turningpoints              - finds all extrema of the loaded function");
            Console.WriteLine("\\turningpoints [min/max]    - finds all minima or maximima of the loaded function");
            Console.WriteLine("\\tangent [value]            - finds the equation of a tangent to the curve of the loaded function at given value");
            Console.WriteLine("\\normal [value]             - finds the equation of a normal to the curve of the loaded function at given value");
            Console.WriteLine();
            Console.WriteLine("\\evaleqn                    - input and evaluate a single/multiple variable equation");
            Console.WriteLine("\\kinematics                 - opens the kinematics menu");
            Console.WriteLine("\\optimisation               - opens the optimisation menu");
            Console.WriteLine();
        }
        private static Polynomial LoadFunction(string name)
        {
            try
            {
                using (StreamReader sr = new StreamReader(name))
                {
                    var terms = new List<Term>();
                    while (!sr.EndOfStream)
                    {
                        string[] line = sr.ReadLine().Split(',');
                        if (line.Length > 1)
                            terms.Add(new Term(Convert.ToDecimal(line[0]), Convert.ToChar(line[1]), Convert.ToDecimal(line[2])));
                        else terms.Add(new Term(Convert.ToDecimal(line[0])));
                    }
                    return new Polynomial(terms);
                }
            }
            catch
            {
                Console.WriteLine("Couldn't find file with that name");
                return null;
            }
        }
        private static void ListSavedFunctions()
        {
            try
            {
                using (StreamReader sr = new StreamReader("SavedFunctions"))
                {
                    if (sr.EndOfStream)
                        Console.WriteLine("No functions saved");
                    while (!sr.EndOfStream)
                    {
                        string fileName = sr.ReadLine();
                        Console.Write($"    {fileName}: ");
                        Console.WriteLine(LoadFunction(fileName).Formula);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Something went wrong - try rerunning program");
            }
        }
        private static void SaveFunction(string fileName, Polynomial f)
        {
            if (!File.Exists(fileName)) // if the file does not already exist, add it to the list of saved files
            {
                using (StreamWriter sw = new StreamWriter("SavedFunctions", true))
                    sw.WriteLine(fileName);
            }
            using (StreamWriter sw = new StreamWriter(fileName)) // will create a file with that name if one doesn't exist, else it will overwrite
            {
                if (f == null)
                    Console.WriteLine("Create a new function first, or enter function as an argument");
                else
                {
                    for (int i = 0; i < f.Terms.Count; i++)
                    {
                        if (f.Terms[i].isConstant)
                            sw.WriteLine(f.Terms[i].Coefficient);
                        else sw.WriteLine("{0},{1},{2}", f.Terms[i].Coefficient, f.Terms[i].Variable, f.Terms[i].Exponent);
                    }
                }
            }
            Console.WriteLine("Saved function to file successfully");
        }
        private static void DeleteFile(string name)
        {
            if (File.Exists(name))
            {
                Console.WriteLine("{0}: {1}", name, LoadFunction(name).Formula);
                Console.WriteLine("Are you sure you want to delete this file? y/n");
                string input = Console.ReadLine().ToLower();
                if (input == "y" || input == "yes")
                {
                    File.Delete(name);
                    using (var sr = new StreamReader("SavedFunctions"))
                    using (var sw = new StreamWriter("SavedFunctionsTemp"))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line != name)
                                sw.WriteLine(line);
                        }
                    }
                    File.Delete("SavedFunctions");
                    File.Move("SavedFunctionsTemp", "SavedFunctions");
                    Console.WriteLine("File deleted succesfully");
                }
                else Console.WriteLine("Operation cancelled");
            }
            else Console.WriteLine("No file found with that name");
        }
        private static string[] ParseCommand(string command)
        {
            command = command.ToLower();
            command = command.Replace(" ", ",");
            return command.Split(',');
        }
        private static void TurningPoints(Polynomial f)
        {
            List<decimal> turningPoints = f.TurningPoints(-200m, 200m);
            if (turningPoints.Count > 0)
            {
                Console.WriteLine("Local turning points at: ");
                for (int i = 0; i < turningPoints.Count; i++)
                    Console.WriteLine("    ({0:0.###}, {1:0.###})", turningPoints[i], f.Evaluate(turningPoints[i]));
            }
            if (turningPoints.Count == 0)
                Console.WriteLine("Function has no turning points");
        }
        private static void TurningPoints(Polynomial f, string maxOrMin)
        {
            if (maxOrMin == "min")
            {
                List<decimal> minPoints = f.MinPoints(-200m, 200m);
                if (minPoints.Count > 0)
                {
                    Console.WriteLine("Local minimum points at: ");
                    for (int i = 0; i < minPoints.Count; i++)
                        Console.WriteLine("    ({0:0.###}, {1:0.###})", minPoints[i], f.Evaluate(minPoints[i]));
                }
                if (minPoints.Count == 0)
                    Console.WriteLine("Function has no minimum points");
            }
            else if (maxOrMin == "max")
            {
                List<decimal> maxPoints = f.MaxPoints(-200m, 200m);
                if (maxPoints.Count > 0)
                {
                    Console.WriteLine("Local maximum points at: ");
                    for (int i = 0; i < maxPoints.Count; i++)
                        Console.WriteLine("    ({0:0.###}, {1:0.###})", maxPoints[i], f.Evaluate(maxPoints[i]));
                }
                if (maxPoints.Count == 0)
                    Console.WriteLine("Function has no maximum points");
            }
        }
        private static bool IsPolynomial(string formula) // checks if single variable polynomial
        {
            bool flag = false;
            char variable = '\0';
            var forbiddenChars = new char[] { '(', ')', '/', '*' }; // these chars will cause bugs in polynomial initialisation
            for (int i = 0; i < formula.Length; i++)
            {
                foreach (char ch in forbiddenChars) // check if contains forbidden characters
                {
                    if (formula[i] == ch)
                        flag = true;
                }

                if (variable == '\0' && char.IsLetter(formula[i])) // save first variable found
                    variable = formula[i];
                if (variable != '\0' && char.IsLetter(formula[i]) && formula[i] != variable) // checks if has multiple variables
                    flag = true;

                if (flag) // break early
                    break;
            }
            if (!flag)
            {
                var f = new Polynomial(formula);
                foreach (var term in f.Terms)
                {
                    if ((Math.Round(term.Exponent) != term.Exponent || term.Exponent <= 0) && term.isConstant == false) // exponenent is not a positive whole number
                        flag = true;
                }
            }
            return !flag;
        }
        private static void Equation()
        {
            Console.WriteLine("Equation must be fully explicit - include all necessary brackets and multiplication signs");
            bool loop = true;
            Console.Write("Input equation: ");
            Equation eqn;
            string str = Console.ReadLine();
            if (str.Contains("="))
                eqn = new Equation(str);
            else
                eqn = new Equation("y = " + str);
            while (loop)
            {
                Console.Write("loaded equation: ");
                eqn.Print();
                var variables = new List<Variable>();
                foreach (char ch in eqn.Variables)
                {
                    while (true)
                    {
                        Console.Write($"{ch} = ");
                        string value = Console.ReadLine();
                        if (decimal.TryParse(value, out _))
                        {
                            variables.Add(new Variable(ch, Convert.ToDecimal(value)));
                            break;
                        }
                        else
                            Console.WriteLine("Invalid input - must be a number");
                    }
                }
                Console.WriteLine(eqn.Evaluate(variables));
                Console.Write("Evaluate again? y/n: ");
                loop = Console.ReadLine().ToLower()[0] == 'y' ? true : false;
            }
        }
        private static void KinematicsMenu()
        {
            Console.WriteLine("Note that these commands ignore the integral constant\nwhen going from acceleration to velocity or displacement, answers are only correct if particle starts from rest");
            bool loop1 = true;
            while (loop1)
            {
                int type = 0;
                var units = new string[] { "s", "v", "a" };
                Console.WriteLine("What do you have an expression for? \n(1) displacement \n(2) velocity \n(3) acceleration");
                while (true)
                {
                    string result = Console.ReadLine();
                    if (int.TryParse(result, out int resultInt) && (resultInt >= 1 && resultInt <= 3))
                    {
                        type = resultInt - 1; // 0,1,2 because array indexing
                        break;
                    }
                    else
                        Console.WriteLine("Input must be 1, 2, or 3");
                }
                Polynomial f;
                while (true)
                {
                    Console.Write($"{units[type]} = ");
                    string expression = Console.ReadLine();
                    if (IsPolynomial(expression))
                    {
                        f = new Polynomial(expression);
                        break;
                    }
                    else
                        Console.WriteLine("Expression must be a single variable polynomial");
                }

                bool loop2 = true;
                while (loop2)
                {
                    Console.Write("Loaded equation: ");
                    Console.WriteLine($"{units[type]} = {f.Formula}");
                    Console.WriteLine();
                    Console.WriteLine("(1) find an equation\n(2) find a value at a given time\n(3) find time when a value is equal to zero, or changes in direction");

                    int input;
                    while (true)
                    {
                        string result = Console.ReadLine();
                        if (int.TryParse(result, out int resultInt) && (resultInt >= 1 && resultInt <= 3))
                        {
                            input = resultInt;
                            break;
                        }
                        else
                            Console.WriteLine("Input must be an integer between 1 and 3");
                    }

                    switch (input)
                    {
                        case 1: // find equation
                            {
                                while (true)
                                {
                                    Console.WriteLine("Find equation for:\n(1) displacement\n(2) velocity\n(3) acceleration");
                                    string result = Console.ReadLine();
                                    if (int.TryParse(result, out int resultInt) && (resultInt >= 1 && resultInt <= 3))
                                    {
                                        int numberOfDifs = resultInt - (type + 1); // e.g. s = 1 (current), a = 3 (wanted) => find the 3-1 th derivative, 2nd derivative
                                        if (numberOfDifs >= 0)
                                            Console.WriteLine($"{units[resultInt - 1]} = {f.Derivative(numberOfDifs).Formula}");
                                        else if (numberOfDifs < 0)
                                        {
                                            var f1 = f;
                                            for (int i = 0; i < Math.Abs(numberOfDifs) - 1; i++)
                                            {
                                                var terms = f1.Integral(f.Variable).Terms; // gets the terms of the integral
                                                terms.RemoveAt(terms.Count - 1); // removes + c
                                                f1 = new Polynomial(terms);
                                            }
                                            Console.WriteLine($"{units[resultInt - 1]} = {f1.Integral(f.Variable).Formula}"); // integrates without removing + c
                                        }
                                        break;
                                    }
                                    else
                                        Console.WriteLine("Input must be an integer between 1 and 3");
                                }
                                break;
                            }
                        case 2: // find value at a given time
                            {
                                Polynomial f1 = f;
                                int numberOfDifs;
                                while (true) // enter value to find
                                {
                                    Console.WriteLine("Find:\n(1) displacement\n(2) velocity\n(3) acceleration");
                                    string result = Console.ReadLine();
                                    if (int.TryParse(result, out int resultInt) && (resultInt >= 1 && resultInt <= 3))
                                    {
                                        numberOfDifs = resultInt - (type + 1); // e.g. s = 1 (current), a = 3 (wanted) => find the 3-1 th derivative, 2nd derivative
                                        if (numberOfDifs >= 0)
                                            f1 = f.Derivative(numberOfDifs);
                                        else if (numberOfDifs < 0)
                                        {
                                            for (int i = 0; i < Math.Abs(numberOfDifs) - 1; i++)
                                            {
                                                var terms = f1.Integral(f.Variable).Terms; // gets the terms of the integral
                                                terms.RemoveAt(terms.Count - 1); // removes + c
                                                f1 = new Polynomial(terms);
                                            }
                                            f1 = f1.Integral(f.Variable); // integrates without removing + c
                                        }
                                        break;
                                    }
                                    else
                                        Console.WriteLine("Input must be an integer between 1 and 3");
                                }
                                decimal t;
                                while (true) // enter time
                                {
                                    Console.Write("Enter time: ");
                                    string result = Console.ReadLine();
                                    if (decimal.TryParse(result, out decimal resultDec) && resultDec >= 0)
                                    {
                                        t = resultDec;
                                        break;
                                    }
                                    else
                                        Console.WriteLine("Input must be a real number greater than zero");
                                }
                                decimal eval = RoundToSF(f1.Evaluate(t), 5);
                                Console.WriteLine($"{units[type + numberOfDifs]} = {f1.Evaluate(t)} at {f.Variable} = {t}");
                                break;
                            }
                        case 3: // find when equal to zero
                            {
                                Polynomial f1 = f;
                                int numberOfDifs;
                                while (true) // enter value to find
                                {
                                    Console.WriteLine("Find:\n(1) times when the particle is at the origin\n(2) times when the particle changes direction/is stationary\n(3) times when the particle begins to speed up/slow down");
                                    string result = Console.ReadLine();
                                    if (int.TryParse(result, out int resultInt) && (resultInt >= 1 && resultInt <= 3))
                                    {
                                        numberOfDifs = resultInt - (type + 1); // e.g. s = 1 (current), a = 3 (wanted) => find the 3-1 th derivative, 2nd derivative
                                        if (numberOfDifs >= 0)
                                            f1 = f.Derivative(numberOfDifs);
                                        else if (numberOfDifs < 0)
                                        {
                                            for (int i = 0; i < Math.Abs(numberOfDifs) - 1; i++)
                                            {
                                                var terms = f1.Integral(f.Variable).Terms; // gets the terms of the integral
                                                terms.RemoveAt(terms.Count - 1); // removes + c
                                                f1 = new Polynomial(terms);
                                            }
                                            f1 = f1.Integral(f.Variable); // integrates without removing + c
                                        }
                                        break;
                                    }
                                    else
                                        Console.WriteLine("Input must be an integer between 1 and 3");
                                }
                                List<decimal> roots = f1.Roots(0, 200m);
                                for (int i = 0; i < roots.Count; i++) // removes any roots at a negative time
                                {
                                    if (roots[i] < 0)
                                        roots.RemoveAt(i);
                                }
                                if (type + numberOfDifs == 0) // when is particle at the origin
                                {
                                    var variable = f1.Variable;
                                    if (roots.Count == 0)
                                        Console.WriteLine($"Particle never reaches the origin");
                                    else
                                    {
                                        Console.WriteLine($"Particle is at the origin at:");
                                        foreach (decimal root in roots)
                                            Console.WriteLine($"    {variable} = {RoundToSF(root, 4)}");
                                    }
                                }
                                else if (type + numberOfDifs == 1) // when is particle changing direction or stationary
                                {
                                    var variable = f1.Variable;
                                    if (roots.Count == 0)
                                        Console.WriteLine($"Particle never changes direction/is never stationary");
                                    else
                                    {
                                        Console.WriteLine($"Particle changes direction/is stationary at:");
                                        foreach (decimal root in roots)
                                            Console.WriteLine($"    {variable} = {RoundToSF(root, 4)}");
                                    }
                                }
                                else if (type + numberOfDifs == 2) // when does particle change from speeding up and slowing down or vice versa
                                {
                                    var variable = f1.Variable;
                                    if (roots.Count == 0)
                                        Console.WriteLine($"Particle is always speeding up/slowing down");
                                    else
                                    {
                                        Console.WriteLine($"Particle begins to slow down or speed up at:");
                                        foreach (decimal root in roots)
                                            Console.WriteLine($"    {variable} = {RoundToSF(root, 4)}");
                                    }
                                }
                                break;
                            }
                        default:
                            Console.WriteLine("Input must be a number between 1 and 3");
                            break;
                    }
                    while (true)
                    {
                        Console.WriteLine("(1) perform another operation on expression\n(2) enter new expression\n(3) exit kinematics menu");
                        string result = Console.ReadLine();
                        if (int.TryParse(result, out int resultInt) && (resultInt >= 1 && resultInt <= 3))
                        {
                            if (resultInt == 1)
                                break;
                            else if (resultInt == 2)
                            {
                                loop2 = false;
                                break;
                            }
                            else if (resultInt == 3)
                            {
                                loop1 = false;
                                loop2 = false;
                                break;
                            }
                        }
                        else Console.WriteLine("Input must be an integer between 1 and 3");
                    }
                }
            }
        }
        private static void OptimisationMenu() // cylinder works for constant SA and V, cone for just SA
        {
            Console.WriteLine("Note that cube and cuboids cannot be optimised, as their SA and V can increase to infinity with fixed SA or V");
            Console.WriteLine("Note that spheres cannot be optimised, due to there being only one radius that can exist for each SA or V");
            Console.WriteLine("Select shape:\n(1) cylinder\n(2) cone\n");
            int input1;
            while (true)
            {
                string result = Console.ReadLine();
                if (int.TryParse(result, out int resultInt) && (resultInt >= 1 && resultInt <= 2))
                {
                    input1 = resultInt;
                    break;
                }
                else
                    Console.WriteLine("Input must be a number between 1 and 2");
            }
            Console.WriteLine("Fixed quantity:\n(1) volume\n(2) surface area\n");
            int input2;
            while (true)
            {
                string result = Console.ReadLine();
                if (int.TryParse(result, out int resultInt) && (resultInt >= 1 && resultInt <= 2))
                {
                    input2 = resultInt;
                    break;
                }
                else
                    Console.WriteLine("Input must be a number between 1 and 2");
            }
            Console.Write(input2 == 1 ? "V = " : "SA = ");
            decimal fixedQuant;
            while (true)
            {
                string result = Console.ReadLine();
                if (decimal.TryParse(result, out decimal resultDec) && resultDec > 0)
                {
                    fixedQuant = resultDec;
                    break;
                }
                else
                    Console.WriteLine("Input must be a positive real number");
            }
            decimal pi = Convert.ToDecimal(Math.PI);
            switch (input1)
            {
                case 1: // cylinder
                    if (input2 == 1) // volume is fixed
                    {
                        var terms = new List<Term> { new Term(2 * pi, 'r', 2), new Term(2 * fixedQuant, 'r', -1) }; // formula I derived SA = 2 pi r^2 + (2V)/r
                        var SA = new Polynomial(terms); // I have to enter the terms manually because a polynomial can't have negative powers, but I've made sure it won't cause problems e.g. no div by 0
                        decimal r = SA.MinPoints(0.01m, 200m)[0]; // radius where SA is minimised
                        Console.WriteLine($"Minimum SA = {Math.Round(SA.Evaluate(r), 3)}"); // it MUST have a min point for all V > 0 because I worked it out
                        Console.WriteLine($"When:\n    r = {r}\n    h = {Math.Round(fixedQuant / (pi * r * r), 3)}");
                    }
                    else // surface area is fixed
                    {
                        var terms = new List<Term> { new Term(fixedQuant / 2, 'r', 1), new Term(-pi, 'r', 3) }; // another formula I derived V = (SA r)/2 - pi r^3
                        var V = new Polynomial(terms);
                        decimal r = V.MaxPoints(0.01m, 200m)[0]; // radius where V is maximised
                        Console.WriteLine($"Maximum V = {Math.Round(V.Evaluate(r), 3)}");
                        Console.WriteLine($"When:\n    r = {r}\n    h = {Math.Round((fixedQuant - (2 * pi * r * r)) / (2 * pi * r), 3)}");
                    }
                    break;
                case 2: // cone
                    if (input2 == 1) // volume is fixed
                    {
                        // here the formula is an implicit equation so I will have to approximate the min point
                        var SA = new Equation($"{pi}*r*(r+((((3*{fixedQuant})/({pi}*r^2)))^2+r^2)^(0.5))"); // hefty equation but bear with me
                        decimal minSA = SA.Evaluate(new List<Variable> { new Variable('r', 0.001m) });
                        decimal r = -1;
                        for (decimal i = 0.001m; i < 50m; i += 0.001m) // only checks for radius up to 50 for performance
                        {
                            decimal eval = SA.Evaluate(new List<Variable> { new Variable('r', i) });
                            if (eval < minSA)
                            {
                                minSA = eval;
                                r = i;
                            }
                        }

                        Console.WriteLine($"Minimum SA = {Math.Round(minSA, 3)}");
                        Console.WriteLine($"When:\n    r = {r}\n    h = {Math.Round((3 * fixedQuant) / (pi * r * r), 3)}");
                    }
                    else // surface area is fixed
                    {
                        Console.WriteLine("When surface area is fixed, the volume will increase to infinity as the radius increases, so cannot be optimised.\nWhen volume is fixed, however, a minimum point exists.");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

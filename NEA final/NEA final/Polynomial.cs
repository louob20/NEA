using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NEA_Technical_Solution
{
    public class Polynomial : Mapping
    {
        // (inherited) public string Formula { get; private set; }
        // (inherited) public virtual decimal Evaluate(List<Variable> variables)
        public List<Term> Terms { get; private set; }
        public char Variable { get; private set; }
        public Polynomial(string formula) // constructs object with a given formula string
        {
            Terms = ParseInput(formula);
            Formula = GetFormula();
            Variable = GetVariables()[0];
        }
        public Polynomial(List<Term> terms) // constructs object with a given list of terms
        {
            Terms = terms;
            Formula = GetFormula();
            Variable = GetVariables()[0];
        }
        public override void Print()
        {
            Console.WriteLine($"f({Variable}) = {Formula}");
        }
        private List<Term> ParseInput(string formula) // seperate string into list of terms
        {
            // don't touch, it works.

            formula = formula.Replace(" ", "");
            // hefty regex splits the terms e.g. 1.2, -12x, +x^2 etc.
            var rxTerm = new Regex(@"(^-?(\d+(\.\d+)?)?([a-zA-Z](\^-?\d+(\.\d+)?)?)?)|([\+-](\d+(\.\d+)?)?([a-zA-Z](\^-?\d+(\.\d+)?)?)?)");
            var rxCoef1 = new Regex(@"[\+-]?\d+(\.\d+)?[a-zA-Z]"); // e.g. 12.4x
            var rxCoef2 = new Regex(@"[\+-]?[a-zA-Z]"); // e.g. -x
            var rxVar = new Regex("[a-zA-Z]");
            var rxExp = new Regex(@"\^[\+-]?\d+(\.\d+)?");
            var termMatches = rxTerm.Matches(formula);
            var terms = new List<Term>();
            for (int i = 0; i < termMatches.Count; i++)
            {
                if (decimal.TryParse(termMatches[i].Value, out decimal result)) // if it is parseable, must be a constant
                    terms.Add(new Term(result));
                else // variable
                {
                    decimal coef;
                    decimal exp;
                    if (rxCoef1.IsMatch(termMatches[i].Value)) // in form ax
                    {
                        string match = rxCoef1.Match(termMatches[i].Value).Value;
                        coef = Convert.ToDecimal(match.Remove(match.Length - 1)); // remove the char
                    }
                    else coef = rxCoef2.Match(termMatches[i].Value).Value[0] == '-' ? -1 : 1; // in form +/- x
                    char variable = Convert.ToChar(rxVar.Match(termMatches[i].Value).Value);
                    if (rxExp.IsMatch(termMatches[i].Value))
                    {
                        string expString = rxExp.Match(termMatches[i].Value).Value;
                        exp = Convert.ToDecimal(expString.Replace("^", ""));
                    }
                    else exp = 1;
                    terms.Add(new Term(coef, variable, exp));
                }
            }
            return terms;
        }
        protected override string GetFormula() // creates a printable string from the terms list
        {
            string f = "";
            if (Terms.Count == 0)
                f += 0;
            for (int i = 0; i < Terms.Count; i++)
            {
                decimal coef = Terms[i].Coefficient;
                if (Terms[i].Coefficient != Decimal.Round(Terms[i].Coefficient, 3)) // only displays coefficients to 3dp
                    coef = Decimal.Round(Terms[i].Coefficient, 3);
                if (Terms[i].isConstant) // if term is a constant:
                {
                    if (i == 0 && coef >= 0) // if first term and positive, dont add + symbol
                        f += coef;
                    else if (i > 0 && coef >= 0) // if not first term and positive, add + symbol
                    {
                        f += " + ";
                        f += coef;
                    }
                    else if (i == 0 && coef < 0) // if first term and negative, add - symbol without spaces and make number positive
                        f += "-" + coef * -1;
                    else if (i > 0 && coef < 0) // if not first term and negative, add - symbol without spaces and make number positive
                        f += " - " + coef * -1;
                }
                else // if term has variable
                {
                    if (coef != 1 && coef != -1) // if coefficient not equal to 1 or -1
                    {
                        if (i == 0 && coef >= 0) // if first term and positive, dont add + symbol
                            f += coef;
                        else if (coef >= 0) // if not first term and positive, add + symbol
                            f += " + " + coef;
                        else if (i == 0 && coef < 0) // if first term and negative, add - symbol without spaces
                            f += "-" + coef * -1;
                        else if (coef < 0) // if not first term and negative, add - symbol
                            f += " - " + coef * -1;
                    }
                    else if (i == 0 && coef == -1) // if first term and -1, add only - symbol without spaces
                        f += "-";
                    else if (i != 0) // if not first term and 1 add only + symbol, else add only - symbol
                        f += coef == 1 ? " + " : " - ";

                    f += Terms[i].Variable;

                    if (Terms[i].Exponent != 1)
                        f += "^" + Terms[i].Exponent;
                }
            }
            return f;
        }
        protected override List<char> GetVariables() // returns first variable found in function, if constant function, returns 'x' as default
        {
            char variable = 'x'; // default value. Will be set to the first variable in the function
            for (int i = 0; i < Terms.Count; i++)
            {
                if (!Terms[i].isConstant)
                {
                    variable = Terms[i].Variable;
                    break;
                }
            }
            return new List<char>() { variable };
        }
        public Polynomial Derivative() // finds the first derivative, single variable functions only
        {
            char variable = this.GetVariables()[0];
            return this.Derivative(variable);
        }
        public Polynomial Derivative(int n) // finds the nth derivative, single variable functions only
        {
            var f = new Polynomial(Terms);
            if (n < 1)
                return f;
            for (int i = 0; i < n; i++)
                f = f.Derivative();
            return f;
        }
        public Polynomial Derivative(char variable) // finds the first derivative, differentiated with respect to a variable
        {
            var dTerms = new List<Term>(); // empty list of terms for the derivative
            for (int i = 0, j = 0; i < Terms.Count; i++, j++)
            {
                if (!Terms[i].isConstant && Terms[i].Variable == variable) // differentiate with respect to the variable e.g. x^2 => 2x, y^2 => 0 for dx
                {
                    dTerms.Add(new Term(Terms[i].Coefficient, Terms[i].Variable, Terms[i].Exponent)); // duplicates
                    dTerms[j].Coefficient = dTerms[j].Coefficient * Terms[i].Exponent; // kx^e => ekx^e-1
                    dTerms[j].Exponent -= 1;
                    if (dTerms[j].Exponent == 0)
                    {
                        dTerms[j].isConstant = true;
                        dTerms[j].Variable = '\0';
                    }
                }
                else j--;
                // because if term is a constant it will be skipped, a term wont be added to the derivative but i will still increment
            }
            var derivative = new Polynomial(dTerms);
            return derivative;
        }
        public Polynomial Integral(char variable) // indefinite
        {
            char v = this.GetVariables()[0];
            var iTerms = new List<Term>();
            for (int i = 0; i < Terms.Count; i++)
            {
                if (Terms[i].isConstant) // 8 => 8x
                    iTerms.Add(new Term(Terms[i].Coefficient, variable, 1));
                else // 2x -> 2/2 x^2 = x^2
                {
                    iTerms.Add(new Term(Terms[i].Coefficient, Terms[i].Variable, Terms[i].Exponent));
                    iTerms[i].Exponent++;
                    iTerms[i].Coefficient /= iTerms[i].Exponent; // note that integrating x^-1 will cause error
                }
                if (i == Terms.Count - 1) // 2x => x^2 + c
                    iTerms.Add(new Term(1, 'c', 1));
            }
            return new Polynomial(iTerms);
        }
        public decimal Integral(decimal a, decimal b, char variable) // definite
        {
            Polynomial indef = this.Integral(variable);
            for (int i = 0; i < indef.Terms.Count; i++)
            {
                if (indef.Terms[i].Variable == 'c') // removes all terms with variable c because they cancel out in indefinite
                    indef.Terms.RemoveAt(i);
            }
            decimal fa = indef.Evaluate(a);
            decimal fb = indef.Evaluate(b);
            return Decimal.Round(fb - fa, 3);
        }
        public decimal IntegralArea(decimal a, decimal b, char variable) // finds the area underneath the graph
        {
            List<decimal> bounds = new List<decimal> { a }; // list of x values to integrate between
            List<decimal> roots = this.Roots(a, b);
            foreach (var root in roots)
            {
                if (root < b && root > a)
                    bounds.Add(root);
            }
            bounds.Add(b);
            decimal area = 0;
            for (int i = 0; i < bounds.Count - 1; i++)
                area += Math.Abs(this.Integral(bounds[i], bounds[i + 1], this.Variable));
            return area;
        }
        public decimal Evaluate(decimal x) // for single variable functions
        {
            if (Terms.Count == 0)
                return 0;
            if (Terms[Terms.Count - 1].Variable == 'c')
                Terms.RemoveAt(Terms.Count - 1); // removes unknown variable from integration if present. Will cause bugs if user uses c as a variable
            decimal sum = 0;
            for (int i = 0; i < Terms.Count; i++)
            {
                decimal term = 0; // evaluates each term seperately and adds them to the sum
                if (Terms[i].isConstant)
                    sum += Convert.ToDecimal(Terms[i].Coefficient);
                else
                {
                    if (!(Terms[i].Exponent < 0 && x == 0))
                    {
                        if (Terms[i].Exponent < 0 || Terms[i].Exponent != Math.Round(Terms[i].Exponent)) // if negative or fractional power
                            term = (decimal)Math.Pow((double)x, (double)Terms[i].Exponent);
                        // Math.Pow doesn't accept decimal as arguments
                        else // for higher degree of precision, keep as decimal and calc without Math.Pow
                        {
                            decimal temp = x;
                            for (int j = 1; j < Terms[i].Exponent; j++)
                                temp *= x;
                            term += temp;
                        }
                        term *= Terms[i].Coefficient;
                        sum += term;
                    }
                }
            }
            return sum;
        }
        public decimal NewtonsMethod(decimal x, int maxRecursions) // finds to a high degree of accuracy the root closest to input. Note that graphs with asymptotes will give false results
        {
            if (maxRecursions > 0)
            {
                if (this.Evaluate(x) == 0) // checks for case where input is exactly a root, prevents attempt to divide by zero
                    return x; // base case when input is exactly the root
                var derivative = this.Derivative();
                if (derivative.Terms.Count == 0 || (derivative.Terms.Count == 1 && derivative.Terms[0].isConstant))
                    return 0.123m;
                decimal xn = x - (this.Evaluate(x) / this.Derivative().Evaluate(x));
                if (Math.Abs(this.Evaluate(x) - this.Evaluate(xn)) < 0.0000000001m) // difference between iterations is some very small number
                    return xn; // base case
                else
                {
                    maxRecursions--;
                    return Decimal.Round(NewtonsMethod(xn, maxRecursions), 3); // general case
                }
            }
            else return 0.123m; // returns placeholder value. Unlikely root is actually at 0.123
        }
        public List<decimal> Roots(decimal lowerBound, decimal upperBound) // approximates roots between two bounds
        {
            var roots = new List<decimal>();
            decimal increment = 0.05m;
            for (decimal x = lowerBound; x < upperBound; x += increment)
            {
                decimal currentY = this.Evaluate(x);
                decimal nextY = this.Evaluate(x + increment);
                if ((currentY >= 0 && nextY < 0) || (currentY <= 0 && nextY > 0)) // must have crossed x-axis
                    roots.Add(NewtonsMethod(x, 1000)); // Newtons method provides higher degree of accuracy
            }
            if (roots.Count == 0) // in case it has missed a root that is tangent to the x-axis e.g. x^2
            {
                decimal root = NewtonsMethod(0.123m, 1000);
                if (root != 0.123m) // 0.123 is sentinel value
                    roots.Add(root);
            }
            return roots;
        }
        public List<decimal> TurningPoints(decimal lowerBound, decimal upperBound) // approximates turning points between two bounds
        {
            return !(this.IsLinear()) ? this.Derivative().Roots(lowerBound, upperBound) : new List<decimal>();
        }
        public string DetermineNature(decimal x) // determines nature of turning point. Returns "convex", "concave", or "inflection"
                                                 // used to determine the nature of a turning point
        {
            decimal f2 = this.Derivative(2).Evaluate(x);
            if (f2 > 0)
                return "convex"; // minimum
            else if (f2 < 0)
                return "concave"; // maximium
            else
                return "inflection";
        }
        public List<decimal> MaxPoints(decimal lowerBound, decimal upperBound)
        {
            var maxPoints = new List<decimal>();
            List<decimal> turningPoints = this.TurningPoints(lowerBound, upperBound);
            foreach (decimal x in turningPoints)
            {
                if (this.DetermineNature(x) == "concave")
                    maxPoints.Add(x);
            }
            return maxPoints;
        }
        public List<decimal> MinPoints(decimal lowerBound, decimal upperBound)
        {
            var minPoints = new List<decimal>();
            var turningPoints = this.TurningPoints(lowerBound, upperBound);
            foreach (decimal x in turningPoints)
            {
                if (this.DetermineNature(x) == "convex")
                    minPoints.Add(x);
            }
            return minPoints;
        }
        public bool IsLinear()
        {
            bool linear = true;
            foreach (Term term in Terms)
            {
                if (term.isConstant == false && term.Exponent != 1)
                {
                    linear = false;
                    break;
                }
            }
            return linear;
        }
        public Equation Tangent(decimal x) // finds the tangent to the function at a value of x
        {
            if (this.IsLinear()) // the tangent of a linear line is the line itself
                return new Equation("y=" + this.Formula);
            else
            {
                string tangent = "y=";
                decimal m = this.Derivative().Evaluate(x); // gradient
                if (m != 0) // if m=0, 0x = 0, so disappears
                    tangent += m + this.GetVariables()[0].ToString(); // function shouldn't have vertical tangents so don't check for x = c case
                decimal c = this.Evaluate(x) - (m * x); // constant (y-axis intercept)
                if (c != 0 && m != 0) // if there is an x term, add +/- c
                {
                    if (c < 0)
                        tangent += c;
                    else
                        tangent += "+" + c;
                }
                else if (m == 0) // if there isn't an x term, don't add +/-
                    tangent += c;
                return new Equation(tangent);
            }
        }
        public Equation Normal(decimal x) // finds the normal to the function at a value of x
        {
            string normal = "y=";
            decimal tangGrad = this.Derivative().Evaluate(x);
            // if (tangGrad == 0) // normal will have infinite gradient, vertical line, x=a
            if (tangGrad > -0.0001m && tangGrad < 0.0001m) // normal has extremely high gradient, essentially vertical
            {
                normal = this.GetVariables()[0] + "=" + x;
                return new Equation(normal);
            }
            decimal m = Program.RoundToSF(-1 / tangGrad, 3); // gradient 
            if (m != 0) // if m=0, 0x = 0, so disappears
                normal += m + this.GetVariables()[0].ToString();
            decimal c = Program.RoundToSF(this.Evaluate(x) - (m * x), 3); // constant
            if (c != 0) // if there is an x term, add +/- c
            {
                if (c < 0)
                    normal += c;
                else
                    normal += "+" + c;
            }
            return new Equation(normal);
        }
    }
}
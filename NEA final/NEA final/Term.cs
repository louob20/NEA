using System;

namespace NEA_Technical_Solution
{
    public class Term // represents each term in a polynomial
    {
        public decimal Exponent { get; set; }
        public decimal Coefficient { get; set; }
        public char Variable { get; set; }
        public bool isConstant { get; set; }
        public Term(decimal coefficient, char variable, decimal exponent)
        {
            this.Coefficient = coefficient;
            this.Variable = variable;
            this.Exponent = exponent;
            this.isConstant = false;
        }
        public Term(decimal constant)
        {
            this.Coefficient = constant;
            this.isConstant = true;
        }
    }
}

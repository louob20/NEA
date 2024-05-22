using System;
using System.Collections.Generic;

namespace NEA_Technical_Solution
{

    // represents e.g. y = mx + c as well as non polynomials e.g. y = 1/(x^(1/2) - 1)
    public class Equation : Mapping
    {
        // (inherited) public string Formula { get; private set; } // right side of equation
        // (inherited) public virtual decimal Evaluate(List<Variable> variables)
        public string Subject { get; private set; } // left side of equation i.e. y
        public List<char> Variables { get; private set; }
        private string expression;
        public Equation(string expression)
        {
            if (!expression.Contains("="))
                expression = expression.Insert(0, "y=");
            this.expression = expression;
            Subject = GetSubject();
            Formula = GetFormula();
            Variables = GetVariables();
        }
        public override void Print()
        {
            Console.WriteLine($"{Subject} = {Formula}");
        }
        private string GetSubject()
        {
            string[] sides = expression.Replace(" ", "").Split('=');
            return sides[0];
        }
        protected override string GetFormula()
        {
            string[] sides = expression.Replace(" ", "").Split('=');
            return sides[1];
        }
        protected override List<char> GetVariables()
        {
            var chars = new List<char>();
            for (int i = 0; i < Formula.Length; i++)
            {
                char ch = Formula[i];
                if (char.IsLetter(ch) && !chars.Contains(ch))
                    chars.Add(ch);
            }
            return chars;
        }
    }
}


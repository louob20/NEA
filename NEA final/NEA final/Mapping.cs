using System;
using System.Collections.Generic;

namespace NEA_Technical_Solution
{
    public struct Variable
    {
        public Variable(char c, decimal d)
        {
            character = c;
            value = d;
        }
        public char character;
        public decimal value;
    }

    public abstract class Mapping
    {
        public string Formula { get; protected set; }
        protected abstract string GetFormula();
        protected abstract List<char> GetVariables();
        public abstract void Print();
        public virtual decimal Evaluate(List<Variable> variables) // evaluates from postfix notation, using an array of tuples
        {
            // substitute values in
            string expression = SubValues(this.Formula, variables);
            // convert to RPN
            expression = InfixToRPN(expression);

            // evaluate
            var evStack = new Stack(); // object stack: will hold only decimals
            string[] items = expression.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in items)
            {
                if (char.IsDigit(item[0])) // is an operand
                    evStack.Push(Convert.ToDecimal(item));
                else // is operator
                {
                    if (item == "~") // because only requires one operand
                    {
                        decimal a = Convert.ToDecimal(evStack.Pop());
                        evStack.Push(a * -1);
                    }
                    else
                    {
                        decimal b = Convert.ToDecimal(evStack.Pop());
                        decimal a = Convert.ToDecimal(evStack.Pop());
                        switch (item)
                        {
                            case "^":
                                evStack.Push(Math.Pow((double)a, (double)b));
                                break;
                            case "/":
                                evStack.Push(a / b);
                                break;
                            case "*":
                                evStack.Push(a * b);
                                break;
                            case "+":
                                evStack.Push(a + b);
                                break;
                            case "-":
                                evStack.Push(a - b);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return Convert.ToDecimal(evStack.Pop());
        }
        protected string InfixToRPN(string expression)
        {
            var opStack = new Stack(); // operator stack: object stack but will hold only chars
            var outQueue = new Queue(); // output queue: string queue
            char[] operators = new char[] { '-', '+', '*', '/', '^', '~' };

            expression = expression.Replace(" ", "");
            for (int i = 0; i < expression.Length; i++) // differentiates between subtraction operator and unary negative operator
            {
                if (expression[i] == '-')
                {
                    if (i == 0 || !char.IsDigit(expression[i - 1]))
                        expression = expression.Remove(i, 1).Insert(i, "~");
                }
            }
            for (int i = 0; i < expression.Length; i++)
            {
                char token = expression[i];
                if (char.IsDigit(token)) // finds the next string of digits
                {
                    string num = "";
                    while (char.IsDigit(token) || token == '.')
                    {
                        num += token;
                        i++;
                        if (i < expression.Length)
                        {
                            token = expression[i];
                        }
                        else break;
                    }
                    outQueue.Enqueue(num.ToString());
                    i--;
                }
                else if (token == '~')
                    opStack.Push(token);
                else if (token != '(' && token != ')') // is an operator
                {
                    if (token == '+' || token == '-' || token == '*' || token == '/') // operator is left-associative
                    {
                        while (opStack.Length > 0 && OpPrecedence(Convert.ToChar(opStack.Peek())) >= OpPrecedence(token))
                            outQueue.Enqueue(opStack.Pop().ToString());
                        opStack.Push(token);

                    }
                    else if (token == '^' || token == '~') // operator is right-associative
                    {
                        while (opStack.Length > 0 && Array.IndexOf(operators, opStack.Peek()) > Array.IndexOf(operators, token)) // precedence of previous operator > precedence of current operator
                            outQueue.Enqueue(opStack.Pop().ToString());
                        opStack.Push(token);
                    }
                }
                else // is a bracket
                {
                    if (token == '(')
                        opStack.Push(token);
                    else if (token == ')')
                    {
                        while (opStack.Peek().ToString()[0] != '(')
                            outQueue.Enqueue(opStack.Pop().ToString());
                        opStack.Pop();
                    }
                }
            }
            while (opStack.Length != 0)
                outQueue.Enqueue(opStack.Pop().ToString());
            string output = "";
            while (!outQueue.IsEmpty())
            {
                output += outQueue.Dequeue();
                output += " "; // seperates numbers i.e. 1 12 + rather than 112+
            }
            return output;
        }
        protected int OpPrecedence(char op)
        {
            if (op == '+' || op == '-')
                return 1;
            else if (op == '*' || op == '/')
                return 2;
            else if (op == '^' || op == '~')
                return 3;
            else return -1;
        }
        protected string SubValues(string expression, List<Variable> variables) // sub variable values in
        {
            expression = expression.Replace(" ", "");
            for (int i = 0; i < expression.Length; i++)
            {
                if (char.IsLetter(expression[i]))
                {
                    foreach (var variable in variables)
                    {
                        if (expression[i] == variable.character) // adds multiplication sign as needed
                        {
                            bool left = false;
                            bool right = false;
                            if (i != 0 && char.IsLetterOrDigit(expression[i - 1]))
                                left = true;
                            if (i != expression.Length - 1 && char.IsLetterOrDigit(expression[i + 1]))
                                right = true;
                            expression = expression.Remove(i, 1);
                            expression = expression.Insert(i, variable.value.ToString());
                            if (right)
                                expression = expression.Insert(i + 1, "*");
                            if (left)
                                expression = expression.Insert(i, "*");
                        }
                    }
                }
            }
            return expression;
        }
    }
}
using System;
using System.Collections;

namespace ReversePolishConverter
{
    class Program
    {
        static void Main(string[] args) // Dijkstra's shunting yard algorithm
        {
            var stack = new Stack(); // operator stack
            string output = "";
            char[] operators = new char[] { '-', '+', '*', '/', '^' };
            string str = "-(1+2)";

            for (int i = 0; i < str.Length; i++) // differentiates between subtraction operator and unary negative operator
            {
                if (str[i] == '-')
                {
                    if (i == 0 || !char.IsDigit(str[i - 1]))
                        str = str.Remove(i, 1).Insert(i, "~");
                }
            }

            for (int i = 0; i < str.Length; i++)
            {
                char token = str[i];
                if (char.IsDigit(token))
                    output += token;
                else if (token == '~')
                    stack.Push(token);
                else if (token != '(' && token != ')')                
                {
                    if (token == '+' || token == '-' || token == '*' || token == '/')
                    {
                        if (stack.Count > 0)
                        {
                            int tokenPrecedence = Array.IndexOf(operators, token);
                            int stackPrecedence = Array.IndexOf(operators, stack.Peek());
                            while (stack.Count > 0 && Array.IndexOf(operators, stack.Peek()) >= Array.IndexOf(operators, token))
                                output += stack.Pop();
                        }
                        stack.Push(token);
                    }
                    else if (token == '^' || token == '~')
                    {
                        int tokenPrecedence = Array.IndexOf(operators, token);
                        int stackPrecedence = Array.IndexOf(operators, stack.Peek().ToString()[0]);
                        while (stackPrecedence > tokenPrecedence)
                        {
                            output += stack.Pop();
                            stack.Push(token);
                        }
                    }
                }
                else // is a bracket
                {
                    if (token == '(')
                        stack.Push(token);
                    else if (token == ')')
                    {
                        while (stack.Peek().ToString()[0] != '(')
                            output += stack.Pop();
                        stack.Pop();
                    }
                }
            }
            while (stack.Count != 0)
                output += stack.Pop();

            Console.WriteLine(output);
        }
    }
}

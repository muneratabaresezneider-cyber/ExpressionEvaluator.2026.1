namespace ExpressionEvaluator.Core;

using System;
using System.Collections.Generic;
using System.Globalization;

public class Evaluator
{
    public static double Evaluate(string infix)
    {
        var postfix = InfixToPostfix(infix);
        return EvaluatePostfix(postfix);
    }

    private static string InfixToPostfix(string infix)
    {
        var postFix = string.Empty;
        var stack = new Stack<char>();
        for (int i = 0; i < infix.Length; i++)
        {
            char c = infix[i];

            if (char.IsWhiteSpace(c)) continue;

            if (char.IsDigit(c) || c == '.')
            {
                string number = "";
                while (i < infix.Length && (char.IsDigit(infix[i]) || infix[i] == '.'))
                {
                    number += infix[i];
                    i++;
                }
                postFix += number + " ";
                i--;
                continue;
            }

            if (IsOperator(c))
            {
                if (stack.Count == 0)
                {
                    stack.Push(c);
                }
                else
                {
                    if (c == ')')
                    {
                        while (stack.Count > 0 && stack.Peek() != '(')
                        {
                            postFix += stack.Pop() + " ";
                        }

                        if (stack.Count > 0)
                        {
                            stack.Pop();
                        }
                        else
                        {
                            throw new Exception("Error: Missing opening parenthesis");
                        }
                    }
                    else
                    {
                        while (stack.Count > 0 && PriorityStack(stack.Peek()) >= PriorityInfix(c))
                        {
                            postFix += stack.Pop() + " ";
                        }
                        stack.Push(c);
                    }
                }
            }
            else
            {
                throw new Exception($"Syntax error: invalid character '{c}'");
            }
        }
        while (stack.Count > 0)
        {
            postFix += stack.Pop() + " ";
        }
        return postFix.Trim();
    }

    private static int PriorityStack(char item) => item switch
    {
        '^' => 3,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 0,
        _ => throw new Exception("Sintax error."),
    };

    private static int PriorityInfix(char item) => item switch
    {
        '^' => 4,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 5,
        _ => throw new Exception("Sintax error."),
    };

    private static double EvaluatePostfix(string postfix)
    {
        var stack = new Stack<double>();
        var tokens = postfix.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (string token in tokens)
        {
            if (token.Length == 1 && IsOperator(token[0]))
            {
                var b = stack.Pop();
                var a = stack.Pop();
                var result = token[0] switch
                {
                    '+' => a + b,
                    '-' => a - b,
                    '*' => a * b,
                    '/' => a / b,
                    '^' => Math.Pow(a, b),
                    _ => throw new Exception("Sintax error."),
                };

                stack.Push(result);
            }

            else
            {
                stack.Push(double.Parse(token, CultureInfo.InvariantCulture));
            }
        }
        return stack.Pop();
    }

    private static bool IsOperator(char item) => "+-*/^()".Contains(item);
}
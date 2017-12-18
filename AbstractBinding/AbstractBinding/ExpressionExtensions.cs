using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstractBinding
{
    internal static class ExpressionExtensions
    {
        public static MethodInfo GetMethodInfo(this Expression exp)
        {
            switch (exp)
            {
                case UnaryExpression uex:
                    if ((uex == null) || (uex.NodeType != ExpressionType.Convert))
                    {
                        throw new InvalidOperationException("Neither a MethodCallExpression nor a LambdaExpression wrapped MethodCallExpression nor a ConvertExpression wrapped MethodCallExpression!");   //LOCSTR
                    }
                    else
                    {
                        return GetMethodInfo(uex.Operand);
                    }
                case LambdaExpression lex:
                    return GetMethodInfo(lex.Body);
                case MethodCallExpression mcex:
                    return mcex.Method;
                default:
                    throw new ArgumentException("Invalid expression.", nameof(exp));
            }
        }
    }
}


// Type: SafeLiquid.Util.ExpressionUtility




using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SafeLiquid.Util
{
    public static class ExpressionUtility
    {
        private static readonly Dictionary<Type, Type[]> NumericTypePromotions = new Dictionary<Type, Type[]>();

        static ExpressionUtility()
        {
            Add(typeof(byte), new Type[9]
            {
        typeof (ushort),
        typeof (short),
        typeof (uint),
        typeof (int),
        typeof (ulong),
        typeof (long),
        typeof (Decimal),
        typeof (float),
        typeof (double)
            });
            Add(typeof(sbyte), new Type[6]
            {
        typeof (short),
        typeof (int),
        typeof (long),
        typeof (Decimal),
        typeof (float),
        typeof (double)
            });
            Add(typeof(short), new Type[5]
            {
        typeof (int),
        typeof (long),
        typeof (Decimal),
        typeof (float),
        typeof (double)
            });
            Add(typeof(ushort), new Type[7]
            {
        typeof (uint),
        typeof (int),
        typeof (ulong),
        typeof (long),
        typeof (Decimal),
        typeof (float),
        typeof (double)
            });
            Add(typeof(char), new Type[8]
            {
        typeof (ushort),
        typeof (uint),
        typeof (int),
        typeof (ulong),
        typeof (long),
        typeof (Decimal),
        typeof (float),
        typeof (double)
            });
            Add(typeof(int), new Type[4]
            {
        typeof (long),
        typeof (Decimal),
        typeof (float),
        typeof (double)
            });
            Add(typeof(uint), new Type[5]
            {
        typeof (long),
        typeof (ulong),
        typeof (Decimal),
        typeof (float),
        typeof (double)
            });
            Add(typeof(long), new Type[3]
            {
        typeof (Decimal),
        typeof (float),
        typeof (double)
            });
            Add(typeof(ulong), new Type[3]
            {
        typeof (Decimal),
        typeof (float),
        typeof (double)
            });
            Add(typeof(float), new Type[1] { typeof(double) });
            Add(typeof(Decimal), new Type[2]
            {
        typeof (float),
        typeof (double)
            });
            Add(typeof(double), new Type[0]);

        }
        public static void Add(Type key, Type[] types)
        {
            ExpressionUtility.NumericTypePromotions[key] = types;
        }


        internal static Type BinaryNumericResultType(Type left, Type right)
        {
            if (left == right)
                return left;
            if (!ExpressionUtility.NumericTypePromotions.ContainsKey(left))
                throw new ArgumentException("Argument is not numeric", nameof(left));
            if (!ExpressionUtility.NumericTypePromotions.ContainsKey(right))
                throw new ArgumentException("Argument is not numeric", nameof(right));
            if (((IEnumerable<Type>)ExpressionUtility.NumericTypePromotions[right]).Contains<Type>(left))
                return left;
            return ((IEnumerable<Type>)ExpressionUtility.NumericTypePromotions[left]).Contains<Type>(right) ? right : ((IEnumerable<Type>)ExpressionUtility.NumericTypePromotions[right]).First<Type>((Func<Type, bool>)(p => ((IEnumerable<Type>)ExpressionUtility.NumericTypePromotions[left]).Contains<Type>(p)));
        }

        private static void Cast(
          Expression lhs,
          Expression rhs,
          Type leftType,
          Type rightType,
          Type resultType,
          out Expression castLhs,
          out Expression castRhs)
        {
            castLhs = leftType == resultType ? lhs : (Expression)Expression.Convert(lhs, resultType);
            castRhs = rightType == resultType ? rhs : (Expression)Expression.Convert(rhs, resultType);
        }

        public static Delegate CreateExpression(
          Func<Expression, Expression, BinaryExpression> body,
          Type leftType,
          Type rightType)
        {
            ParameterExpression parameterExpression1 = Expression.Parameter(leftType, "lhs");
            ParameterExpression parameterExpression2 = Expression.Parameter(rightType, "rhs");
            try
            {
                try
                {
                    Type resultType = ExpressionUtility.BinaryNumericResultType(leftType, rightType);
                    Expression castLhs;
                    Expression castRhs;
                    ExpressionUtility.Cast((Expression)parameterExpression1, (Expression)parameterExpression2, leftType, rightType, resultType, out castLhs, out castRhs);
                    return Expression.Lambda((Expression)body(castLhs, castRhs), parameterExpression1, parameterExpression2).Compile();
                }
                catch (InvalidOperationException ex1)
                {
                    try
                    {
                        Type resultType = leftType;
                        Expression castLhs;
                        Expression castRhs;
                        ExpressionUtility.Cast((Expression)parameterExpression1, (Expression)parameterExpression2, leftType, rightType, resultType, out castLhs, out castRhs);
                        return Expression.Lambda((Expression)body(castLhs, castRhs), parameterExpression1, parameterExpression2).Compile();
                    }
                    catch (InvalidOperationException ex2)
                    {
                        Type resultType = rightType;
                        Expression castLhs;
                        Expression castRhs;
                        ExpressionUtility.Cast((Expression)parameterExpression1, (Expression)parameterExpression2, leftType, rightType, resultType, out castLhs, out castRhs);
                        return Expression.Lambda((Expression)body(castLhs, castRhs), parameterExpression1, parameterExpression2).Compile();
                    }
                }
            }
            catch (Exception ex)
            {
                return Expression.Lambda((Expression)Expression.Throw((Expression)Expression.Constant((object)new InvalidOperationException(ex.Message))), parameterExpression1, parameterExpression2).Compile();
            }
        }
    }
}

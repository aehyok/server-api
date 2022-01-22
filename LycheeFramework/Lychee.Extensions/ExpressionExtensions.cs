using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lychee.Extensions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 创建属性表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="propertyName">属性名，支持多级属性名，与句点分隔，范例：Customer.Name</param>
        /// <returns></returns>
        public static Expression Property(this Expression expression, string propertyName)
        {
            if (propertyName.All(t => t != '.'))
            {
                return Expression.Property(expression, propertyName);
            }

            var propertyNameList = propertyName.Split('.');
            Expression result = null;

            for (var i = 0; i < propertyNameList.Length; i++)
            {
                if (i == 0)
                {
                    result = Expression.Property(expression, propertyNameList[0]);
                    continue;
                }

                result = result.Property(propertyNameList[i]);
            }

            return result;
        }

        /// <summary>
        /// 创建属性表达式
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="memberInfo">属性</param>
        /// <returns></returns>
        public static Expression Property(this Expression expression, MemberInfo memberInfo)
        {
            return Expression.MakeMemberAccess(expression, memberInfo);
        }

        /// <summary>
        /// 与操作表达式
        /// </summary>
        /// <param name="left">左操作</param>
        /// <param name="right">右操作</param>
        /// <returns></returns>
        public static Expression And(this Expression left, Expression right)
        {
            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            return Expression.AndAlso(left, right);
        }

        public static Expression<T> And<T>(this Expression<T> left, Expression<T> right)
        {
            return MakeBinary(left, right, Expression.AndAlso);
        }

        public static Expression<T> Or<T>(this Expression<T> left, Expression<T> right)
        {
            return MakeBinary(left, right, Expression.OrElse);
        }

        public static Expression<T> MakeBinary<T>(this Expression<T> left, Expression<T> right, Func<Expression, Expression, Expression> func)
        {
            return MakeBinary((LambdaExpression)left, right, func) as Expression<T>;
        }

        public static LambdaExpression MakeBinary(this LambdaExpression left, LambdaExpression right, Func<Expression, Expression, Expression> func)
        {
            var data = Combinate(right.Parameters, left.Parameters).ToArray();
            right = ParameterReplace.Replace(right, data) as LambdaExpression;
            return Expression.Lambda(func(left.Body, right.Body), left.Parameters.ToArray());
        }

        private static IEnumerable<KeyValuePair<T, T>> Combinate<T>(IEnumerable<T> left, IEnumerable<T> right)
        {
            var a = left.GetEnumerator();
            var b = left.GetEnumerator();

            while (a.MoveNext() && b.MoveNext())
            {
                yield return new KeyValuePair<T, T>(a.Current, b.Current);
            }
        }
    }

    internal sealed class ParameterReplace : ExpressionVisitor
    {
        public static Expression Replace(Expression e, IEnumerable<KeyValuePair<ParameterExpression, ParameterExpression>> paramList)
        {
            var item = new ParameterReplace(paramList);
            return item.Visit(e);
        }

        private Dictionary<ParameterExpression, ParameterExpression> parameters = null;

        public ParameterReplace(IEnumerable<KeyValuePair<ParameterExpression, ParameterExpression>> paramList)
        {
            parameters = paramList.ToDictionary(p => p.Key, p => p.Value, new ParameterEquality());
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression result;
            if (parameters.TryGetValue(p, out result))
                return result;
            else
                return base.VisitParameter(p);
        }

        private class ParameterEquality : IEqualityComparer<ParameterExpression>
        {
            public bool Equals(ParameterExpression x, ParameterExpression y)
            {
                if (x == null || y == null)
                    return false;

                return x.Type == y.Type;
            }

            public int GetHashCode(ParameterExpression obj)
            {
                if (obj == null)
                    return 0;

                return obj.Type.GetHashCode();
            }
        }
    }
}
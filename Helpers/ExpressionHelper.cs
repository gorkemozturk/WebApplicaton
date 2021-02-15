using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApplication.Helpers
{
    public class ExpressionHelper
    {
        private static readonly MethodInfo LambdaMethod = typeof(Expression)
            .GetMethods()
            .First(x => x.Name == "Lambda" && x.ContainsGenericParameters && x.GetParameters().Length == 2);

        private static MethodInfo[] QueryableMethods = typeof(Queryable)
            .GetMethods()
            .ToArray();

        private static MethodInfo GetLambdaFuncBuilder(Type source, Type dest)
        {
            var info = typeof(Func<,>).MakeGenericType(source, dest);
            
            return LambdaMethod.MakeGenericMethod(info);
        }

        public static PropertyInfo GetPropertyInfo<T>(string name)
        {
            return typeof(T).GetProperties().Single(p => p.Name == name);
        }

        public static ParameterExpression Parameter<T>()
        {
            return Expression.Parameter(typeof(T));
        }

        public static MemberExpression GetPropertyExpression(ParameterExpression obj, PropertyInfo property)
        {
            return Expression.Property(obj, property);
        }

        public static LambdaExpression GetLambda<TSource, TDest>(ParameterExpression obj, Expression arg)
        {
            return GetLambda(typeof(TSource), typeof(TDest), obj, arg);
        }

        public static LambdaExpression GetLambda(Type source, Type dest, ParameterExpression obj, Expression arg)
        {
            var builder = GetLambdaFuncBuilder(source, dest);
            
            return (LambdaExpression)builder.Invoke(null, new object[] { arg, new[] { obj } });
        }

        public static IQueryable<T> CallWhere<T>(IQueryable<T> query, LambdaExpression predicate)
        {
            var builder = QueryableMethods
                .First(x => x.Name == "Where" && x.GetParameters().Length == 2)
                .MakeGenericMethod(new[] { typeof(T) });

            return (IQueryable<T>)builder.Invoke(null, new object[] { query, predicate });
        }

        public static IQueryable<TEntity> CallOrderByOrThenBy<TEntity>(
            IQueryable<TEntity> modifiedQuery,
            bool useThenBy,
            bool descending,
            Type propertyType,
            LambdaExpression keySelector)
        {
            var methodName = "OrderBy";

            if (useThenBy)
            {
                methodName = "ThenBy";
            }

            if (descending)
            {
                methodName += "Descending";
            }

            var method = QueryableMethods
                .First(x => x.Name == methodName && x.GetParameters().Length == 2)
                .MakeGenericMethod(new[] { typeof(TEntity), propertyType });

            return (IQueryable<TEntity>)method.Invoke(null, new object[] { modifiedQuery, keySelector });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApplication.Interfaces;

namespace WebApplication.Providers
{
    public class SearchExpressionProvider : ISearchExpressionProvider
    {
        protected const string EqualsOperator = "eq";

        public virtual IEnumerable<string> GetOperators()
        {
            yield return EqualsOperator;
        }

        public virtual Expression GetComparison(MemberExpression left, string op, ConstantExpression right)
        {
            switch (op.ToLower())
            {
                case EqualsOperator: 
                    return Expression.Equal(left, right);
                
                default: 
                    throw new ArgumentException($"The value '{op}' is not valid operator for search term.");
            }
        }

        public virtual ConstantExpression GetValue(string input)
        {
            return Expression.Constant(input);
        }
    }
}

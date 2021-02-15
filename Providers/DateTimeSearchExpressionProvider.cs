using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApplication.Providers
{
    public class DateTimeSearchExpressionProvider : ComparableSearchExpressionProvider
    {
        public override ConstantExpression GetValue(string input)
        {
            if (!DateTimeOffset.TryParse(input, out var value))
                throw new ArgumentException($"The value '{input}' is not valid operator for search term.");

            return Expression.Constant(value);
        }
    }
}

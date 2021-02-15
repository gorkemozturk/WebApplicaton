using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApplication.Providers
{
    public class DecimalToIntSearchExpressionProvider : SearchExpressionProvider
    {
        public override ConstantExpression GetValue(string input)
        {
            if (!decimal.TryParse(input, out var dec))
            {
                throw new ArgumentException($"The value '{input}' is not valid for search term.");
            }

            var places = BitConverter.GetBytes(decimal.GetBits(dec)[3])[2];

            if (places < 2)
            {
                places = 2;
            }

            var digits = (int)(dec * (decimal)Math.Pow(10, places));

            return Expression.Constant(digits);
        }

        public override Expression GetComparison(MemberExpression left, string op, ConstantExpression right)
        {
            switch (op.ToLower())
            {
                case "gt":
                    return Expression.GreaterThan(left, right);

                case "gte":
                    return Expression.GreaterThanOrEqual(left, right);

                case "lt":
                    return Expression.LessThan(left, right);

                case "lte":
                    return Expression.LessThanOrEqual(left, right);

                default:
                    return base.GetComparison(left, op, right);
            }
        }
    }
}

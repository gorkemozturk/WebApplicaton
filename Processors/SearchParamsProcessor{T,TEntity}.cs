using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using WebApplication.Attributes;
using WebApplication.Helpers;
using WebApplication.Models;

namespace WebApplication.Processors
{
    public class SearchParamsProcessor<T, TEntity>
    {
        private readonly string[] _search;

        public SearchParamsProcessor(string[] search)
        {
            _search = search;
        }

        public IEnumerable<SearchTerm> GerTerms()
        {
            if (_search == null)
            {
                yield break;
            }

            foreach (var expression in _search)
            {
                if (string.IsNullOrEmpty(expression))
                {
                    continue;
                }

                var tokens = expression.Split(' ');

                if (tokens.Length == 0)
                {
                    yield return new SearchTerm
                    {
                        ValidSyntax = false,
                        Name = expression
                    };

                    continue;
                }

                if (tokens.Length < 3)
                {
                    yield return new SearchTerm
                    {
                        ValidSyntax = false,
                        Name = tokens[0]
                    };

                    continue;
                }

                yield return new SearchTerm
                {
                    ValidSyntax = true,
                    Name = tokens[0],
                    Operator = tokens[1],
                    Value = string.Join(" ", tokens.Skip(2))
                };
            }
        }

        public IEnumerable<SearchTerm> GetValidTerms()
        {
            var query = GerTerms().Where(t => t.ValidSyntax).ToArray();

            if (!query.Any())
            {
                yield break;
            }

            var declaredTerms = GetTermsFromModel();

            foreach (var term in query)
            {
                var declaredTerm = declaredTerms
                    .FirstOrDefault(t => t.Name.Equals(term.Name, StringComparison.OrdinalIgnoreCase));

                if (declaredTerm == null)
                {
                    continue;
                }

                yield return new SearchTerm
                {
                    ValidSyntax = term.ValidSyntax,
                    Name = declaredTerm.Name,
                    EntityName = declaredTerm.EntityName,
                    Operator = term.Operator,
                    Value = term.Value,
                    ExpressionProvider = declaredTerm.ExpressionProvider
                };
            }
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var terms = GetValidTerms().ToArray();

            if (!terms.Any())
            {
                return query;
            }

            var modifiedQuery = query;

            foreach (var term in terms)
            {
                var info = ExpressionHelper.GetPropertyInfo<TEntity>(term.EntityName ?? term.Name);
                var obj = ExpressionHelper.Parameter<TEntity>();

                var left = ExpressionHelper.GetPropertyExpression(obj, info);
                var right = term.ExpressionProvider.GetValue(term.Value);

                var expression = term.ExpressionProvider.GetComparison(left, term.Operator, right);
                var lambda = ExpressionHelper.GetLambda<TEntity, bool>(obj, expression);
                
                modifiedQuery = ExpressionHelper.CallWhere(modifiedQuery, lambda);
            }

            return modifiedQuery;
        }

        private static IEnumerable<SearchTerm> GetTermsFromModel()
        {
            return typeof(T).GetTypeInfo()
                .DeclaredProperties
                .Where(p => p.GetCustomAttributes<SearchableAttribute>().Any())
                .Select(p =>
                {
                    var attribute = p.GetCustomAttribute<SearchableAttribute>();

                    return new SearchTerm
                    {
                        Name = p.Name,
                        EntityName = attribute.EntityProperty,
                        ExpressionProvider = attribute.ExpressionProvider
                    };
                });
        }
    }
}

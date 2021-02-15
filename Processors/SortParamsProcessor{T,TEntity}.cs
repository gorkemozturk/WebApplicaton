using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApplication.Attributes;
using WebApplication.Helpers;
using WebApplication.Models;

namespace WebApplication.Processors
{
    public class SortParamsProcessor<T, TEntity>
    {
        private readonly string[] _orderBy;

        public SortParamsProcessor(string[] orderBy)
        {
            _orderBy = orderBy;
        }

        public IEnumerable<SortTerm> GetTerms()
        {
            if (_orderBy == null)
            {
                yield break;
            }

            foreach (var term in _orderBy)
            {
                if (string.IsNullOrEmpty(term))
                {
                    continue;
                }

                var tokens = term.Split(" ");

                if (tokens.Length == 0)
                {
                    yield return new SortTerm
                    {
                        Name = term
                    };

                    continue;
                }

                var desc = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

                yield return new SortTerm
                {
                    Name = tokens[0],
                    Desc = desc
                };
            }
        }

        public IEnumerable<SortTerm> GetValidTerms()
        {
            var terms = GetTerms().ToArray();

            if (!terms.Any())
            {
                yield break;
            }

            var declaredTerms = GetTermsFromModel();

            foreach (var term in terms)
            {
                var declaredTerm = declaredTerms
                    .FirstOrDefault(t => t.Name.Equals(term.Name, StringComparison.OrdinalIgnoreCase));

                if (declaredTerm == null)
                {
                    continue;
                }

                yield return new SortTerm
                {
                    Name = declaredTerm.Name,
                    Desc = term.Desc,
                    Default = declaredTerm.Default
                };
            }
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var terms = GetValidTerms().ToArray();

            if (!terms.Any())
            {
                terms = GetTermsFromModel().Where(t => t.Default).ToArray();
            }

            if (!terms.Any())
            {
                return query;
            }

            var modifiedQuery = query;
            var useThenBy = false;

            foreach (var term in terms)
            {
                var info = ExpressionHelper.GetPropertyInfo<TEntity>(term.Name);
                var obj = ExpressionHelper.Parameter<TEntity>();

                var key = ExpressionHelper.GetPropertyExpression(obj, info);
                var selector = ExpressionHelper.GetLambda(typeof(TEntity), info.PropertyType, obj, key);

                modifiedQuery = ExpressionHelper.CallOrderByOrThenBy(
                    modifiedQuery, 
                    useThenBy, 
                    term.Desc,
                    info.PropertyType,
                    selector);

                useThenBy = true;
            }

            return modifiedQuery;
        }

        private static IEnumerable<SortTerm> GetTermsFromModel()
        {
            return typeof(T)
                .GetTypeInfo()
                .DeclaredProperties
                .Where(p => p.GetCustomAttributes<SortableAttribute>().Any())
                .Select(p => new SortTerm 
                { 
                    Name = p.Name,
                    Default = p.GetCustomAttribute<SortableAttribute>().Default
                });
        }
    }
}

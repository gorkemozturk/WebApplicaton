using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Processors;

namespace WebApplication.Models
{
    public class SearchParams<T, TEntity> : IValidatableObject
    {
        public string[] Search { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var processor = new SearchParamsProcessor<T, TEntity>(Search);

            var valids = processor.GetValidTerms().Select(t => t.Name);
            var invalids = processor.GerTerms().Select(t => t.Name).
                Except(valids, StringComparer.OrdinalIgnoreCase);

            foreach (var term in invalids)
            {
                yield return new ValidationResult(
                    $"The value '{term}' is not valid for search term.",
                    new[] { nameof(Search) });
            }
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            return new SearchParamsProcessor<T, TEntity>(Search).Apply(query);
        }
    }
}

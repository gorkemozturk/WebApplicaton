using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Processors;

namespace WebApplication.Models
{
    public class SortParams<T, TEntity> : IValidatableObject
    {
        public string[] OrderBy { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var processor = new SortParamsProcessor<T, TEntity>(OrderBy);

            var valids = processor.GetValidTerms().Select(t => t.Name);
            var invalids = processor.GetTerms().Select(t => t.Name)
                .Except(valids, StringComparer.OrdinalIgnoreCase);

            foreach (var term in invalids)
            {
                yield return new ValidationResult(
                    $"The value '{term}' is not valid for sort term.",
                    new[] { nameof(OrderBy) });
            }
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            return new SortParamsProcessor<T, TEntity>(OrderBy).Apply(query);
        }
    }
}

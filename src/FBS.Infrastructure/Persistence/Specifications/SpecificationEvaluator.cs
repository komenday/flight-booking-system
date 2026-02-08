using FBS.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FBS.Infrastructure.Persistence.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> specification) where T : class
    {
        var query = inputQuery;

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        query = specification.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        query = specification.IncludeStrings
            .Aggregate(query, (current, include) => current.Include(include));

        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.Skip.HasValue)
        {
            query = query.Skip(specification.Skip.Value);
        }

        if (specification.Take.HasValue)
        {
            query = query.Take(specification.Take.Value);
        }

        return query;
    }
}
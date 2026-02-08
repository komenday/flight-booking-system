using FBS.Domain.Common.Exceptions;
using FBS.Domain.Common.Interfaces;

namespace FBS.Domain.Common.Base;

public static class EntityBusinessRulesExtensions
{
    public static void CheckRule<TId>(this Entity<TId> entity, IBusinessRule rule) where TId : notnull
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }

    public static void CheckRules<TId>(this Entity<TId> entity, params IBusinessRule[] rules) where TId : notnull
    {
        foreach (var rule in rules)
        {
            entity.CheckRule(rule);
        }
    }
}

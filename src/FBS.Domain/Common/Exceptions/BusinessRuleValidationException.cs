using FBS.Domain.Common.Interfaces;
using FBS.Domain.Common.Rules;

namespace FBS.Domain.Common.Exceptions;

public class BusinessRuleValidationException(IBusinessRule brokenRule) : DomainException(brokenRule.Message)
{
    public IBusinessRule BrokenRule { get; } = brokenRule;

    public RuleErrorType RuleErrorType => BrokenRule.ErrorType;

    public override string ToString()
    {
        return string.Format("{0} ({1}): {2}", BrokenRule.GetType().Name, RuleErrorType, BrokenRule.Message);
    }
}

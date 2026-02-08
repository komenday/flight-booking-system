using FBS.Application.Common.Result;
using FBS.Domain.Common.Rules;

namespace FBS.Application.Extensions;

public static class RuleErrorTypeExtensions
{
    public static ErrorType ToErrorType(this RuleErrorType ruleErrorType)
    {
        return ruleErrorType switch
        {
            RuleErrorType.Validation => ErrorType.Validation,
            RuleErrorType.NotFound => ErrorType.NotFound,
            RuleErrorType.Conflict => ErrorType.Conflict,
            _ => ErrorType.Failure
        };
    }
}
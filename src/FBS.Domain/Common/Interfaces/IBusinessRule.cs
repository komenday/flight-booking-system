using FBS.Domain.Common.Rules;

namespace FBS.Domain.Common.Interfaces;

public interface IBusinessRule
{
    bool IsBroken();

    string Message { get; }

    RuleErrorType ErrorType { get; }
}
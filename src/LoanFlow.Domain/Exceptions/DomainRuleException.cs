namespace LoanFlow.Domain.Exceptions;

public sealed class DomainRuleException(string message) : InvalidOperationException(message)
{
}

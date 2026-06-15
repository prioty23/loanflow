using LoanFlow.Application.LoanApplications;

namespace LoanFlow.Web.Components;

public static class CustomerApplicationWizardRoutes
{
    public static string GetEditPath(int applicationId)
    {
        return $"/customer/applications/{applicationId}/edit";
    }

    public static string GetViewPath(int applicationId)
    {
        return $"/customer/applications/{applicationId}";
    }

    public static string GetTimelinePath(int applicationId)
    {
        return $"/customer/applications/{applicationId}/timeline";
    }

    public static string GetStepPath(int applicationId, LoanApplicationWizardStep step)
    {
        return $"{GetEditPath(applicationId)}/{GetSlug(step)}";
    }

    public static string GetSlug(LoanApplicationWizardStep step)
    {
        return step switch
        {
            LoanApplicationWizardStep.Employment => "employment",
            LoanApplicationWizardStep.MonthlyFinances => "monthly-finances",
            LoanApplicationWizardStep.LoanRequest => "loan-request",
            LoanApplicationWizardStep.ReviewAndDeclaration => "review",
            _ => throw new ArgumentOutOfRangeException(nameof(step))
        };
    }

    public static LoanApplicationWizardStep? ParseStep(string? slug)
    {
        return slug?.ToLowerInvariant() switch
        {
            "employment" => LoanApplicationWizardStep.Employment,
            "monthly-finances" => LoanApplicationWizardStep.MonthlyFinances,
            "loan-request" => LoanApplicationWizardStep.LoanRequest,
            "review" => LoanApplicationWizardStep.ReviewAndDeclaration,
            null or "" => null,
            _ => null
        };
    }
}

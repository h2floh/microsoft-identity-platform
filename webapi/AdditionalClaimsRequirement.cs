using Microsoft.AspNetCore.Authorization;

namespace AuthorizationPoliciesSample.Policies.Requirements;

public class AdditionalClaimsRequirement : IAuthorizationRequirement
{
    public AdditionalClaimsRequirement() {}
}
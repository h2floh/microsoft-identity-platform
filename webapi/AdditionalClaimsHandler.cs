using System.Security.Claims;
using AuthorizationPoliciesSample.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace AuthorizationPoliciesSample.Policies.Handlers;

public class AdditionalClaimsHandler : AuthorizationHandler<AdditionalClaimsRequirement>
{
    private readonly ILogger _logger;

    public AdditionalClaimsHandler(ILogger<AdditionalClaimsHandler> logger)
    {
        _logger = logger;
        _logger.LogDebug("Instantiated AdditionalClaimsHandler");
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, AdditionalClaimsRequirement requirement)
    {

        // for understanding how this works
        foreach(var claim in context.User.Claims) {
            _logger.LogDebug($"Claim: {claim.Type} {claim.Value}");
        }

        var claim_to_validate = context.User.FindFirst(
            c => c.Type == ClaimTypes.Email);

        if (claim_to_validate is null)
        {
            return Task.CompletedTask;
        }

        if (claim_to_validate.Value == "FWagner@alegri.eu")
        {
            _logger.LogDebug($"Claim '{claim_to_validate.Type}'='{claim_to_validate.Value}' is authorized");
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
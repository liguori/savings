﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Savings.SPA.Authorization
{
    public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public CustomAuthorizationMessageHandler(IConfiguration configuration, IAccessTokenProvider provider,
            NavigationManager navigationManager)
            : base(provider, navigationManager)
        {
            ConfigureHandler(
                authorizedUrls: new[] { configuration["SavingsApiServiceUrl"] ?? throw new ArgumentNullException("SavingsApiServiceUrl") },
                onAccessTokenNotAvailable: HandleTokenRenewal);
        }

        private async Task HandleTokenRenewal(AccessTokenNotAvailableException ex)
        {
            ex.Redirect();
        }
    }
}

using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Soma.Platform.Web.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            Console.WriteLine("CustomAuthenticationStateProvider.GetAuthenticationStateAsync called");
            var token = await _localStorage.GetItemAsync<string>("authToken");
            
            Console.WriteLine($"Token retrieved in auth state provider: {(string.IsNullOrEmpty(token) ? "null/empty" : "present")}");
            
            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(_anonymous);

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            Console.WriteLine($"User authenticated: {user.Identity?.IsAuthenticated}");
            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAuthenticationStateAsync: {ex.Message}");
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task NotifyUserAuthentication(string token)
    {
        Console.WriteLine("NotifyUserAuthentication called");
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        
        Console.WriteLine($"Notifying authentication state change - User authenticated: {user.Identity?.IsAuthenticated}");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        
        return token.Claims;
    }
}
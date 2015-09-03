using Microsoft.Practices.ServiceLocation;
using MiniBlog.Contracts;

public static class AuthenticationService
{
    private static readonly IAuthenticationService Service = ServiceLocator.Current.GetInstance<IAuthenticationService>();

    public static bool Authenticate(string userName, string password)
    {
        return Service.Authenticate(userName, password);
    }
}
namespace MiniBlog.Contracts
{
    public interface IAuthenticationService
    {
        bool Authenticate(string userName, string password);
    }
}
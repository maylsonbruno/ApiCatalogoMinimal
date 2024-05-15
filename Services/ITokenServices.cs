using ApiCatalogoMinimal.Models;

namespace ApiCatalogoMinimal.Services
{
    public interface ITokenServices
    {
        string GerarToken(string key, string issuer, string audience, UserModel user);
    }
}

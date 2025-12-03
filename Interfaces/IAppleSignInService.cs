using System.Threading.Tasks;
using AsadorMoron.Models;

namespace AsadorMoron.Interfaces
{
    public interface IAppleSignInService
    {
        bool IsAvailable { get; }
        Task<AppleSignInCredentialState> GetCredentialStateAsync(string userId);
        Task<AppleAccount> SignInAsync();
    }
}

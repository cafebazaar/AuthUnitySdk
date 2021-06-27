using CafeBazaar.Core;
namespace CafeBazaar.AuthAndStorage
{
    public class SignInResult : CafeBaseResult
    {
        public string AccountId { get; set; }
        public CoreSignInStatus Status { get; set; }
    }
}
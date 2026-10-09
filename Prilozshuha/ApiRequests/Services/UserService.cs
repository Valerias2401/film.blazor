using Prilozshuha.ApiRequests.Models;

namespace Prilozshuha.ApiRequests.Services
{
    public class UserService
    {
        public UserDataShort? CurrentUser { get; set; }
        public event Action? OnChange;
        public bool IsAuthenticated => CurrentUser != null;
        public bool IsAdmin => CurrentUser?.Role?.Trim().ToLower() == "admin";
        public void SetCurrentUser(UserDataShort user)
        {
            CurrentUser = user;
            OnChange?.Invoke();
        }
        public void ClearCurrentUser()
        {
            CurrentUser = null;
            OnChange?.Invoke();
        }
    }
}
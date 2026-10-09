namespace Prilozshuha.ApiRequests.Models
{
    public class UserDataShort
    {
        public int id_User { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
    }

    public class UsersResponse
    {
        public bool status { get; set; }
        public UserDataContainer? data { get; set; }
    }

    public class UserDataContainer
    {
        public List<UserDataShort> users { get; set; } = new();
    }

    public class ReqDataUser
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserAddResponse
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty;
        public int userId { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty;
        public int userId { get; set; }
        public string userName { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
    }

    public class UserUpdateRequest
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? RoleId { get; set; }
    }
}
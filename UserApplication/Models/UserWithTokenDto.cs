namespace UserApplication.Models
{
    public class UserWithTokenDto
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }
}

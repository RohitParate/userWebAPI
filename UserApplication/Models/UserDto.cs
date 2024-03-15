namespace UserApplication.Models
{
    /// <summary>
    /// DTO for Getting users
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Id of user
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email of user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// password of user
        /// </summary>
        public string Password { get; set; }


    }
}

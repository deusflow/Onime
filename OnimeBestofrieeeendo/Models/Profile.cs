namespace OnimeBestofrieeeendo.Models;


    public class UserProfile
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime JoinDate { get; set; }
        public decimal Balance { get; set; }
        public string Role { get; set; } = "";
        public string? AvatarUrl { get; set; }
        public int Level { get; set; }
        public string? Bio { get; set; }
    }
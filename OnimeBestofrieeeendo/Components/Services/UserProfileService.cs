using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using OnimeBestofrieeeendo.Models;

namespace OnimeBestofrieeeendo.Components.Services
{
    public class UserProfileService
    {
        private readonly IConfiguration _configuration;

        public UserProfileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<UserProfile>> LoadProfilesAsync()
        {
            var profiles = new List<UserProfile>();
            var connString = _configuration.GetConnectionString("DefaultConnection");
            
            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();
                
                var cmd = new NpgsqlCommand(@"
                    SELECT u.id, u.username, u.email, u.join_date, u.balance, u.role,
                           p.avatar_url, p.level, p.bio
                    FROM users u
                    JOIN profiles p ON u.id = p.user_id", conn);
                
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    profiles.Add(new UserProfile
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        JoinDate = reader.GetDateTime(3),
                        Balance = reader.GetDecimal(4),
                        Role = reader.GetString(5),
                        AvatarUrl = reader.IsDBNull(6) ? null : reader.GetString(6),
                        Level = reader.GetInt32(7),
                        Bio = reader.IsDBNull(8) ? null : reader.GetString(8)
                    });
                }
            }
            catch (Exception)
            {
                // Возвращаем пустой список если что-то пошло не так
            }
            
            return profiles;
        }

        public string GetAvatarUrl(UserProfile profile)
        {
            if (string.IsNullOrWhiteSpace(profile.AvatarUrl))
            {
                return "/images/default-avatar.jpg";
            }
            var avatarPath = profile.AvatarUrl.StartsWith("/") ? profile.AvatarUrl : "/" + profile.AvatarUrl;
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", avatarPath.TrimStart('/'));
            return File.Exists(fullPath) ? avatarPath : "/images/default-avatar.svg";
        }


    }
}

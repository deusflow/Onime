
using Npgsql;
using OnimeBestofrieeeendo.Components.Pages;

namespace OnimeBestofrieeeendo.Components.Services;


public class ContactService
{
    private readonly string _connectionString;
    
    public ContactService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task SaveContactAsync(Home.ContactFormModel contact)
    {
        try
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            
            var cmd = new NpgsqlCommand("INSERT INTO contacts (name, email, subject, message) VALUES (@name, @email, @subject, @message)", conn);
            cmd.Parameters.AddWithValue("name", contact.Name);
            cmd.Parameters.AddWithValue("email", contact.Email);
            cmd.Parameters.AddWithValue("subject", contact.Subject);
            cmd.Parameters.AddWithValue("message", contact.Message);

            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception)
        {
            // Простая обработка ошибок
            throw;
        }
    }
    
}
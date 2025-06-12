using Npgsql;
using OnimeBestofrieeeendo.Models;

namespace OnimeBestofrieeeendo.Components.Services

{
    public class DbService
    {
        private readonly string? _connectionString;

        public DbService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<TradeItems>> GetShopItemsAsync()
        {
            var items = new List<TradeItems>();

            const string sql = @"
                SELECT 
                    shop.shop_item_id,
                    shop.price,
                    shop.quantity,
                    items.item_name,
                    items.item_type,
                    items.rarity,
                    items.image_url
                FROM shop
                JOIN items ON shop.item_id = items.item_id
                WHERE shop.available = TRUE
                ORDER BY shop.shop_item_id;";

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new NpgsqlCommand(sql, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var item = new TradeItems
                {
                    ShopItemId = reader.GetInt32(reader.GetOrdinal("shop_item_id")),
                    Price = reader.GetInt32(reader.GetOrdinal("price")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    ItemName = reader.GetString(reader.GetOrdinal("item_name")),
                    ItemType = reader.GetString(reader.GetOrdinal("item_type")),
                    Rarity = reader.GetString(reader.GetOrdinal("rarity")),
                    ImageUrl = reader.IsDBNull(reader.GetOrdinal("image_url"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("image_url"))
                };

                items.Add(item);
            }

            return items;
        }


        public async Task<TradeItems?> GetItemDetailsAsync(int id)
        {
            const string sql = @"
        SELECT 
            shop.shop_item_id,
            shop.price,
            shop.quantity,
            shop.available,
            items.item_name,
            items.item_type,
            items.rarity,
            items.image_url,
            items.description
        FROM shop
        JOIN items ON shop.item_id = items.item_id
        WHERE shop.shop_item_id = @id;";

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var item = new TradeItems
                {
                    ShopItemId = reader.GetInt32(reader.GetOrdinal("shop_item_id")),
                    Price = reader.GetInt32(reader.GetOrdinal("price")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    Available = reader.GetBoolean(reader.GetOrdinal("available")),
                    ItemName = reader.GetString(reader.GetOrdinal("item_name")),
                    ItemType = reader.GetString(reader.GetOrdinal("item_type")),
                    Rarity = reader.GetString(reader.GetOrdinal("rarity")),
                    ImageUrl = reader.IsDBNull(reader.GetOrdinal("image_url"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("image_url")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("description"))
                };

                return item;
            }

            return null;
        }

        public async Task DeleteShopItemAsync(int shopItemId)
        {
            const string sql = "DELETE FROM shop WHERE shop_item_id = @shopItemId";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("shopItemId", shopItemId);

            await command.ExecuteNonQueryAsync();
        }


        public async Task<UserProfile?> GetUserProfileAsync() 
        {
            const string sql = @"
            SELECT
                users.id,
                users.username,
                users.balance
            FROM users WHERE users.id = 1";

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new NpgsqlCommand( sql, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var user = new UserProfile
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Username = reader.GetString(reader.GetOrdinal("username")),
                    Balance = reader.GetInt32(reader.GetOrdinal("balance"))
                };
                return user;
            }
            return null;
        }

        public async Task<bool> UpdateBalance(UserProfile? user, int price)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var transaction = await connection.BeginTransactionAsync(); // Begin transactionas

            try
            {
                // Get current balance
                var getBalanceCmd = new NpgsqlCommand("SELECT balance FROM users WHERE id = 1", connection, transaction);

                var result = await getBalanceCmd.ExecuteScalarAsync();
                var currentBalance = Convert.ToInt32(result);
                var newBalance = currentBalance - price;

                // Update balance
                var updateCmd = new NpgsqlCommand("UPDATE users SET balance = @balance WHERE id = @id", connection, transaction);
                updateCmd.Parameters.AddWithValue("@balance", newBalance);
                updateCmd.Parameters.AddWithValue("@id", user.Id);

                await updateCmd.ExecuteNonQueryAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Ошибка: " + ex.Message);
                return false;
            }
        }

        public async Task CreateItem(TradeItems item)
        {
            const string sqlItems = @"
        INSERT INTO items (item_name, item_type, rarity, image_url, description)
        VALUES (@name, @type, @rarity, @imageUrl, @description)
        RETURNING item_id;
    ";

            const string sqlShop = @"
        INSERT INTO shop (item_id, price)
        VALUES (@itemId, @price);
    ";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // insert into items
                await using var cmdItems = new NpgsqlCommand(sqlItems, connection, transaction);
                cmdItems.Parameters.AddWithValue("@name", item.ItemName);
                cmdItems.Parameters.AddWithValue("@type", item.ItemType);
                cmdItems.Parameters.AddWithValue("@rarity", (object?)item.Rarity ?? DBNull.Value);
                cmdItems.Parameters.AddWithValue("@imageUrl", (object?)item.ImageUrl ?? DBNull.Value);
                cmdItems.Parameters.AddWithValue("@description", (object?)item.Description ?? DBNull.Value);

                var itemId = (int)(await cmdItems.ExecuteScalarAsync())!;

                // insert into shop
                await using var cmdShop = new NpgsqlCommand(sqlShop, connection, transaction);
                cmdShop.Parameters.AddWithValue("@itemId", itemId);
                cmdShop.Parameters.AddWithValue("@price", item.Price);

                await cmdShop.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


    }
}

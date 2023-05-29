using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using MySqlConnector;

namespace MySqlDbLib
{
    public class MySqlDbLib
    {
        public MySqlConnection Connection { get; }
        public MySqlDbLib(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
        }

        public async Task<List<List<object>>> GetBanksParams()
        {
            try 
            { 
                if (Connection.State.ToString() == "Closed")
                    await Connection.OpenAsync();
                var banksList = new List<List<object>>();
                using var command = new MySqlCommand("SELECT * FROM banks_total;", Connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    List<object> bank = new()
                    {
                        reader.GetValue(0),
                        reader.GetValue(1),
                        reader.GetValue(2)
                    };
                    banksList.Add(bank);
                }
                return banksList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<object>> GetBankParamsByName(string bankName)
        {
            try
            {
                if (Connection.State.ToString() == "Closed")
                    await Connection.OpenAsync();
                using var command = new MySqlCommand($"SELECT * FROM banks_total WHERE (bank = \"{bankName}\");;", Connection);
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    List<object> bankParams = new()
                    {
                        reader.GetValue(0),
                        reader.GetValue(1),
                        reader.GetValue(2)
                    };
                    return bankParams;
                }
                else
                {
                    return null;
                }
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task SetBankTotal(string bankName, decimal total)
        {
            try
            {
                if (Connection.State.ToString() == "Closed")
                    await Connection.OpenAsync();
                NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                using var commandUpdate = new MySqlCommand($"UPDATE banks_total SET total = {total.ToString("G", nfi)} WHERE (bank = \"{bankName}\");", Connection);
                await commandUpdate.ExecuteNonQueryAsync();
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task CreateBank(Guid id, string name, decimal total)
        {
            try
            {
                if (Connection.State.ToString() == "Closed")
                await Connection.OpenAsync();
                using var commandCreateBank = new MySqlCommand($"INSERT INTO banks_total (id, bank, total) VALUES (\"{id}\", \"{name}\", \"{total}\")", Connection);
                await commandCreateBank.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
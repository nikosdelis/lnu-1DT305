using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace lnu_function_app
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly string? dbConnString;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
            dbConnString = Environment.GetEnvironmentVariable("postgres-conn-string");
        }

        [Function("RegisterTemperature")]
        public async Task<IActionResult> RegisterTemperature([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            req.Query.TryGetValue("temperature", out var temperature);
            req.Query.TryGetValue("type", out var type);


            using (NpgsqlConnection conn = new NpgsqlConnection(dbConnString))
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand($"INSERT INTO \"temperature-{type}\" (\"timestamp\", temperature) VALUES (NOW(), {temperature})", conn);
                await cmd.ExecuteNonQueryAsync();                
            }

            return new OkResult();
        }

        [Function("RegisterGeolocation")]
        public async Task<IActionResult> RegisterGeolocation([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            req.Query.TryGetValue("geox", out var geox);
            req.Query.TryGetValue("geoy", out var geoy);


            using (NpgsqlConnection conn = new NpgsqlConnection(dbConnString))
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand($"INSERT INTO \"geolocation\" (\"timestamp\", geox, geoy) VALUES (NOW(), {geox}, {geoy})", conn);
                await cmd.ExecuteNonQueryAsync();
            }

            return new OkResult();
        }

    }
}

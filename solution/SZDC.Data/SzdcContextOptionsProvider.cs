using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace SZDC.Data {

    public class SzdcContextOptionsProvider {

        public static DbContextOptions<SzdcContext> Get(string connectionString) {

            return new DbContextOptionsBuilder<SzdcContext>()
                .UseNpgsql(new NpgsqlConnection(connectionString))
                .Options;
        }
    }
}

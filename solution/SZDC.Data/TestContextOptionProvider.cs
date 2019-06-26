using Microsoft.EntityFrameworkCore;

namespace SZDC.Data {

    public class TestContextOptionsProvider {

        public static DbContextOptions<SzdcContext> Get() {

            return new DbContextOptionsBuilder<SzdcContext>()
                .UseInMemoryDatabase(databaseName: "test szdc database")
                .Options;
        }
    }
}

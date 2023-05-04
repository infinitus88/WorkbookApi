using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using WorkbookApi.DAL.Entities;

namespace WorkbookApi.DAL
{
    public class WorkbookDbContextSeed
    {
        public static async Task SeedAsync(WorkbookDbContext context,
            ILogger logger,
            int retry = 0)
        {
            var retryForAvailability = retry;
            try
            {
                if (context.Database.IsSqlite())
                {
                    context.Database.Migrate();
                }

                if (!await context.Users.AnyAsync())
                {
                    await context.Users.AddRangeAsync(
                        GetPreconfiguredUsers());

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailability >= 10) throw;

                retryForAvailability++;

                logger.LogError(ex.Message);
                await SeedAsync(context, logger, retryForAvailability);
                throw;
            }
        }

        static IEnumerable<User> GetPreconfiguredUsers()
        {
            return new List<User>
            {
                new("minnikhanovrus@gmail.com", "infinitus81", "19551977")
            };
        }
    }
}

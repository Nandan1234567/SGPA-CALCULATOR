using Microsoft.EntityFrameworkCore;

namespace SGPA_CALCULATOR.Infrastructure.Data
{
    public class SgpaDbContext : DbContext
    {
        public SgpaDbContext(DbContextOptions<SgpaDbContext> options) : base(options)
        {
        }

        
    }
}

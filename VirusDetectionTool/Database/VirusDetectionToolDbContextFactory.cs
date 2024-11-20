using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirusDetectionTool.Database
{
    public class VirusDetectionToolDbContextFactory : IDesignTimeDbContextFactory<VirusDetectionToolDbContext>
    {
        public VirusDetectionToolDbContext CreateDbContext(string[] args=null)
        {
            var optionBuilder = new DbContextOptionsBuilder<VirusDetectionToolDbContext>().UseSqlServer("Server=localhost;Database=VirusDetectionTool;User ID=sxr;Password=sxr123;TrustServerCertificate = true;");
            return new VirusDetectionToolDbContext(optionBuilder.Options);
        }
    }
}

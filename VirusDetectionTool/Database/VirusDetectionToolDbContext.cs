using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirusDetectionTool.MVVM.Models;

namespace VirusDetectionTool.Database
{
    public class VirusDetectionToolDbContext:DbContext
    {
        public DbSet<VirusSignature> VirusSignatures {  get; set; }
        public VirusDetectionToolDbContext(DbContextOptions<VirusDetectionToolDbContext> options)
        : base(options)
        {
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirusDetectionTool.MVVM.Models;

namespace VirusDetectionTool.Database.DbServices
{
    public class VirusSignaturesDataService
    {
        private readonly VirusDetectionToolDbContextFactory dbContextFactory;
        public VirusSignaturesDataService() 
        {
            dbContextFactory = new VirusDetectionToolDbContextFactory();
        }
        public async Task<VirusSignature> SearchHashValuesAsync(string hashValue)
        {
            using (VirusDetectionToolDbContext context = dbContextFactory.CreateDbContext())
            {
                var virusSignature = await context.VirusSignatures.Where(u => u.HashValue == hashValue).FirstOrDefaultAsync();
                return virusSignature;
            }
        }
    }
}

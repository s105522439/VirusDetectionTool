using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirusDetectionTool.MVVM.Models
{
    public class VirusSignature
    {
        public int Id { get; set; }
        public string VirusName { get; set; }
        public string HashValue {  get; set; }

    }
}

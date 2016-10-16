using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication12
{
    public class ProcessingUnit
    {
        public Thread Thread { get; set; }
        public long FromByte { get; set; }
        public long ToByte { get; set; }
    }
}

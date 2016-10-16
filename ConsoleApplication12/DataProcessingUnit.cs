using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication12
{
    public class DataProcessingUnit
    {
        public EventWaitHandle WaitHandle { get; set; }
        public Thread Thread { get; set; }
        public byte[] Data { get; set; }
    }
}

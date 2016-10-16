using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication12
{
    public class ArgsParser
    {
        public Dictionary<string, string> Args = new Dictionary<string, string>(); 

        public void Parse(string[] args, List<string> argNames)
        {
            for (int i = 0; i < argNames.Count; i++)
            {
                if(args.Length <= i)
                    throw new ArgumentException($"Invalid argument count, actual: {args.Length}, expected: {argNames.Count}");
                this.Args.Add(argNames[i], args[i]);
            }
        }
    }
}

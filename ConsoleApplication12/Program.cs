using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dapplo.Jenkins;
using KafkaNet;
using KafkaNet.Model;
using KafkaNet.Protocol;
using NLog;
using Renci.SshNet;

namespace ConsoleApplication12
{
    public class Program
    {
        static void Main(string[] args)
        {
            var gzipProcessor = new GzipProcessor();

            var argsParser = new ArgsParser();
            argsParser.Parse(args, new List<string> {"method","inFile","outFile"});

            switch (argsParser.Args["method"])
            {
                case "compress":
                    gzipProcessor.Compress(argsParser.Args["inFile"], argsParser.Args["outFile"]); break;
                case "decompress":
                    gzipProcessor.Decompress(argsParser.Args["inFile"], argsParser.Args["outFile"]); break; ;
            }

            Console.WriteLine("finish");
        }

    }
}
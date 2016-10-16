using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace ConsoleApplication12
{
    public class GzipProcessor
    {
        public static int ThreadNumber = Environment.ProcessorCount;
        public static int DataChunkSize = 1024*1024*32;//Environment.SystemPageSize;

        public void Compress(string fileNameIn, string fileNameOut)
        {
            Console.WriteLine("Compress");

            var inStream = new FileStream(fileNameIn, FileMode.Open);
            var outStream = new FileStream(fileNameOut, FileMode.Create);

            var processingUnits = new List<DataProcessingUnit>();
            while (true)
            {
                var inBuffer = new byte[DataChunkSize];
                if(inStream.Read(inBuffer, 0, inBuffer.Length)==0) break;
                
                var processingUnit = new DataProcessingUnit();
                var waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                var thread = new Thread(() => {
                    var compressedBytes = CompressBytes(inBuffer).ToList();
                    var dataSize = BitConverter.GetBytes(compressedBytes.Count);
                    compressedBytes.InsertRange(0, dataSize);
                    processingUnit.Data = compressedBytes.ToArray();
                    processingUnit.WaitHandle.Set();

                    Console.WriteLine($"Thread input:{inBuffer.Length/1024/1024} output:{compressedBytes.Count/1024/1024} size:{compressedBytes.Count}");
                });
                thread.Start();
                processingUnit.Thread = thread;
                processingUnit.WaitHandle = waitHandle;
                processingUnits.Add(processingUnit);

                if (processingUnits.Count == ThreadNumber)
                    FlushCompressedData(processingUnits, outStream);
                
            }

            if (processingUnits.Any())
                FlushCompressedData(processingUnits, outStream);

            inStream.Close();
            outStream.Close();
        }

        public void Decompress(string fileNameIn, string fileNameOut)
        {
            Console.WriteLine("Decompress");

            var inStream = new FileStream(fileNameIn, FileMode.Open);
            var outStream = new FileStream(fileNameOut, FileMode.Create);

            var processingUnits = new List<DataProcessingUnit>();
            while (true)
            {
                var dataSize = 0;
                var dataSizeBuffer = new byte[4];
                if (inStream.Read(dataSizeBuffer, 0, dataSizeBuffer.Length) != 0)
                {
                    dataSize = BitConverter.ToInt32(dataSizeBuffer, 0);
                    Console.WriteLine($"DataSize: {dataSize}");
                }
                if (dataSize == 0) break;

                var inBuffer = new byte[dataSize];
                if (inStream.Read(inBuffer, 0, inBuffer.Length) == 0) break;

                var processingUnit = new DataProcessingUnit();
                var waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                var thread = new Thread(() => {
                                                  var decompressedData = DecompressBytes(inBuffer);
                                                  processingUnit.Data = decompressedData;
                                                  processingUnit.WaitHandle.Set();
                });
                thread.Start();
                processingUnit.Thread = thread;
                processingUnit.WaitHandle = waitHandle;
                processingUnits.Add(processingUnit);

                if (processingUnits.Count == ThreadNumber)
                    FlushCompressedData(processingUnits, outStream);
            }

            if(processingUnits.Any())
                FlushCompressedData(processingUnits, outStream);

            inStream.Close();
            outStream.Close();
        }

        private static void FlushCompressedData(List<DataProcessingUnit> processingUnits, FileStream outStream)
        {
            WaitHandle.WaitAll(processingUnits.Select(x => x.WaitHandle).ToArray());
            var compressedData = processingUnits.SelectMany(x => x.Data).ToArray();
            outStream.Write(compressedData, 0, compressedData.Length);
            processingUnits.Clear();
        }

        private byte[] CompressBytes(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream compressStream = new GZipStream(memoryStream, CompressionMode.Compress))
                    compressStream.Write(bytes, 0, bytes.Length);
                return memoryStream.ToArray();
            }
        }

        private byte[] DecompressBytes(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (GZipStream decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    var outputMemory = new MemoryStream();
                    decompressStream.CopyTo(outputMemory);
                    return outputMemory.ToArray();
                }
            }
        }
    }
}

using System;
using System.IO;
using System.IO.Compression;

namespace Neme.Utils
{
    public static class FileCompressor
    {
        public static byte[] CompressFile(string filePath)
        {
            using (FileStream fsInput = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (MemoryStream msOutput = new MemoryStream())
            using (GZipStream gzip = new GZipStream(msOutput, CompressionMode.Compress))
            {
                fsInput.CopyTo(gzip);
                gzip.Flush();
                return msOutput.ToArray();
            }
        }

        public static byte[] DecompressFile(byte[] compressedData)
        {
            using (MemoryStream msInput = new MemoryStream(compressedData))
            using (MemoryStream msOutput = new MemoryStream())
            using (GZipStream gzip = new GZipStream(msInput, CompressionMode.Decompress))
            {
                gzip.CopyTo(msOutput);
                return msOutput.ToArray();
            }
        }
    }
}

using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Models
{
    public static class GzipHelper
    {
        public static string Compress(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] compressedBytes;

            using (var result = new MemoryStream())
            {
                using (var compressionStream = new GZipStream(result, CompressionMode.Compress))
                {
                    compressionStream.Write(inputBytes, 0, inputBytes.Length);
                }

                compressedBytes = result.ToArray();
            }

            return Convert.ToBase64String(compressedBytes);
        }

        public static string Decompress(string input)
        {
            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] decompressedBytes = new byte[inputBytes.Length];

            var result = string.Empty;

            using (var inputMemoryStream = new MemoryStream(inputBytes))
            {
                using (var outputMemoryStream = new MemoryStream())
                {
                    using (var decompressionStream = new GZipStream(inputMemoryStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(outputMemoryStream);
                    }

                    decompressedBytes = outputMemoryStream.ToArray();
                }
            }

            return Encoding.UTF8.GetString(decompressedBytes);
        }
    }
}

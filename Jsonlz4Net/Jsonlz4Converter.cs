using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Jsonlz4Net
{
    public class Jsonlz4Converter
    {
        private readonly byte[] header = { 109, 111, 122, 76, 122, 52, 48, 0 };
        private byte[] mozLz4List;

        public Jsonlz4Converter(string filePath)
        {
            mozLz4List = File.ReadAllBytes(filePath);
        }

        public bool Check()
        {
            return BytesEquals(header, mozLz4List.Take(header.Length).ToArray());
        }

        private bool BytesEquals(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }

        public int GetLz4Lengh()
        {
            var headerLengh = header.Length;
            return mozLz4List[headerLengh]
                | mozLz4List[headerLengh + 1] << 8
                | mozLz4List[headerLengh + 2] << 16
                | mozLz4List[headerLengh + 3] * 0x1000000;
        }

        public byte[] UnCompressLz4()
        {
            var lz4Length = GetLz4Lengh();
            byte[] unLz4List = Enumerable.Repeat((byte)0, lz4Length).ToArray(); ;
            mozLz4List = mozLz4List.Skip(header.Length + 4).ToArray();
            int i = 0, j = 0;
            while (i < mozLz4List.Length)
            {
                var token = mozLz4List[i];
                i++;
                var literalsLength = token >> 4;
                if (literalsLength > 0)
                {
                    var li = literalsLength + 240;
                    while (li == 255)
                    {
                        li = mozLz4List[i];
                        i += 1;
                        literalsLength += li;
                    }
                    var endi = i + literalsLength;
                    while (i < endi)
                    {
                        unLz4List[j] = mozLz4List[i];
                        j += 1;
                        i += 1;
                    }
                    if (i == mozLz4List.Length)
                    {
                        return unLz4List;
                    }
                }
                var offset = mozLz4List[i] | (mozLz4List[i + 1] << 8);
                i += 2;
                if (offset == 0 || offset > j)
                {
                    return null;
                }
                var matchLength = token & 0xf;
                var l = matchLength + 240;
                while (l == 255)
                {
                    l = mozLz4List[i];
                    i += 1;
                    matchLength += l;
                }
                var pos = j - offset;
                var end = j + matchLength + 4;
                while (j < end)
                {
                    unLz4List[j] = unLz4List[pos];
                    j += 1;
                    pos += 1;
                }
            }
            return null;
        }

        public byte[] Unc()
        {
            if (!Check())
            {
                return null;
            }
            var lz4Length = GetLz4Lengh();
            if (lz4Length > 0)
            {
                var unLz4 = UnCompressLz4();
                string unLz4Hex = string.Join("", unLz4.Select(x => x.ToString("x2")));
                byte[] unLzBin = String2ByteArray(unLz4Hex);
                return unLzBin;
            }
            return null;
        }

        private byte[] String2ByteArray(string hex)
        {
            int numberOfChars = hex.Length;
            byte[] bytes = new byte[numberOfChars / 2];
            for (int i = 0; i < numberOfChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public void SaveToFile(string filePath)
        {
            var bs = Unc();
            File.WriteAllBytes(filePath, bs);
        }
    }
}

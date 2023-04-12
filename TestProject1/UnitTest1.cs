using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jsonlz4Net;
using System.Diagnostics;

namespace Jsonlz4Net.Tests
{
    [TestClass()]
    public class UnitTest1
    {
        Jsonlz4Converter jsonlz4Converter;
        public UnitTest1()
        {
            jsonlz4Converter = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testfiles", "recovery.jsonlz4"));
        }

        [TestMethod()]
        public void CheckTest()
        {
            Assert.IsTrue(jsonlz4Converter.Check());
        }

        [TestMethod()]
        public void GetLz4LenghTest()
        {
            Debug.WriteLine(jsonlz4Converter.GetLz4Lengh());
        }

        [TestMethod()]
        public void UnCompressLz4Test()
        {
            Debug.WriteLine(string.Join("", jsonlz4Converter.UnCompressLz4()));
        }

        [TestMethod()]
        public void UncTest()
        {
            var bs = jsonlz4Converter.Unc();
            Debug.WriteLine(string.Join("", bs));
            File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testfiles", "recovery.json"),
                bs);
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using menu_service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace menu_service.Tests
{
    [TestClass()]
    public class HashManagerTests
    {
        [TestMethod()]
        public void GetHashTest()
        {
            string hash = HashManager.GetHash("test");
            Assert.AreEqual("9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08", hash);
        }

        [TestMethod()]
        public void CompareStringToHashTest()
        {
            Assert.IsTrue(HashManager.CompareStringToHash("test", "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08"));
        }
    }
}
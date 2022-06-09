using Microsoft.VisualStudio.TestTools.UnitTesting;
using menu_service;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWT.Exceptions;

namespace menu_service.Tests
{
    [TestClass()]
    public class JWTManagerTests
    {
        JWTManager _jwtManager;

        [TestInitialize()]
        public void Initialize()
        {
            _jwtManager = new JWTManager("test", 3);
        }

        [TestMethod()]
        public void CreateValidTokenTest()
        {
            string token = _jwtManager.Create(new Dictionary<string, object> { { "first", 1 }, { "second", 2 } });
            Assert.IsTrue(!string.IsNullOrEmpty(token));
        }

        [TestMethod()]
        public void CreateEmptyTokenTest()
        {
            string token = _jwtManager.Create(new Dictionary<string, object>());
            Assert.IsTrue(!string.IsNullOrEmpty(token));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateNullTokenTest()
        {
            _jwtManager.Create(null);
        }

        [TestMethod()]
        public void DecodeValidTokenTest()
        {
            string token = _jwtManager.Create(new Dictionary<string, object> { { "first", 1 }, { "second", 2 } });
            Dictionary<string, object> json = _jwtManager.Decode(token);

            Assert.IsTrue(
                json != null && json.Count == 3 &&
                json.ContainsKey("first") && Convert.ToInt32(json["first"]) == 1 &&
                json.ContainsKey("second") && Convert.ToInt32(json["second"]) == 2 &&
                json.ContainsKey("exp"));
        }

        [TestMethod()]
        public void DecodeEmptyTokenTest()
        {
            string token = _jwtManager.Create(new Dictionary<string, object> { });
            Dictionary<string, object> json = _jwtManager.Decode(token);

            Assert.IsTrue(json != null && json.Count == 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(TokenExpiredException))]
        public void DecodeExpiredTokenTest()
        {
            string token = _jwtManager.Create(new Dictionary<string, object> { });
            Thread.Sleep(5000);

            _jwtManager.Decode(token);
        }

        [TestMethod()]
        [ExpectedException(typeof(SignatureVerificationException))]
        public void DecodeTokenWithInvalidSignatureTest()
        {
            string token = _jwtManager.Create(new Dictionary<string, object> { });
            JWTManager newJwtManager = new JWTManager("Different");


            newJwtManager.Decode(token);
        }
    }
}
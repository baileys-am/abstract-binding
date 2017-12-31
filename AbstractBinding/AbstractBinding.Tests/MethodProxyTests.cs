using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AbstractBinding.Messages;
using AbstractBinding.SenderInternals;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class MethodProxyTests
    {
        private const string _testCategory = "Method Proxy Tests";

        private readonly Mock<IProxyClient> _clientMock;

        public MethodProxyTests()
        {
            // Initialize client mock
            _clientMock = new Mock<IProxyClient>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void InvokeVoidReturnTest()
        {
            // Arrange
            string objectId = "obj1Id";
            var objDescFactory = new ObjectDescriptionFactory();
            var objDesc = objDescFactory.Create<IRegisteredObject>();
            var proxyObj = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);
            object[] args = null;
            _clientMock.Setup(o => o.Request(It.IsAny<InvokeRequest>())).Returns<InvokeRequest>(req =>
            {
                args = new object[]
                {
                    req.methodArgs[0],
                    req.methodArgs[1]
                };
                return new InvokeResponse()
                {
                    objectId = objectId,
                    methodId = objDesc.Methods.First(kvp => kvp.Key.Contains(nameof(IRegisteredObject.VoidReturnMethodStrVal))).Key,
                    result = null
                };
            });

            // Act
            proxyObj.VoidReturnMethodStrVal("string", 2.0);

            // Assert
            _clientMock.VerifyAll();
            Assert.AreEqual("string", args[0]);
            Assert.AreEqual(2.0, args[1]);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void InvokeVoidReturnParamsTest()
        {
            // Arrange
            string objectId = "obj1Id";
            var objDescFactory = new ObjectDescriptionFactory();
            var objDesc = objDescFactory.Create<IRegisteredObject>();
            var proxyObj = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);
            object[] args = null;
            _clientMock.Setup(o => o.Request(It.IsAny<InvokeRequest>())).Returns<InvokeRequest>(req =>
            {
                args = new object[]
                {
                    (req.methodArgs[0] as object[])[0],
                    (req.methodArgs[0] as object[])[1]
                };
                return new InvokeResponse()
                {
                    objectId = objectId,
                    methodId = objDesc.Methods.First(kvp => kvp.Key.Contains(nameof(IRegisteredObject.VoidReturnMethod))).Key,
                    result = null
                };
            });

            // Act
            proxyObj.VoidReturnMethod("string", 2.0);

            // Assert
            _clientMock.VerifyAll();
            Assert.AreEqual("string", args[0]);
            Assert.AreEqual(2.0, args[1]);
        }
    }
}

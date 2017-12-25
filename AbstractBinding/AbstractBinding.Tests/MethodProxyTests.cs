﻿using System;
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

        private readonly Mock<IAbstractClient> _clientMock;
        private readonly ISerializer _serializer = new Serializer();

        public MethodProxyTests()
        {
            // Initialize client mock
            _clientMock = new Mock<IAbstractClient>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void InvokeVoidReturnTest()
        {
            // Arrange
            string objectId = "obj1Id";
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var proxyObj = proxyFactory.Create<IRegisteredObject>(objectId);
            object[] args = null;
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                var reqObj = _serializer.DeserializeObject<InvokeRequest>(req);
                args = new object[]
                {
                    reqObj.methodArgs[0],
                    reqObj.methodArgs[1]
                };
                return _serializer.SerializeObject(new InvokeResponse()
                {
                    objectId = objectId,
                    methodId = nameof(IRegisteredObject.VoidReturnMethodStrVal),
                    result = null
                });
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
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var proxyObj = proxyFactory.Create<IRegisteredObject>(objectId);
            object[] args = null;
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                var reqObj = _serializer.DeserializeObject<InvokeRequest>(req);
                args = new object[]
                {
                    _serializer.DeserializeObject<object[]>(_serializer.SerializeObject(reqObj.methodArgs[0]))[0],
                    _serializer.DeserializeObject<object[]>(_serializer.SerializeObject(reqObj.methodArgs[0]))[1]
                };
                return _serializer.SerializeObject(new InvokeResponse()
                {
                    objectId = objectId,
                    methodId = nameof(IRegisteredObject.VoidReturnMethod),
                    result = null
                });
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

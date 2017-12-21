using System;
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
        private readonly Mock<ISerializer> _serializerMock;

        public MethodProxyTests()
        {
            // Initialize client mock
            _clientMock = new Mock<IAbstractClient>();

            // Initialize serializer mock
            _serializerMock = new Mock<ISerializer>();
            _serializerMock.Setup(o => o.SerializeObject(It.IsAny<object>())).Returns<object>(obj =>
            {
                return Serializer.Serialize(obj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<Response>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<Response>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<ExceptionResponse>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<ExceptionResponse>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<InvokeResponse>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<InvokeResponse>(serObj);
            });
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void InvokeVoidReturnTest()
        {
            // Arrange
            string objectId = "obj1Id";
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
            var proxyObj = proxyFactory.Create<IRegisteredObject>(objectId);
            object[] args = null;
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                var reqObj = Serializer.Deserialize<InvokeRequest>(req);
                args = new object[]
                {
                    reqObj.methodArgs[0],
                    reqObj.methodArgs[1]
                };
                return Serializer.Serialize(new InvokeResponse()
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
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
            var proxyObj = proxyFactory.Create<IRegisteredObject>(objectId);
            object[] args = null;
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                var reqObj = Serializer.Deserialize<InvokeRequest>(req);
                args = new object[]
                {
                    Serializer.Deserialize<object[]>(Serializer.Serialize(reqObj.methodArgs[0]))[0],
                    Serializer.Deserialize<object[]>(Serializer.Serialize(reqObj.methodArgs[0]))[1]
                };
                return Serializer.Serialize(new InvokeResponse()
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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AbstractBinding.Messages;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class RecipientMethodApiTests
    {
        private const string _testCategory = "Recipient Method API";
        private readonly Mock<IRecipientCallback> _serviceMock;
        private readonly Mock<IRegisteredObject> _regObjectMock;
        private readonly ISerializer _serializer = new Serializer();

        public RecipientMethodApiTests()
        {
            // Initialize service mock
            _serviceMock = new Mock<IRecipientCallback>();

            // Initialize registered object mock
            _regObjectMock = new Mock<IRegisteredObject>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void InvokeVoidReturnWithoutExceptionTest()
        {
            // Arrange
            var objectId = "objId1";
            double args0 = 1.1;
            string args1 = "test";

            _regObjectMock.Setup(o => o.VoidReturnMethod(new object[] { args0, args1 }));
            var server = new Recipient(_serializer);
            var requestObj = new InvokeRequest()
            {
                objectId = objectId,
                methodId = nameof(IRegisteredObject.VoidReturnMethod),
                methodArgs = new object[] { new object[] { args0, args1 } }
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            string response = server.Request(_serializer.SerializeObject(requestObj));

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var responseObj = _serializer.DeserializeObject<InvokeResponse>(response);
            Assert.AreEqual(ResponseType.invoke, responseObj.responseType);
            Assert.AreEqual(requestObj.objectId, responseObj.objectId);
            Assert.AreEqual(requestObj.methodId, responseObj.methodId);
            Assert.IsNull(responseObj.result);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void InvokeVoidReturnWithExceptionTest()
        {
            // Arrange
            var objectId = "objId1";
            double args0 = 1.1;
            string args1 = "test";
            NotImplementedException exception = null;
            _regObjectMock.Setup(o => o.VoidReturnMethod(new object[] { args0, args1 })).Callback<object[]>(objs =>
            {
                exception = new NotImplementedException("THIS IS AN EMERGENCY BROADCAST!");
                throw exception;
            });
            var server = new Recipient(_serializer);
            var requestObj = new InvokeRequest()
            {
                objectId = objectId,
                methodId = nameof(IRegisteredObject.VoidReturnMethod),
                methodArgs = new object[] { args0, args1 }
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            string response = server.Request(_serializer.SerializeObject(requestObj));

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var responseObj = _serializer.DeserializeObject<ExceptionResponse>(response);
            Assert.IsTrue(responseObj.exception.Message.Contains(objectId) &&
                          responseObj.exception.Message.Contains(requestObj.methodId));
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void InvokeVoidReturnStrTest()
        {
            // Arrange
            var objectId = "objId1";
            string args0 = "test";

            _regObjectMock.Setup(o => o.VoidReturnMethodStr(args0));
            var server = new Recipient(_serializer);
            var requestObj = new InvokeRequest()
            {
                objectId = objectId,
                methodId = nameof(IRegisteredObject.VoidReturnMethodStr),
                methodArgs = new object[] { args0 }
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            string response = server.Request(_serializer.SerializeObject(requestObj));

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var responseObj = _serializer.DeserializeObject<InvokeResponse>(response);
            Assert.AreEqual(ResponseType.invoke, responseObj.responseType);
            Assert.AreEqual(requestObj.objectId, responseObj.objectId);
            Assert.AreEqual(requestObj.methodId, responseObj.methodId);
            Assert.IsNull(responseObj.result);
        }
    }
}

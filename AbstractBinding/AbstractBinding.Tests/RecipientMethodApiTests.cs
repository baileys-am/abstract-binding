using System;
using System.Linq;
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
            var server = new Recipient();
            var request = new InvokeRequest()
            {
                objectId = objectId,
                methodId = ObjectDescriptor.GetMethodId<IRegisteredObject>(nameof(IRegisteredObject.VoidReturnMethod)),
                methodArgs = new object[] { new object[] { args0, args1 } }
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            var response = server.Request(request) as InvokeResponse;

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();
            
            Assert.AreEqual(ResponseType.invoke, response.responseType);
            Assert.AreEqual(request.objectId, response.objectId);
            Assert.AreEqual(request.methodId, response.methodId);
            Assert.IsNull(response.result);
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
            var server = new Recipient();
            var request = new InvokeRequest()
            {
                objectId = objectId,
                methodId = ObjectDescriptor.GetMethodId<IRegisteredObject>(nameof(IRegisteredObject.VoidReturnMethod)),
                methodArgs = new object[] { args0, args1 }
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            var response = server.Request(request) as ExceptionResponse;

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();
            
            Assert.IsTrue(response.exception.Message.Contains(objectId) &&
                          response.exception.Message.Contains(request.methodId));
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void InvokeVoidReturnStrTest()
        {
            // Arrange
            var objectId = "objId1";
            string args0 = "test";

            _regObjectMock.Setup(o => o.VoidReturnMethodStr(args0));
            var server = new Recipient();
            var request = new InvokeRequest()
            {
                objectId = objectId,
                methodId = ObjectDescriptor.GetMethodId<IRegisteredObject>(nameof(IRegisteredObject.VoidReturnMethodStr)),
                methodArgs = new object[] { args0 }
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            var response = server.Request(request) as InvokeResponse;

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();
            
            Assert.AreEqual(ResponseType.invoke, response.responseType);
            Assert.AreEqual(request.objectId, response.objectId);
            Assert.AreEqual(request.methodId, response.methodId);
            Assert.IsNull(response.result);
        }
    }
}

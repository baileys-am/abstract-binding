﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AbstractBinding.Messages;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class RecipientMethodApiTests
    {
        private const string _testCategory = "Recipient Method API";
        private Mock<IRecipientCallback> _serviceMock;
        private Mock<ISerializer> _serializerMock;
        private Mock<IRegisteredObject> _regObjectMock;
        
        public RecipientMethodApiTests()
        {
            // Initialize service mock
            _serviceMock = new Mock<IRecipientCallback>();

            // Initialize serializer  mock
            _serializerMock = new Mock<ISerializer>();
            _serializerMock.Setup(o => o.SerializeObject(It.IsAny<object>())).Returns<object>(obj =>
            {
                return Serializer.Serialize(obj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<Request>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<Request>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<InvokeRequest>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<InvokeRequest>(serObj);
            });

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
            var server = new Recipient(_serializerMock.Object);
            var requestObj = new InvokeRequest()
            {
                objectId = objectId,
                methodId = nameof(IRegisteredObject.VoidReturnMethod),
                methodArgs = new object[] { args0, args1 }
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            string response = server.Request(Serializer.Serialize(requestObj));

            // Assert
            _serializerMock.Verify();
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var responseObj = Serializer.Deserialize<InvokeResponse>(response);
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
            var server = new Recipient(_serializerMock.Object);
            var requestObj = new InvokeRequest()
            {
                objectId = objectId,
                methodId = nameof(IRegisteredObject.VoidReturnMethod),
                methodArgs = new object[] { args0, args1 }
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            string response = server.Request(Serializer.Serialize(requestObj));

            // Assert
            _serializerMock.Verify();
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var responseObj = Serializer.Deserialize<ExceptionResponse>(response);
            Assert.IsTrue(responseObj.exception.Message.Contains(objectId) &&
                          responseObj.exception.Message.Contains(requestObj.methodId));
        }
    }
}

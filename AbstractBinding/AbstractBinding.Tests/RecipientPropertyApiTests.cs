using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AbstractBinding.Messages;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class RecipientPropertyApiTests
    {
        private const string _testCategory = "Recipient Property API";
        private Mock<IRecipientCallback> _serviceMock;
        private Mock<ISerializer> _serializerMock;
        private Mock<IRegisteredObject> _regObjectMock;
        
        public RecipientPropertyApiTests()
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
            _serializerMock.Setup(o => o.DeserializeObject<PropertyGetRequest>(It.IsAny<string>())).Returns<string>(serObj =>
            {
                return Serializer.Deserialize<PropertyGetRequest>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<PropertySetRequest>(It.IsAny<string>())).Returns<string>(serObj =>
            {
                return Serializer.Deserialize<PropertySetRequest>(serObj);
            });

            // Initialize registered object mock
            _regObjectMock = new Mock<IRegisteredObject>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void GetValueWithoutExceptionTest()
        {
            // Arrange
            string objectId = "objId1";
            string expectedValue = "toBeExpected";
            _regObjectMock.Setup(o => o.StringValueProperty).Returns(expectedValue);
            var recipient = new Recipient(_serializerMock.Object);
            var requestObj = new PropertyGetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty)
            };

            // Act
            recipient.Register(objectId, _regObjectMock.Object);
            string response = recipient.Request(Serializer.Serialize(requestObj));

            // Assert
            _serializerMock.Verify();
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var responseObj = Serializer.Deserialize<PropertyGetResponse>(response);
            Assert.AreEqual(ResponseType.propertyGet, responseObj.responseType);
            Assert.AreEqual(requestObj.objectId, responseObj.objectId);
            Assert.AreEqual(requestObj.propertyId, responseObj.propertyId);
            Assert.AreEqual(expectedValue, responseObj.value);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void GetValueWithExceptionTest()
        {
            // Arrange
            string objectId = "objId1";
            _regObjectMock.SetupGet(o => o.StringValueProperty).Throws(new NotImplementedException());
            var recipient = new Recipient(_serializerMock.Object);
            var requestObj = new PropertyGetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty)
            };

            // Act
            recipient.Register(objectId, _regObjectMock.Object);
            string response = recipient.Request(Serializer.Serialize(requestObj));

            // Assert
            _serializerMock.Verify();
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var responseObj = Serializer.Deserialize<ExceptionResponse>(response);
            Assert.AreEqual(ResponseType.exception, responseObj.responseType);
            Assert.IsTrue(responseObj.exception.Message.Contains(objectId) &&
                          responseObj.exception.Message.Contains(nameof(IRegisteredObject.StringValueProperty)));
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SetValueWithoutExceptionTest()
        {
            // Arrange
            string objectId = "objId1";
            string expectedValue = "toBeExpected";
            _regObjectMock.Setup(o => o.StringValueProperty).Returns(expectedValue);
            var recipient = new Recipient(_serializerMock.Object);
            var requestObj = new PropertySetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty),
                value = expectedValue
            };

            // Act
            recipient.Register(objectId, _regObjectMock.Object);
            string response = recipient.Request(Serializer.Serialize(requestObj));

            // Assert
            _serializerMock.Verify();
            _serviceMock.Verify();
            _regObjectMock.Verify();

            Assert.AreEqual(expectedValue, _regObjectMock.Object.StringValueProperty);

            var responseObj = Serializer.Deserialize<PropertySetResponse>(response);
            Assert.AreEqual(ResponseType.propertySet, responseObj.responseType);
            Assert.AreEqual(requestObj.objectId, responseObj.objectId);
            Assert.AreEqual(requestObj.propertyId, responseObj.propertyId);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SetValueWithExceptionTest()
        {
            // Arrange
            string objectId = "objId1";
            _regObjectMock.SetupSet(o => o.StringValueProperty = It.IsAny<string>()).Throws(new NotImplementedException());
            var recipient = new Recipient(_serializerMock.Object);
            var requestObj = new PropertySetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty),
                value = "toBeExpected"
            };

            // Act
            recipient.Register(objectId, _regObjectMock.Object);
            string response = recipient.Request(Serializer.Serialize(requestObj));

            // Assert
            _serializerMock.Verify();
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var responseObj = Serializer.Deserialize<ExceptionResponse>(response);
            Assert.AreEqual(ResponseType.exception, responseObj.responseType);
            Assert.IsTrue(responseObj.exception.Message.Contains(objectId) &&
                          responseObj.exception.Message.Contains(nameof(IRegisteredObject.StringValueProperty)));
        }
    }
}

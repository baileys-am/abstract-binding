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
        private readonly Mock<IRecipientCallback> _serviceMock;
        private readonly Mock<IRegisteredObject> _regObjectMock;
        private readonly ISerializer _serializer = new Serializer();

        public RecipientPropertyApiTests()
        {
            // Initialize service mock
            _serviceMock = new Mock<IRecipientCallback>();

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
            var recipient = new Recipient(_serializer);
            var requestObj = new PropertyGetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty)
            };

            // Act
            recipient.Register(objectId, _regObjectMock.Object);
            string response = recipient.Request(_serializer.SerializeObject(requestObj));

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var responseObj = _serializer.DeserializeObject<PropertyGetResponse>(response);
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
            var recipient = new Recipient(_serializer);
            var requestObj = new PropertyGetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty)
            };

            // Act
            recipient.Register(objectId, _regObjectMock.Object);
            string response = recipient.Request(_serializer.SerializeObject(requestObj));

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var responseObj = _serializer.DeserializeObject<ExceptionResponse>(response);
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
            var recipient = new Recipient(_serializer);
            var requestObj = new PropertySetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty),
                value = expectedValue
            };

            // Act
            recipient.Register(objectId, _regObjectMock.Object);
            string response = recipient.Request(_serializer.SerializeObject(requestObj));

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();

            Assert.AreEqual(expectedValue, _regObjectMock.Object.StringValueProperty);

            var responseObj = _serializer.DeserializeObject<PropertySetResponse>(response);
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
            var recipient = new Recipient(_serializer);
            var requestObj = new PropertySetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty),
                value = "toBeExpected"
            };

            // Act
            recipient.Register(objectId, _regObjectMock.Object);
            string response = recipient.Request(_serializer.SerializeObject(requestObj));

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var responseObj = _serializer.DeserializeObject<ExceptionResponse>(response);
            Assert.AreEqual(ResponseType.exception, responseObj.responseType);
            Assert.IsTrue(responseObj.exception.Message.Contains(objectId) &&
                          responseObj.exception.Message.Contains(nameof(IRegisteredObject.StringValueProperty)));
        }
    }
}
